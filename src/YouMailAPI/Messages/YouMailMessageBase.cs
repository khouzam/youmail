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

    public partial class YouMailMessageBase
    {
        [XmlElement(YMST.c_source)]
        [JsonProperty(YMST.c_source)]
        public string Source { get; set; }

        [XmlElement(YMST.c_destination)]
        [JsonProperty(YMST.c_destination)]
        public string Destination { get; set; }

        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public long Id { get; set; }

        [XmlElement(YMST.c_length)]
        [JsonProperty(YMST.c_length)]
        public long LengthInMS { get; set; }

        [XmlElement(YMST.c_callerName)]
        [JsonProperty(YMST.c_callerName)]
        public string CallerName { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public DateTime Created { get; set; }

        [XmlElement(YMST.c_created)]
        [JsonProperty(YMST.c_created)]
        public long _createdLong
        {
            get { return Created.ToMillisecondsFromEpoch(); }
            set { Created = value.FromMillisecondsFromEpoch(); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public VoicemailResult Result { get; set; }

        [XmlElement(YMST.c_result)]
        [JsonProperty(YMST.c_result)]
        public int _resultInt
        {
            get { return (int)Result; }
            set { Result = (VoicemailResult)value; }
        }

        [XmlElement(YMST.c_imageUrl)]
        [JsonProperty(YMST.c_imageUrl)]
        public string ImageUrl { get; set; }

        [XmlElement(YMST.c_createSource)]
        [JsonProperty(YMST.c_createSource)]
        public int _createSourceInt
        {
            get { return (int)CreateSource; }
            set { CreateSource = (YouMailCreateSource)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailCreateSource CreateSource { get; set; }

        [XmlElement(YMST.c_phonebookSourceType)]
        [JsonProperty(YMST.c_phonebookSourceType)]
        public string _phoneBookSourceType
        {
            get
            {
                return PhoneBookSourceType.ToString();
            }
            set
            {
                YouMailPhoneBookSourceType sourceType = YouMailPhoneBookSourceType.Invalid;
                Enum.TryParse(value, true, out sourceType);
                PhoneBookSourceType = sourceType;
            }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailPhoneBookSourceType PhoneBookSourceType { get; set; }

        [XmlElement(YMST.c_phonebookSourceId)]
        [JsonProperty(YMST.c_phonebookSourceId)]
        public int PhoneBookSourceId { get; set; }

        [XmlElement(YMST.c_city)]
        [JsonProperty(YMST.c_city)]
        public string City { get; set; }

        [XmlElement(YMST.c_countryState)]
        [JsonProperty(YMST.c_countryState)]
        public string CountryState { get; set; }

        public virtual bool IsSpecialItem
        {
            get { throw new NotImplementedException(); }
        }
    }
}
