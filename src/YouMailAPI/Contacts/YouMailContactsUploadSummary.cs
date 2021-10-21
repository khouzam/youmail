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
    using System.Text;

    namespace MagikInfo.YouMailAPI.Contacts
    {
        class YouMailContactsUploadSummary
        {
            // contactUploadSummary": {
            public string ClientRefId { get; set; }
            public int Status { get; set; }
            public string Started { get; set; }
            public string Ended { get; set; }
            public int DeletedCount { get; set; }
            public int ErrorCount { get; set; }
            public int ExactMactchCount { get; set; }
            public bool Expired { get; set; }
            public int IgnoredCount { get; set; }
            public int ImportingTotal { get; set; }
            public int MergedCount { get; set; }
            public int NewCount { get; set; }
            public int Processed { get; set; }
            public int YmTotak { get; set; }
            //"clientRefId": "string",
            //"status": 1,
            //"started": "string",
            //"ended": "string",
            //"deletedCount": 0,
            //"errorCount": 0,
            //"exactMatchCount": 0,
            //"expired": true,
            //"ignoredCount": 0,
            //"importingTotal": 0,
            //"mergedCount": 0,
            //"newCount": 0,
            //"processed": 0,
            //"ymTotal": 0
        }
    };
}
