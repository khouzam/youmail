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
    public class FolderTests
    {
        YouMailService service = YouMailTestService.Service;

        [TestMethod]
        public async Task GetFolderList()
        {
            var folders = await service.GetFolderListAsync();
            bool found = false;
            // Find the inbox
            foreach (var folder in folders)
            {
                if (folder.Id == YouMailService.InboxFolder)
                {
                    found = true;
                }
            }

            Assert.IsTrue(found);
        }

        [TestMethod]
        public async Task CreateAndDeleteFolder()
        {
            var rand = new Random();

            var random = rand.Next(1000);

            var folderName = string.Format("TestFolder {0}", random);
            var folderDescription = string.Format("Random Test Folder {0}", random);

            var newFolder = await service.CreateFolderAsync(folderName, folderDescription);

            Assert.IsNotNull(newFolder, "Folder did not return an object");
            Assert.IsTrue(newFolder.Id != 0, "FolderId is 0, this should not happen.");

            //await service.MoveAllMessageFromFolderAsync(newFolder.Id, service.Trash)

            // Delete the folder first in case the name or description doesn't match
            await Task.Delay(1000);
            await service.DeleteFolderAsync(newFolder.Id);

            Assert.IsTrue(newFolder.Name.CompareTo(folderName) == 0, $"Folder name doesn't match.{Environment.NewLine}Expected: {folderName}, got {newFolder.Name}");
            Assert.IsTrue(newFolder.Description.CompareTo(folderDescription) == 0, $"Folder description doesn't match.{Environment.NewLine}Expected: {folderDescription}, got {newFolder.Description}");
        }
    }
}
