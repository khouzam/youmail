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
    public class AccountTests
    {
        private static string _defaultEmail;
        private static string _testUsername;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _defaultEmail = context.Properties["AccountCreationEmail"].ToString();
            _testUsername = context.Properties["TestUsername"].ToString();
        }

        [TestMethod]
        public async Task CreateAccount()
        {
            string user = "0002091234";
            string password = "Abc12345";
            var youmail = YouMailTestService.PrivateService;

            if (await youmail.AccountExistsAsync(user))
            {
                try
                {
                    await youmail.DeleteAccountAsync();
                }
                catch (YouMailException)
                {
                    Assert.Fail("Failed to delete old existing account");
                }
            }

            await youmail.CreateAccountAsync(user, password, "Test", "Account", string.Format(_defaultEmail, user));
            {
                var newService = new YouMailService(user, password, null, YouMailTestService.UserAgent, true);
                await newService.DeleteAccountAsync();

                // This should fail the second time
                bool failed = false;
                try
                {
                    await newService.DeleteAccountAsync();
                }
                catch (YouMailException e)
                {
                    // Either the account doesn't exist, or the user isn't authenticated anymore
                    if (e.StatusCode == HttpStatusCode.NotFound ||
                        e.StatusCode == HttpStatusCode.Forbidden)
                    {
                        failed = true;
                    }
                }
                Assert.IsTrue(failed);
            }
        }

        [TestMethod]
        public async Task VerifyExistingAccount()
        {
            await VerifyAccountAsync(_testUsername, true);
        }

        [TestMethod]
        public async Task VerifyNonExistentAccount()
        {
            await VerifyAccountAsync("0000000000", false);
        }

        [TestMethod]
        public async Task VerifyInvalidAccount()
        {
            bool exception = false;
            try
            {
                await VerifyAccountAsync("AbcDEF", false);
            }
            catch (YouMailException e)
            {
                Assert.IsTrue(e.StatusCode == HttpStatusCode.BadRequest);
                exception = true;
            }
            Assert.IsTrue(exception);
        }

        private async Task VerifyAccountAsync(string user, bool expected)
        {
            var youmail = YouMailTestService.PrivateService;
            var registration = await youmail.AccountRegistrationVerification(user);
            bool found = registration.Properties[YMST.c_registrationStatus] == YMST.c_accountExists;

            Assert.IsTrue(found == expected);

            found = await youmail.AccountExistsAsync(user);
            Assert.IsTrue(found == expected);
        }
    }
}
