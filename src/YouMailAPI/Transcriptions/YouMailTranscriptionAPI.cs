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
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
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
        /// Get the current transcription status for the user
        /// </summary>
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
                            // TODO: This only supports XML
                            //returnValue = DeserializeObject<YouMailTranscriptionStatus>(response.GetResponseStream());
                            returnValue = response.GetResponseStream().FromXml<YouMailTranscriptionStatus>();
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
                            returnValue = DeserializeObject<TranscriptionSettings>(response.GetResponseStream(), YMST.c_transcriptionSettings);
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
    }
}
