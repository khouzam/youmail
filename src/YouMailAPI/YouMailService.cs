/*************************************************************************************************
 * Copyright (c) 2018 Gilles Khouzam
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software withou
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/*************************************************************************************************/

namespace MagikInfo.YouMailAPI
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Synchronization;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using XmlSerializerExtensions;

#if WINDOWS_UWP
    using Windows.Networking.Connectivity;
    using Windows.Storage;
#endif

    public partial class YouMailService
    {
        /// <summary>
        /// The default size of page that we query
        /// </summary>
        private const int s_PageLength = 100;

        /// <summary>
        /// The pending operation count of this object changed
        /// </summary>
        /// <param name="addOperation"></param>
        public delegate void PendingOperationHandler(bool addOperation);
        public static event PendingOperationHandler PendingOperationChanged;

        /// <summary>
        /// The authentication cookie of this object changed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="authToken"></param>
        public delegate void AuthenticationChangedHandler(string username, string authToken);
        public event AuthenticationChangedHandler AuthenticationChanged;

        /// <summary>
        /// Change the logger based on the settings
        /// </summary>
        /// <param name="logger"></param>
        public static void SetLogger(ITraceLog logger)
        {
            s_logger = logger;
        }

        /// <summary>
        /// Add a new pending operation to the service
        /// </summary>
        internal static void AddPendingOp()
        {
            PendingOperationChanged?.Invoke(true);
        }

        /// <summary>
        /// Complete a new operation on the service
        /// </summary>
        internal static void RemovePendingOp()
        {
            PendingOperationChanged?.Invoke(false);
        }

        private static bool IsGZipSupported
        {
            get
            {
                bool GZipSupport = false;
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var compressedStream = new GZipStream(stream, CompressionMode.Compress, true))
                        {
                        }
                        using (var deflatStream = new DeflateStream(stream, CompressionLevel.Fastest, true))
                        {
                        }
                        GZipSupport = true;
                    }
                }
                catch
                {
                }
                return GZipSupport;
            }
        }

        /// <summary>
        /// Create a new YouMail object
        /// </summary>
        /// <param name="username">The username to login</param>
        /// <param name="password">The user's password</param>
        /// <param name="authToken">An authentication token to user</param>
        /// <param name="userAgent">The UserAgent to use for the web requests</param>
        /// <param name="responseFormat">The format of the response, JSON or XML</param>
        /// <param name="secureConnections">Flag to specify if we use secure connections for our requests</param>
        public YouMailService(
            string username,
            string password,
            string authToken,
            string userAgent,
            ResponseFormat responseFormat = ResponseFormat.JSON,
            bool secureConnections = true)
        {
            _username = username;
            _password = password;
            _userAgent = userAgent;
            _responseFormat = responseFormat;

            // Create the HttpClient before setting the AuthToken
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);

            // Set the response format for the requests
            if (_responseFormat == ResponseFormat.JSON)
            {
                _responseFormatString = "application/json";
            }
            else if (_responseFormat == ResponseFormat.XML)
            {
                _responseFormatString = "application/xml";
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unsupported ResponseFormat: {0}, is not supported.", _responseFormat.ToString()));
            }

            _httpClient.DefaultRequestHeaders.Add("Accept", ResponseFormatString);

            if (s_gzipSupported)
            {
                _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,defalte");
            }

            AuthToken = authToken;

            SecureConnections = secureConnections;
#if WINDOWS_UWP
            NetworkInformation.NetworkStatusChanged += NetworkStatusChanged;
#endif
            _disconnectedCalls = new YouMailAPICalls();
            InvalidateNetworkStatus();
        }

        private void InvalidateNetworkStatus()
        {
#if WINDOWS_UWP
            _connected = false;
            var network = NetworkInformation.GetInternetConnectionProfile();
            if (network != null)
            {
                var connectivity = network.GetNetworkConnectivityLevel();
                _connected = connectivity == NetworkConnectivityLevel.InternetAccess;
            }

            if (_connected)
            {
                FlushCachedAPICallsAsync().IgnoreException();
            }
#else
            _connected = true;
#endif
        }

#if WINDOWS_UWP
        private async Task FlushCachedAPICallsAsync()
        {
            bool fReleaseSemaphore = false;
            AddPendingOp();
            StorageFile storageFile = null;
            YouMailAPICalls APICalls = null;
            try
            {
                await _disconnectedSemaphore.WaitAsync();
                fReleaseSemaphore = true;
                // We are now connected. Clear the disconnected calls and replay the new ones.
                _disconnectedCalls.APICalls.Clear();
                storageFile = await ApplicationData.Current.TemporaryFolder.GetFileAsync(DisconnectedFileName);
                using (var stream = await ApplicationData.Current.TemporaryFolder.OpenStreamForReadAsync(DisconnectedFileName))
                {
                    try
                    {
                        APICalls = stream.FromXml<YouMailAPICalls>();
                    }
                    catch
                    {
                    }
                }
                try
                {
                    // Delete the file now
                    await storageFile.DeleteAsync();
                }
                catch
                {
                }
                _disconnectedSemaphore.Release();
                fReleaseSemaphore = false;

                if (APICalls != null && APICalls.APICalls != null)
                {
                    foreach (var call in APICalls.APICalls)
                    {
                        try
                        {
                            StringContent content = null;
                            if (call.Data != null)
                            {
                                // For now expect only to serialize strings
                                content = new StringContent(call.Data, Encoding.UTF8, "text/xml");
                            }
                            await YouMailApiAsync(call.URL, content, call.Verb, call.Auth);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
                // Something wrong happened. Ignore the failure
            }
            finally
            {
                if (fReleaseSemaphore)
                {
                    _disconnectedSemaphore.Release();
                }
                RemovePendingOp();
            }
        }
#endif

        private void NetworkStatusChanged(object sender)
        {
            InvalidateNetworkStatus();
        }

        /// <summary>
        /// Is the service connected to the internet
        /// </summary>
        public bool IsConnected
        {
            get { return _connected; }
        }

        /// <summary>
        /// Use secure connections or not
        /// </summary>
        public bool SecureConnections
        {
            get { return _secureConnections; }
            set
            {
                _secureConnections = value;
                _protocol = _secureConnections ? YMST.c_secureProtocol : YMST.c_unsecureProtocol;
            }
        }

        /// <summary>
        /// Generate the final login URL
        /// </summary>
        private string LoginUrl
        {
            get { return YMST.c_loginUrl; }
        }

        /// <summary>
        /// Is the youmailService logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get { return HasCredentials && !_authenticationFailed; }
        }

        /// <summary>
        /// Does the service has the user credential set
        /// </summary>
        public bool HasCredentials
        {
            get { return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password); }
        }

        /// <summary>
        /// Have we tried to login
        /// </summary>
        public bool HasTriedLogin
        {
            get { return _login || Username == null || Password == null; }
        }

        /// <summary>
        /// Log an operation
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void TraceLog(TraceLevelPriority logLevel, string message, params object[] args)
        {
            if (s_logger != null)
            {
                if (!string.IsNullOrEmpty(_password))
                {
                    // Replace the password if present
                    message = message.Replace(_password, "********");
                }
                s_logger.TraceLog(logLevel, message, args);
            }
#if DEBUG
            else
            {
                Debug.WriteLine(message, args);
            }
#endif
        }
#if WINDOWS_UWP
        /// <summary>
        /// Log an API call when offline
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="data"></param>
        /// <param name="verb"></param>
        /// <param name="auth"></param>
        /// <returns></returns>
        protected async Task LogApiCallAsync(
            string URL,
            HttpContent data,
            HttpMethod verb,
            bool auth = true)
        {
            if (verb != HttpMethod.Get)
            {
                try
                {
                    string dataString = null;
                    if (data != null)
                    {
                        if (data is StringContent)
                        {
                            dataString = await data.ReadAsStringAsync();
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot serialize this API call");
                        }
                    }
                    // Create a new APICall and add it to our collection of disconnected calls
                    var APICall = new YouMailAPICall(URL, dataString, verb, auth);
                    await _disconnectedSemaphore.WaitAsync();
                    {
                        try
                        {
                            _disconnectedCalls.APICalls.Add(APICall);
                            await SaveAPICallsToFileAsync(_disconnectedCalls);
                        }
                        finally
                        {
                            _disconnectedSemaphore.Release();
                        }
                    }

                }
                catch (Exception e)
                {
                    TraceLog(TraceLevelPriority.High, "Offline call failed: {0}", e.ToString());
                }
            }
        }

        private async Task SaveAPICallsToFileAsync(YouMailAPICalls APICalls)
        {
            try
            {
                var disconnectedFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
                    DisconnectedFileName, CreationCollisionOption.ReplaceExisting);
                using (var stream = await disconnectedFile.OpenStreamForWriteAsync())
                {
                    APICalls.ToXml(stream);
                }
            }
            catch
            {
            }
        }
#endif

        /// <summary>
        /// Call a YouMail API
        /// </summary>
        /// <param name="URL">The Url for the API</param>
        /// <param name="data">optional data string to give as an HTTP POST or PUT</param>
        /// <param name="verb">Specify the verb to use for the WebRequest</param>
        /// <returns>an HttpWebResponse to the API</returns>
        protected async Task<HttpResponseMessage> YouMailApiAsync(
            string URL,
            HttpContent data,
            HttpMethod verb,
            bool auth = true,
            IList<KeyValuePair<string, string>> extraHeaders = null,
            string forceProtocol = null)
        {
#if WINDOWS_UWP
            if (!IsConnected)
            {
                await LogApiCallAsync(URL, data, verb, auth);
                return null;
            }
#endif

            bool fForce = false;
            bool fRetryAuthentication = false;
            YouMailException weRetry = null;
            HttpResponseMessage response = null;
            if (!auth || !string.IsNullOrEmpty(_auth))
            {
                try
                {
                    bool redirect;
                    do
                    {
                        redirect = false;
                        response = await CreateYouMailApiResponseAsync(URL, data, verb, auth, extraHeaders, forceProtocol).ConfigureAwait(false);
                        if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                            response.StatusCode == HttpStatusCode.Redirect ||
                            response.StatusCode == HttpStatusCode.RedirectKeepVerb)
                        {
                            // Grab the queryString from the previous URL and copy it over
                            string queryString = string.Empty;
                            int queryStringndex = URL.IndexOf('?');
                            if (queryStringndex > 0)
                            {
                                queryString = URL.Substring(queryStringndex);
                            }
                            URL = response.Headers.Location.ToString() + queryString;
                            // The protocol should be part of the URL now, so send an empty one.
                            forceProtocol = string.Empty;
                            redirect = true;
                        }
                    }
                    while (redirect);
                    EnsureYouMailResponse(response);
                }
                catch (WebException we)
                {
                    YouMailException yme = ConvertException(we);
                    if (yme != null)
                    {
                        if (yme.StatusCode == HttpStatusCode.Forbidden && auth)
                        {
                            weRetry = yme;
                            fRetryAuthentication = true;
                        }
                        else
                        {
                            throw yme;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (YouMailException re)
                {
                    if (re.StatusCode == HttpStatusCode.Forbidden && auth)
                    {
                        weRetry = re;
                        fRetryAuthentication = true;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                fForce = fRetryAuthentication = true;
            }

            if (fRetryAuthentication && auth)
            {
                if (await TryReauthenticationAsync(weRetry, fForce).ConfigureAwait(false))
                {
                    try
                    {
                        // We can retry the request now
                        bool redirect;
                        do
                        {
                            redirect = false;
                            response = await CreateYouMailApiResponseAsync(URL, data, verb, auth, extraHeaders, forceProtocol).ConfigureAwait(false);
                            if (response.StatusCode == HttpStatusCode.MovedPermanently ||
                                response.StatusCode == HttpStatusCode.Redirect ||
                                response.StatusCode == HttpStatusCode.RedirectKeepVerb)
                            {
                                URL = response.Headers.Location.ToString();
                                // The protocol should be part of the URL now, so send an empty one.
                                forceProtocol = string.Empty;
                                redirect = true;
                            }
                        }
                        while (redirect);
                        EnsureYouMailResponse(response);
                    }
                    catch (WebException we)
                    {
                        // Convert the WebException to a YouMailException
                        var yme = ConvertException(we);
                        if (yme != null)
                        {
                            throw yme;
                        }
                        throw we;
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Create a WebRequest for a generic YouMailAPI
        /// </summary>
        /// <param name="URL">The URL for the request</param>
        /// <param name="data">The data that we should post</param>
        /// <param name="verb">The type of request to create</param>
        /// <param name="auth">Should we authenticate the request (add token)</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> CreateYouMailApiResponseAsync(
            string URL,
            HttpContent data,
            HttpMethod verb,
            bool auth,
            IList<KeyValuePair<string, string>> extraHeaders,
            string forceProtocol = null)
        {
            string protocol;
            if (forceProtocol != null)
            {
                protocol = forceProtocol;
            }
            else
            {
                protocol = auth ? _protocol : YMST.c_unsecureProtocol;
            }

            // Windows Phone caches requests, so add a random query string on GET requests
            if (verb == HttpMethod.Get)
            {
                char separator = (URL.IndexOf('?') >= 0) ? '&' : '?';
                URL += separator + DateTime.Now.Ticks.ToString();
            }

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(protocol + URL),
                Method = verb
            };
            HttpResponseMessage response = null;

            // Add the extra headers
            if (extraHeaders != null)
            {
                foreach (var keys in extraHeaders)
                {
                    request.Headers.Add(keys.Key, keys.Value);
                }
            }

            TraceLog(TraceLevelPriority.Normal, request.RequestUri.ToString());
            if (verb == HttpMethod.Post || verb == HttpMethod.Put)
            {
                request.Content = data;
            }

            response = await _httpClient.SendAsync(request);
            return response;
        }

        /// <summary>
        /// On failure check if we need to reauthenticate and try again
        /// </summary>
        /// <param name="ye">The WebException that occured</param>
        /// <returns>Whether we should retry the request</returns>
        private async Task<bool> TryReauthenticationAsync(YouMailException ye, bool fForce)
        {
            bool fTryAgain = false;
            // If we get a forbidden response, then we might need to try again
            if (fForce || ye.StatusCode == HttpStatusCode.Forbidden)
            {
                // Only one client tries to re-authenticate
                if (0 == Interlocked.CompareExchange(ref _fHadTriedAuthentication, 1, 0))
                {
                    TraceLog(TraceLevelPriority.High, "Reauthenticating");
                    try
                    {
                        await PrivateLoginAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        // We didn't login, remove the auth token.
                        AuthToken = null;
                        _authenticationFailed = true;

                        throw;
                    }
                    finally
                    {
                        // Set the event for other clients to start working...
                        _authEvent.Set();
                    }
                }
                else
                {
                    TraceLog(TraceLevelPriority.Normal, "Reauthentication in progress, waiting....");

                    // Wait until we have a new authentication token.
                    await _authEvent.WaitOneAsync();
                }

                fTryAgain = true;
            }

            return fTryAgain;
        }

        /// <summary>
        /// Convert a string to Base64
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public static string StringToBase64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Convert a Base64 string to a regular string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string Base64ToString(string base64)
        {
            byte[] fromBase64 = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(fromBase64, 0, fromBase64.Length);
        }

        /// <summary>
        /// The current Authentication Token
        /// </summary>
        public string AuthToken
        {
            get
            {
                return _auth;
            }
            set
            {
                _auth = value;

                // Update the default Authorization header.
                _httpClient.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Cannot use YouMail for Authorization Header.
                    _httpClient.DefaultRequestHeaders.Add(YMST.c_authorization, YMST.c_YouMail + _auth);
                }

                AuthenticationChanged?.Invoke(Username, _auth);
            }
        }


        /// <summary>
        /// Take an object and deserialize it base on the requested format for the serivce.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="s">The stream that has the data</param>
        /// <param name="rootElement">An optional root element for JSON that will contain the object</param>
        /// <returns>The object requested</returns>
        private T DeserializeObject<T>(Stream s, string rootElement = null) where T : class
        {
            T returnObject = null;
            switch (_responseFormat)
            {
                case ResponseFormat.JSON:

                    var serializer = new JsonSerializer();
                    using (var sr = new StreamReader(s))
                    {
                        using (var jsonTextReader = new JsonTextReader(sr))
                        {
                            if (!string.IsNullOrEmpty(rootElement))
                            {
                                var root = (JObject)serializer.Deserialize(jsonTextReader, typeof(JObject));
                                returnObject = root[rootElement].ToObject<T>();
                            }
                            else
                            {
                                returnObject = (T)serializer.Deserialize(jsonTextReader, typeof(T));
                            }
                        }
                    }

                    break;

                case ResponseFormat.XML:
                    returnObject = s.FromXml<T>();
                    break;

                default:
                    throw new InvalidOperationException("Invalid conversion format");
            }

            return returnObject;
        }

        /// <summary>
        /// Take an object and deserialize it base on the requested format for the serivce.
        /// This will also print out the response receieved to the debug console
        /// </summary>
        /// <typeparam name="T">The type to deserialize to</typeparam>
        /// <param name="s">The stream that has the data</param>
        /// <param name="rootElement">An optional root element for JSON that will contain the object</param>
        /// <returns>The object requested</returns>
        private T DeserializeObjectDebug<T>(Stream s, string rootElement = null) where T : class
        {
            T returnObject = null;
            switch (_responseFormat)
            {
                case ResponseFormat.JSON:
                    {
                        using (var sr = new StreamReader(s))
                        {
                            var content = sr.ReadToEnd();
                            Debug.WriteLine(content);
                            if (!string.IsNullOrEmpty(rootElement))
                            {
                                var root = JObject.Parse(content);
                                returnObject = root[rootElement].ToObject<T>();
                            }
                            else
                            {
                                returnObject = JsonConvert.DeserializeObject<T>(content);
                            }
                        }
                    }
                    break;

                case ResponseFormat.XML:
                    returnObject = s.FromXmlDebug<T>();
                    break;

                default:
                    throw new InvalidOperationException("Invalid conversion format");
            }

            return returnObject;
        }

        private string SerializeObject<T>(T obj, string rootObject = null) where T : class
        {
            string serializedObject = null;
            switch (_responseFormat)
            {
                case ResponseFormat.JSON:
                    if (rootObject != null)
                    {
                        // Wrap the object into an object to add the root node.
                        var objSer = new Dictionary<string, Object> { { rootObject, obj } };
                        serializedObject = JsonConvert.SerializeObject(objSer, _jsonSettings);
                    }
                    else
                    {
                        serializedObject = JsonConvert.SerializeObject(obj, _jsonSettings);
                    }
                    break;

                case ResponseFormat.XML:
                    serializedObject = obj.ToXml();
                    break;

                default:
                    throw new InvalidOperationException("Invalid conversion format specified");
            }

            return serializedObject;
        }

        /// <summary>
        /// Take an object and serialize it as an HttpContentType
        /// </summary>
        /// <typeparam name="T">The type of the object that we are serializing</typeparam>
        /// <param name="obj">The object that we are serializing</param>
        /// <param name="rootObject">For Json, a root node name that is inserted</param>
        /// <returns>An HttpContent object</returns>
        private HttpContent SerializeObjectToHttpContent<T>(T obj, string rootObject = null) where T : class
        {
            var serializedString = SerializeObject(obj, rootObject);
            return new StringContent(serializedString, Encoding.UTF8, ResponseFormatString);
        }

        /// <summary>
        /// Throws an exception if the API call failed
        /// </summary>
        /// <param name="responseMessage">The ResponseMessage returned from the API</param>
        private void EnsureYouMailResponse(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                string message = null;
                YouMailAPIError apiError = null;
                var messageStream = responseMessage.GetResponseStream();
                if (messageStream != null)
                {
                    try
                    {
                        apiError = DeserializeObject<YouMailAPIError>(messageStream);
                        message = apiError.GetErrorMessage();
                    }
                    catch
                    {
                    }
                }
                throw new YouMailException(message, apiError, responseMessage.StatusCode, null);
            }
        }

        /// <summary>
        /// Get the error message from the YouMail response stream
        /// </summary>
        /// <param name="s">The ResponseStream</param>
        /// <returns>A string with the error message</returns>
        private string GetYouMailMessageFromStream(Stream s)
        {
            string message = null;
            bool fFoundMessage = false;
            try
            {
                var apiError = DeserializeObjectDebug<YouMailAPIError>(s);
                if (apiError != null)
                {
                    foreach (var error in apiError.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.LongMessage))
                        {
                            message = error.LongMessage;
                            fFoundMessage = true;
                            break;
                        }
                    }

                    if (!fFoundMessage)
                    {
                        foreach (var error in apiError.Errors)
                        {
                            if (!string.IsNullOrEmpty(error.ShortMessage))
                            {
                                message = error.ShortMessage;
                                fFoundMessage = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return message;
        }

        /// <summary>
        /// Convert a WebException into a YouMailException
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private YouMailException ConvertException(WebException e)
        {
            if (e is WebException)
            {
                WebException we = e as WebException;
                if (we.Response is HttpWebResponse)
                {
                    var webResponse = we.Response as HttpWebResponse;
                    var s = webResponse.GetResponseStream();
                    YouMailAPIError error = null;
                    try
                    {
                        error = DeserializeObject<YouMailAPIError>(s);
                        return new YouMailException(error.GetErrorMessage(), error, webResponse.StatusCode, e);
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The user's Username
        /// </summary>
        public string Username
        {
            get { return _username; }
        }

        /// <summary>
        /// The user's password
        /// </summary>
        public string Password
        {
            get { return _password; }
        }

#if WINDOWS_UWP
        private string DisconnectedFileName
        {
            get { return $"YouMailAPIDisconnected{Username}.xml"; }
        }
#endif

        public string ResponseFormatString
        {
            get { return _responseFormatString; }
        }

        private readonly ResponseFormat _responseFormat;
        private readonly string _responseFormatString;
        private readonly string _username;
        private readonly string _password;
        private readonly string _userAgent;
        private string _auth;
        private ManualResetEvent _authEvent = new ManualResetEvent(false);
        private int _fHadTriedAuthentication = 0;
        private bool _secureConnections;
        private string _protocol;
        private bool _login = false;
        private Task _loginTask = null;
        private bool _authenticationFailed = false;
        private bool _connected = true;
        private static bool s_gzipSupported = IsGZipSupported;

        private YouMailAPICalls _disconnectedCalls = null;
        private AsyncSemaphore _disconnectedSemaphore = new AsyncSemaphore(1, 1);

        public const int InboxFolder = 0;
        public const int DitchedCallerGreeting = 9132;

        private static ITraceLog s_logger = null;
        private HttpClient _httpClient = null;

        // Serializer
        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };
    }
}
