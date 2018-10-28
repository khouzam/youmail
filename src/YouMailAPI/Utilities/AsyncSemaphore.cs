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

namespace MagikInfo.Synchronization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class AsyncSemaphore : IDisposable
    {
        private Semaphore _semaphore = null;

        // Summary:
        //     Initializes a new instance of the System.Threading.Semaphore class, specifying
        //     the maximum number of concurrent entries and optionally reserving some entries.
        //
        // Parameters:
        //   initialCount:
        //     The initial number of requests for the semaphore that can be granted concurrently.
        //
        //   maximumCount:
        //     The maximum number of requests for the semaphore that can be granted concurrently.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     initialCount is greater than maximumCount.
        //
        //   System.ArgumentOutOfRangeException:
        //     maximumCount is less than 1.-or-initialCount is less than 0.
        public AsyncSemaphore(int initialCount, int maximumCount)
            : this(initialCount, maximumCount, null)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Threading.Semaphore class, specifying
        //     the maximum number of concurrent entries, optionally reserving some entries
        //     for the calling thread, and optionally specifying the name of a system semaphore
        //     object.
        //
        // Parameters:
        //   initialCount:
        //     The initial number of requests for the semaphore that can be granted concurrently.
        //
        //   maximumCount:
        //     The maximum number of requests for the semaphore that can be granted concurrently.
        //
        //   name:
        //     The name of a named system semaphore object.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     initialCount is greater than maximumCount.-or-name is longer than 260 characters.
        //
        //   System.ArgumentOutOfRangeException:
        //     maximumCount is less than 1.-or-initialCount is less than 0.
        //
        //   System.IO.IOException:
        //     A Win32 error occurred.
        //
        //   System.UnauthorizedAccessException:
        //     The named semaphore exists and has access control security, and the user
        //     does not have System.Security.AccessControl.SemaphoreRights.FullControl.
        //
        //   System.Threading.WaitHandleCannotBeOpenedException:
        //     The named semaphore cannot be created, perhaps because a wait handle of a
        //     different type has the same name.
        public AsyncSemaphore(int initialCount, int maximumCount, string name)
        {
            _semaphore = new Semaphore(initialCount, maximumCount, name);
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Threading.Semaphore class, specifying
        //     the maximum number of concurrent entries, optionally reserving some entries
        //     for the calling thread, optionally specifying the name of a system semaphore
        //     object, and specifying a variable that receives a value indicating whether
        //     a new system semaphore was created.
        //
        // Parameters:
        //   initialCount:
        //     The initial number of requests for the semaphore that can be satisfied concurrently.
        //
        //   maximumCount:
        //     The maximum number of requests for the semaphore that can be satisfied concurrently.
        //
        //   name:
        //     The name of a named system semaphore object.
        //
        //   createdNew:
        //     When this method returns, contains true if a local semaphore was created
        //     (that is, if name is null or an empty string) or if the specified named system
        //     semaphore was created; false if the specified named system semaphore already
        //     existed. This parameter is passed uninitialized.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     initialCount is greater than maximumCount. -or-name is longer than 260 characters.
        //
        //   System.ArgumentOutOfRangeException:
        //     maximumCount is less than 1.-or-initialCount is less than 0.
        //
        //   System.IO.IOException:
        //     A Win32 error occurred.
        //
        //   System.UnauthorizedAccessException:
        //     The named semaphore exists and has access control security, and the user
        //     does not have System.Security.AccessControl.SemaphoreRights.FullControl.
        //
        //   System.Threading.WaitHandleCannotBeOpenedException:
        //     The named semaphore cannot be created, perhaps because a wait handle of a
        //     different type has the same name.
        public AsyncSemaphore(int initialCount, int maximumCount, string name, out bool createdNew)
        {
            _semaphore = new Semaphore(initialCount, maximumCount, name, out createdNew);
        }

        //
        // Summary:
        //     Exits the semaphore and returns the previous count.
        //
        // Returns:
        //     The count on the semaphore before the Overload:System.Threading.Semaphore.Release
        //     method was called.
        //
        // Exceptions:
        //   System.Threading.SemaphoreFullException:
        //     The semaphore count is already at the maximum value.
        //
        //   System.IO.IOException:
        //     A Win32 error occurred with a named semaphore.
        //
        //   System.UnauthorizedAccessException:
        //     The current semaphore represents a named system semaphore, but the user does
        //     not have System.Security.AccessControl.SemaphoreRights.Modify.-or-The current
        //     semaphore represents a named system semaphore, but it was not opened with
        //     System.Security.AccessControl.SemaphoreRights.Modify.
        public int Release()
        {
            return _semaphore.Release();
        }

        //
        // Summary:
        //     Exits the semaphore a specified number of times and returns the previous
        //     count.
        //
        // Parameters:
        //   releaseCount:
        //     The number of times to exit the semaphore.
        //
        // Returns:
        //     The count on the semaphore before the Overload:System.Threading.Semaphore.Release
        //     method was called.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     releaseCount is less than 1.
        //
        //   System.Threading.SemaphoreFullException:
        //     The semaphore count is already at the maximum value.
        //
        //   System.IO.IOException:
        //     A Win32 error occurred with a named semaphore.
        //
        //   System.UnauthorizedAccessException:
        //     The current semaphore represents a named system semaphore, but the user does
        //     not have System.Security.AccessControl.SemaphoreRights.Modify rights.-or-The
        //     current semaphore represents a named system semaphore, but it was not opened
        //     with System.Security.AccessControl.SemaphoreRights.Modify rights.
        public int Release(int releaseCount)
        {
            return _semaphore.Release(releaseCount);
        }


        public void Dispose()
        {
            _semaphore.Dispose();
        }

        //
        // Summary:
        //     Blocks the current thread until the current System.Threading.WaitHandle receives
        //     a signal.
        //
        // Returns:
        //     true if the current instance receives a signal. If the current instance is
        //     never signaled, System.Threading.WaitHandle.WaitOne(System.Int32,System.Boolean)
        //     never returns.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The current instance has already been disposed.
        //
        //   System.Threading.AbandonedMutexException:
        //     The wait completed because a thread exited without releasing a mutex. This
        //     exception is not thrown on Windows 98 or Windows Millennium Edition.
        //
        //   System.InvalidOperationException:
        //     The current instance is a transparent proxy for a System.Threading.WaitHandle
        //     in another application domain.
        public async Task WaitAsync()
        {
            await Task.Run(() =>
            {
                _semaphore.WaitOne();
            });
        }
        //
        // Summary:
        //     Blocks the current thread until the current System.Threading.WaitHandle receives
        //     a signal, using a 32-bit signed integer to specify the time interval.
        //
        // Parameters:
        //   millisecondsTimeout:
        //     The number of milliseconds to wait, or System.Threading.Timeout.Infinite
        //     (-1) to wait indefinitely.
        //
        // Returns:
        //     true if the current instance receives a signal; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The current instance has already been disposed.
        //
        //   System.ArgumentOutOfRangeException:
        //     millisecondsTimeout is a negative number other than -1, which represents
        //     an infinite time-out.
        //
        //   System.Threading.AbandonedMutexException:
        //     The wait completed because a thread exited without releasing a mutex. This
        //     exception is not thrown on Windows 98 or Windows Millennium Edition.
        //
        //   System.InvalidOperationException:
        //     The current instance is a transparent proxy for a System.Threading.WaitHandle
        //     in another application domain.
        public async Task<bool> WaitOne(int millisecondsTimeout)
        {
            return await Task.Run<bool>(() => _semaphore.WaitOne(millisecondsTimeout));

            //Task.Factory.StartNew<bool>((x) => { return_semaphore.WaitOne(x); });
            //await Task.Run<bool>(waitOnSemaphore(millisecondsTimeout));
        }

    }
}
