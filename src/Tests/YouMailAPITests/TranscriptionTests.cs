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
    public class TranscriptionTests
    {
        YouMailService service = YouMailTestService.Service;
        const int delay = 1000;

        [TestMethod]
        public async Task TranscriptionStatus()
        {
            var status = await service.GetTranscriptionStatusAsync();
            Assert.IsNotNull(status);
        }

        [TestMethod]
        public async Task TranscriptionSetttings()
        {
            var settings = await service.GetTranscriptionSettingsAsync();
            Assert.IsNotNull(settings);
        }

        // This test is flaky, ignore it temporarily
        [TestMethod, Ignore]
        public async Task ChangeTranscriptionSettings()
        {
            // Use my private account
            var service = YouMailTestService.MyService;
            var settings = await service.GetTranscriptionSettingsAsync();
            var oldEnabled = settings.Enabled;
            var newEnabled = !oldEnabled;

            if (oldEnabled)
            {
                await VerifySmsCountAsync(settings, service);
            }

            await service.SetTranscriptionSettingAsync(YMST.c_enabled, newEnabled.ToString().ToLower());
            await Task.Delay(delay);
            var settings2 = await service.GetTranscriptionSettingsAsync();
            Assert.AreEqual(newEnabled, settings2.Enabled);

            if (newEnabled)
            {
                await VerifySmsCountAsync(settings2, service);
            }

            await service.SetTranscriptionSettingAsync(YMST.c_enabled, oldEnabled.ToString().ToLower());
            await Task.Delay(delay);
            var settings3 = await service.GetTranscriptionSettingsAsync();
            Assert.AreEqual(oldEnabled, settings3.Enabled);
        }

        private static async Task VerifySmsCountAsync(TranscriptionSettings settings, YouMailService service)
        {
            var oldCount = settings.SmsCount;
            int newCount = 0;
            if (oldCount == 0)
            {
                newCount = 3;
            }

            await service.SetTranscriptionSettingAsync(YMST.c_smsCount, newCount.ToString());
            await Task.Delay(delay);
            var newSettings = await service.GetTranscriptionSettingsAsync();
            Assert.AreEqual(newCount, newSettings.SmsCount);

            await service.SetTranscriptionSettingAsync(YMST.c_smsCount, oldCount.ToString());
            await Task.Delay(delay);
            var oldSettings = await service.GetTranscriptionSettingsAsync();
            Assert.AreEqual(oldCount, oldSettings.SmsCount);
        }
    }
}
