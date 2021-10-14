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
        public async Task GetGreetings()
        {
            var greetings = await service.GetGreetingsAsync(DateTime.MinValue);
            Assert.IsNotNull(greetings);
            Assert.IsTrue(greetings.LastUpdated < DateTime.Now + TimeSpan.FromMinutes(1), $"LastUpdated: {greetings.LastUpdated}, Time: {DateTime.Now}");
            Assert.IsTrue(greetings.LastUpdated > DateTime.MinValue);
        }

        [TestMethod]
        public async Task GetGreetingStatuses()
        {
            var statuses = await service.GetGreetingStatusesAsync();
            Assert.IsTrue(statuses.Statuses.Count > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(statuses.Statuses[0].Name));
        }

        [TestMethod]
        public async Task CreateGreeting()
        {
            byte[] data = System.IO.File.ReadAllBytes(@"Data\Recording.wav");
            var rand = new Random();
            var now = DateTime.Now;
            string name = $"New Test Greeting {rand.Next(100)}";
            string description = $"Creating a new test greeting";
            var response = await service.CreateGreetingAsync(name, description, data);
            var greetingId = long.Parse(response.Properties[YMST.c_greetingId]);

            Assert.IsTrue(greetingId != 0, "The newly created greeting has an invalid Id");

            var greeting = await service.GetGreetingAsync(greetingId);
            Assert.IsNotNull(greeting);

            await service.DeleteGreetingAsync(greeting.Id);

            Assert.AreEqual(name, greeting.Name, "Greeting name doesn't match");
            Assert.AreEqual(greetingId, greeting.Id, "Greeting Id doesn't match");
        }

    }
}
