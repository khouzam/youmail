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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        /// <summary>
        /// Get all the messages in a folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailMessage[]>> GetMessagesAsync(int folderId, int imageSize, DataFormat dataFormat, int maxMessages = int.MaxValue)
        {
            try
            {
                AddPendingOp();
                GetUpdateResult<YouMailMessage[]> returnValue = null;
                if (await LoginWaitAsync())
                {
                    TraceLog(TraceLevelPriority.Normal, "Getting messages for folder: {0}", folderId);
                    var query = new YouMailMessageQuery(QueryType.GetMessages)
                    {
                        FolderId = folderId,
                        DataFormat = dataFormat,
                        MaxResults = maxMessages,
                        Offset = 0,
                        IncludePreview = true,
                        IncludeContactInfo = true,
                        IncludeImageUrl = true,
                        ImageSize = imageSize,
                        PageLength = s_PageLength,
                        Page = 1
                    };

                    returnValue = await ExecuteMessageQueryAsync(query);
                }
                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        private async Task<GetUpdateResult<YouMailMessage[]>> ExecuteMessageQueryAsync(YouMailQuery query)
        {
            HttpResponseMessage response = null;
            List<YouMailMessage> list = new List<YouMailMessage>();
            int count;
            do
            {
                count = 0;
                query.Offset = count;
                if (query.MaxResults < query.PageLength)
                {
                    query.PageLength = query.MaxResults;
                }
                string queryString = query.GetQueryString();

                using (response = await YouMailApiAsync(YMST.c_messageBoxEntryQuery + queryString, null, HttpMethod.Get))
                {
                    // If we didn't get a response, break out of the loop and return null
                    if (response == null)
                    {
                        return null;
                    }
                    try
                    {
                        YouMailMessages messages = DeserializeObject<YouMailMessages>(response.GetResponseStream());
                        if (messages != null && messages.Messages != null)
                        {
                            count = messages.Messages.Length;
                            list.AddRange(messages.Messages);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                    query.Offset += count;
                    query.MaxResults -= count;
                }
            } while (count == query.PageLength && query.MaxResults > 0);

            string date = null;
            date = response.Headers.Date.ToString();

            return new GetUpdateResult<YouMailMessage[]>(DateTime.Parse(date), list.ToArray());
        }

        public async Task<GetUpdateResult<YouMailMessage[]>> GetMessagesFromQuery(YouMailMessageQuery query)
        {
            try
            {
                AddPendingOp();
                GetUpdateResult<YouMailMessage[]> returnValue = null;
                if (await LoginWaitAsync())
                {
                    returnValue = await ExecuteMessageQueryAsync(query);
                }
                return returnValue;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Get a specific message from the server
        /// </summary>
        /// <param name="item">The message we want to get</param>
        /// <param name="messageFormat">The format of the message to download</param>
        public async Task<YouMailMessage> GetMessageAsync(long item, int imageSize, DataFormat messageFormat)
        {
            try
            {
                AddPendingOp();
                YouMailMessage message = null;
                if (await (LoginWaitAsync()))
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessage)
                    {
                        ImageSize = imageSize,
                        DataFormat = messageFormat,
                        IncludePreview = true,
                        IncludeContactInfo = true,
                        IncludeImageUrl = true,
                        IncludeExtraInfo = true
                    };
                    query.AddItemId(item);

                    var results = await ExecuteMessageQueryAsync(query);
                    if (results != null && results.Data != null && results.Data.Length > 0)
                    {
                        message = results.Data[0];
                        if (!string.IsNullOrEmpty(message.Preview))
                        {
                            var uri = string.Format(YMST.c_messageBoxEntryDetails, item);
                            using (var response = await YouMailApiAsync(uri, null, HttpMethod.Get))
                            {
                                if (response != null)
                                {
                                    var resp = DeserializeObject<YouMailMessageResponse>(response.GetResponseStream());
                                    if (resp != null)
                                    {
                                        message.Transcription = resp.Message.Transcription;
                                    }
                                }
                            }
                        }
                    }
                }
                return message;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Delete a message from the server
        /// </summary>
        /// <param name="item">The message to be deleted</param>
        /// <returns></returns>
        public async Task DeleteMessageAsync(long item)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    await MoveMessageAsync(item, YMST.c_trash);
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Move a message to a different folder
        /// </summary>
        /// <param name="item">The message to be moved</param>
        /// <param name="folder">The destination folder</param>
        /// <returns></returns>
        public async Task MoveMessageAsync(long item, string folder)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_messageboxMoveToFolderUrl, item, folder);

                    TraceLog(TraceLevelPriority.Normal, "Moving message {0} to folder {1}", item, folder);

                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
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
        /// Mark a message (new/listened, flag/unflag)
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="state">The state (on/off)</param>
        /// <param name="flag">Flag or not</param>
        /// <returns></returns>
        public async Task MarkMessageAsync(long item, bool state, bool flag)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string param;
                    bool silent = false;

                    if (flag)
                    {
                        param = state ? "true" : "false";
                    }
                    else
                    {
                        param = state ? ((int)MessageStatus.Read).ToString() : ((int)MessageStatus.New).ToString();
                    }

                    string put = String.Format(flag ? YMST.c_messageboxFlagUrl : YMST.c_messageboxMarkUrl, item, param, silent.ToString());

                    TraceLog(TraceLevelPriority.Normal, "Marking message {0}", put);

                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
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
        /// Hide the message on the server
        /// </summary>
        /// <param name="item">The item id we want to hide</param>
        public async Task HideMessageAsync(long item)
        {
            try
            {
                AddPendingOp();
                TraceLog(TraceLevelPriority.Normal, "Hidding message {0}", item);
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_messageboxMarkUrl, item, ((int)MessageStatus.Hidden).ToString(), "false");
                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
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
        /// Hide a history item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task HideHistoryAsync(long item)
        {
            try
            {
                AddPendingOp();
                TraceLog(TraceLevelPriority.Normal, "Hidding missed call {0}", item);
                if (await LoginWaitAsync())
                {
                    string put = String.Format(YMST.c_historyClearMessage, item);
                    using (var response = await YouMailApiAsync(put, null, HttpMethod.Put))
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
        /// Get a message history based on a YouMailQuery
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailHistory[]>> GetMessageHistoryFromQueryAsync(YouMailQuery query)
        {
            GetUpdateResult<YouMailHistory[]> returnValue = null;
            HttpResponseMessage response = null;
            List<YouMailHistory> callList = new List<YouMailHistory>();
            DateTime lastQueryUpdated = DateTime.MinValue;
            int count = 0;
            do
            {
                count = 0;
                string queryString = query.GetQueryString();

                using (response = await YouMailApiAsync(YMST.c_messageBoxHistoryQuery + queryString, null, HttpMethod.Get))
                {
                    if (response != null)
                    {
                        var histories = DeserializeObject<YouMailHistories>(response.GetResponseStream());

                        // Get all the call histories where the result is no message left
                        if (histories != null && histories.Histories != null)
                        {
                            var messageQuery = from c in histories.Histories
                                               where c.Result != VoicemailResult.LeftMessage
                                               select c;

                            count = messageQuery.Count();
                            callList.AddRange(messageQuery);
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

            returnValue = new GetUpdateResult<YouMailHistory[]>(lastQueryUpdated, callList.ToArray());
            return returnValue;
        }


        /// <summary>
        /// Get the list of messages
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <param name="callback"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailHistory[]>> GetMessageHistoryAsync(DateTime lastUpdated, int imageSize)
        {
            GetUpdateResult<YouMailHistory[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetHistory)
                    {
                        MaxResults = int.MaxValue,
                        Offset = 0,
                        IncludeImageUrl = true,
                        UpdatedFrom = lastUpdated,
                        IncludeContactInfo = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    returnValue = await GetMessageHistoryFromQueryAsync(query);
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get a specified call
        /// </summary>
        /// <param name="messageId">The message that we want to update</param>
        /// <returns></returns>
        public async Task<YouMailHistory> GetMessageHistoryAsync(long messageId, int imageSize)
        {
            YouMailHistory returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessageHistory)
                    {
                        MaxResults = int.MaxValue,
                        Offset = 0,
                        IncludeImageUrl = true,
                        IncludeContactInfo = true,
                        IncludeLocation = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };
                    query.AddItemId(messageId);

                    var result = await GetMessageHistoryFromQueryAsync(query);
                    if (result != null && result.Data != null && result.Data.Length > 0)
                    {
                        returnValue = result.Data[0];
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
        /// Get the list of messages since we last checked
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public async Task<YouMailMessage[]> GetUpdatedMessagesSinceAsync(DateTime lastUpdated, int imageSize, DataFormat dataFormat)
        {
            YouMailMessage[] returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var query = new YouMailMessageQuery(QueryType.GetMessagesSince)
                    {
                        UpdatedFrom = lastUpdated,
                        DataFormat = dataFormat,
                        IncludePreview = true,
                        IncludeImageUrl = true,
                        IncludeContactInfo = true,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength
                    };

                    List<YouMailMessage> list = new List<YouMailMessage>();
                    int count;

                    do
                    {
                        count = 0;
                        string post = query.GetQueryString();

                        TraceLog(TraceLevelPriority.Normal, "Getting MessageList: {0}", post);

                        using (var response = await YouMailApiAsync(YMST.c_messageBoxEntryQuery + post, null, HttpMethod.Get))
                        {
                            if (response != null)
                            {
                                var messages = DeserializeObject<YouMailMessages>(response.GetResponseStream());

                                if (messages != null && messages.Messages != null)
                                {
                                    count = messages.Messages.Length;
                                    list.AddRange(messages.Messages);
                                }
                            }
                        }
                        query.Page++;
                    } while (count == query.PageLength);

                    returnValue = list.ToArray();
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get the data for the message
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public async Task<Stream> GetMessageDataAsync(string URL)
        {
            Stream returnValue = null;
            bool fRetry = false;
            YouMailException weRetry = null;
            HttpResponseMessage response = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    try
                    {
                        if (IsConnected)
                        {
                            response = await GetMessageDownloadResponseAsync(URL);
                            response.EnsureYouMailResponse();
                        }
                    }
                    catch (WebException we)
                    {
                        weRetry = we.ConvertException();
                        if (weRetry.StatusCode == HttpStatusCode.Forbidden)
                        {
                            fRetry = true;
                        }
                        else
                            throw;
                    }
                    catch (YouMailException re)
                    {
                        weRetry = re;
                        if (weRetry.StatusCode == HttpStatusCode.Forbidden)
                        {
                            fRetry = true;
                        }
                        else
                            throw;
                    }
                    if (fRetry)
                    {
                        if (await TryReauthenticationAsync(weRetry, false))
                        {
                            TraceLog(TraceLevelPriority.Normal, "Getting message: {0}", URL);
                            response = await GetMessageDownloadResponseAsync(URL);
                        }
                    }
                    if (response != null)
                    {
                        returnValue = response.GetResponseStream();
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        private async Task<HttpResponseMessage> GetMessageDownloadResponseAsync(string URL)
        {
            // Replace the http:// with the protocol selection for the user
            int index = URL.IndexOf(YMST.c_protocolToken);
            URL = _protocol + URL.Substring(index + YMST.c_protocolToken.Length);

            // Need to add the authToken to the URI
            char separator = (URL.IndexOf('?') >= 0) ? '&' : '?';
            URL += separator + "authtoken=" + AuthToken;

            TraceLog(TraceLevelPriority.Normal, "{0} {1}", HttpMethod.Get, URL);

            var response = await _httpClient.GetAsync(URL);
            return response;
        }
    }
}
