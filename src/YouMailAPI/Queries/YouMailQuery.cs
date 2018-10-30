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
    using System.Text;

    public enum QueryType
    {
        GetMessages,
        GetMessage,
        GetMessagesSince,
        GetMessagesCreatedSince,
        GetHistory,
        GetMessageHistory,
        GetContact,
        GetContacts,
        GetGreetings,
    }

    public abstract class YouMailQuery
    {
        public YouMailQuery(QueryType type)
        {
            _queryValues = new Dictionary<string, string>();
            _includeParams = new HashSet<string>();
            _itemIds = new List<long>();
            QueryType = type;
            MaxResults = int.MaxValue;
            Offset = 0;
            PageLength = 100;
        }

        /// <summary>
        /// Method allowing derived classes to update state right before the query is issued.
        /// </summary>
        protected virtual void PrepareQuery() { }

        /// <summary>
        /// Get the QueryString
        /// </summary>
        /// <returns></returns>
        public string GetQueryString()
        {
            PrepareQuery();
            StringBuilder sb = new StringBuilder(1024);
            bool firstItem = true;
            foreach (var element in _queryValues)
            {
                AddQueryItem(sb, firstItem, element.Key, element.Value);
                firstItem = false;
            }

            BuildIncludeParams(sb, firstItem);
            firstItem = false;

            IncludeItems(sb, firstItem);
            firstItem = false;

            return sb.ToString();
        }

        protected bool HasIncludeParam(string includeParam)
        {
            return _includeParams.Contains(includeParam);
        }

        protected void SetIncludeParam(string includeParam, bool value)
        {
            if (value)
            {
                AddIncludeParam(includeParam);
            }
            else
            {
                RemoveIncludeParam(includeParam);
            }
        }

        /// <summary>
        /// Add an item to the include list
        /// </summary>
        /// <param name="includeParam"></param>
        protected void AddIncludeParam(string includeParam)
        {
            if (!_includeParams.Contains(includeParam))
            {
                _includeParams.Add(includeParam);
            }
        }

        protected void RemoveIncludeParam(string includeParam)
        {
            if (_includeParams.Contains(includeParam))
            {
                _includeParams.Remove(includeParam);
            }
        }

        protected void AddStatusQueryItem(string item, MessageStatus? value)
        {
            if (value.HasValue)
            {
                AddQueryItem(item, ((int)value.Value).ToString());
            }
        }

        protected void AddQueryItem(string item, string value)
        {
            // Overwrite the old item if there is one
            RemoveQueryItem(item);
            _queryValues.Add(item, value);
        }

        protected void RemoveQueryItem(string item)
        {
            if (_queryValues.ContainsKey(item))
            {
                _queryValues.Remove(item);
            }
        }

        protected string GetQueryItem(string key)
        {
            _queryValues.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// Add a Query item to the Query string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="item"></param>
        /// <param name="value"></param>
        protected void AddQueryItem(StringBuilder sb, bool firstItem, string item, string value)
        {
            // Add the query sympbol
            if (firstItem)
            {
                sb.Append('?');
            }
            else
            {
                sb.Append('&');
            }

            // Add the query param
            sb.Append(item);

            // Add the query value
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append("=" + value);
            }
        }

        public int MaxResults
        {
            get
            {
                var value = GetQueryItem(YMST.c_maxResult);
                int.TryParse(value, out int result);
                return result;
            }
            set { AddQueryItem(YMST.c_maxResult, value.ToString()); }
        }

        public int Offset
        {
            get
            {
                var value = GetQueryItem(YMST.c_offset);
                int.TryParse(value, out int result);
                return result;
            }
            set { AddQueryItem(YMST.c_offset, value.ToString()); }
        }


        public DateTime UpdatedFrom { get; set; }

        public int PageLength
        {
            get
            {
                var value = GetQueryItem(YMST.c_pageLength);
                int.TryParse(value, out int result);
                return result;
            }
            set { AddQueryItem(YMST.c_pageLength, value.ToString()); }
        }

        public int Page
        {
            get
            {
                var value = GetQueryItem(YMST.c_pageNumber);
                int.TryParse(value, out int result);
                return result;
            }
            set { AddQueryItem(YMST.c_pageNumber, value.ToString()); }
        }

        protected QueryType QueryType { get; private set; }

        public void AddItemId(long item)
        {
            _itemIds.Add(item);
        }

        public void SetItemIds(List<long> items)
        {
            _itemIds = items;
        }

        protected void BuildIncludeParams(StringBuilder sb, bool firstItem)
        {
            if (_includeParams.Count != 0)
            {
                var sbIncludeParams = new StringBuilder(512);
                bool first = true;
                foreach (var param in _includeParams)
                {
                    if (!first)
                    {
                        sbIncludeParams.Append(',');
                    }
                    sbIncludeParams.Append(param);
                    first = false;
                }
                AddQueryItem(sb, firstItem, YMST.c_includeList, sbIncludeParams.ToString());
            }
        }

        protected void IncludeItems(StringBuilder sb, bool firstItem)
        {
            if (_itemIds.Count != 0)
            {
                var sbItems = new StringBuilder(_itemIds.Count * 10);
                sbItems.Append(_itemIds[0]);
                for (int i = 1; i < _itemIds.Count; i++)
                {
                    sbItems.Append(',');
                    sbItems.Append(_itemIds[i]);
                }
                AddQueryItem(sb, firstItem, YMST.c_ids, sbItems.ToString());
            }
        }

        protected List<long> _itemIds;
        protected Dictionary<string, string> _queryValues;
        protected HashSet<string> _includeParams;
    }
}
