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
    public enum VoicemailResult
    {
        General = 1,
        DeviceLogForward = 51,
        DeviceLogAnswer = 52,
        DeviceLogOutBound = 53,
        LeftMessage = 101,
        NoMessageLeft = 102,
        Blocked = 103,
        Complete = 104,
        Pickup = 105,
        Activation = 106,
        NonUser = 107,
        Conference = 108,
        MenuForward = 109,
        MenuVMDifferentUser = 110,
        MenuVMInFolder = 111,
        ForwardingVerification = 112,
        VMForwardingVerification = 113,
        TuiRetrievalSwitch = 114,
        VMSpamDeny = 115,
        VMNonUserRetrievalPickup = 116,
        BlockedNumberPrompt = 117,
        BlockedPassword = 118,
        VirtualNumberForward = 119,
        VirtualNumberForwardHangup = 120,
        MenuForwardHangug = 121,
        ConferenceForward = 122,
    }
}
