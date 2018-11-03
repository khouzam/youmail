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
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_account)]
    public class YouMailAccount
    {
        public YouMailAccount()
        {
            PhoneModel = YMST.c_phoneModelWP7;
        }

        [XmlElement(YMST.c_phone)]
        [JsonProperty(YMST.c_phone)]
        public string Phone { get; set; }

        [XmlElement(YMST.c_password)]
        [JsonProperty(YMST.c_password)]
        public string Password { get; set; }

        [XmlElement(YMST.c_firstName)]
        [JsonProperty(YMST.c_firstName)]
        public string FirstName { get; set; }

        [XmlElement(YMST.c_lastName)]
        [JsonProperty(YMST.c_lastName)]
        public string LastName { get; set; }

        [XmlElement(YMST.c_email)]
        [JsonProperty(YMST.c_email)]
        public string Email { get; set; }

        [XmlElement(YMST.c_phoneModel)]
        [JsonProperty(YMST.c_phoneModel)]
        public string PhoneModel { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string Username
        {
            get { return FirstName + LastName; }
        }

        [XmlElement(YMST.c_accountTemplate)]
        [JsonProperty(YMST.c_accountTemplate)]
        public string AccountTemplate { get; set; }
    }
}
