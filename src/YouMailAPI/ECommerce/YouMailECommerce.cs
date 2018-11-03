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

    /// <summary>
    /// The action to take when we try to transcribe a message
    /// </summary>
    public enum TranscriptionAction
    {
        Undefined,
        GetSubscriptionPlan,
        Transcribe,
        TranscribeGetBiggerPlan,
    }

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_ecommerceStatus)]
    public class YouMailECommerce
    {
        [XmlArray(YMST.c_orders)]
        [XmlArrayItem(YMST.c_order)]
        public YouMailOrder[] Orders;

        [XmlArray(YMST.c_trials)]
        [XmlArrayItem(YMST.c_order)]
        public YouMailOrder[] Trials;

        [XmlArray(YMST.c_recommendations)]
        [XmlArrayItem(YMST.c_recommendation)]
        public YouMailRecommendation[] Recommendations;

        private TranscriptionAction _defaultAction = TranscriptionAction.Undefined;
        private YouMailOrder _transcriptionPlan = null;
        public YouMailTranscriptionStatus TranscriptionStatus = null;

        public YouMailECommerce()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public YouMailOrder TranscriptionPlan
        {
            get
            {
                if (_transcriptionPlan == null && Orders != null)
                {
                    foreach (YouMailOrder order in Orders)
                    {
                        if (order.ProductType == ProductType.Transcription && order.ProductStatus == ProductStatus.Active)
                        {
                            _transcriptionPlan = order;
                            break;
                        }
                    }
                }
                return _transcriptionPlan;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public TranscriptionAction TranscriptionAction
        {
            get
            {
                if (_defaultAction == TranscriptionAction.Undefined)
                {
                    if (TranscriptionPlan != null)
                    {
                        if (TranscriptionStatus != null && TranscriptionStatus.PlanUsedCount >= TranscriptionStatus.PlanMaxCount)
                        {
                            _defaultAction = TranscriptionAction.TranscribeGetBiggerPlan;
                        }
                        else
                        {
                            _defaultAction = TranscriptionAction.Transcribe;
                        }
                    }
                    else
                    {
                        _defaultAction = TranscriptionAction.GetSubscriptionPlan;
                    }
                }

                return _defaultAction;
            }
        }
    }
}
