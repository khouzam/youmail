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
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public static class YouMailExtensions
    {
        public static YouMailMessageTranscriptionStatus ToYouMailMessageTranscriptionStatus(this string s)
        {
            YouMailMessageTranscriptionStatus status = YouMailMessageTranscriptionStatus.None;
            if (s == YMST.c_transcriptionNone)
            {
                status = YouMailMessageTranscriptionStatus.None;
            }
            else if (s == YMST.c_transcriptionInProgress)
            {
                status = YouMailMessageTranscriptionStatus.InProgress;
            }
            else if (s == YMST.c_transcribed)
            {
                status = YouMailMessageTranscriptionStatus.Transcribed;
            }
            else if (s == YMST.c_transcriptionError)
            {
                status = YouMailMessageTranscriptionStatus.Error;
            }
            else if (s == YMST.c_transcriptionExceeded)
            {
                status = YouMailMessageTranscriptionStatus.Exceeded;
            }
            else if (s == YMST.c_transcriptionNotCorrectContact)
            {
                status = YouMailMessageTranscriptionStatus.NotCorrectContact;
            }
            else if (s == YMST.c_transcriptionInactivePlan)
            {
                status = YouMailMessageTranscriptionStatus.InactivePlan;
            }
            return status;
        }

        public static string FromYouMailMessageTranscriptionStatus(this YouMailMessageTranscriptionStatus status)
        {
            string retVal = YMST.c_transcriptionNone;
            switch (status)
            {
                case YouMailMessageTranscriptionStatus.Error:
                    retVal = YMST.c_transcriptionError;
                    break;

                case YouMailMessageTranscriptionStatus.Exceeded:
                    retVal = YMST.c_transcriptionExceeded;
                    break;

                case YouMailMessageTranscriptionStatus.InactivePlan:
                    retVal = YMST.c_transcriptionInactivePlan;
                    break;

                case YouMailMessageTranscriptionStatus.InProgress:
                    retVal = YMST.c_transcriptionInProgress;
                    break;

                case YouMailMessageTranscriptionStatus.None:
                    retVal = YMST.c_transcriptionNone;
                    break;

                case YouMailMessageTranscriptionStatus.NotCorrectContact:
                    retVal = YMST.c_transcriptionNotCorrectContact;
                    break;

                case YouMailMessageTranscriptionStatus.Transcribed:
                    retVal = YMST.c_transcribed;
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// An async implementation to wait on an event
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public async static Task WaitOneAsync(this EventWaitHandle handle)
        {
            await Task.Factory.StartNew(() => handle.WaitOne());
            return;
        }

        /// <summary>
        /// Helper function to convert a DateTime to a long representing the number of milliseconds since Jan 1 1970
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ToMillisecondsFromEpoch(this DateTime time)
        {
            // Round to the nearest millisecond
            return (long)Math.Round(time.ToUniversalTime().Subtract(c_epoch).TotalMilliseconds);
        }

        /// <summary>
        /// Helper function to convert a long representing the number of milliseconds since Jan 1 1970 to a DateTime
        /// /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static DateTime FromMillisecondsFromEpoch(this long milliseconds)
        {
            return c_epoch.AddMilliseconds(milliseconds).ToLocalTime();
        }

        public static Stream GetResponseStream(this HttpResponseMessage response)
        {
            var task = response.Content.ReadAsStreamAsync();
            task.Wait();
            if (task.IsFaulted)
            {
                throw task.Exception;
            }
            else
            {
                Stream responseStream = task.Result;
                var encodings = response.Content.Headers.ContentEncoding;
                if (encodings != null)
                {
                    if (encodings.Contains("gzip"))
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    }
                    else if (encodings.Contains("deflate"))
                    {
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                    }
                }
                return responseStream;
            }
        }

        internal static DateTime c_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
