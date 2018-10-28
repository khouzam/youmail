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
    using System.Text;

    class YouMailGreetingQuery : YouMailQuery
    {
        public YouMailGreetingQuery(QueryType type)
            : base(type)
        {
            AddQueryItem(YMST.c_dataFormat, ((int)DataFormat.MP3).ToString());
            AddIncludeParam(YMST.c_id);
            AddIncludeParam(YMST.c_description);
            AddIncludeParam(YMST.c_duration);
            AddIncludeParam(YMST.c_imageUrl);
            AddIncludeParam(YMST.c_dataUrl);
            AddIncludeParam(YMST.c_communityGreetingId);
            AddIncludeParam(YMST.c_source);
            AddIncludeParam(YMST.c_greetingType);
        }
    }
}
