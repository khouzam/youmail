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
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        /// <summary>
        /// Get the list of greetings for the user
        /// </summary>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailGreeting[]>> GetGreetingsAsync(DateTime lastUpdated)
        {
            GetUpdateResult<YouMailGreeting[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    YouMailGreetingQuery query = new YouMailGreetingQuery(QueryType.GetGreetings)
                    {
                        UpdatedFrom = lastUpdated,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    HttpResponseMessage response = null;
                    List<YouMailGreeting> greetings = new List<YouMailGreeting>();
                    DateTime lastQueryUpdated = DateTime.MinValue;
                    int count = 0;
                    do
                    {
                        count = 0;
                        using (response = await YouMailApiAsync(YMST.c_getGreetings + query.GetQueryString(), null, HttpMethod.Get))
                        {
                            if (response != null)
                            {
                                var stream = response.GetResponseStream();
                                var greetingResponse = stream.FromXml<YouMailGreetingsResponse>();
                                if (greetingResponse != null)
                                {
                                    count = greetingResponse.Greetings.Greetings.Length;
                                    if (count > 0)
                                    {
                                        greetings.AddRange(greetingResponse.Greetings.Greetings);
                                    }
                                }

                                if (lastQueryUpdated == DateTime.MinValue)
                                {
                                    var date = response.Headers.Date.ToString();
                                    lastQueryUpdated = DateTime.Parse(date);
                                }
                            }
                        }
                        query.Page++;
                    } while (count == query.PageLength);

                    returnValue = new GetUpdateResult<YouMailGreeting[]>(lastQueryUpdated, greetings.ToArray());
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
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="waveData"></param>
        public async Task CreateGreetingAsync(string name, string description, byte[] waveData)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    HttpContent post = null;
                    var greeting = new YouMailGreeting
                    {
                        Name = name,
                        Description = description,
                        Data = Convert.ToBase64String(waveData)
                    };

                    // Generate the POST request
                    post = greeting.ToXmlHttpContent();

                    using (await YouMailApiAsync(YMST.c_getGreetings, post, HttpMethod.Post))
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
        /// Delete a greeting from the user account
        /// </summary>
        /// <param name="id">The greeting to be deleted</param>
        public async Task DeleteGreetingAsync(long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (await YouMailApiAsync(string.Format(YMST.c_deleteGreeting, id), null, HttpMethod.Delete))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        public async Task<YouMailGreetingStatuses> GetGreetingStatusesAsync()
        {
            YouMailGreetingStatuses statuses = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {

                    using (var response = await YouMailApiAsync(YMST.c_statusGreetings, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var stream = response.GetResponseStream();
                            statuses = stream.FromXml<YouMailGreetingStatuses>();
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return statuses;
        }
    }
}
