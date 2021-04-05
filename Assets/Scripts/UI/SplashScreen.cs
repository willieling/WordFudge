using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace WordFudge.Ui
{
    public class SplashScreen : BaseMenu
    {
        [SerializeField]
        private Text message;
        [SerializeField]
        private ProgressBar progressBar;

        public float Progress
        {
            set { progressBar.Progress = value; }
        }

        public string MainText
        {
            set { message.text = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
            Assert.IsNotNull(message);
            Assert.IsNotNull(progressBar);
        }
    }
}
