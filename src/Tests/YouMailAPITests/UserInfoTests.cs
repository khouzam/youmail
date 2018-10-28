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
    public class UserInfoTests
    {
        private YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task YouMail_GetUserInfo()
        {
            try
            {
                var userInfo = await service.GetUserInfoAsync();
                Assert.AreEqual(service.Username, userInfo.PrimaryPhoneNumber);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

        [TestMethod]
        public async Task YouMail_ChangePin()
        {
            try
            {
                var userInfo = await service.GetUserInfoAsync();
                userInfo.Pin = "1234";
                await service.SetUserInfoAsync(userInfo);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

        [TestMethod]
        public async Task YouMail_ChangePassword()
        {
            var newService = YouMailTestService.NewService;
            var userInfo = await newService.GetUserInfoAsync();
            userInfo.Password = "Abc12345";
            await newService.SetUserInfoAsync(userInfo);

            try
            {
                // Try a bad password
                userInfo.Password = "abc";
                await newService.SetUserInfoAsync(userInfo);
            }
            catch (YouMailException e)
            {
                Assert.IsTrue(e.StatusCode == HttpStatusCode.BadRequest);
            }

            userInfo.Password = newService.Password;
            await newService.SetUserInfoAsync(userInfo);
        }
    }
}
