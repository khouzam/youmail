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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_response)]
    public class YouMailGreetingsResponse : YouMailResponse
    {
        [XmlElement(YMST.c_greetings)]
        [JsonProperty(YMST.c_greetings)]
        public YouMailGreeting[] Greetings { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_greetings)]
    public partial class YouMailGreetings
    {
        [XmlElement(YMST.c_greeting)]
        [JsonProperty(YMST.c_greetings)]
        public YouMailGreeting[] Greetings { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_greeting)]
    public partial class YouMailGreeting
    {
        [XmlElement(YMST.c_name)]
        [JsonProperty(YMST.c_name)]
        public string Name { get; set; }

        [XmlElement(YMST.c_description)]
        [JsonProperty(YMST.c_description)]
        public string Description { get; set; }

        [XmlElement(YMST.c_duration)]
        [JsonProperty(YMST.c_duration)]
        public int ItemDuration { get; set; }

        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public int Id { get; set; }

        [XmlElement(YMST.c_imageUrl)]
        [JsonProperty(YMST.c_imageUrl)]
        public string ImageUrl { get; set; }

        [XmlElement(YMST.c_communityGreetingId)]
        [JsonProperty(YMST.c_communityGreetingId)]
        public int CommunityGreetingId { get; set; }

        [XmlElement(YMST.c_source)]
        [JsonProperty(YMST.c_source)]
        public int _sourceInt
        {
            get { return (int)Source; }
            set { Source = (YouMailGreetingSource)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailGreetingSource Source { get; set; }

        [XmlElement(YMST.c_dataUrl)]
        [JsonProperty(YMST.c_dataUrl)]
        public string DataUrl { get; set; }

        [XmlElement(YMST.c_audioData)]
        [JsonProperty(YMST.c_audioData)]
        public string AudioData { get; set; }

        [XmlElement(YMST.c_greetingType)]
        [JsonProperty(YMST.c_greetingType)]
        public int _greetingTypeInt
        {
            get { return (int)GreetingType; }
            set { GreetingType = (YouMailGreetingType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailGreetingType GreetingType { get; set; }
    }
}
