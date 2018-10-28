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
    public class ContactTests
    {
        private YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task YouMail_Contacts()
        {
            try
            {
                var contacts = await service.GetContactsAsync(DateTime.MinValue, 200);
                Assert.IsNotNull(contacts);
                Assert.IsTrue(contacts.LastUpdated < DateTime.Now + TimeSpan.FromMinutes(1), $"LastUpdated: {contacts.LastUpdated}, Time: {DateTime.Now}");
                Assert.IsTrue(contacts.LastUpdated > DateTime.MinValue);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }

        [TestMethod]
        public async Task YouMail_UploadContacts()
        {
            try
            {
                var contacts = new YouMailContacts
                {
                    Contacts = new YouMailContact[]
                    {
                        new YouMailContact
                        {
                            DisplayName = "Test Contact",
                            FirstName = "Test",
                            LastName = "Contact",
                            MobileNumber = "4255551212"
                        },
                        new YouMailContact
                        {
                            DisplayName = "John Doe",
                            FirstName = "John",
                            LastName = "Doe",
                            MobileNumber = "2065551212"
                        }
                    }
                };

                var uploadTime = DateTime.Now;
                await service.UploadContactsAsync(contacts);

                var result = await service.GetContactsAsync(uploadTime, 96, YouMailContactType.Personal);

                int found = 0;
                foreach (var contact in result.Data)
                {
                    if (contact.MobileNumber == contacts.Contacts[0].MobileNumber ||
                        contact.MobileNumber == contacts.Contacts[1].MobileNumber)
                    {
                        var lookup = await service.GetContactAsync(contact.Id, 200);
                        Assert.IsTrue(contact.MobileNumber == lookup.MobileNumber &&
                            contact.LastName == lookup.LastName);
                        await service.DeleteContactAsync(contact.Id);
                        found++;
                    }
                }

                Assert.IsTrue(found == 2);
            }
            catch (YouMailException yme)
            {
                Assert.Fail(yme.Message);
            }
        }
    }
}
