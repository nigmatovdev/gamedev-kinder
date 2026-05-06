namespace PipePuzzle
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public GameObject winPanel;
        public TextMeshProUGUI levelText;
        public Button restartButton;
        public Button nextLevelButton;

        private int currentLevel = 0;

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
            levelText.text = "Level " + (level + 1);
            winPanel.SetActive(false);
            PipeManager.Instance.StartGame(level);
        }

        public void ShowWin()
        {
            winPanel.SetActive(true);
        }

        void RestartLevel()
        {
            StartLevel(currentLevel);
        }

        void NextLevel()
        {
            StartLevel(currentLevel + 1);
        }
    }
}
