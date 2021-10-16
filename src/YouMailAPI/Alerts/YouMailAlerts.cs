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
        [JsonIgnore]
        public int _ditchedCallInt
        {
            get { return (int)DitchedCall; }
            set { DitchedCall = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonProperty(YMST.c_ditchedCallAlertEmail)]
        public bool DitchedCallEmail { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_ditchedCallAlertTxt)]
        public bool DitchedCallText { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_ditchedCallAlertPush)]
        public bool DitchedCallPush { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes DitchedCall
        {
            get
            {
                return FromNotificationTypes(DitchedCallEmail, DitchedCallEmail, DitchedCallPush);
            }
            set
            {
                DitchedCallEmail = value.HasFlag(NotificationTypes.SendEmail);
                DitchedCallText = value.HasFlag(NotificationTypes.SendText);
                DitchedCallPush = value.HasFlag(NotificationTypes.SendPush);
            }
        }

        [XmlElement(YMST.c_ditchedCallMask)]
        [JsonIgnore]
        public int _ditchedCallMaskInt
        {
            get { return (int)DitchedCallMask; }
            set { DitchedCallMask = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes DitchedCallMask { get; set; } = NotificationTypes.SendAll;

        [XmlElement(YMST.c_newSpamMessage)]
        [JsonIgnore]
        public int _newSpamMessageInt
        {
            get { return (int)SpamMessage; }
            set { SpamMessage = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonProperty(YMST.c_spamMessageAlertEmail)]
        public bool SpamMessageEmail { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_spamMessageAlertTxt)]
        public bool SpamMessageText { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_spamMessageAlertPush)]
        public bool SpamMessagePush { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes SpamMessage
        {
            get
            {
                return FromNotificationTypes(SpamMessageEmail, SpamMessageText, SpamMessagePush);
            }
            set
            {
                SpamMessageEmail = value.HasFlag(NotificationTypes.SendEmail);
                SpamMessageText = value.HasFlag(NotificationTypes.SendText);
                SpamMessagePush = value.HasFlag(NotificationTypes.SendPush);
            }
        }

        [XmlElement(YMST.c_newSpamMessageMask)]
        [JsonProperty(YMST.c_newSpamMessageMask)]
        public int _spamMessageMaskInt
        {
            get { return (int)SpamMessageMask; }
            set { SpamMessageMask = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes SpamMessageMask { get; set; } = NotificationTypes.SendAll;

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
        [JsonIgnore]
        public int _missedCallInt
        {
            get { return (int)MissedCall; }
            set { MissedCall = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonProperty(YMST.c_missedCallAlertEmail)]
        public bool MissedCallEmail { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_missedCallAlertTxt)]
        public bool MissedCallText { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_missedCallAlertPush)]
        public bool MissedCallPush { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes MissedCall
        {
            get
            {
                return FromNotificationTypes(MissedCallEmail, MissedCallText, MissedCallPush);
            }
            set
            {
                MissedCallEmail = value.HasFlag(NotificationTypes.SendEmail);
                MissedCallText = value.HasFlag(NotificationTypes.SendText);
                MissedCallPush = value.HasFlag(NotificationTypes.SendPush);
            }
        }

        [XmlElement(YMST.c_missedCallMask)]
        [JsonIgnore]
        public int _missedCallMaskInt
        {
            get { return (int)MissedCallMask; }
            set { MissedCallMask = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes MissedCallMask { get; set; } = NotificationTypes.SendAll;

        [XmlIgnore]
        [JsonProperty(YMST.c_messageAlertEmail)]
        public bool MessageEmail { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_messageAlertTxt)]
        public bool MessageText { get; set; }

        [XmlIgnore]
        [JsonProperty(YMST.c_messageAlertPush)]
        public bool MessagePush { get; set; }

        [XmlElement(YMST.c_newMessage)]
        [JsonIgnore]
        public int _newMessageInt
        {
            get { return (int)NewMessage; }
            set { NewMessage = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes NewMessage
        {
            get
            {
                return FromNotificationTypes(MessageEmail, MessageText, MessagePush);
            }
            set
            {
                MessageEmail = value.HasFlag(NotificationTypes.SendEmail);
                MessageText = value.HasFlag(NotificationTypes.SendText);
                MessagePush = value.HasFlag(NotificationTypes.SendPush);
            }
        }

        [XmlElement(YMST.c_newMessageMask)]
        [JsonIgnore]
        public int _newMessageMaskInt
        {
            get { return (int)NewMessageMask; }
            set { NewMessageMask = (NotificationTypes)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public NotificationTypes NewMessageMask { get; set; } = NotificationTypes.SendAll;

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

        private static NotificationTypes FromNotificationTypes(bool email, bool text, bool push)
        {
            return (email ? NotificationTypes.SendEmail : NotificationTypes.None) |
                (text ? NotificationTypes.SendText : NotificationTypes.None) |
                (push ? NotificationTypes.SendPush : NotificationTypes.None);
        }
    }
}
