namespace PipePuzzle
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.SceneManagement;

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public GameObject winPanel;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI winText;
        public Button restartButton;
        public Button nextLevelButton;
        public TextMeshProUGUI nextButtonText;

        private int currentLevel = 0;
        private const int MAX_LEVELS = 3;

        void Awake()
        {
            Instance = this;
            winPanel.SetActive(false);
            restartButton.onClick.AddListener(RestartLevel);
            nextLevelButton.onClick.AddListener(NextLevel);
        }

        void Start()
        {
            StartLevel(0);
        }

        public void StartLevel(int level)
        {
            currentLevel = level;
            if (levelText != null) levelText.text = "Level " + (level + 1);
            winPanel.SetActive(false);
            PipeManager.Instance.StartGame(level);
        }

        public void ShowWin()
        {
            winPanel.SetActive(true);
            if (currentLevel + 1 >= MAX_LEVELS)
            {
                if (winText != null) winText.text = "GAME COMPLETE!";
                if (nextButtonText != null) nextButtonText.text = "MAIN MENU";
            }
            else
            {
                if (winText != null) winText.text = "LEVEL COMPLETE!";
                if (nextButtonText != null) nextButtonText.text = "NEXT LEVEL";
            }
        }

        void RestartLevel()
        {
            StartLevel(currentLevel);
        }

        void NextLevel()
        {
            if (currentLevel + 1 >= MAX_LEVELS)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                StartLevel(currentLevel + 1);
            }
        }
    }
}
