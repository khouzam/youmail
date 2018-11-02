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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_response)]
    public class YouMailTranscriptionRequestResponse
    {
        [XmlElement(YMST.c_transcriptionState)]
        public int TranscriptionStateInt
        {
            get
            {
                return (int)this.TranscriptionState;
            }
            set
            {
                this.TranscriptionState = (TranscriptionState)value;
            }
        }

        [XmlIgnore]
        public TranscriptionState TranscriptionState
        {
            get;
            set;
        }
    }

    public enum TranscriptionState
    {
        None = 0,
        Allowed = 1,
        Started = 2,
        Transcribed = 3,
        Corrected = 4,
        Retry = 5,
        RetryStarted = 6,
        InitialRequest = 7,
        Busy = 8,
        TimedOut = 9,
        Error = 10,
        Failover = 11,
        Rejected = 12,
        Spam = 51,
        ExceededAllotment = 101,
        InactivePlan = 102,
        NotInContact = 103,
        WrongContact = 104,
        UserDisabled = 106,
        ExpiredPlan = 107,
        UserPhoneRestricted = 108,
        SuspendedPlan = 109
    }

    public enum TranscriptionReason
    {
        None = 0,
        Plan = 1,
        Overage = 2,
        VmDrop = 3,
        NotClean = 10,
        Error = 11,
        InNetwork = 12,
        TooLong = 13,
        System = 14,
        Overloaded = 15
    }
}
