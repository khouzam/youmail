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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_contactSyncSummary)]
    public class YouMailContactsUploadStatus
    {
        public enum StatusEnum
        {
            None = 1,
            Pending = 2,
            Started = 3,
            Ended = 4,
            Error = 5
        }

        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public long Id { get; set; }

        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public int _statusInt
        {
            get { return (int)Status; }
            set { Status = (StatusEnum)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public StatusEnum Status { get; set; }


        [XmlElement(YMST.c_clientRefId)]
        [JsonProperty(YMST.c_clientRefId)]
        public string ClientRefId { get; set; }

        [XmlElement(YMST.c_deletedCount)]
        [JsonProperty(YMST.c_deletedCount)]
        public int DeletedCount { get; set; }

        [XmlElement(YMST.c_errorCount)]
        [JsonProperty(YMST.c_errorCount)]
        public int ErrorCount { get; set; }

        [XmlElement(YMST.c_exactMatchCount)]
        [JsonProperty(YMST.c_exactMatchCount)]
        public int ExactMatchCount { get; set; }

        [XmlElement(YMST.c_expired)]
        [JsonProperty(YMST.c_expired)]
        public bool Expired { get; set; }

        [XmlElement(YMST.c_ignoredCount)]
        [JsonProperty(YMST.c_ignoredCount)]
        public int IgnoredCount { get; set; }

        [XmlElement(YMST.c_importingTotal)]
        [JsonProperty(YMST.c_importingTotal)]
        public int ImportingTotal { get; set; }

        [XmlElement(YMST.c_mergedCount)]
        [JsonProperty(YMST.c_mergedCount)]
        public int MergedCount { get; set; }

        [XmlElement(YMST.c_newCount)]
        [JsonProperty(YMST.c_newCount)]
        public int NewCount { get; set; }

        [XmlElement(YMST.c_processed)]
        [JsonProperty(YMST.c_processed)]
        public int Processed { get; set; }

        [XmlElement(YMST.c_ymTotal)]
        [JsonProperty(YMST.c_ymTotal)]
        public int YMTotal { get; set; }
    }
}
