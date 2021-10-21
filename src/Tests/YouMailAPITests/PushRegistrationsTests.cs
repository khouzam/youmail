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
    using System.Threading.Tasks;

    [TestClass]
    public class PushRegistrationsTests
    {
        YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task GetPushRegistrations()
        {
            var registrations = await service.GetDeviceRegistrationsAsync();
            if (registrations != null)
            {
                foreach (var registration in registrations)
                {
                    Assert.IsTrue(registration.Id != 0, "Registration didn't have a valid Id");
                }
            }
        }

        [TestMethod]
        public async Task RegisterAndDeregisterDevice()
        {
            var deviceReg = new YouMailPushRegistration
            {
                ClientType = YouMailPushClientType.ISeeVM,
                DeviceId = "ABCDEFG",
                AppVersion = "ISeeVM",
                Version = 9,
                PushType = YouMailPushType.MWI |
                           YouMailPushType.MissedCall |
                           YouMailPushType.NewMessage |
                           YouMailPushType.NewMessageWithTranscription |
                           YouMailPushType.AdminFreeFormMessage |
                           YouMailPushType.AdminMessage |
                           YouMailPushType.MessageNotif |
                           YouMailPushType.MissedCallNotif |
                           YouMailPushType.SettingsNotif |
                           YouMailPushType.SystemEvent |
                           YouMailPushType.SMS |
                           YouMailPushType.MMS |
                           YouMailPushType.IncomingExtraLine |
                           YouMailPushType.MessageDeliveryReceipt,
                MessageTypes = "vm,sms,mms"
            };

            var registration = await service.RegisterDeviceAsync(deviceReg);
            Assert.IsNotNull(registration);
            Assert.IsTrue(registration.Id != 0);

            var registrations = await service.GetDeviceRegistrationsAsync();

            await service.DeleteDeviceRegistrationAsync(registration.Id);
            Assert.IsTrue(registration.DeviceId == deviceReg.DeviceId);
            Assert.IsTrue(registration.AppVersion == deviceReg.AppVersion);
            Assert.IsTrue(registration.Version == deviceReg.Version);
            Assert.IsTrue(registration.PushType == deviceReg.PushType);
            Assert.IsTrue(registration.MessageTypes == deviceReg.MessageTypes);

            bool fFound = false;

            foreach (var reg in registrations)
            {
                if (reg.Id == registration.Id)
                {
                    fFound = true;
                    break;
                }
            }

            Assert.IsTrue(fFound, $"Did not find a registration with Id: {registration.Id}");

            fFound = false;
            registrations = await service.GetDeviceRegistrationsAsync();
            foreach (var reg in registrations)
            {
                if (reg.Id == registration.Id)
                {
                    fFound = true;
                    break;
                }
            }
            Assert.IsFalse(fFound, $"Found a registration with Id: {registration.Id}, even though we deleted it");
        }
    }
}
