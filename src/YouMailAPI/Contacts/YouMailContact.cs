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
    using System;
    using System.IO;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_response)]
    public partial class YouMailContactsResponse : YouMailResponse
    {
        [XmlElement(YMST.c_contacts)]
        [JsonProperty(YMST.c_contacts)]
        public YouMailContacts Contacts;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_contacts)]
    public partial class YouMailContacts
    {
        [XmlElement(YMST.c_contact)]
        [JsonProperty(YMST.c_contact)]
        public YouMailContact[] Contacts;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_contact)]
    public class YouMailContact
    {
        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public long Id { get; set; }

        [XmlElement(YMST.c_firstName)]
        [JsonProperty(YMST.c_firstName)]
        public string FirstName { get; set; }

        [XmlElement(YMST.c_middleName)]
        [JsonProperty(YMST.c_middleName)]
        public string MiddleName { get; set; }

        [XmlElement(YMST.c_lastName)]
        [JsonProperty(YMST.c_lastName)]
        public string LastName { get; set; }

        [XmlElement(YMST.c_displayName)]
        [JsonProperty(YMST.c_displayName)]
        public string DisplayName { get; set; }

        [XmlElement(YMST.c_organization)]
        [JsonProperty(YMST.c_organization)]
        public string Organization { get; set; }

        [XmlElement(YMST.c_emailAddress)]
        [JsonProperty(YMST.c_emailAddress)]
        public string EmailAddress { get; set; }

        [XmlElement(YMST.c_homeNumber)]
        [JsonProperty(YMST.c_homeNumber)]
        public string HomeNumber { get; set; }

        [XmlElement(YMST.c_workNumber)]
        [JsonProperty(YMST.c_workNumber)]
        public string WorkNumber { get; set; }

        [XmlElement(YMST.c_mobileNumber)]
        [JsonProperty(YMST.c_mobileNumber)]
        public string MobileNumber { get; set; }

        [XmlElement(YMST.c_pagerNumber)]
        [JsonProperty(YMST.c_pagerNumber)]
        public string PagerNumber { get; set; }

        [XmlElement(YMST.c_otherNumber1)]
        [JsonProperty(YMST.c_otherNumber1)]
        public string OtherNumber1 { get; set; }

        [XmlElement(YMST.c_otherNumber2)]
        [JsonProperty(YMST.c_otherNumber2)]
        public string OtherNumber2 { get; set; }

        [XmlElement(YMST.c_otherNumber3)]
        [JsonProperty(YMST.c_otherNumber3)]
        public string OtherNumber3 { get; set; }

        [XmlElement(YMST.c_otherNumber4)]
        [JsonProperty(YMST.c_otherNumber4)]
        public string OtherNumber4 { get; set; }

        [XmlElement(YMST.c_blocked)]
        [JsonProperty(YMST.c_blocked)]
        public bool Blocked { get; set; }

        [XmlElement(YMST.c_street)]
        [JsonProperty(YMST.c_street)]
        public string Street { get; set; }

        [XmlElement(YMST.c_city)]
        [JsonProperty(YMST.c_city)]
        public string City { get; set; }

        [XmlElement(YMST.c_state)]
        [JsonProperty(YMST.c_state)]
        public string State { get; set; }

        [XmlElement(YMST.c_zipCode)]
        [JsonProperty(YMST.c_zipCode)]
        public string ZipCode { get; set; }

        [XmlElement(YMST.c_country)]
        [JsonProperty(YMST.c_country)]
        public string Country { get; set; }

        [XmlElement(YMST.c_imageUrl)]
        [JsonProperty(YMST.c_imageUrl)]
        public string Image { get; set; }

        [XmlElement(YMST.c_avatarData)]
        [JsonProperty(YMST.c_avatarData)]
        public string _avatarString
        {
            get
            {
                // Convert the stream to based64 for upload
                string avatarString = null;
                if (AvatarData != null)
                {
                    byte[] array = new byte[AvatarData.Length];
                    AvatarData.Read(array, 0, (int)AvatarData.Length);
                    avatarString = Convert.ToBase64String(array);
                }
                return avatarString;
            }
            set { AvatarData = null; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public Stream AvatarData { get; set; }

        [XmlElement(YMST.c_deleted)]
        [JsonProperty(YMST.c_deleted)]
        public bool Deleted { get; set; }

        [XmlElement(YMST.c_greetingId)]
        [JsonProperty(YMST.c_greetingId)]
        public long GreetingId { get; set; }

        [XmlElement(YMST.c_actionType)]
        [JsonProperty(YMST.c_actionType)]
        public int _actionTypeInt
        {
            get { return (int)ActionType; }
            set { ActionType = (YouMailActionType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailActionType ActionType { get; set; }

        [XmlElement(YMST.c_targetUserId)]
        [JsonProperty(YMST.c_targetUserId)]
        public int TargetUserId { get; set; }

        [XmlElement(YMST.c_inviteType)]
        [JsonProperty(YMST.c_inviteType)]
        public int _inviteTypeInt { get; set; }

        [XmlElement(YMST.c_contactType)]
        [JsonProperty(YMST.c_contactType)]
        public int _contactTypeInt
        {
            get { return (int)ContactType; }
            set { ContactType = (YouMailContactType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailContactType ContactType { get; set; }

        [XmlElement(YMST.c_group)]
        [JsonProperty(YMST.c_group)]
        public bool Group { get; set; }

        [XmlElement(YMST.c_playGroup)]
        [JsonProperty(YMST.c_playGroup)]
        public bool PlayGroup { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool HasPhoneNumber
        {
            get
            {
                return !(String.IsNullOrEmpty(HomeNumber) &&
                        String.IsNullOrEmpty(WorkNumber) &&
                        String.IsNullOrEmpty(MobileNumber) &&
                        String.IsNullOrEmpty(PagerNumber) &&
                        String.IsNullOrEmpty(OtherNumber1) &&
                        String.IsNullOrEmpty(OtherNumber2) &&
                        String.IsNullOrEmpty(OtherNumber3) &&
                        String.IsNullOrEmpty(OtherNumber4)
                        );
            }
        }
    }
}
