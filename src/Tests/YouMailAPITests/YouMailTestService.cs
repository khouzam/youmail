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

    [TestClass]
    public class YouMailTestService
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            TestUser = context.Properties["TestUsername"].ToString();
            TestPassword = context.Properties["TestPassword"].ToString();
            TestFullName = context.Properties["TestFullName"].ToString();
            _testAuth = context.Properties["TestAuthToken"].ToString();
            _myUser = context.Properties["RealUsername"].ToString();
            _myPwd = context.Properties["RealPassword"].ToString();
            _myAuth = context.Properties["RealAuthToken"].ToString();
            _privateUser = context.Properties["PrivateUsername"].ToString();
            _privatePwd = context.Properties["PrivatePassword"].ToString();
            _privateAuth = context.Properties["PrivateAuthToken"].ToString();
            UserAgent = context.Properties["DefaultUserAgent"].ToString();
        }

        private static YouMailService _service = null;
        private static YouMailService _myService = null;
        private static string _testAuth;
        private static string _myUser;
        private static string _myPwd;
        private static string _myAuth;
        private static string _privateUser;
        private static string _privatePwd;
        private static string _privateAuth;

        public static string TestUser { get; private set; }
        public static string TestPassword { get; private set; }
        public static string TestFullName { get; private set; }
        public static string UserAgent { get; private set; }

        public static YouMailService Service
        {
            get
            {
                if (_service == null)
                {
                    ResetService();
                }
                return _service;
            }
        }

        public static YouMailService MyService
        {
            get
            {
                if (_myService == null)
                {
                    _myService = new YouMailService(_myUser, _myPwd, _myAuth, UserAgent);
                }

                return _myService;
            }
        }

        public static YouMailService PrivateService => new YouMailService(_privateUser, _privatePwd, _privateAuth, UserAgent);

        /// <summary>
        /// Provide a new service that is not the global one.
        /// </summary>
        public static YouMailService NewService
        {
            get
            {
                var service = new YouMailService(TestUser, TestPassword, _testAuth, UserAgent);
                service.AuthenticationChanged += OnTestServiceAuthenticationChanged;
                return service;
            }
        }
        private static void ResetService()
        {
            _service = new YouMailService(TestUser, TestPassword, _testAuth, UserAgent);
            _service.AuthenticationChanged += OnTestServiceAuthenticationChanged;
        }

        private static void OnTestServiceAuthenticationChanged(string username, string authToken)
        {
            _testAuth = authToken;
        }
    }
}
