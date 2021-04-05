using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using WordFudge.CloudService;
using WordFudge.Save;

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

            dbLoader.FinishedLoadingDatabase += OnFinishedLoadingDatabase;
            dbLoader.LoadDatabase();
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError($"Couldn't log into PlayFab!\n{error.GenerateErrorReport()}");
            throw new NotImplementedException();
        }

        private void OnFinishedLoadingDatabase(bool success)
        {
            if(!success)
            {
                //todo error
                Debug.LogError("Coulnd't load database.");

                return;
            }

            Debug.Log("Finished loading database.");

            //load main menu?
        }
    }
}
