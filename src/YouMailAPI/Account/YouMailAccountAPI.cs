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
    using MagikInfo.XmlSerializerExtensions;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
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
        /// <param name="userInfo"></param>
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

        /// <summary>
        /// Get the UserId for the current user
        /// </summary>
        /// <returns></returns>
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
                            var userId = DeserializeObject<YouMailUserId>(response.GetResponseStream());
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
    }
}
