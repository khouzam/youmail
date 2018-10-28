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
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_account)]
    public class YouMailUserInfo
    {
        [XmlElement(YMST.c_username)]
        public string Username { get; set; }

        [XmlElement(YMST.c_password)]
        public string Password { get; set; }

        [XmlElement(YMST.c_pin)]
        public string Pin { get; set; }

        [XmlElement(YMST.c_secretWord)]
        public string SecretWord { get; set; }

        [XmlElement(YMST.c_title)]
        public string Title { get; set; }

        [XmlElement(YMST.c_organization)]
        public string Organization { get; set; }

        [XmlElement(YMST.c_firstName)]
        public string FirstName { get; set; }

        [XmlElement(YMST.c_lastName)]
        public string LastName { get; set; }

        [XmlElement(YMST.c_emailAddress)]
        public string EmailAddress { get; set; }

        [XmlElement(YMST.c_createTime)]
        public string _createTimeString
        {
            get { return CreateTime.ToString(); }
            set
            {
                DateTime result = DateTime.Now;
                DateTime.TryParse(value, out result);
                CreateTime = result;
            }
        }

        [XmlIgnore]
        public DateTime CreateTime { get; set; }

        [XmlElement(YMST.c_primaryPhoneNumber)]
        public string PrimaryPhoneNumber { get; set; }

        [XmlElement(YMST.c_primaryPhoneNickname)]
        public string PrimaryPhoneNickname { get; set; }

        [XmlElement(YMST.c_zipCode)]
        public string ZipCode { get; set; }

        [XmlElement(YMST.c_primaryPhoneId)]
        public long PrimaryPhoneId { get; set; }

        [XmlElement(YMST.c_city)]
        public string City { get; set; }

        [XmlElement(YMST.c_state)]
        public string State { get; set; }

        [XmlElement(YMST.c_country)]
        public string Country { get; set; }

        [XmlElement(YMST.c_gender)]
        public string Gender { get; set; }

        [XmlElement(YMST.c_aolHandle)]
        public string AolHandle { get; set; }

        [XmlElement(YMST.c_googleHandle)]
        public string GoogleHandle { get; set; }

        [XmlElement(YMST.c_yahooHandle)]
        public string YahooHandle { get; set; }

        [XmlElement(YMST.c_msnHandle)]
        public string MsnHandle { get; set; }

        [XmlElement(YMST.c_description)]
        public string Description { get; set; }

        [XmlElement(YMST.c_status)]
        public int Status { get; set; }

        [XmlElement(YMST.c_carrierId)]
        public int CarrierId { get; set; }

        [XmlElement(YMST.c_carrierHint)]
        public string CarrierHint { get; set; }

        [XmlElement(YMST.c_timeZone)]
        public string TimeZone { get; set; }

        [XmlElement(YMST.c_website)]
        public string Website { get; set; }

        [XmlElement(YMST.c_phoneModel)]
        public string PhoneModel { get; set; }

        [XmlElement(YMST.c_phoneModelId)]
        public int PhoneModelId { get; set; }

        [XmlElement(YMST.c_autoLoginFlag)]
        public bool AutoLoginFlag { get; set; }

        [XmlElement(YMST.c_emailFormat)]
        public string EmailFormat { get; set; }

        [XmlElement(YMST.c_emailAttachmentType)]
        public string EmailAttachmentType { get; set; }

        [XmlElement(YMST.c_additionalPhoneNumbers)]
        public YouMailPhoneNumbers AdditionalPhoneNumbers;
    }
}
