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
    using System.Net;
    using System.Threading.Tasks;

    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public async Task GetYouMailError()
        {
            var service = YouMailTestService.Service;

            try
            {
                // Issue a call that will fail
                await service.CreateFolderAsync(string.Empty, string.Empty);
            }
            catch (YouMailException yme)
            {
                Assert.IsNotNull(yme.Error, "Did not get a YouMailError");
                Assert.IsNotNull(yme.Error.Errors, "Did not get an internal set of errors");
                Assert.IsNotNull(yme.Error.Errors[0].ErrorCode, "Did not get an ErrorCode");
                Assert.IsNotNull(yme.Error.Errors[0].ShortMessage, "Did not get a ShortMessage");
                Assert.IsNotNull(yme.Error.Errors[0].LongMessage, "Did not get a LongMessage");
                return;
            }
            Assert.Fail("YouMailException was not thrown");
        }
    }
}
