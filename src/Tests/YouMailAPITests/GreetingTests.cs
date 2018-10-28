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
    using System.Threading.Tasks;

    [TestClass]
    public class GreetingTests
    {
        private YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task YouMail_GetGreetings()
        {
            try
            {
                var greetings = await service.GetGreetingsAsync(DateTime.MinValue);
                Assert.IsNotNull(greetings);
                Assert.IsTrue(greetings.LastUpdated < DateTime.Now + TimeSpan.FromMinutes(1), $"LastUpdated: {greetings.LastUpdated}, Time: {DateTime.Now}");
                Assert.IsTrue(greetings.LastUpdated > DateTime.MinValue);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

        [TestMethod]
        public async Task YouMail_GetGreetingStatuses()
        {
            try
            {
                var statuses = await service.GetGreetingStatusesAsync();
                Assert.IsNotNull(statuses);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

    }
}
