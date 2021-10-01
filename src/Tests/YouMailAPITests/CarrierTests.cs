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

namespace MagikInfo.YouMailAPI.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    [TestClass]
    public class CarrierTests
    {
        YouMailService Service = YouMailTestService.Service;

        [TestMethod]
        public async Task CarrierInfo()
        {
            try
            {
                var carrierInfo = await Service.GetCarrierInfoAsync();
                var forwardingInfo = await Service.GetForwardingInstructionsAsync();
                Assert.IsTrue(carrierInfo.Id == forwardingInfo.CarrierId);
                var carriers = await Service.GetSupportedCarriersAsync();
                // Pick a new random carrier
                var rand = new Random();
                var prefCarriers = new List<YouMailCarrier>();
                foreach (var carrier in carriers.Carriers)
                {
                    if (carrier.CarrierClass == 1)
                    {
                        prefCarriers.Add(carrier);
                    }
                }
                YouMailCarrier newCarrier = null;
                do
                {
                    newCarrier = prefCarriers[rand.Next(prefCarriers.Count)];
                }
                // Don't choose AT&T as the name is not proper XML
                while (!newCarrier.SupportedFlag || newCarrier.Id == carrierInfo.Id ||
                    newCarrier.Id == 1);

                await Service.SetCarrierInfoAsync(newCarrier.Id);
                var changedCarrier = await Service.GetCarrierInfoAsync();
                var changedForward = await Service.GetForwardingInstructionsAsync();
                Assert.IsTrue(changedCarrier.Id == newCarrier.Id);
                Assert.IsTrue(changedForward.CarrierId == newCarrier.Id);

                await Service.SetCarrierInfoAsync(carrierInfo.Id);

            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

        [TestMethod]
        public async Task TestAllCarriers()
        {
            var carriers = await Service.GetSupportedCarriersAsync();

            foreach (var carrier in carriers.Carriers)
            {
                try
                {
                    await Service.SetCarrierInfoAsync(carrier.Id);
                    var info = await Service.GetForwardingInstructionsAsync();
                }
                catch (YouMailException yme)
                {
                    Debug.WriteLine($"Failed to set the carrier {carrier.Name}, Supported: {carrier.SupportedFlag}");
                    Debug.WriteLine($"Failed with {yme.Message}");
                }
            }
        }
    }
}
