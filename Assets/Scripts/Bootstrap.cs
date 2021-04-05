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
        private MainMenu mainMenu;
        [SerializeField]
        private SplashScreen splashScreen;
        [SerializeField]
        private Leaderboards leaderboards;

        private readonly DatabaseLoader dbLoader = new DatabaseLoader();

        private void Start()
        {
            Assert.IsNotNull(mainMenu);
            Assert.IsNotNull(splashScreen);
            Assert.IsNotNull(leaderboards);

            ShowSplashScreen();

            PlayFabRequests.Login(OnLoginSuccess, OnLoginFailure);

            splashScreen.Progress = 0;
            dbLoader.ProgressUpdate += OnProgressUpdate;
        }

        private void OnDestroy()
        {
            dbLoader.ProgressUpdate -= OnProgressUpdate;
        }

        private void OnProgressUpdate(float progress)
        {
            splashScreen.Progress = progress;
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
                splashScreen.MainText = "Coulnd't load database.";

                return;
            }

            ShowMainMenu();

            Debug.Log("Finished loading database.");
        }

        private void ShowSplashScreen()
        {
            mainMenu.Hide();
            splashScreen.Show();
            leaderboards.Hide();
        }

        private void ShowMainMenu()
        {
            mainMenu.Show();
            splashScreen.Hide();
            leaderboards.Hide();
        }
    }
}
