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
        public async Task GetUserInfo()
        {
            var userInfo = await service.GetUserInfoAsync();
            Assert.AreEqual(service.Username, userInfo.PrimaryPhoneNumber);
        }

        [TestMethod]
        public async Task ChangePin()
        {
            var userInfo = await service.GetUserInfoAsync();
            userInfo.Pin = "1234";
            await service.SetUserInfoAsync(userInfo);
        }

        [TestMethod]
        public async Task ChangePassword()
        {
            var newService = YouMailTestService.NewService;

            var ymPasswordChange = new YouMailPasswordChange
            {
                CurrentPassword = newService.Password,
                NewPassword = "MyNewPassword123"
            };

            var ymPasswordRestore = new YouMailPasswordChange
            {
                CurrentPassword = ymPasswordChange.NewPassword,
                NewPassword = ymPasswordChange.CurrentPassword
            };

            await newService.ChangePasswordAsync(ymPasswordChange);

            // Verify that we can login with the new password
            var verifyService = new YouMailService(newService.Username, ymPasswordChange.NewPassword, null, YouMailTestService.UserAgent, YouMailTestService.ResponseFormat);
            var userInfo = await verifyService.GetUserInfoAsync();

            // Restore the password
            await verifyService.ChangePasswordAsync(ymPasswordRestore);
        }
    }
}
