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
    using System.Runtime.Serialization;

    [DataContract]
    public class YouMailPushNotificationData
    {
        public YouMailPushType PushType { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "Type")]
        public int _pushTypeInt
        {
            get { return (int)PushType; }
            set { PushType = (YouMailPushType)value; }
        }

        [DataMember]
        public int InboxNewCount { get; set; }

        [DataMember]
        public string CallerName { get; set; }

        [DataMember]
        public string CallerNumber { get; set; }

        [DataMember]
        public long Length { get; set; }

        [DataMember]
        public long MessageId { get; set; }

        [DataMember]
        public string Transcription { get; set; }

        [DataMember]
        public int AdminType { get; set; }

        [DataMember]
        public string CampKey { get; set; }

        [DataMember]
        public string AdminMessage { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "Timestamp")]
        public long _timestamp
        {
            get { return Timestamp.ToMillisecondsFromEpoch(); }
            set { Timestamp = value.FromMillisecondsFromEpoch(); }
        }

        public DateTime Timestamp { get; set; }

        [DataMember]
        public string SettingType { get; set; }

        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public string Reason { get; set; }

        [DataMember]
        public string DisplayText { get; set; }

        [DataMember]
        public int MessageType { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public long PushId { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "HistoryResult")]
        public int _historyResultInt
        {
            get { return (int)HistoryResult; }
            set { HistoryResult = (VoicemailResult)value; }
        }

        public VoicemailResult HistoryResult { get; set; }

        #region IncomingExtraLine
        [DataMember]
        [JsonProperty(PropertyName = "destination-number-formatted")]
        public string DestinationNumberFormatted { get; set; }

        [DataMember]
        [JsonProperty(PropertyName = "destination-name")]
        public string DestinationName { get; set; }
        #endregion

        #region Flags
        public bool IsMissedCallNotification
        {
            get
            {
                return PushType.HasFlag(YouMailPushType.MissedCall) ||
                    PushType.HasFlag(YouMailPushType.MissedCallNotif);
            }
        }

        public bool IsVoicemailNotification
        {
            get
            {
                return PushType.HasFlag(YouMailPushType.NewMessage) ||
                    PushType.HasFlag(YouMailPushType.NewMessageWithTranscription);
            }
        }

        public bool IsMWINotification
        {
            get { return PushType.HasFlag(YouMailPushType.MWI); }
        }

        public bool IsAdminNotification
        {
            get { return PushType.HasFlag(YouMailPushType.AdminMessage); }
        }

        public bool IsSystemNotification
        {
            get { return PushType.HasFlag(YouMailPushType.SystemEvent); }
        }

        public bool IsIncomingExtraLineNotification
        {
            get { return PushType.HasFlag(YouMailPushType.IncomingExtraLine); }
        }
        #endregion
    }
}
