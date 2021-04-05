using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using WordFudge.CloudService;
using WordFudge.Ui;

namespace WordFudge
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private MainMenu MainMenu;
        [SerializeField]
        private SplashScreen SplashScreen;

        private readonly DatabaseLoader dbLoader = new DatabaseLoader();

        private void Start()
        {
            Assert.IsNotNull(MainMenu);
            Assert.IsNotNull(SplashScreen);

            ShowSplashScreen();

            PlayFabRequests.Login(OnLoginSuccess, OnLoginFailure);

            SplashScreen.Progress = 0;
            dbLoader.ProgressUpdate += OnProgressUpdate;
        }

        private void OnDestroy()
        {
            dbLoader.ProgressUpdate -= OnProgressUpdate;
        }

        private void OnProgressUpdate(float progress)
        {
            SplashScreen.Progress = progress;
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
                SplashScreen.MainText = "Coulnd't load database.";

                return;
            }

            ShowMainMenu();

            Debug.Log("Finished loading database.");
        }

        private void ShowSplashScreen()
        {
            MainMenu.Hide();
            SplashScreen.Show();
        }

        private void ShowMainMenu()
        {
            MainMenu.Show();
            SplashScreen.Hide();
        }
    }
}
