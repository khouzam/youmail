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
    using System.Xml.Serialization;

    public class YouMailTestCallDetails
    {
        [XmlElement(YMST.c_testCallDetails)]
        [JsonProperty(YMST.c_testCallDetails)]
        public YouMailCallVerifyStatus[] TestCallDetails;
    }

    /// <summary>
    /// A class representing the different alerts
    /// </summary>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_testCallDetail)]
    public class YouMailCallVerifyStatus
    {
        [XmlElement(YMST.c_userId)]
        [JsonProperty(YMST.c_userId)]
        public int UserId { get; set; }

        [XmlElement(YMST.c_userPhoneNumber)]
        [JsonProperty(YMST.c_userPhoneNumber)]
        public string UserPhoneNumber { get; set; }

        [XmlElement(YMST.c_outboundPhoneNumber)]
        [JsonProperty(YMST.c_outboundPhoneNumber)]
        public string OutboundPhoneNumber { get; set; }

        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public string Status { get; set; }

        [XmlElement(YMST.c_initiatedTime)]
        [JsonProperty(YMST.c_initiatedTime)]
        public long _initiatedTime
        {
            get
            {
                return InitiatedTime.ToMillisecondsFromEpoch();
            }
            set
            {
                InitiatedTime = value.FromMillisecondsFromEpoch();
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime InitiatedTime { get; set; }

        [XmlElement(YMST.c_completedTime)]
        [JsonProperty(YMST.c_completedTime)]
        public long _completedTime
        {
            get
            {
                return CompletedTime.ToMillisecondsFromEpoch();
            }
            set
            {
                CompletedTime = value.FromMillisecondsFromEpoch();
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime CompletedTime { get; set; }
    }
}
