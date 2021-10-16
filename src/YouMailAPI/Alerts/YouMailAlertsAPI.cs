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
                        returnValue = DeserializeObjectDebug<YouMailAlerts>(response.GetResponseStream(), YMST.c_alertSettings);
                    }
                }

                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task SetAlertSettingsAsync(YouMailAlerts alerts)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var content = SerializeObjectToHttpContent(alerts);
                    using (var response = await YouMailApiAsync(YMST.c_alertSettingsUrl, content, HttpMethod.Put))
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
