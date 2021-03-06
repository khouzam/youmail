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
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_customs)]
    public partial class YouMailCustoms
    {
        [XmlElement(YMST.c_custom)]
        public List<YouMailCustom> Customs;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_custom)]
    public class YouMailCustom
    {
        [XmlElement(YMST.c_key)]
        public string Key { get; set; }

        [XmlElement(YMST.c_value)]
        public string Value { get; set; }
    }


    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_response)]
    public class YouMailCustomResponse : YouMailResponse
    {
        [XmlElement(YMST.c_customs)]
        public YouMailCustoms Customs
        {
            set
            {
                Properties = new Dictionary<string, string>();
                foreach (var item in value.Customs)
                {
                    Properties.Add(item.Key, item.Value);
                }
            }
            get
            {
                YouMailCustoms customs = null;
                if (Properties != null)
                {
                    customs = new YouMailCustoms
                    {
                        Customs = new List<YouMailCustom>()
                    };
                    foreach (var item in Properties)
                    {
                        customs.Customs.Add(new YouMailCustom
                        {
                            Key = item.Key,
                            Value = item.Value
                        });
                    }
                }
                return customs;
            }
        }

        /// <summary>
        /// A Dictionary representation of the custom properties
        /// </summary>
        [XmlIgnore]
        public IDictionary<string, string> Properties { get; set; }
    }
}
