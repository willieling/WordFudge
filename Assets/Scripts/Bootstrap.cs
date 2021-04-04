using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using WordFudge.CloudService;

namespace WordFudge
{
    public class Bootstrap : MonoBehaviour
    {
        private readonly DatabaseLoader dbLoader = new DatabaseLoader();

        private void Start()
        {
            PlayFabRequests.Login(OnLoginSuccess, OnLoginFailure);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Logged into PlayFab!");
            dbLoader.LoadDatabase();
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError($"Couldn't log into PlayFab!\n{error.GenerateErrorReport()}");
            throw new NotImplementedException();
        }
    }
}
