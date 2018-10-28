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

namespace MagikInfo.XmlSerializerExtensions
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
#if UWP
    using Windows.Foundation;
#endif

    public static class TaskExtensions
    {
        public static void IgnoreException(this Task task)
        {
            task.ContinueWith((result) =>
            {
                var exception = result.Exception;
                Debug.WriteLine($"Task Failed: {result.Exception.ToString()}");
            },
            TaskContinuationOptions.OnlyOnFaulted |
            TaskContinuationOptions.ExecuteSynchronously);
        }

        public static void FailFastOnException(this Task task)
        {
            task.ContinueWith((result) =>
                Environment.FailFast($"Task Failed, Exiting: {result.Exception.ToString()}"),
                TaskContinuationOptions.OnlyOnFaulted |
                TaskContinuationOptions.ExecuteSynchronously);
        }

#if UWP
        public static void IgnoreException(this IAsyncAction action)
        {
            action.AsTask().IgnoreException();
        }
        public static void IgnoreException<TResult>(this IAsyncOperation<TResult> action)
        {
            action.AsTask().IgnoreException();
        }
#endif

        public static bool IsSuccessful(this Task task)
        {
            return task.IsCompleted && !task.IsCanceled && !task.IsFaulted;
        }
    }
}
