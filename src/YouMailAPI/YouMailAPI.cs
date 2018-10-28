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
    using Synchronization;
    using XmlSerializerExtensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
#if UWP
    using Windows.Networking.Connectivity;
    using Windows.Storage;
#endif

    /// <summary>
    /// A Class to access the YouMail Service APIs
    /// </summary>
    public partial class YouMailService
    {
        /// <summary>
        /// Async wait for our login to succeed.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> LoginWaitAsync(Task task = null)
        {
            if (_loginTask != null && HasCredentials)
            {
                TraceLog(TraceLevelPriority.Normal, "Waiting for login to succeed.");

                await _loginTask;
            }

            return IsLoggedIn;
        }

        /// <summary>
        /// Helper method that performs the login
        /// </summary>
        private async Task PrivateLoginAsync()
        {
            if (HasCredentials && IsConnected)
            {
                try
                {
                    // We're trying to login, mark it.
                    _login = true;
                    _auth = null;

                    // Perform the login synchronously
                    var headers = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(YMST.c_YouMailUser, Username),
                        new KeyValuePair<string, string>(YMST.c_YouMailPassword, Password)
                    };

                    var loginTask = YouMailApiAsync(LoginUrl, null, HttpMethod.Get, false, headers, YMST.c_loginProtocol);
                    await loginTask;

                    using (var response = loginTask.Result)
                    {
                        if (response != null)
                        {
                            YouMailAuthToken token = response.GetResponseStream().FromXml<YouMailAuthToken>();
                            AuthToken = token.AuthToken;

                            if (_auth == null)
                            {
                                TraceLog(TraceLevelPriority.High, YMST.c_errorInvalidResponse);
                                throw new InvalidOperationException(YMST.c_errorInvalidResponse);
                            }
                        }
                    }
                }
                catch
                {
                    // If we've failed, mark the authentication as failed
                    _authenticationFailed = true;
                    throw;
                }
            }
        }

        /// <summary>
        /// Login to the YouMail account
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            try
            {
                AddPendingOp();
                TraceLog(TraceLevelPriority.Normal, "Logging in user: {0}", _username);

                if (_username != null && _password != null)
                {
                    _loginTask = PrivateLoginAsync();
                    await _loginTask;
                    _loginTask = null;
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Result of an update
        /// </summary>
        /// <typeparam name="T">The type of data that gets returned</typeparam>
        public class GetUpdateResult<T>
        {
            public DateTime LastUpdated;
            public T Data;

            public GetUpdateResult(DateTime lastUpdated, T data)
            {
                LastUpdated = lastUpdated;
                Data = data;
            }
        }

        /// <summary>
        /// Get all the messages in a folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailMessage[]>> GetMessagesAsync(int folderId, int imageSize, DataFormat dataFormat, int maxMessages = int.MaxValue)
        {
            try
            {
                AddPendingOp();
                GetUpdateResult<YouMailMessage[]> returnValue = null;
                if (await LoginWaitAsync())
                {
                    TraceLog(TraceLevelPriority.Normal, "Getting messages for folder: {0}", folderId);
                    var query = new YouMailMessageQuery(QueryType.GetMessages)
                    {
                        FolderId = folderId,
                        DataFormat = dataFormat,
                        MaxResults = 0,
                        Offset = 0,
                        IncludePreview = true,
                        IncludeContactInfo = true,
                        IncludeImageUrl = true,
                        ImageSize = imageSize,
                        PageLength = s_PageLength,
                        Page = 1
                    };

                    returnValue = await ExecuteMessageQueryAsync(query, maxMessages);
                }
                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        private async Task<GetUpdateResult<YouMailMessage[]>> ExecuteMessageQueryAsync(YouMailQuery query, int maxMessages = int.MaxValue)
        {
            HttpResponseMessage response = null;
            List<YouMailMessage> list = new List<YouMailMessage>();
            int count;
            do
            {
                count = 0;
                string queryString = query.GetQueryString();

                using (response = await YouMailApiAsync(YMST.c_messageBoxEntryQuery + queryString, null, HttpMethod.Get))
                {
                    // If we didn't get a response, break out of the loop and return null
                    if (response == null)
                    {
                        return null;
                    }
                    try
                    {
                        YouMailMessages messages = response.GetResponseStream().FromXml<YouMailMessages>();
                        if (messages != null && messages.Messages != null)
                        {
                            count = messages.Messages.Length;
                            list.AddRange(messages.Messages);
                            maxMessages -= count;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                    query.Page++;
                }
            } while (count == query.PageLength && maxMessages > 0);

            string date = null;
            date = response.Headers.Date.ToString();

            return new GetUpdateResult<YouMailMessage[]>(DateTime.Parse(date), list.ToArray());
        }

        public async Task<GetUpdateResult<YouMailMessage[]>> GetMessagesFromQuery(YouMailMessageQuery query)
        {
            try
            {
                AddPendingOp();
                GetUpdateResult<YouMailMessage[]> returnValue = null;
                if (await LoginWaitAsync())
                {
                    returnValue = await ExecuteMessageQueryAsync(query);
                }
                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get a specific message from the server
        /// </summary>
        /// <param name="item">The message we want to get</param>
        /// <param name="messageFormat">The format of the message to download</param>
        public async Task<YouMailMessage> GetMessageAsync(long item, int imageSize, DataFormat messageFormat)
        {
            try
            {
                AddPendingOp();
                YouMailMessage message = null;
                if (await (LoginWaitAsync()))
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessage)
                    {
                        ImageSize = imageSize,
                        DataFormat = messageFormat,
                        IncludePreview = true,
                        IncludeContactInfo = true,
                        IncludeImageUrl = true,
                        IncludeExtraInfo = true
                    };
                    query.AddItemId(item);

                    var results = await ExecuteMessageQueryAsync(query);
                    if (results != null && results.Data != null && results.Data.Length > 0)
                    {
                        message = results.Data[0];
                        if (!string.IsNullOrEmpty(message.Preview))
                        {
                            var uri = string.Format(YMST.c_messageBoxEntryDetails, item);
                            using (var response = await YouMailApiAsync(uri, null, HttpMethod.Get))
                            {
                                if (response != null)
                                {
                                    var stream = response.GetResponseStream();
                                    {
                                        var resp = stream.FromXml<YouMailMessageResponse>();
                                        if (resp != null)
                                        {
                                            message.Transcription = resp.Message.Transcription;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return message;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        private static int CompareFolderIdPairs(YouMailFolder a, YouMailFolder b)
        {
            return a.Id - b.Id;
        }

        /// <summary>
        /// Get the list of available folders for the user
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailFolder[]> GetFolderListAsync()
        {
            YouMailFolder[] folderList = null;
            try
            {
                AddPendingOp();

                if (await (LoginWaitAsync()))
                {
                    using (var response = await YouMailApiAsync(YMST.c_messageboxFoldersUrl, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();

                            var folders = stream.FromXml<YouMailFolders>();
                            if (folders != null && folders.Folders != null)
                            {
                                var list = folders.Folders.ToList();
                                foreach (var folder in list)
                                {
                                    if (!folder.IsValid())
                                    {
                                        list.Remove(folder);
                                    }
                                }

                                list.Sort(CompareFolderIdPairs);
                                folderList = list.ToArray();
                            }
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return folderList;
        }

        /// <summary>
        /// Delete a message from the server
        /// </summary>
        /// <param name="item">The message to be deleted</param>
        /// <returns></returns>
        public async Task DeleteMessageAsync(long item)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    await MoveMessageAsync(item, "trash");
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Move a message to a different folder
        /// </summary>
        /// <param name="item">The message to be moved</param>
        /// <param name="folder">The destination folder</param>
        /// <returns></returns>
        public async Task MoveMessageAsync(long item, string folder)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_messageboxMoveToFolderUrl, item, folder);

                    TraceLog(TraceLevelPriority.Normal, "Moving message {0} to folder {1}", item, folder);

                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Mark a message (new/listened, flag/unflag)
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="state">The state (on/off)</param>
        /// <param name="flag">Flag or not</param>
        /// <returns></returns>
        public async Task MarkMessageAsync(long item, bool state, bool flag)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string param;
                    bool silent = false;

                    if (flag)
                    {
                        param = state ? "true" : "false";
                    }
                    else
                    {
                        param = state ? ((int)MessageStatus.Read).ToString() : ((int)MessageStatus.New).ToString();
                    }

                    string put = String.Format(flag ? YMST.c_messageboxFlagUrl : YMST.c_messageboxMarkUrl, item, param, silent.ToString());

                    TraceLog(TraceLevelPriority.Normal, "Marking message {0}", put);

                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Hide the message on the server
        /// </summary>
        /// <param name="item">The item id we want to hide</param>
        public async Task HideMessageAsync(long item)
        {
            try
            {
                AddPendingOp();
                TraceLog(TraceLevelPriority.Normal, "Hidding message {0}", item);
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_messageboxMarkUrl, item, ((int)MessageStatus.Hidden).ToString(), "false");
                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Hide a history item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task HideHistoryAsync(long item)
        {
            try
            {
                AddPendingOp();
                TraceLog(TraceLevelPriority.Normal, "Hidding missed call {0}", item);
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_historyClearMessage, item);
                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the YouMail alert settings
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailAlerts> GetAlertSettingsAsync()
        {
            try
            {
                AddPendingOp();
                YouMailAlerts returnValue = null;
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_alertSettingsUrl, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            var ymAlerts = s.FromXml<YouMailAlerts>();
                            returnValue = ymAlerts;
                        }
                    }
                }

                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Update the user's settings
        /// </summary>
        /// <param name="value"></param>
        /// <param name="setting"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public async Task UpdateSettingAsync(int value, string setting)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string url = String.Format(YMST.c_setAlertSettingUrl, setting, value.ToString());
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailForwardingInstructions> GetForwardingInstructionsAsync()
        {
            YouMailForwardingInstructions instructions = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string url = string.Format(YMST.c_getForwardingInstructions, _username);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            instructions = s.FromXml<YouMailForwardingInstructions>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return instructions;
        }

        /// <summary>
        /// Get the carrier information for the user
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailCarrier> GetCarrierInfoAsync()
        {
            YouMailCarrier carrier = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string url = string.Format(YMST.c_getCarrierInfo, _username);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            carrier = s.FromXml<YouMailCarrier>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return carrier;
        }

        /// <summary>
        /// Set the carrier for the user
        /// </summary>
        /// <param name="carrierId"></param>
        /// <returns></returns>
        public async Task SetCarrierInfoAsync(int carrierId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string url = string.Format(YMST.c_setCarrierInfo, _username, carrierId);
                    using (await YouMailApiAsync(url, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return;
        }

        /// <summary>
        /// Get the list of supported carriers
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailCarriers> GetSupportedCarriersAsync()
        {
            YouMailCarriers carriers = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_getSupportedCarriers, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            carriers = s.FromXml<YouMailCarriers>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return carriers;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public async Task<YouMailAccessPoint> GetAccessPointAsync(string phonenumber)
        {
            YouMailAccessPoint returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string site = string.Format(YMST.c_accesspoint, phonenumber);
                    using (var response = await YouMailApiAsync(site, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            returnValue = s.FromXml<YouMailAccessPoint>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get the Account registration information to see if an account exists
        /// </summary>
        /// <param name="phoneNumber">The account to lookup</param>
        /// <returns>YouMailCustomResponse</returns>
        public async Task<YouMailCustomResponse> AccountRegistrationVerification(string phoneNumber)
        {
            YouMailCustomResponse customResponse = null;
            try
            {
                AddPendingOp();
                if (await (LoginWaitAsync()))
                {
                    string url = string.Format(YMST.c_registrationVerify, phoneNumber);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Get))
                    {
                        customResponse = response.GetResponseStream().FromXml<YouMailCustomResponse>();
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return customResponse;
        }

        public async Task<TranscriptionState> GetTranscriptionRequestStateAsync(long id)
        {
            return await ProcessTranscriptionAsync(id, HttpMethod.Get);
        }

        public async Task<TranscriptionState> RequestTranscriptionAsync(long id)
        {
            return await ProcessTranscriptionAsync(id, HttpMethod.Put);
        }

        private async Task<TranscriptionState> ProcessTranscriptionAsync(long id, HttpMethod verb)
        {
            var retVal = TranscriptionState.None;
            try
            {
                AddPendingOp();
                if (await (LoginWaitAsync()))
                {
                    string uri = string.Format(YMST.c_messageTranscriptionRequest, id);
                    using (var response = await YouMailApiAsync(uri, null, verb))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();
                            var transcriptionResponse = stream.FromXml<YouMailTranscriptionRequestResponse>();
                            retVal = transcriptionResponse.TranscriptionState;
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return retVal;
        }

        /// <summary>
        /// Verify if an account exists for the user
        /// </summary>
        /// <param name="phoneNumber">The number to lookup</param>
        /// <returns></returns>
        public async Task<bool> AccountExistsAsync(string phoneNumber)
        {
            bool returnValue = true;
            AddPendingOp();
            try
            {
                var result = await AccountRegistrationVerification(phoneNumber);
                // On Success we found the account;
                if (result.Properties != null && result.Properties.ContainsKey(YMST.c_registrationStatus))
                {
                    returnValue = result.Properties[YMST.c_registrationStatus] != YMST.c_open;
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        /// <returns></returns>
        public async Task CreateAccountAsync(string username, string password, string firstName, string lastName, string email)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var account = new YouMailAccount
                    {
                        Phone = username,
                        Password = password,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        AccountTemplate = "Magikinfo"
                    };

                    using (await YouMailApiAsync(YMST.c_createAccountUrl, account.ToXmlHttpContent(), HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Delete the user account
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAccountAsync()
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string url = string.Format(YMST.c_accountDetailsUrl, Username);
                    using (await YouMailApiAsync(url, null, HttpMethod.Delete))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the list of greetings for the user
        /// </summary>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailGreeting[]>> GetGreetingsAsync(DateTime lastUpdated)
        {
            GetUpdateResult<YouMailGreeting[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    YouMailGreetingQuery query = new YouMailGreetingQuery(QueryType.GetGreetings)
                    {
                        UpdatedFrom = lastUpdated,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    HttpResponseMessage response = null;
                    List<YouMailGreeting> greetings = new List<YouMailGreeting>();
                    DateTime lastQueryUpdated = DateTime.MinValue;
                    int count = 0;
                    do
                    {
                        count = 0;
                        using (response = await YouMailApiAsync(YMST.c_getGreetings + query.GetQueryString(), null, HttpMethod.Get))
                        {
                            if (response != null)
                            {
                                var stream = response.GetResponseStream();
                                var greetingResponse = stream.FromXml<YouMailGreetingsResponse>();
                                if (greetingResponse != null)
                                {
                                    count = greetingResponse.Greetings.Greetings.Length;
                                    if (count > 0)
                                    {
                                        greetings.AddRange(greetingResponse.Greetings.Greetings);
                                    }
                                }

                                if (lastQueryUpdated == DateTime.MinValue)
                                {
                                    var date = response.Headers.Date.ToString();
                                    lastQueryUpdated = DateTime.Parse(date);
                                }
                            }
                        }
                        query.Page++;
                    } while (count == query.PageLength);

                    returnValue = new GetUpdateResult<YouMailGreeting[]>(lastQueryUpdated, greetings.ToArray());
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="waveData"></param>
        public async Task CreateGreetingAsync(string name, string description, byte[] waveData)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    HttpContent post = null;
                    var greeting = new YouMailGreeting
                    {
                        Name = name,
                        Description = description,
                        Data = Convert.ToBase64String(waveData)
                    };

                    // Generate the POST request
                    post = greeting.ToXmlHttpContent();

                    using (await YouMailApiAsync(YMST.c_getGreetings, post, HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Delete a greeting from the user account
        /// </summary>
        /// <param name="id">The greeting to be deleted</param>
        public async Task DeleteGreetingAsync(long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (await YouMailApiAsync(string.Format(YMST.c_deleteGreeting, id), null, HttpMethod.Delete))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the ECommerce status of the user
        /// </summary>
        /// <returns>The Ecommerce status for the user</returns>
        public async Task<YouMailECommerce> GetECommerceStatusAsync()
        {
            YouMailECommerce eCommerceResult = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_ecommerce, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            eCommerceResult = s.FromXml<YouMailECommerce>();
                            if (eCommerceResult != null &&
                                eCommerceResult.TranscriptionPlan != null &&
                                eCommerceResult.TranscriptionPlan.ProductStatus == ProductStatus.Active)
                            {
                                var status = await GetTranscriptionStatusAsync();
                                eCommerceResult.TranscriptionStatus = status;
                                eCommerceResult.TranscriptionPlan.ProductStatus = status.Active ? ProductStatus.Active : ProductStatus.Canceled;
                            }
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return eCommerceResult;
        }

        /// <summary>
        /// Get the user info
        /// </summary>
        /// <returns>The user info</returns>
        public async Task<YouMailUserInfo> GetUserInfoAsync(string user = null)
        {
            YouMailUserInfo returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    if (user == null)
                    {
                        user = _username;
                    }
                    string uri = string.Format(YMST.c_accountDetailsUrl, user);
                    using (var response = await YouMailApiAsync(uri, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            returnValue = s.FromXml<YouMailUserInfo>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <param name="post">The post string that we'll send</param>
        /// <returns></returns>
        public async Task SetUserInfoAsync(YouMailUserInfo userInfo)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string uri = string.Format(YMST.c_accountDetailsUrl, _username);
                    using (await YouMailApiAsync(uri, userInfo.ToXmlHttpContent(), HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task<string> GetUserIdAsync()
        {
            string returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_settingsUserId, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            var userId = s.FromXml<YouMailUserId>();

                            returnValue = userId.UserId;
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get the current transcription status for the user
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public async Task<YouMailTranscriptionStatus> GetTranscriptionStatusAsync()
        {
            YouMailTranscriptionStatus returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_transcriptionStatusApi, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            returnValue = s.FromXml<YouMailTranscriptionStatus>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get the current transcription settings for the user
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public async Task<TranscriptionSettings> GetTranscriptionSettingsAsync()
        {
            TranscriptionSettings returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_transcriptionSettingsApi, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var s = response.GetResponseStream();
                            returnValue = s.FromXml<TranscriptionSettings>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Set the transcription setting for the use
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SetTranscriptionSettingAsync(string setting, string value)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string apiString = string.Format(YMST.c_transcriptionSettingsSet, setting, value);
                    using (await YouMailApiAsync(apiString, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Add an order to the user's account
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task AddOrderAsync(string product)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string api = string.Format(YMST.c_addProduct, product);
                    using (await YouMailApiAsync(api, null, HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get a message history based on a YouMailQuery
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailHistory[]>> GetMessageHistoryFromQueryAsync(YouMailQuery query)
        {
            GetUpdateResult<YouMailHistory[]> returnValue = null;
            HttpResponseMessage response = null;
            List<YouMailHistory> callList = new List<YouMailHistory>();
            DateTime lastQueryUpdated = DateTime.MinValue;
            int count = 0;
            do
            {
                count = 0;
                string queryString = query.GetQueryString();

                using (response = await YouMailApiAsync(YMST.c_messageBoxHistoryQuery + queryString, null, HttpMethod.Get))
                {
                    if (response != null)
                    {
                        Stream stream = response.GetResponseStream();
                        var histories = stream.FromXml<YouMailHistories>();

                        // Get all the call histories where the result is no message left
                        if (histories != null && histories.Histories != null)
                        {
                            var messageQuery = from c in histories.Histories
                                               where c.Result != VoicemailResult.LeftMessage
                                               select c;

                            count = messageQuery.Count();
                            callList.AddRange(messageQuery);
                        }
                        if (lastQueryUpdated == DateTime.MinValue)
                        {
                            var date = response.Headers.Date.ToString();
                            lastQueryUpdated = DateTime.Parse(date);
                        }
                    }
                }
                query.Page++;
            } while (count == query.PageLength);

            returnValue = new GetUpdateResult<YouMailHistory[]>(lastQueryUpdated, callList.ToArray());
            return returnValue;
        }


        /// <summary>
        /// Get the list of messages
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailHistory[]>> GetMessageHistoryAsync(DateTime lastUpdated, int imageSize)
        {
            GetUpdateResult<YouMailHistory[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetHistory)
                    {
                        MaxResults = 0,
                        Offset = 0,
                        IncludeImageUrl = true,
                        UpdatedFrom = lastUpdated,
                        IncludeContactInfo = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    returnValue = await GetMessageHistoryFromQueryAsync(query);
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get a specified call
        /// </summary>
        /// <param name="messageId">The message that we want to update</param>
        /// <returns></returns>
        public async Task<YouMailHistory> GetMessageHistoryAsync(long messageId, int imageSize)
        {
            YouMailHistory returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessageHistory)
                    {
                        MaxResults = 0,
                        Offset = 0,
                        IncludeImageUrl = true,
                        IncludeContactInfo = true,
                        IncludeLocation = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };
                    query.AddItemId(messageId);

                    var result = await GetMessageHistoryFromQueryAsync(query);
                    if (result != null && result.Data != null && result.Data.Length > 0)
                    {
                        returnValue = result.Data[0];
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Create a new folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="folderDescription"></param>
        /// <returns></returns>
        public async Task CreateFolderAsync(string folderName, string folderDescription)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var folder = new YouMailFolder
                    {
                        Name = folderName,
                        Description = folderDescription
                    };

                    using (await YouMailApiAsync(YMST.c_createFolder, folder.ToXmlHttpContent(), HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<long> CreateContactAsync(YouMailContact contact)
        {
            long id = 0;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_createContact, contact.ToXmlHttpContent(), HttpMethod.Post))
                    {
                        YouMailContact returned = response.GetResponseStream().FromXml<YouMailContact>();
                        id = returned.Id;
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return id;
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task UpdateContactAsync(YouMailContact contact, long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {

                    using (await YouMailApiAsync(string.Format(YMST.c_updateContact, id), contact.ToXmlHttpContent(), HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        private const int _contactUploadSize = 15;

        /// <summary>
        /// Upload a whole list of contacts
        /// </summary>
        /// <param name="contacts">The contacts to upload and update.</param>
        /// <returns></returns>
        public async Task UploadContactsAsync(YouMailContacts contacts)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    int currentIndex = 0;
                    YouMailContacts currentContacts = new YouMailContacts();
                    // We can only send 15 contacts at a time
                    var contactsList = new List<YouMailContact>();
                    do
                    {
                        contactsList.Clear();
                        int elements = Math.Min(_contactUploadSize, contacts.Contacts.Length - currentIndex);
                        for (int i = 0; i < elements; i++)
                        {
                            contactsList.Add(contacts.Contacts[currentIndex++]);
                        }

                        // Add the 15 contacts to the currentContacts
                        currentContacts.Contacts = contactsList.ToArray();

                        // Create a MultipartForm for uploading a compressed gz file.
                        var boundaryMarker = "Boundary--" + Guid.NewGuid().ToString();
                        using (var content = new MultipartFormDataContent(boundaryMarker))
                        {
                            // There's an issue with the boundary having extra quotes that YouMail doesn't like
                            // create the boundary ourselves.
                            content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                            content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", boundaryMarker));
                            // Create a memory stream that will compress the XML representaion of the contacts
                            using (var stream = new MemoryStream())
                            {
                                using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Compress, true))
                                {
                                    currentContacts.ToXml(gzStream);
                                }

                                // Seek back to the origin of the stream to stick it into the http message
                                stream.Seek(0, SeekOrigin.Begin);

                                using (var compressedContacts = new ByteArrayContent(stream.ToArray()))
                                {
                                    compressedContacts.Headers.ContentType = new MediaTypeHeaderValue("application/gzip");
                                    compressedContacts.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                    {
                                        FileName = "contacts.xml.gz",
                                        Name = "datafile"
                                    };
                                    content.Add(compressedContacts);
                                    using (await YouMailApiAsync(YMST.c_uploadContacts, content, HttpMethod.Post))
                                    {
                                    }
                                }
                            }
                        }

                        bool completed = false;
                        do
                        {
                            // We've uploaded a batch of contacts, poll to see if we can send the next ones
                            using (var response = await YouMailApiAsync(YMST.c_uploadContactsStatus, null, HttpMethod.Get))
                            {
                                var status = response.GetResponseStream().FromXml<YouMailContactsUploadStatus>();
                                if (status.Status == YouMailContactsUploadStatus.StatusEnum.Started ||
                                    status.Status == YouMailContactsUploadStatus.StatusEnum.Pending)
                                {
                                    await Task.Delay(2000);
                                }
                                else
                                {
                                    completed = true;
                                }
                            }
                        }
                        while (!completed);
                    }
                    while (currentIndex < contacts.Contacts.Length);
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the times the last items were updated
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DateTime[]> GetLastUpdateTimesAsync(string userId)
        {
            try
            {
                AddPendingOp();
                DateTime[] times = null;
                if (!string.IsNullOrEmpty(userId))
                {
                    string apiCall = string.Format(YMST.c_pollMessages, userId[userId.Length - 1], userId);
                    using (var response = await YouMailApiAsync(apiCall, null, HttpMethod.Get, false))
                    {
                        if (response != null)
                        {
                            Stream s = response.GetResponseStream();

                            if (s != null)
                            {
                                TextReader reader = new StreamReader(s);
                                string stamps = reader.ReadToEnd();
                                string[] values = stamps.Split(',');

                                times = new DateTime[values.Length];

                                for (int i = 0; i < values.Length; i++)
                                {
                                    try
                                    {
                                        // Convert epoch to DateTime
                                        times[i] = long.Parse(values[i]).FromMillisecondsFromEpoch();
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        // We got an invalid value. Ignore it for now
                                    }
                                }
                            }
                        }
                    }
                }
                return times;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the list of messages since we last checked
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public async Task<YouMailMessage[]> GetUpdatedMessagesSinceAsync(DateTime lastUpdated, int imageSize, DataFormat dataFormat)
        {
            YouMailMessage[] returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessagesSince)
                    {
                        UpdatedFrom = lastUpdated,
                        DataFormat = dataFormat,
                        IncludePreview = true,
                        IncludeImageUrl = true,
                        IncludeContactInfo = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    List<YouMailMessage> list = new List<YouMailMessage>();
                    int count;

                    do
                    {
                        count = 0;
                        string post = query.GetQueryString();

                        TraceLog(TraceLevelPriority.Normal, "Getting MessageList: {0}", post);

                        using (var response = await YouMailApiAsync(YMST.c_messageBoxEntryQuery + post, null, HttpMethod.Get))
                        {
                            if (response != null)
                            {
                                var messages = response.GetResponseStream().FromXml<YouMailMessages>();

                                if (messages != null && messages.Messages != null)
                                {
                                    count = messages.Messages.Length;
                                    list.AddRange(messages.Messages);
                                }
                            }
                        }
                        query.Page++;
                    } while (count == query.PageLength);

                    returnValue = list.ToArray();
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get the data for the message
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public async Task<Stream> GetMessageDataAsync(string URL)
        {
            Stream returnValue = null;
            bool fRetry = false;
            YouMailException weRetry = null;
            HttpResponseMessage response = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    try
                    {
                        if (IsConnected)
                        {
                            response = await GetMessageDownloadResponseAsync(URL);
                            response.EnsureYouMailResponse();
                        }
                    }
                    catch (WebException we)
                    {
                        weRetry = we.ConvertException();
                        if (weRetry.StatusCode == HttpStatusCode.Forbidden)
                        {
                            fRetry = true;
                        }
                        else
                            throw;
                    }
                    catch (YouMailException re)
                    {
                        weRetry = re;
                        if (weRetry.StatusCode == HttpStatusCode.Forbidden)
                        {
                            fRetry = true;
                        }
                        else
                            throw;
                    }
                    if (fRetry)
                    {
                        if (await TryReauthenticationAsync(weRetry, false))
                        {
                            TraceLog(TraceLevelPriority.Normal, "Getting message: {0}", URL);
                            response = await GetMessageDownloadResponseAsync(URL);
                        }
                    }
                    if (response != null)
                    {
                        returnValue = response.GetResponseStream();
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        private async Task<HttpResponseMessage> GetMessageDownloadResponseAsync(string URL)
        {
            // Replace the http:// with the protocol selection for the user
            int index = URL.IndexOf(YMST.c_protocolToken);
            URL = _protocol + URL.Substring(index + YMST.c_protocolToken.Length);

            // Need to add the authToken to the URI
            char separator = (URL.IndexOf('?') >= 0) ? '&' : '?';
            URL += separator + "authtoken=" + AuthToken;

            TraceLog(TraceLevelPriority.Normal, "{0} {1}", HttpMethod.Get, URL);

            var response = await _httpClient.GetAsync(URL);
            return response;
        }

        /// <summary>
        /// Get or create a contact if one doesn't exist.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<YouMailContact> GetOrCreateContactAsync(string number, YouMailContact contact)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_getOrCreateContact, number), contact.ToXmlHttpContent(), HttpMethod.Post))
                    {
                        return response.GetResponseStream().FromXml<YouMailContact>();
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return null;
        }

        /// <summary>
        /// Get the contacts for the user
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailContact[]>> GetContactsAsync(DateTime lastUpdated, int imageSize, YouMailContactType contactType = YouMailContactType.NoRestriction)
        {
            GetUpdateResult<YouMailContact[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    YouMailContactQuery query = new YouMailContactQuery(QueryType.GetContacts)
                    {
                        UpdatedFrom = lastUpdated,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength,
                        ContactType = contactType
                    };

                    returnValue = await ExecuteContactQueryAsync(query);
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get a YouMailContact from an ID
        /// </summary>
        /// <param name="id">The id of the contact</param>
        /// <returns>The YouMailContact</returns>
        public async Task<YouMailContact> GetContactAsync(long id, int imageSize)
        {
            YouMailContact contact = null;
            try
            {
                AddPendingOp();
                YouMailContactQuery query = new YouMailContactQuery(QueryType.GetContact)
                {
                    ImageSize = imageSize,
                };
                query.AddItemId(id);

                var results = await ExecuteContactQueryAsync(query);
                if (results != null && results.Data != null && results.Data.Length > 0)
                {
                    contact = results.Data[0];
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return contact;
        }

        private async Task<GetUpdateResult<YouMailContact[]>> ExecuteContactQueryAsync(YouMailQuery query)
        {
            HttpResponseMessage response = null;
            List<YouMailContact> contacts = new List<YouMailContact>();
            DateTime lastQueryUpdated = DateTime.MinValue;
            int count = 0;
            do
            {
                count = 0;
                using (response = await YouMailApiAsync(YMST.c_getContacts + query.GetQueryString(), null, HttpMethod.Get))
                {
                    if (response != null)
                    {
                        var stream = response.GetResponseStream();
                        var contactResponse = stream.FromXml<YouMailContactsResponse>();
                        if (contactResponse != null)
                        {
                            count = contactResponse.Contacts.Contacts.Length;
                            if (count > 0)
                            {
                                contacts.AddRange(contactResponse.Contacts.Contacts);
                            }
                        }

                        if (lastQueryUpdated == DateTime.MinValue)
                        {
                            var date = response.Headers.Date.ToString();
                            lastQueryUpdated = DateTime.Parse(date);
                        }
                    }
                }
                query.Page++;
            } while (count == query.PageLength);

            return new GetUpdateResult<YouMailContact[]>(lastQueryUpdated, contacts.ToArray());
        }

        /// <summary>
        /// Delete a YouMail contact
        /// </summary>
        /// <param name="id">The id of the contact</param>
        /// <returns></returns>
        public async Task DeleteContactAsync(long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await ContactApiAsync(id, HttpMethod.Delete))
                    {
                    };
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Generic call to the contact API REST endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> ContactApiAsync(long id, HttpMethod verb)
        {
            return await YouMailApiAsync(string.Format(YMST.c_contactsSelect, id), null, verb);
        }

        /// <summary>
        /// Set the specific greeting for a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="greetingId"></param>
        /// <returns></returns>
        public async Task SetContactGreetingAsync(long contactId, long greetingId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_setContactGreeting, contactId, greetingId), null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }


        /// <summary>
        /// Set the specific action for a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task SetContactActionAsync(long contactId, YouMailActionType action)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_setContactAction, contactId, (int)action), null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Register a device for a custom push notification
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<YouMailPushRegistration> RegisterDeviceAsync(YouMailPushRegistration pushRegistration)
        {
            YouMailPushRegistration registration = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_devicePushRegistrations, pushRegistration.ToXmlHttpContent(), HttpMethod.Post))
                    {
                        if (response != null)
                        {
                            registration = response.GetResponseStream().FromXml<YouMailPushRegistration>();
                        }
                    }
                }

                return registration;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get the current device registrations
        /// </summary>
        /// <returns>The list of device registrations</returns>
        public async Task<YouMailPushRegistration[]> GetDeviceRegistrationsAsync()
        {
            YouMailPushRegistration[] returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_devicePushRegistrations, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();
                            var registrations = stream.FromXml<YouMailPushRegistrations>();
                            if (registrations != null)
                            {
                                returnValue = registrations.PushRegistrations;
                            }
                        }
                    }
                }

                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task RenewDeviceRegistrationAsync(int deviceRegistration, string deviceId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_deviceUpdateRegistration, deviceRegistration, deviceId), null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task DeleteDeviceRegistrationAsync(int deviceRegistration)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_devicePushRegistration, deviceRegistration), null, HttpMethod.Delete))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task VerifyCallSetup()
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_verifyCallSetupInitiate, Username), null, HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task<YouMailCallVerifyStatus> VerifyCallSetupGetStatus()
        {
            YouMailCallVerifyStatus retVal = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_verifyCallSetupValidate, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();
                            retVal = stream.FromXml<YouMailCallVerifyStatus>();
                        }
                    }
                }

                return retVal;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task<YouMailGreetingStatuses> GetGreetingStatusesAsync()
        {
            YouMailGreetingStatuses statuses = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {

                    using (var response = await YouMailApiAsync(YMST.c_statusGreetings, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();
                            statuses = stream.FromXml<YouMailGreetingStatuses>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return statuses;
        }

        public async Task<YouMailPhoneRecord> LookupPhoneNumberAsync(string number)
        {
            YouMailPhoneRecord phoneRecord = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var url = string.Format(YMST.c_directoryLookup, number);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Get, true, null))
                    {
                        if (response != null)
                        {
                            var records = response.GetResponseStream().FromXml<YouMailPhoneRecords>();
                            if (records != null && records.PhoneRecords != null && records.PhoneRecords.Count > 0)
                            {
                                phoneRecord = records.PhoneRecords[0];
                            }
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return phoneRecord;
        }

        /// <summary>
        /// Send a text message to confirm that the user can receive them.
        /// </summary>
        /// <returns></returns>
        public async Task ConfirmTextMessageStatusAsync()
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var url = string.Format(YMST.c_confirmTextMessages, Username);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Post))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Confirm that the text message has been received by the user.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task ConfirmTextMessageStatusValidationAsync(string code)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var url = string.Format(YMST.c_confirmTextMessagesValidation, Username, code.ToUpper());
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Gets the current spam settings
        /// </summary>
        /// <returns></returns>
        public async Task<SpamLevel> GetSpamSettingsAsync()
        {
            SpamLevel retVal = SpamLevel.Moderate;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_spamSettings, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var spamSettings = response.GetResponseStream().FromXml<YouMailSpamSettings>();
                            retVal = spamSettings.SpamLevel;
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return retVal;
        }

        /// <summary>
        /// Change the user's spam settings
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task SetSpamSettingsAsync(SpamLevel level)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var uri = string.Format(YMST.c_spamSettingsSet, (int)level);
                    using (var response = await YouMailApiAsync(uri, null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }


        /// <summary>
        /// Get the list of virtual numbers for the user
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailVirtualNumber[]> GetVirtualNumbersAsync()
        {
            YouMailVirtualNumber[] returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_extraLines, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var numbers = response.GetResponseStream().FromXml<YouMailVirtualNumbers>();
                            returnValue = numbers.VirtualNumbers.ToArray();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }
    }
}
