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

    class YouMailContactQuery : YouMailQuery
    {
        public YouMailContactType ContactType { get; set; }

        public int ImageSize { get; set; }

        public YouMailContactQuery(QueryType type)
            : base(type)
        {
            ContactType = YouMailContactType.NoRestriction;
            AddQueryItem(YMST.c_groupType, ((int)YouMailGroupType.NoRestriction).ToString());
            AddQueryItem(YMST.c_contactType, ((int)ContactType).ToString());
            AddQueryItem(YMST.c_imageSize, FindBestMatch(ImageSize).ToString());

            AddIncludeParam(YMST.c_firstName);
            AddIncludeParam(YMST.c_middleName);
            AddIncludeParam(YMST.c_lastName);
            AddIncludeParam(YMST.c_displayName);
            AddIncludeParam(YMST.c_organization);
            AddIncludeParam(YMST.c_avatarId);
            AddIncludeParam(YMST.c_imageUrl);
            AddIncludeParam(YMST.c_homeNumber);
            AddIncludeParam(YMST.c_mobileNumber);
            AddIncludeParam(YMST.c_workNumber);
            AddIncludeParam(YMST.c_pagerNumber);
            AddIncludeParam(YMST.c_otherNumber1);
            AddIncludeParam(YMST.c_otherNumber2);
            AddIncludeParam(YMST.c_otherNumber3);
            AddIncludeParam(YMST.c_otherNumber4);
            AddIncludeParam(YMST.c_emailAddress);
            AddIncludeParam(YMST.c_greetingId);
            AddIncludeParam(YMST.c_actionType);
            AddIncludeParam(YMST.c_deleted);
            AddIncludeParam(YMST.c_street);
            AddIncludeParam(YMST.c_city);
            AddIncludeParam(YMST.c_state);
            AddIncludeParam(YMST.c_country);
            AddIncludeParam(YMST.c_zipCode);
            AddIncludeParam(YMST.c_contactType);
        }

        protected override void PrepareQuery()
        {
            base.PrepareQuery();
            // check for deleted contacts only when we have a real date
            if (UpdatedFrom != DateTime.MinValue)
            {
                AddQueryItem(YMST.c_updatedFrom, UpdatedFrom.ToMillisecondsFromEpoch().ToString());
                AddQueryItem(YMST.c_deleteType, ((int)YouMailDeleteType.NoRestriction).ToString());
            }
            else
            {
                RemoveQueryItem(YMST.c_updatedFrom);
                RemoveQueryItem(YMST.c_deleteType);
            }
        }

        static int[] _sizes = { 50, 100, 200 };

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
    }
}
