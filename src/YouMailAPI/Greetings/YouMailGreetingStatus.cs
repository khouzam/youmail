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
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_statuses)]
    public class YouMailGreetingStatuses
    {
        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public List<YouMailGreetingStatus> Statuses;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_status)]
    public class YouMailGreetingStatus
    {
        [XmlElement(YMST.c_greetingId)]
        [JsonProperty(YMST.c_greetingId)]
        public long GreetingId { get; set; }

        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public int Id { get; set; }

        [XmlElement(YMST.c_playType)]
        [JsonProperty(YMST.c_playType)]
        public int _playTypeInt
        {
            get { return (int)PlayType; }
            set { PlayType = (YouMailPlayType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailPlayType PlayType { get; set; }

        [XmlElement(YMST.c_statusType)]
        [JsonProperty(YMST.c_statusType)]
        public int _statusTypeInt
        {
            get { return (int)StatusType; }
            set { StatusType = (YouMailStatusType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailStatusType StatusType { get; set; }

        [XmlElement(YMST.c_name)]
        [JsonProperty(YMST.c_name)]
        public string Name { get; set; }

        [XmlElement(YMST.c_system)]
        [JsonProperty(YMST.c_system)]
        public bool System { get; set; }

        [XmlElement(YMST.c_userId)]
        [JsonProperty(YMST.c_userId)]
        public int UserId { get; set; }
    }
}
