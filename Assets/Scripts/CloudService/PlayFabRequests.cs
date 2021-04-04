using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.CloudService
{
    public static class PlayFabRequests
    {
        public static void Login(Action<LoginResult> SuccessCallback, Action<PlayFabError> FailCallback)
        {
#if UNITY_IOS
            // I don't have a mac LOL
#elif UNITY_ANDROID
            LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest()
            {
                CreateAccount = true,
                AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                OS = SystemInfo.operatingSystem,
                AndroidDevice = SystemInfo.deviceModel
            };
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, SuccessCallback, FailCallback);
#else
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
            {
                CreateAccount = true,
                CustomId = SystemInfo.deviceUniqueIdentifier
            };
            PlayFabClientAPI.LoginWithCustomID(request, SuccessCallback, FailCallback);
#endif //Platforms
        }

        public static void GetTitleData(List<string> keys, Action<GetTitleDataResult> SuccessCallback, Action<PlayFabError> FailCallback)
        {
            GetTitleDataRequest request = new GetTitleDataRequest()
            {
                Keys = keys
            };
            PlayFabClientAPI.GetTitleData(request, SuccessCallback, FailCallback);
        }

 #if ENABLE_PLAYFABSERVER_API && !DISABLE_PLAYFAB_STATIC_API
        public static void SetTitleData(int index, string value, Action<PlayFab.ServerModels.SetTitleDataResult> successCallback, Action<PlayFabError> failureCallback)
        {
            const string WORD_TITLE_DATA_KEY = "Words_{0}";

            PlayFab.ServerModels.SetTitleDataRequest request = new PlayFab.ServerModels.SetTitleDataRequest()
            {
                Key = string.Format(WORD_TITLE_DATA_KEY, index),
                Value = value
            };
            PlayFabServerAPI.SetTitleData(request, successCallback, failureCallback);
        }
#endif //ENABLE_PLAYFABSERVER_API && !DISABLE_PLAYFAB_STATIC_API
    }
}
