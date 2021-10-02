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

namespace MagikInfo.YouMailAPI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        private const int _contactUploadSize = 15;

        /// <summary>
        /// Upload a whole list of contacts
        /// </summary>
        /// <param name="contacts">The contacts to upload and update.</param>
        /// <returns></returns>
        public async Task UploadContactsAsync(YouMailContacts contacts)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    int currentIndex = 0;
                    YouMailContacts currentContacts = new YouMailContacts();

                    // We can only send 15 contacts at a time
                    var contactsList = new List<YouMailContact>();
                    do
                    {
                        contactsList.Clear();
                        int elements = Math.Min(_contactUploadSize, contacts.Contacts.Length - currentIndex);
                        for (int i = 0; i < elements; i++)
                        {
                            contactsList.Add(contacts.Contacts[currentIndex++]);
                        }

                        // Add the 15 contacts to the currentContacts
                        currentContacts.Contacts = contactsList.ToArray();

                        // Create a MultipartForm for uploading a compressed gz file.
                        var boundaryMarker = "Boundary--" + Guid.NewGuid().ToString();
                        using (var content = new MultipartFormDataContent(boundaryMarker))
                        {
                            // There's an issue with the boundary having extra quotes that YouMail doesn't like
                            // create the boundary ourselves.
                            content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                            content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", boundaryMarker));
                            // Create a memory stream that will compress the XML representaion of the contacts
                            using (var stream = new MemoryStream())
                            {
                                using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Compress, true))
                                {
                                    using (StreamWriter writer = new StreamWriter(gzStream))
                                    {
                                        var serializedContacts = SerializeObject(currentContacts.Contacts);
                                        writer.Write(serializedContacts);
                                    }
                                }

                                // Seek back to the origin of the stream to stick it into the http message
                                stream.Seek(0, SeekOrigin.Begin);

                                using (var compressedContacts = new ByteArrayContent(stream.ToArray()))
                                {
                                    compressedContacts.Headers.ContentType = new MediaTypeHeaderValue(ResponseFormatString);
                                    compressedContacts.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                    {
                                        FileName = "contacts.gz",
                                        Name = "datafile"
                                    };
                                    content.Add(compressedContacts);
                                    using (await YouMailApiAsync(YMST.c_uploadContacts, content, HttpMethod.Post))
                                    {
                                    }
                                }
                            }
                        }

                        bool completed = false;
                        do
                        {
                            // We've uploaded a batch of contacts, poll to see if we can send the next ones
                            using (var response = await YouMailApiAsync(YMST.c_uploadContactsStatus, null, HttpMethod.Get))
                            {
                                var status = DeserializeObject<YouMailContactsUploadStatus>(response.GetResponseStream(), YMST.c_contactSyncSummary);
                                if (status.Status == YouMailContactsUploadStatus.StatusEnum.Started ||
                                    status.Status == YouMailContactsUploadStatus.StatusEnum.Pending)
                                {
                                    await Task.Delay(2000);
                                }
                                else
                                {
                                    completed = true;
                                }
                            }
                        }
                        while (!completed);
                    }
                    while (currentIndex < contacts.Contacts.Length);
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }
        /// <summary>
        /// Get the contacts for the user
        /// </summary>
        /// <param name="lastUpdated"></param>
        /// <returns></returns>
        public async Task<GetUpdateResult<YouMailContact[]>> GetContactsAsync(DateTime lastUpdated, int imageSize, YouMailContactType contactType = YouMailContactType.NoRestriction)
        {
            GetUpdateResult<YouMailContact[]> returnValue = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    YouMailContactQuery query = new YouMailContactQuery(QueryType.GetContacts)
                    {
                        UpdatedFrom = lastUpdated,
                        ImageSize = imageSize,
                        Page = 1,
                        PageLength = s_PageLength,
                        ContactType = contactType
                    };

                    returnValue = await ExecuteContactQueryAsync(query);
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return returnValue;
        }

        /// <summary>
        /// Get a YouMailContact from an ID
        /// </summary>
        /// <param name="id">The id of the contact</param>
        /// <returns>The YouMailContact</returns>
        public async Task<YouMailContact> GetContactAsync(long id, int imageSize)
        {
            YouMailContact contact = null;
            try
            {
                AddPendingOp();
                YouMailContactQuery query = new YouMailContactQuery(QueryType.GetContact)
                {
                    ImageSize = imageSize,
                };
                query.AddItemId(id);

                var results = await ExecuteContactQueryAsync(query);
                if (results != null && results.Data != null && results.Data.Length > 0)
                {
                    contact = results.Data[0];
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return contact;
        }

        private async Task<GetUpdateResult<YouMailContact[]>> ExecuteContactQueryAsync(YouMailQuery query)
        {
            HttpResponseMessage response = null;
            List<YouMailContact> contacts = new List<YouMailContact>();
            DateTime lastQueryUpdated = DateTime.MinValue;
            int count = 0;
            do
            {
                count = 0;
                using (response = await YouMailApiAsync(YMST.c_getContacts + query.GetQueryString(), null, HttpMethod.Get))
                {
                    if (response != null)
                    {
                        var contactResponse = DeserializeObject<YouMailContactsResponse>(response.GetResponseStream());
                        if (contactResponse != null)
                        {
                            count = contactResponse.Contacts.Length;
                            if (count > 0)
                            {
                                contacts.AddRange(contactResponse.Contacts);
                            }
                        }

                        if (lastQueryUpdated == DateTime.MinValue)
                        {
                            var date = response.Headers.Date.ToString();
                            lastQueryUpdated = DateTime.Parse(date);
                        }
                    }
                }
                query.Page++;
            } while (count == query.PageLength);

            return new GetUpdateResult<YouMailContact[]>(lastQueryUpdated, contacts.ToArray());
        }

        /// <summary>
        /// Delete a YouMail contact
        /// </summary>
        /// <param name="id">The id of the contact</param>
        /// <returns></returns>
        public async Task DeleteContactAsync(long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await ContactApiAsync(id, HttpMethod.Delete))
                    {
                    };
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Generic call to the contact API REST endpoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> ContactApiAsync(long id, HttpMethod verb)
        {
            return await YouMailApiAsync(string.Format(YMST.c_contactsSelect, id), null, verb);
        }

        /// <summary>
        /// Set the specific greeting for a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="greetingId"></param>
        /// <returns></returns>
        public async Task SetContactGreetingAsync(long contactId, long greetingId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_setContactGreeting, contactId, greetingId), null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Set the specific action for a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task SetContactActionAsync(long contactId, YouMailActionType action)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_setContactAction, contactId, (int)action), null, HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }
        /// <summary>
        /// Get or create a contact if one doesn't exist.
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<YouMailContact> GetOrCreateContactAsync(string number, YouMailContact contact)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(string.Format(YMST.c_getOrCreateContact, number), SerializeObjectToHttpContent(contact), HttpMethod.Post))
                    {
                        return DeserializeObject<YouMailContact>(response.GetResponseStream());
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return null;
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<long> CreateContactAsync(YouMailContact contact)
        {
            long id = 0;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_createContact, SerializeObjectToHttpContent(contact, YMST.c_contact), HttpMethod.Post))
                    {
                        var ymResponse = DeserializeObject<YouMailResponse>(response.GetResponseStream());
                        id = long.Parse(ymResponse.Properties[YMST.c_contactId]);
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
            return id;
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task UpdateContactAsync(YouMailContact contact, long id)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {

                    using (await YouMailApiAsync(string.Format(YMST.c_updateContact, id), SerializeObjectToHttpContent(contact), HttpMethod.Put))
                    {
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }
        }
    }
}
