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
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class MessageTests
    {
        YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task GetMessages()
        {
            var messages = await service.GetMessagesAsync(YouMailService.InboxFolder, 96, DataFormat.MP3);
            Assert.IsTrue(messages != null, "Messages are null");
            Assert.IsTrue(messages.LastUpdated < DateTime.Now + TimeSpan.FromSeconds(10) && messages.LastUpdated > DateTime.MinValue,
                $"Messages last updated: {messages.LastUpdated}, Now: {DateTime.Now}");

            Assert.IsNotNull(messages.Data);

            // Validate that the messages are in creation order
            for (int i = 1; i < messages.Data.Length; i++)
            {
                Assert.IsTrue(messages.Data[i - 1].Created >= messages.Data[i].Created,
                 "Messages are not sorted according to creation time");
            }
            // Get one message
            var stream = await service.GetMessageDataAsync(messages.Data[0].MessageDataUrl);
            Assert.IsNotNull(stream);

            // Modify 2 messages
            int count = messages.Data.Length;
            int modified = Math.Min(count, 2);
            var messageIds = new long[modified];

            // Change 2 messages.
            for (int i = 0; i < modified; i++)
            {
                messageIds[i] = messages.Data[i].Id;
                bool read = messages.Data[i].Status == MessageStatus.Read;
                await service.MarkMessageAsync(messageIds[i], !read, false);

                var message = await service.GetMessageAsync(messageIds[i], 200, DataFormat.MP3);
                Assert.AreNotEqual(read ? MessageStatus.Read : MessageStatus.New, message.Status);
            }

            var modifiedMessages = await service.GetUpdatedMessagesSinceAsync(messages.LastUpdated, 200, DataFormat.MP3);
            Assert.AreEqual(modified, modifiedMessages.Length);
            for (int i = 0; i < modifiedMessages.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < modified; j++)
                {
                    if (messageIds[j] == modifiedMessages[i].Id)
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found);
            }

            // Find the messages from the inbox that have been updated since the last week
            var query = new YouMailMessageQuery(QueryType.GetMessagesSince)
            {
                FolderId = YouMailService.InboxFolder,
                IncludeFullName = true,
                UpdatedFrom = DateTime.Now - TimeSpan.FromDays(7)
            };
            var lastWeekMessages = (await service.GetMessagesFromQueryAsync(query)).Data;

            int newMessages = 0;
            int readMessages = 0;
            foreach (var message in lastWeekMessages)
            {
                if (message.Status == MessageStatus.New)
                {
                    newMessages++;
                }
                else if (message.Status == MessageStatus.Read)
                {
                    readMessages++;
                }
            }

            // Get the messages based on a filter
            query.MessageStatusFilter = MessageStatus.New;
            var lastWeekNewMessages = (await service.GetMessagesFromQueryAsync(query)).Data;
            // Reset the page on the query
            query.Page = 0;
            query.MessageStatusFilter = MessageStatus.Read;
            var lastWeekReadMessages = (await service.GetMessagesFromQueryAsync(query)).Data;

            Assert.AreEqual(newMessages, lastWeekNewMessages.Length);
            Assert.AreEqual(readMessages, lastWeekReadMessages.Length);
        }

        [TestMethod]
        public async Task MessageHistory()
        {
            var queryTime = DateTime.Now - TimeSpan.FromDays(30);
            var histories = await service.GetMessageHistoryAsync(queryTime, 96);
            Assert.IsNotNull(histories);
            Assert.IsTrue(histories.LastUpdated < (DateTime.Now + TimeSpan.FromSeconds(10)) && histories.LastUpdated > queryTime,
                $"Histories LastUpdated: {histories.LastUpdated}, Now: {DateTime.Now}");
            if (histories.Data.Length > 0)
            {
                var history = await service.GetMessageHistoryAsync(histories.Data[0].Id, 200);
                Assert.IsNotNull(history);
            }
        }

        [TestMethod]
        public async Task DeleteAndMoveMessage()
        {
            // TODO: Should simply create a new message and then delete it
            const int numberofmessages = 3;

            // Get 3 messages...
            var messages = await service.GetMessagesAsync(YouMailService.InboxFolder, 200, DataFormat.MP3, numberofmessages);
            Assert.IsTrue(messages.Data.Length <= numberofmessages);

            // Delete a message
            await service.DeleteMessageAsync(messages.Data[0].Id);

            var message = await service.GetMessageAsync(messages.Data[0].Id, 200, DataFormat.MP3);
            var folder = await service.GetFolderAsync(YMST.c_trash);
            Assert.IsTrue(message.FolderId == folder.Id, $"The message was not deleted but moved to ${message.FolderId}");

            // Move the message back to the inbox
            await service.MoveMessageAsync(message.Id, YouMailService.InboxFolder.ToString());

            message = await service.GetMessageAsync(message.Id, 200, DataFormat.MP3);
            Assert.IsTrue(message.FolderId == YouMailService.InboxFolder);
        }

        [TestMethod]
        public async Task CreateMessage()
        {
            var now = DateTime.Now;
            await Task.Delay(100);

            Random rand = new Random();

            byte[] messageData = System.IO.File.ReadAllBytes(@"Data\Recording.mp3");
            string transcription = $"Hello, this is a new message {rand.Next(100)}";
            string callerName = $"Random Caller {rand.Next(100)}";

            YouMailMessageCreateEntry message = new YouMailMessageCreateEntry
            {
                Created = now,
                Source = service.Username,
                CallerName = callerName,
                Destination = service.Username,
                Transcription = transcription,
                MessageData = Convert.ToBase64String(messageData)
            };

            var response = await service.CreateMessageEntryAsync(message);

            var query = new YouMailMessageQuery(QueryType.GetMessagesCreatedSince)
            {
                FolderId = YouMailService.InboxFolder,
                UpdatedFrom = now,
                IncludePreview = true
            };

            // Wait a bit for the message to be fully created
            await Task.Delay(1000);

            var messages = await service.GetMessagesFromQueryAsync(query);

            Assert.IsTrue(messages.Data.Length == 1, "We should have only gotten one new message");

            await service.DeleteMessageAsync(messages.Data[0].Id);

            Assert.AreEqual(transcription, messages.Data[0].Preview, "Message Transcriptions don't match");
        }
    }
}
