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
    using System.Net;

    public class YouMailException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public YouMailResponse Response { get; private set; }

        public YouMailException(
            string message,
            YouMailResponse response,
            HttpStatusCode statusCode,
            Exception innerException)
            : base(message, innerException)
        {
            Response = response;
            StatusCode = statusCode;
        }

        public override string Message
        {
            get
            {
                string message = null;
                if (Response != null)
                {
                    message = Response.GetErrorMessage();
                }
                if (string.IsNullOrEmpty(message))
                {
                    if (!string.IsNullOrEmpty(base.Message))
                    {
                        return base.Message;
                    }
                    if (InnerException != null)
                    {
                        return InnerException.Message;
                    }
                }
                return message;
            }
        }
    }
}
