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

        void Start()
        {
            Assert.IsNotNull(startGamebutton);
            Assert.IsNotNull(leaderboardsButton);

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
            throw new NotImplementedException();
        }
    }
}
