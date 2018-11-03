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

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_transcriptionStatus)]
    public class YouMailTranscriptionStatus
    {
        [XmlElement(YMST.c_active)]
        [JsonProperty(YMST.c_active)]
        public bool Active { get; set; }

        [XmlElement(YMST.c_enabled)]
        [JsonProperty(YMST.c_enabled)]
        public bool Enabled { get; set; }

        [XmlElement(YMST.c_freeCount)]
        [JsonProperty(YMST.c_freeCount)]
        public int FreeCount { get; set; }

        [XmlElement(YMST.c_planMaxCount)]
        [JsonProperty(YMST.c_planMaxCount)]
        public int PlanMaxCount { get; set; }

        [XmlElement(YMST.c_planUsedCount)]
        [JsonProperty(YMST.c_planUsedCount)]
        public int PlanUsedCount { get; set; }

        [XmlElement(YMST.c_renewalDate)]
        [JsonProperty(YMST.c_renewalDate)]
        public long _planRenewalLong
        {
            get { return PlanRenewal.ToMillisecondsFromEpoch(); }
            set { PlanRenewal = value.FromMillisecondsFromEpoch(); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime PlanRenewal { get; set; }
    }
}
