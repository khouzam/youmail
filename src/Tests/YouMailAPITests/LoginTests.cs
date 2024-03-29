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
    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class LoginTests
    {
        [TestMethod]
        public async Task Login()
        {
            var service = YouMailTestService.NewService;
            var task = service.LoginAsync();
            Assert.IsTrue(await service.LoginWaitAsync());
            Assert.IsTrue(task.IsCompleted && !task.IsFaulted);
            Assert.IsTrue(service.IsLoggedIn);
            Assert.IsTrue(service.HasCredentials);
            Assert.IsTrue(service.HasTriedLogin);
        }

        [TestMethod, Ignore]
        public async Task Private_Login()
        {
            var youmail = YouMailTestService.PrivateService;
            youmail.AuthToken = null;
            await youmail.LoginAsync();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(youmail.AuthToken));
        }

        [TestMethod]
        public async Task Login_Fail()
        {
            var service = new YouMailService("0000", "0000", null, YouMailTestService.UserAgent);
            try
            {
                await service.LoginAsync();
                Assert.Fail("The login should not have succeeded");
            }
            catch (YouMailException ye)
            {
                Assert.IsTrue(ye.StatusCode == HttpStatusCode.Forbidden, "Wrong StatusCode back");
                Assert.IsFalse(service.IsLoggedIn);
            }
        }

        [TestMethod]
        public async Task ValidateAuth()
        {
            var service = YouMailTestService.NewService;

            // Call any API that needs to login
            await service.GetUserIdAsync();
        }
    }
}
