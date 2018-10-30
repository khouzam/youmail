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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using XmlSerializerExtensions;
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
