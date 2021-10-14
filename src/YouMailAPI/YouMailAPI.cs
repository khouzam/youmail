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
                            YouMailAuthToken token = DeserializeObject<YouMailAuthToken>(response.GetResponseStream());
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
        /// Update the user's settings
        /// </summary>
        /// <param name="value"></param>
        /// <param name="setting"></param>
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
                return times;
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
                        var numbers = response.GetResponseStream().FromXml<YouMailVirtualNumbers>();
                        returnValue = numbers.VirtualNumbers.ToArray();
                    }
                }
                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }
    }
}
