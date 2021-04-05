using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace WordFudge.Ui
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image fill;

        private float progress;

        public float Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                UpdateVisualProgress();
            }
        }

        // Start is called before the first frame update
        private void Awake()
        {
            Assert.IsNotNull(fill);
        }

        private void UpdateVisualProgress()
        {
            fill.fillAmount = Progress;
        }
    }
}
