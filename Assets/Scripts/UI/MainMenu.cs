using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WordFudge.Ui
{
    public class MainMenu : BaseMenu
    {
        [SerializeField]
        private Button startGamebutton;
        [SerializeField]
        private Button leaderboardsButton;
        [SerializeField]
        private Leaderboards leaderboards;

        void Start()
        {
            Assert.IsNotNull(startGamebutton);
            Assert.IsNotNull(leaderboardsButton);
            Assert.IsNotNull(leaderboards);

            startGamebutton.onClick.AddListener(OnStartGameButtonClicked);
            leaderboardsButton.onClick.AddListener(OnLeaderboardsbuttonClicked);
        }

        private void OnDestroy()
        {
            startGamebutton.onClick.RemoveAllListeners();
            leaderboardsButton.onClick.RemoveAllListeners();
        }

        private void OnStartGameButtonClicked()
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        }

        private void OnLeaderboardsbuttonClicked()
        {
            leaderboards.Show();
        }
    }
}
