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
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_carriers)]
    public partial class YouMailCarriers
    {
        [XmlElement(YMST.c_carrier)]
        [JsonProperty(YMST.c_carriers)]
        public YouMailCarrier[] Carriers;
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_carrier)]
    [JsonObject(YMST.c_carrier)]
    public class YouMailCarrier
    {
        [XmlElement(YMST.c_id)]
        [JsonProperty(YMST.c_id)]
        public int Id { get; set; }

        [XmlElement(YMST.c_name)]
        [JsonProperty(YMST.c_name)]
        public string Name { get; set; }

        [XmlElement(YMST.c_carrierClass)]
        [JsonProperty(YMST.c_carrierClass)]
        public int CarrierClass { get; set; }

        [XmlElement(YMST.c_activeFlag)]
        [JsonProperty(YMST.c_activeFlag)]
        public bool ActiveFlag { get; set; }

        [XmlElement(YMST.c_supportedFlag)]
        [JsonProperty(YMST.c_supportedFlag)]
        public bool SupportedFlag { get; set; }
    }
}
