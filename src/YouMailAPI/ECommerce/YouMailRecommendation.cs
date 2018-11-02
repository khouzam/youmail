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
    using System;
    using System.Xml.Serialization;

    public class YouMailRecommendation
    {
        [XmlElement(YMST.c_productName)]
        public string ProductName { get; set; }

        [XmlElement(YMST.c_pitch)]
        public string Pitch { get; set; }

        [XmlElement(YMST.c_productType)]
        public int _productTypeInt
        {
            get { return (int)ProductType; }
            set { ProductType = (ProductType)value; }
        }
        [XmlIgnore]
        public ProductType ProductType { get; set; }

        [XmlElement(YMST.c_unitPrice)]
        public float UnitPrice { get; set; }

        [XmlElement(YMST.c_recommendationType)]
        public string _recommendationTypeString
        {
            get { return RecommendationType.ToString(); }
            set
            {
                OrderType order = OrderType.Unknown;
                Enum.TryParse<OrderType>(value, out order);
                RecommendationType = order;
            }
        }

        [XmlIgnore]
        public OrderType RecommendationType { get; set; }

        [XmlElement(YMST.c_webProductUrl)]
        public string WebProductUrl { get; set; }

        [XmlElement(YMST.c_mobileProductUrl)]
        public string MobileProductUrl { get; set; }

        [XmlElement(YMST.c_productSku)]
        public string ProductSku { get; set; }
    }
}
