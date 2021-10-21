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
    using MagikInfo.YouMailAPI;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class AlertsTests
    {
        [TestMethod]
        public async Task GetAlertsTest()
        {
            var service = YouMailTestService.Service;
            var alerts = await service.GetAlertSettingsAsync();
            Assert.IsNotNull(alerts, "Did not get an AlertsSettings back");
            Assert.IsTrue(alerts.EmailFormat != 0, $"Email Format is invalid with a value of {alerts.EmailFormat}");
            Assert.IsTrue(alerts.PushConditions != 0, $"PushConditions are unexpected with a value of {alerts.PushConditions}");
        }

        [TestMethod]
        public async Task ChangeAlerts()
        {
            var service = YouMailTestService.Service;

            var alerts = await service.GetAlertSettingsAsync();

            // Don't change text settings since that requires a premium plan
            alerts.DitchedCallEmail = !alerts.DitchedCallEmail;
            alerts.MessageEmail = !alerts.MessageEmail;
            alerts.MissedCallEmail = !alerts.MissedCallEmail;
            alerts.SpamMessageEmail = !alerts.SpamMessageEmail;

            await service.SetAlertSettingsAsync(alerts);

            await Task.Delay(1000);

            var newAlerts = await service.GetAlertSettingsAsync();

            Assert.AreEqual(alerts.DitchedCallEmail, newAlerts.DitchedCallEmail, "DitchCallEmail wasn't changed");
            Assert.AreEqual(alerts.MessageEmail, newAlerts.MessageEmail, "MessageEmail wasn't changed");
            Assert.AreEqual(alerts.MissedCallEmail, newAlerts.MissedCallEmail, "MissedCallPush wasn't changed");
            Assert.AreEqual(alerts.SpamMessageEmail, newAlerts.SpamMessageEmail, "SpamMessageEmail wasn't changed");
        }
    }
}
