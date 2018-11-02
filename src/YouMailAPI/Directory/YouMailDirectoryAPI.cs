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
    using MagikInfo.XmlSerializerExtensions;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        public async Task<YouMailPhoneRecord> LookupPhoneNumberAsync(string number)
        {
            YouMailPhoneRecord phoneRecord = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    var url = string.Format(YMST.c_directoryLookup, number);
                    using (var response = await YouMailApiAsync(url, null, HttpMethod.Get, true, null))
                    {
                        if (response != null)
                        {
                            var records = response.GetResponseStream().FromXml<YouMailPhoneRecords>();
                            if (records != null && records.PhoneRecords != null && records.PhoneRecords.Count > 0)
                            {
                                phoneRecord = records.PhoneRecords[0];
                            }
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return phoneRecord;
        }

    }
}
