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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_entries)]
    public partial class YouMailMessages
    {
        [XmlElement(YMST.c_entry)]
        [JsonProperty(YMST.c_entry)]
        public YouMailMessage[] Messages;
    }


    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "response")]
    public partial class YouMailMessageResponse
    {
        [XmlElement(YMST.c_entry)]
        [JsonProperty(YMST.c_entry)]
        public YouMailMessage Message { get; set; }
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_entry)]
    public partial class YouMailMessage : YouMailMessageBase
    {
        [XmlIgnore]
        [JsonIgnore]
        public MessageStatus Status { get; set; }

        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public int _statusInt
        {
            get { return (int)Status; }
            set { Status = (MessageStatus)value; }
        }

        [XmlElement(YMST.c_folderId)]
        [JsonProperty(YMST.c_folderId)]
        public int FolderId { get; set; }

        [XmlElement(YMST.c_messageDataUrl)]
        [JsonProperty(YMST.c_messageDataUrl)]
        public string MessageDataUrl { get; set; }

        [XmlElement(YMST.c_transcriptionState)]
        [JsonProperty(YMST.c_transcriptionState)]
        public int _transcriptionStateInt
        {
            get { return (int)TranscriptionState; }
            set { TranscriptionState = (TranscriptionState)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public TranscriptionState TranscriptionState { get; set; }

        [XmlElement(YMST.c_flagged)]
        [JsonProperty(YMST.c_flagged)]
        public bool Flagged { get; set; }

        [XmlElement(YMST.c_transcript)]
        [JsonProperty(YMST.c_transcript)]
        public string Transcription { get; set; }

        [XmlElement(YMST.c_preview)]
        [JsonProperty(YMST.c_preview)]
        public string Preview { get; set; }

        public override bool IsSpecialItem
        {
            get { return FolderId < 0; }
        }
    }
}
