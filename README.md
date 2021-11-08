# YouMail API .NET Library #

## Copyright (c) Gilles Khouzam 2018 ##

### Licensed under the [MIT License](https://opensource.org/licenses/MIT)

This is a .NET implementation of a [YouMail](https://www.youmail.com) client library. Current support is for .NETCore 2.1, .NETStandard 2.0 and .NET 4.5.

The YouMail API Documentation can be found [here](https://api.youmail.com/docs/)

To use:
1. Instantiate a MagikInfo.YouMailAPI.YouMailService with the user and password, as well as an optional authorization token that you might have obtained earlier and the user-agent to use.
3. Invoke APIs to query the state.

```C#
var service = new YouMailService("user", "password", "AuthToken", "User-Agent/Version");
var messages = await service.GetMessagesAsync(folderId, imageSize, DataFormat.MP3);
...
```

Build Status: ![Build Status](https://magikinfo.visualstudio.com/YouMailAPI/_apis/build/status/YouMailAPI-CI)
