using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BridgeBuilder
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public GameObject winPopup;
        public GameObject retryPopup;
        public TextMeshProUGUI levelText;

        void Awake()
        {
            Instance = this;
            HidePopups();
        }

        public void UpdateLevelText(int level)
        {
            if (levelText) levelText.text = "Level: " + (level + 1);
        }

        public void ShowWin()
        {
            winPopup.SetActive(true);
        }

        public void ShowRetry()
        {
            retryPopup.SetActive(true);
        }

        public void HidePopups()
        {
            if (winPopup) winPopup.SetActive(false);
            if (retryPopup) retryPopup.SetActive(false);
        }

        public void OnTestClick()
        {
            GameManager.Instance.StartTest();
        }

        public void OnResetClick()
        {
            GameManager.Instance.ResetLevel();
        }

        public void OnNextLevelClick()
        {
            GameManager.Instance.NextLevel();
        }
        
        public void OnRetryClick()
        {
            GameManager.Instance.ResetLevel();
        }
        
        public void OnMainMenuClick()
        {
            GameManager.Instance.RestartGame();
        }
    }
}
