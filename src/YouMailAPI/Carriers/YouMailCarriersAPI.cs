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
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        /// <summary>
        /// Lookup the forwarding instructions for the user
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
                        instructions = DeserializeObject<YouMailForwardingInstructions>(response.GetResponseStream(), YMST.c_phoneForwardingInstruction);
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
                        carrier = DeserializeObject<YouMailCarrier>(response.GetResponseStream(), YMST.c_carrier);
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
                        carriers = DeserializeObject<YouMailCarriers>(response.GetResponseStream());
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
        /// Get the accesspoint information for the user
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
                        returnValue = DeserializeObject<YouMailAccessPoint>(response.GetResponseStream());
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
        /// Start a CallSetup Verification
        /// </summary>
        /// <returns></returns>
        public async Task<string> VerifyCallSetupAsync()
        {
            try
            {
                string retVal = null;
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_verifyCallSetupInitiate, Username), null, HttpMethod.Post))
                    {
                        var youMailResponse = DeserializeObject<YouMailResponse>(response.GetResponseStream());
                        if (youMailResponse != null && youMailResponse.Properties != null)
                        {
                            retVal = youMailResponse.Properties[YMST.c_callSetUuid];
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

        /// <summary>
        /// Verify the call setup for a user
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailCallVerifyStatus> VerifyCallSetupGetStatusAsync(string uuid)
        {
            YouMailCallVerifyStatus retVal = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string URL = string.Format(YMST.c_verifyCallSetupValidate, Username, uuid);
                    using (var response = await YouMailApiAsync(URL, null, HttpMethod.Get))
                    {
                        var details = DeserializeObject<YouMailTestCallDetails>(response.GetResponseStream());
                        if (details != null && details.TestCallDetails != null && details.TestCallDetails.Length >= 1)
                        {
                            retVal = details.TestCallDetails[0];
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
    }
}
