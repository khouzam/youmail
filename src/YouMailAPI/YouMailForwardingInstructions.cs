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
    using System.Xml.Serialization;

    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = YMST.c_phoneForwardingInstructions)]
    public class YouMailForwardingInstructions
    {
        [XmlElement(YMST.c_carrierId)]
        public int CarrierId { get; set; }

        [XmlElement(YMST.c_carrierName)]
        public string CarrierName { get; set; }

        [XmlElement(YMST.c_activatingCodePrimary)]
        public string ActivatingCodePrimary { get; set; }

        [XmlElement(YMST.c_activatingCodeSecondary)]
        public string ActivatingCodeSecondary { get; set; }

        [XmlElement(YMST.c_deactivatingCode)]
        public string DeactivatingCode { get; set; }

        [XmlIgnore]
        public string[] ForwardToYoumail
        {
            get
            {
                if (string.IsNullOrEmpty(ActivatingCodePrimary))
                {
                    if (string.IsNullOrEmpty(ActivatingCodeSecondary))
                    {
                        return null;
                    }
                    else
                    {
                        return ActivatingCodeSecondary.Split(',');
                    }
                }
                else
                {
                    return ActivatingCodePrimary.Split(',');
                }
            }
        }

        [XmlElement(YMST.c_enableMessage)]
        public string EnableMessage { get; set; }

        [XmlIgnore]
        public string[] CancelYoumail
        {
            get
            {
                string[] codes = null;
                if (!string.IsNullOrEmpty(DeactivatingCode))
                {
                    codes = DeactivatingCode.Split(',');
                }
                return codes;
            }
        }

        [XmlElement(YMST.c_disableMessage)]
        public string DisableMessage { get; set; }

        [XmlElement(YMST.c_carrierSupportsForwarding)]
        public bool CarrierSupportsForwarding { get; set; }

        [XmlElement(YMST.c_carrierManagesForwarding)]
        public bool CarrierManagesForwarding { get; set; }

        [XmlElement(YMST.c_carrierHasForwardingCharges)]
        public bool CarrierHasForwardingCharges { get; set; }

        [XmlElement(YMST.c_carrierOffersPrepaid)]
        public bool CarrierOffersPrepaid { get; set; }

        [XmlElement(YMST.c_carrierSupportsPrepaidForwarding)]
        public bool CarrierSupportsPrepaidForwarding { get; set; }

        [XmlElement(YMST.c_carrierOnlineAccountRequired)]
        public bool CarrierOnlineAccountRequired { get; set; }

        [XmlElement(YMST.c_callingCarrierSupportRequired)]
        public bool CallingCarrierSupportRequired { get; set; }

        [XmlElement(YMST.c_didNumberRequired)]
        public bool DidNumberRequired { get; set; }

        [XmlElement(YMST.c_localNumberRequired)]
        public bool LocalNumberRequired { get; set; }

        [XmlElement(YMST.c_dropboxOnly)]
        public bool DropboxOnly { get; set; }

        [XmlElement(YMST.c_carrierForwardingUrl)]
        public string CarrierForwardingUrl { get; set; }

        [XmlElement(YMST.c_carrierSupportNumber)]
        public string CarrierSupportNumber { get; set; }

        [XmlElement(YMST.c_forwardingInstructions)]
        public string ForwardingInstructions { get; set; }

        [XmlElement(YMST.c_deactivatingInstructions)]
        public string DeactivatingInstructions { get; set; }

        [XmlElement(YMST.c_specialInstructions)]
        public string SpecialInstructions { get; set; }

        [XmlElement(YMST.c_notes)]
        public string Notes { get; set; }

        [XmlElement(YMST.c_forwardingNumber)]
        public string ForwardingNumber { get; set; }

        [XmlElement(YMST.c_retrievalNumber)]
        public string RetrievalNumber { get; set; }

        [XmlElement(YMST.c_userPhoneNumber)]
        public string UserPhoneNumber { get; set; }

        [XmlElement(YMST.c_anyNotesToShow)]
        public bool AnyNotesToShow { get; set; }
    }
}
