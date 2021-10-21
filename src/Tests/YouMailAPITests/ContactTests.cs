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
    public class ContactTests
    {
        private YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task GetContacts()
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
        public async Task CreateUpdateAndDeleteContact()
        {
            var rand = new Random();
            var randValue = rand.Next(100);
            var contact = new YouMailContact
            {
                FirstName = "Test",
                LastName = $"Contact {randValue}",
                MobileNumber = $"(123) 456-78{randValue}"
            };

            var id = await service.CreateContactAsync(contact);
            Assert.IsTrue(id != 0, "Contact id returned was 0, contact wasn't properly created");

            // Wait for the contact to be created
            await Task.Delay(1000);

            var newContact = await service.GetContactAsync(id);

            Assert.IsNotNull(newContact, "Should have found the updated contact");
            newContact.Organization = "Test Org";

            await service.UpdateContactAsync(newContact, newContact.Id);

            await Task.Delay(1000);

            var updatedContact = await service.GetContactAsync(id);
            await service.DeleteContactAsync(id);

            // Try to get the contact again, it should be deleted
            var contactLookup = await service.GetContactAsync(id);
            Assert.IsNull(contactLookup, "Contact should not have been found");

            // Now that we have completed the test, verify that everything is proper
            Assert.IsTrue(newContact.Id == id, "Contact Id doesn't match");
            Assert.AreEqual(newContact.Organization, updatedContact.Organization, "Organization doesn't match");
            Assert.AreEqual(newContact.FirstName, contact.FirstName, "Contact first name doesn't match");
            Assert.AreEqual(newContact.LastName, contact.LastName, "Contact last name doesn't match");
            Assert.AreEqual(updatedContact.Organization, newContact.Organization, "The contact was not updated as expected");
        }

        [TestMethod, Ignore]
        public async Task UploadContacts()
        {
            Random rand = new Random();
            var contacts = new YouMailContacts
            {
                Contacts = new YouMailContact[]
                {
                    new YouMailContact
                    {
                        DisplayName = $"Test Contact {rand.Next(100)}",
                        FirstName = "Test",
                        LastName = "Contact",
                        MobileNumber = $"42555512{rand.Next(100)}"
                    },
                    new YouMailContact
                    {
                        DisplayName = $"John Doe {rand.Next(100)}",
                        FirstName = "John",
                        LastName = "Doe",
                        MobileNumber = $"20655512 {rand.Next(100)}"
                    }
                }
            };

            var uploadTime = DateTime.Now;
            await service.UploadContactsAsync(contacts);

            var result = await service.GetContactsAsync(uploadTime, 96, YouMailContactType.Personal);

            int found = 0;
            foreach (var contact in result.Data)
            {
                var lookup = await service.GetContactAsync(contact.Id, 200);
                Assert.AreEqual(contact.MobileNumber, lookup.MobileNumber, "Mobile numbers don't match");
                Assert.AreEqual(contact.LastName, lookup.LastName, "Last names don't match");
                await service.DeleteContactAsync(contact.Id);
                found++;
            }

            Assert.IsTrue(found == 2, "Did not find the two contacts");
        }
    }
}
