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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_folders)]
    public class YouMailFolders
    {
        [XmlElement(YMST.c_folder)]
        [JsonProperty(YMST.c_folders)]
        public YouMailFolder[] Folders;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_folder)]
    public class YouMailFolder
    {
        [XmlElement(YMST.c_name)]
        [JsonProperty(YMST.c_name)]
        public string Name { get; set; }

        [XmlElement(YMST.c_description)]
        [JsonProperty(YMST.c_description)]
        public string Description { get; set; }

        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public int Id { get; set; }

        [XmlElement(YMST.c_newEntryCount)]
        [JsonProperty(YMST.c_newEntryCount)]
        public int UnreadCount { get; set; }

        [XmlElement(YMST.c_ackEntryCount)]
        [JsonProperty(YMST.c_ackEntryCount)]
        public int AckCount { get; set; }

        [XmlElement(YMST.c_visibleEntryCount)]
        [JsonProperty(YMST.c_visibleEntryCount)]
        public int VisibleCount { get; set; }

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(Name) && Id >= 0);
        }
    }
}
