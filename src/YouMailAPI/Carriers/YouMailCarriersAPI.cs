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
                        if (response != null)
                        {
                            instructions = response.GetResponseStream().FromXml<YouMailForwardingInstructions>();
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
                            carriers = response.GetResponseStream().FromXml<YouMailCarriers>();
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
                        if (response != null)
                        {
                            returnValue = response.GetResponseStream().FromXml<YouMailAccessPoint>();
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
        /// Verify the call setup for a user
        /// </summary>
        /// <returns></returns>
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
    }
}
