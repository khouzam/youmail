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
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        private static int CompareFolderIdPairs(YouMailFolder a, YouMailFolder b)
        {
            return a.Id - b.Id;
        }

        /// <summary>
        /// Get the details of a folder by name or id
        /// </summary>
        /// <param name="folderName">The folder</param>
        /// <returns></returns>
        public async Task<YouMailFolder> GetFolderAsync(string folderName)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    // TODO: This should probably be converted to a Query format to support all the options
                    var uri = $"{YMST.c_getFolder}?{YMST.c_folderId}={folderName}";
                    using (var response = await YouMailApiAsync(uri, null, HttpMethod.Get))
                    {
                        // YouMail returns an array of folders, limit it to the folder we're
                        // looking for
                        var folders = DeserializeObject<YouMailFolders>(response.GetResponseStream());
                        if (folders != null && folders.Folders != null)
                        {
                            foreach (var folder in folders.Folders)
                            {
                                if (string.Compare(folder.Name, folderName, StringComparison.CurrentCultureIgnoreCase) == 0)
                                {
                                    return folder;
                                }
                            }
                        }
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
        /// Get the list of available folders for the user
        /// </summary>
        /// <returns></returns>
        public async Task<YouMailFolder[]> GetFolderListAsync()
        {
            YouMailFolder[] folderList = null;
            try
            {
                AddPendingOp();

                if (await (LoginWaitAsync()))
                {
                    using (var response = await YouMailApiAsync(YMST.c_messageboxFoldersUrl, null, HttpMethod.Get))
                    {
                        if (response != null)
                        {
                            var folders = DeserializeObject<YouMailFolders>(response.GetResponseStream());

                            if (folders != null && folders.Folders != null)
                            {
                                var list = folders.Folders.ToList();
                                foreach (var folder in list)
                                {
                                    if (!folder.IsValid())
                                    {
                                        list.Remove(folder);
                                    }
                                }

                                list.Sort(CompareFolderIdPairs);
                                folderList = list.ToArray();
                            }
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return folderList;
        }

        /// <summary>
        /// Create a new folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="folderDescription"></param>
        /// <returns></returns>
        public async Task<YouMailFolder> CreateFolderAsync(string folderName, string folderDescription)
        {
            try
            {
                YouMailFolder newFolder = null;
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var folder = new YouMailFolder
                    {
                        Name = folderName,
                        Description = folderDescription
                    };

                    using (var response = await YouMailApiAsync(YMST.c_createFolder, SerializeObjectToHttpContent(folder, "folder"), HttpMethod.Post))
                    {
                        if (response != null)
                        {
                            newFolder = DeserializeObject<YouMailFolder>(response.GetResponseStream(), YMST.c_folder);
                        }
                    }
                }
                return newFolder;
            }
            finally
            {
                RemovePendingOp();
            }
        }

        /// <summary>
        /// Delete a user folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns>A Task to wait on</returns>
        public async Task DeleteFolderAsync(int folderId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var URL = string.Format(YMST.c_deleteFolder, folderId);
                    using (await YouMailApiAsync(URL, null, HttpMethod.Delete))
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
        /// Delete a user folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns>A Task to wait on</returns>
        public async Task MoveAllMessageFromFolderAsync(int sourceFolderId, int destFolderId)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var URL = string.Format(YMST.c_moveAllMessagesFromFolder, sourceFolderId, destFolderId);
                    using (await YouMailApiAsync(URL, null, HttpMethod.Put))
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
