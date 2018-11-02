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

    }
}
