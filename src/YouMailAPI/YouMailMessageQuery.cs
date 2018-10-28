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
    using System.Text;

    public class YouMailMessageQuery : YouMailQuery
    {
        public YouMailMessageQuery(QueryType type)
            : base(type)
        {
            FolderId = -1;
            IncludeFullName = true;
            IncludeExtraInfo = true;
            IncludeImageUrl = false;
            IncludePreview = false;
            IncludeContactInfo = false;
            IncludeLocation = false;
            DataFormat = DataFormat.MP3;
            switch (type)
            {
                case QueryType.GetMessages:
                    AddQueryItem(YMST.c_folderId, FolderId.ToString());
                    AddQueryItem(YMST.c_maxResult, MaxResults.ToString());
                    AddQueryItem(YMST.c_dataFormat, ((int)DataFormat).ToString());
                    AddDefaultMessageParams();
                    break;

                case QueryType.GetMessage:
                    AddQueryItem(YMST.c_dataFormat, ((int)DataFormat).ToString());
                    AddDefaultMessageParams();
                    break;

                case QueryType.GetMessagesSince:
                    // Add the updated from field
                    AddQueryItem(YMST.c_dataFormat, ((int)DataFormat).ToString());
                    AddQueryItem(YMST.c_folderId, FolderId.ToString());
                    AddQueryItem(YMST.c_deleteType, ((int)YouMailMessageVisibility.NoRestriction).ToString());
                    AddDefaultMessageParams();
                    break;

                case QueryType.GetMessagesCreatedSince:
                    // Use the updated from field for the CreatedFrom
                    AddQueryItem(YMST.c_dataFormat, ((int)DataFormat).ToString());
                    AddQueryItem(YMST.c_folderId, FolderId.ToString());
                    AddQueryItem(YMST.c_deleteType, ((int)YouMailMessageVisibility.NoRestriction).ToString());
                    break;

                case QueryType.GetMessageHistory:
                case QueryType.GetHistory:
                    AddDefaultHistoryParams();
                    break;
            }
        }

        private static int[] _sizes = { 50, 64, 100, 200, 400, 480, 800, 960 };
        private int FindBestMatch(int size)
        {
            int requestSize = _sizes[_sizes.Length - 1];
            for (int i = 0; i < _sizes.Length; i++)
            {
                if (_sizes[i] >= size)
                {
                    requestSize = _sizes[i];
                    break;
                }
            }

            return requestSize;
        }

        protected override void PrepareQuery()
        {
            switch (QueryType)
            {
                case QueryType.GetMessages:
                case QueryType.GetMessagesSince:
                    AddQueryItem(YMST.c_updatedFrom, UpdatedFrom.ToMillisecondsFromEpoch().ToString());
                    break;

                case QueryType.GetMessagesCreatedSince:
                    AddQueryItem(YMST.c_createdFrom, UpdatedFrom.ToMillisecondsFromEpoch().ToString());
                    break;

                case QueryType.GetHistory:
                    AddQueryItem(YMST.c_startDate, UpdatedFrom.ToMillisecondsFromEpoch().ToString());
                    break;
            }
        }

        private void AddDefaultMessageParams()
        {
            IncludeExtraInfo = true;
            AddIncludeParam(YMST.c_id);
            AddIncludeParam(YMST.c_source);
            AddIncludeParam(YMST.c_status);
            AddIncludeParam(YMST.c_length);
            AddIncludeParam(YMST.c_created);
            AddIncludeParam(YMST.c_destination);
            AddIncludeParam(YMST.c_folderId);
            AddIncludeParam(YMST.c_messageDataUrl);
            AddIncludeParam(YMST.c_createSource);
        }

        private void AddDefaultHistoryParams()
        {
            IncludeExtraInfo = false;
            AddIncludeParam(YMST.c_id);
            AddIncludeParam(YMST.c_source);
            AddIncludeParam(YMST.c_created);
            AddIncludeParam(YMST.c_destination);
            AddIncludeParam(YMST.c_createSource);
        }

        public bool IncludePreview
        {
            get { return HasIncludeParam(YMST.c_preview); }
            set { SetIncludeParam(YMST.c_preview, value); }
        }

        public bool IncludeFullName
        {
            get { return HasIncludeParam(YMST.c_includeFullCallerName); }
            set { SetIncludeParam(YMST.c_callerName, value); }
        }

        public bool IncludeExtraInfo
        {
            get { return HasIncludeParam(YMST.c_flagged); }
            set { SetIncludeParam(YMST.c_flagged, value); }
        }

        public bool IncludeImageUrl
        {
            get { return HasIncludeParam(YMST.c_imageUrl); }
            set { SetIncludeParam(YMST.c_imageUrl, value); }
        }

        public int ImageSize
        {
            get
            {
                var value = GetQueryItem(YMST.c_imageSize);
                int.TryParse(value, out int result);
                return result;
            }
            set
            {
                if (value != 0)
                {
                    // Force including image url if we set an imagesize
                    IncludeImageUrl = true;
                    int size = FindBestMatch(value);
                    AddQueryItem(YMST.c_imageSize, size.ToString());
                }
                else
                {
                    RemoveQueryItem(YMST.c_imageSize);
                }
            }
        }

        public int FolderId
        {
            get
            {
                var item = GetQueryItem(YMST.c_folderId);
                return int.Parse(item);
            }
            set { AddQueryItem(YMST.c_folderId, value.ToString()); }
        }

        public DataFormat DataFormat
        {
            get
            {
                var item = GetQueryItem(YMST.c_dataFormat);
                return (DataFormat)int.Parse(item);
            }
            set { AddQueryItem(YMST.c_dataFormat, ((int)value).ToString()); }
        }

        public MessageStatus MessageStatusFilter
        {
            get
            {
                var item = GetQueryItem(YMST.c_status);
                return (MessageStatus)int.Parse(item);
            }
            set { AddQueryItem(YMST.c_status, ((int)value).ToString()); }
        }

        public bool IncludeLocation
        {
            get { return HasIncludeParam(YMST.c_city); }
            set
            {
                SetIncludeParam(YMST.c_city, value);
                SetIncludeParam(YMST.c_countryState, value);
            }
        }

        public bool IncludeContactInfo
        {
            get { return HasIncludeParam(YMST.c_phonebookSourceType); }
            set
            {
                SetIncludeParam(YMST.c_phonebookSourceType, value);
                SetIncludeParam(YMST.c_phonebookSourceId, value);
            }
        }
    }
}
