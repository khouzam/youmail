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
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_phoneRecords)]
    public partial class YouMailPhoneRecords
    {
        [XmlElement(YMST.c_phoneRecord)]
        [JsonProperty(YMST.c_phoneRecord)]
        public List<YouMailPhoneRecord> PhoneRecords;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_phoneRecord)]
    public class YouMailPhoneRecord
    {
        [XmlElement(YMST.c_displayName)]
        [JsonProperty(YMST.c_displayName)]
        public string DisplayName { get; set; }

        [XmlElement(YMST.c_displayImageSource)]
        [JsonProperty(YMST.c_displayImageSource)]
        public string DisplayImageSource { get; set; }

        [XmlElement(YMST.c_displayImageMedium)]
        [JsonProperty(YMST.c_displayImageMedium)]
        public string DisplayImageMedium { get; set; }

        [XmlElement(YMST.c_displayImageLarge)]
        [JsonProperty(YMST.c_displayImageLarge)]
        public string DisplayImageLarge { get; set; }

        [XmlElement(YMST.c_displayImageExtraLarge)]
        [JsonProperty(YMST.c_displayImageExtraLarge)]
        public string DisplayImageExtraLarge { get; set; }
    }
}
