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
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_additionalPhoneNumbers)]
    public partial class YouMailPhoneNumbers
    {
        [XmlElement(YMST.c_phoneNumber)]
        public YouMailPhoneNumber[] PhoneNumbers { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_phoneNumber)]
    public class YouMailPhoneNumber
    {
        [XmlElement(YMST.c_id)]
        public long ID { get; set; }

        [XmlElement(YMST.c_phoneNumber)]
        public string PhoneNumber { get; set; }

        [XmlElement(YMST.c_nickName)]
        public string NickName { get; set; }

        [XmlElement(YMST.c_carrierId)]
        public int CarriedId { get; set; }

        [XmlElement(YMST.c_autoLoginFlag)]
        public bool AutoLoginFlag { get; set; }

        [XmlElement(YMST.c_phoneModel)]
        public string PhoneModel { get; set; }

        [XmlElement(YMST.c_phoneModelId)]
        public int PhoneModelId { get; set; }
    }
}
