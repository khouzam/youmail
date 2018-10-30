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

    [Flags]
    public enum YouMailPushType
    {
        MWI = 0x0001, // inboxNewCount  The inbox new message count has changed without receiving a new voicemail (user listens to a voicemail in the inbox).
        MissedCall = 0x0002, // callerName, callerNumber A missed call was received.
        NewMessage = 0x0004, // inboxNewCount, callerName, callerNumber, messageId, length  New voicemail without transcription.
        NewMessageWithTranscription = 0x0008, // inboxNewCount, callerName, callerNumber, messageId, length, transcription  New voicemail with transcription.
        AdminFreeFormMessage = 0x0010, // campKey, adminType, adminMessage  Alert from the server with free form text in the alert. This is a special predefined administration message with adminType=1.
        AdminMessage = 0x0020, // campKey, adminType, adminMessage  Alert from the server for a pre-defined administration message. The request may contain other keys but that is determined by the adminType.
        MessageNotif = 0x0040, // timestamp  Voicemail message have been updated, "timestamp" is the timestamp in millisecond since epoch. Note you may receive this push AND either MWI AND/OR New Message if you register for all three. This is a silence push meant for your application only, user will by default should not be notified.
        MissedCallNotif = 0x0080, // timestamp  Missed call (history) record has been updated, "timestamp" is the timestamp in millisecond since epoch. Note you may receive this push and either Poll Message and/or Missed Call if you register for all three.
        SettingsNotif = 0x0100, // timestamp, settingType  Settings have changed, "timestamp" is the timestamp in millisecond since epoch. "settingType" is one of the following setting types: 3=contact, 4=greeting, 5=user, 6=account, 10=other settings.
        SystemEvent = 0x0200, //event, reason, displayText  System level changes such as account status or order changes. See System Events.
        SMS = 0x0400,
        MMS = 0x0800,
        IncomingExtraLine = 0x4000, // Incoming call to extra line.
        MessageDeliveryReceipt = 0x8000, // Receipt for successful delivery of messages (currently only available for sms/mms of participating carriers).
        KeepAlive = 0x10000, //System event to inspect whether the registered deviceId is still valid. This event is done once a day, and the device must acknowledge when it receives the event.
    }
}
