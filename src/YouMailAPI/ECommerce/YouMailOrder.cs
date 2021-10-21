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

    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_order)]
    public class YouMailOrder
    {
        [XmlElement(YMST.c_orderType)]
        [JsonProperty(YMST.c_orderType)]
        public int _orderTypeInt
        {
            get { return (int)OrderType; }
            set { OrderType = (OrderType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public OrderType OrderType { get; set; }

        [XmlElement(YMST.c_productName)]
        [JsonProperty(YMST.c_productName)]
        public string ProductName { get; set; }

        [XmlElement(YMST.c_productType)]
        [JsonProperty(YMST.c_productType)]
        public int _productTypeInt
        {
            get { return (int)ProductType; }
            set { ProductType = (ProductType)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public ProductType ProductType { get; set; }

        [XmlElement(YMST.c_status)]
        [JsonProperty(YMST.c_status)]
        public int _productStatusInt
        {
            get { return (int)ProductStatus; }
            set { ProductStatus = (ProductStatus)value; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public ProductStatus ProductStatus { get; set; }
    }
}
