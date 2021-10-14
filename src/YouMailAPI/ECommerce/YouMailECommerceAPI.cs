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
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class YouMailService
    {
        /// <summary>
        /// Get the ECommerce status of the user
        /// </summary>
        /// <returns>The Ecommerce status for the user</returns>
        public async Task<YouMailECommerce> GetECommerceStatusAsync()
        {
            YouMailECommerce eCommerceResult = null;
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    using (var response = await YouMailApiAsync(YMST.c_ecommerce, null, HttpMethod.Get))
                    {
                        eCommerceResult = DeserializeObject<YouMailECommerce>(response.GetResponseStream(), YMST.c_ecommerceStatus);
                        if (eCommerceResult != null &&
                            eCommerceResult.TranscriptionPlan != null &&
                            eCommerceResult.TranscriptionPlan.ProductStatus == ProductStatus.Active)
                        {
                            var status = await GetTranscriptionStatusAsync();
                            eCommerceResult.TranscriptionStatus = status;
                            eCommerceResult.TranscriptionPlan.ProductStatus = status.Active ? ProductStatus.Active : ProductStatus.Canceled;
                        }
                    }
                }
            }
            finally
            {
                RemovePendingOp();
            }

            return eCommerceResult;
        }

        /// <summary>
        /// Add an order to the user's account
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task AddOrderAsync(string product)
        {
            try
            {
                AddPendingOp();
                if (await LoginWaitAsync())
                {
                    string api = string.Format(YMST.c_addProduct, product);
                    using (await YouMailApiAsync(api, null, HttpMethod.Post))
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
