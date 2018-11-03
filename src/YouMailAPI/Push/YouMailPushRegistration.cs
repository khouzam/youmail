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

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_pushRegistrations)]
    public partial class YouMailPushRegistrations
    {
        /// <remarks/>
        [XmlElement(YMST.c_pushRegistration)]
        [JsonProperty(YMST.c_pushRegistration)]
        public YouMailPushRegistration[] PushRegistrations { get; set; }
    }

    /// <remarks/>
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_pushRegistration)]
    [XmlType(AnonymousType = true)]
    public partial class YouMailPushRegistration
    {
        [XmlIgnore]
        [JsonIgnore]
        public YouMailPushClientType ClientType { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_clientType)]
        [JsonProperty(YMST.c_clientType)]
        public int ClientTypeInt
        {
            get { return (int)ClientType; }
            set { ClientType = (YouMailPushClientType)value; }
        }

        /// <remarks/>
        [XmlElement(YMST.c_deviceId)]
        [JsonProperty(YMST.c_deviceId)]
        public string DeviceId { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public int Id { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailPushType PushType { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_pushType)]
        [JsonProperty(YMST.c_pushType)]
        public int PushTypeInt
        {
            get { return (int)PushType; }
            set { PushType = (YouMailPushType)value; }
        }

        ///
        [XmlElement(YMST.c_appVersion)]
        [JsonProperty(YMST.c_appVersion)]
        public string AppVersion { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public int StatusInt
        {
            get { return (int)Status; }
            set { Status = (YouMailPushRegistrationStatus)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public YouMailPushRegistrationStatus Status { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_validUntil)]
        [JsonProperty(YMST.c_validUntil)]
        public long ValidUntil { get; set; }

        /// <remarks/>
        [XmlElement(YMST.c_version)]
        [JsonProperty(YMST.c_version)]
        public byte Version { get; set; }
    }
}
