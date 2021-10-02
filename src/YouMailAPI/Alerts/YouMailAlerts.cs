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

    /// <summary>
    /// A class representing the different alerts
    /// </summary>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_alertSettings)]
    public class YouMailAlerts
    {
        [XmlElement(YMST.c_ditchedCall)]
        [JsonProperty(YMST.c_ditchedCall)]
        public int _ditchedCallInt
        {
            get { return (int)DitchedCall; }
            set { DitchedCall = (DitchedCall)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DitchedCall DitchedCall { get; set; }

        [XmlElement(YMST.c_ditchedCallMask)]
        [JsonProperty(YMST.c_ditchedCallMask)]
        public int _ditchedCallMaskInt
        {
            get { return (int)DitchedCallMask; }
            set { DitchedCallMask = (DitchedCall)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DitchedCall DitchedCallMask { get; set; }

        [XmlElement(YMST.c_newSpamMessage)]
        [JsonProperty(YMST.c_newSpamMessage)]
        public int _newSpamMessageInt
        {
            get { return (int)SpamMessage; }
            set { SpamMessage = (SpamMessage)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public SpamMessage SpamMessage { get; set; }

        [XmlElement(YMST.c_newSpamMessageMask)]
        [JsonProperty(YMST.c_newSpamMessageMask)]
        public int _spamMessageMaskInt
        {
            get { return (int)SpamMessageMask; }
            set { SpamMessageMask = (SpamMessage)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public SpamMessage SpamMessageMask { get; set; }

        [XmlElement(YMST.c_emailAttachment)]
        [JsonProperty(YMST.c_emailAttachment)]
        public int _emailAttachmentInt
        {
            get { return (int)EmailAttachment; }
            set { EmailAttachment = (EmailAttachment)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public EmailAttachment EmailAttachment { get; set; }

        [XmlElement(YMST.c_emailCustomFormat)]
        [JsonProperty(YMST.c_emailCustomFormat)]
        public int _emailCustomFormatInt
        {
            get { return (int)EmailCustomFormat; }
            set { EmailCustomFormat = (EmailCustomFormat)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public EmailCustomFormat EmailCustomFormat { get; set; }

        [XmlElement(YMST.c_emailFormat)]
        [JsonProperty(YMST.c_emailFormat)]
        public int _emailFormatInt
        {
            get { return (int)EmailFormat; }
            set { EmailFormat = (EmailFormat)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public EmailFormat EmailFormat { get; set; }

        [XmlElement(YMST.c_missedCall)]
        [JsonProperty(YMST.c_missedCall)]
        public int _missedCallInt
        {
            get { return (int)MissedCall; }
            set { MissedCall = (MissedCall)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public MissedCall MissedCall { get; set; }

        [XmlElement(YMST.c_missedCallMask)]
        [JsonProperty(YMST.c_missedCallMask)]
        public int _missedCallMask
        {
            get { return (int)MissedCallMask; }
            set { MissedCallMask = (MissedCall)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public MissedCall MissedCallMask { get; set; }

        [XmlElement(YMST.c_newMessage)]
        [JsonProperty(YMST.c_newMessage)]
        public int _newMessageInt
        {
            get { return (int)NewMessage; }
            set { NewMessage = (NewMessage)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NewMessage NewMessage { get; set; }

        [XmlElement(YMST.c_newMessageMask)]
        [JsonProperty(YMST.c_newMessageMask)]
        public int _newMessageMaskInt
        {
            get { return (int)NewMessageMask; }
            set { NewMessageMask = (NewMessage)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NewMessage NewMessageMask { get; set; }

        [XmlElement(YMST.c_pushConditions)]
        [JsonProperty(YMST.c_pushConditions)]
        public int PushConditions { get; set; }

        [XmlArray(YMST.c_pushRegistrations)]
        [XmlArrayItem(YMST.c_pushRegistration)]
        public YouMailPushRegistration[] PushRegistrations { get; set; }

        [XmlElement(YMST.c_emailCCEnabled)]
        [JsonProperty(YMST.c_emailCCEnabled)]
        public bool EmailCCEnabled { get; set; }

        [XmlElement(YMST.c_emailCCList)]
        [JsonProperty(YMST.c_emailCCList)]
        public string EmailCCList { get; set; }

        [XmlElement(YMST.c_confirmedSmsPhone)]
        [JsonProperty(YMST.c_confirmedSmsPhone)]
        public bool ConfirmedSmsPhone { get; set; }

        [XmlElement(YMST.c_carrierSupportsSms)]
        [JsonProperty(YMST.c_carrierSupportsSms)]
        public bool CarrierSupportsSms { get; set; }
    }
}
