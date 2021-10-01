/*************************************************************************************************
 * Copyright (c) 2021 Gilles Khouzam
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
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_error)]
    public class YouMailError
    {
        [XmlElement(YMST.c_errorCode)]
        [JsonProperty(YMST.c_errorCode)]
        public string ErrorCode;

        [XmlElement(YMST.c_longMessage)]
        [JsonProperty(YMST.c_longMessage)]
        public string LongMessage;

        [XmlElement(YMST.c_shortMessage)]
        [JsonProperty(YMST.c_shortMessage)]
        public string ShortMessage;
    }

    /// <summary>
    /// A class representing an error returned by a YouMail API
    /// </summary>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_response)]
    public class YouMailAPIError
    {
        [XmlElement(YMST.c_statusCode)]
        [JsonProperty(YMST.c_statusCode)]
        public int StatusCode;

        [XmlArray(YMST.c_errors)]
        [XmlArrayItem(YMST.c_error)]
        [JsonProperty(YMST.c_errors)]
        public YouMailError[] Errors;

        public string GetErrorMessage()
        {
            foreach (var error in Errors)
            {
                if (!string.IsNullOrEmpty(error.LongMessage))
                {
                    return error.LongMessage;
                }
                else if (!string.IsNullOrEmpty(error.ShortMessage))
                {
                    return error.ShortMessage;
                }
            }
            return null;
        }
    }
}
