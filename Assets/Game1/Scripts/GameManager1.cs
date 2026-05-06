using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace Game1
{
    public class GameManager1 : MonoBehaviour
    {
        public static GameManager1 Instance { get; private set; }

        [Header("UI References")]
        public Image animalDisplay;
        public Transform foodTray;
        public Text scoreText;
        public GameObject gameOverPanel;

        [Header("Prefabs")]
        public GameObject foodItemPrefab;

        [Header("Game Data")]
        public List<AnimalData> allAnimals;
        public Sprite[] allFoodSprites;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip correctClip;
        public AudioClip wrongClip;

        private FoodType currentWinningFood;
        private int score = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Start()
        {
            score = 0;
            UpdateScoreUI();
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            SetupLevel();
        }

        public void SetupLevel()
        {
            if (allAnimals == null || allAnimals.Count == 0)
            {
                Debug.LogError("No animal data assigned to GameManager1!");
                return;
            }

            // Clear previous food items
            foreach (Transform child in foodTray)
            {
                Destroy(child.gameObject);
            }

            // Pick a random animal
            AnimalData selectedAnimal = allAnimals[Random.Range(0, allAnimals.Count)];
            animalDisplay.sprite = selectedAnimal.animalSprite;
            currentWinningFood = selectedAnimal.correctFood;

            // Prepare foods
            List<FoodType> selectedFoods = new List<FoodType>();
            selectedFoods.Add(currentWinningFood);

            List<FoodType> possibleWrongFoods = System.Enum.GetValues(typeof(FoodType))
                .Cast<FoodType>()
                .Where(f => f != currentWinningFood)
                .ToList();

            for (int i = 0; i < 3; i++)
            {
                if (possibleWrongFoods.Count > 0)
                {
                    int randomIndex = Random.Range(0, possibleWrongFoods.Count);
                    selectedFoods.Add(possibleWrongFoods[randomIndex]);
                    possibleWrongFoods.RemoveAt(randomIndex);
                }
            }

            Shuffle(selectedFoods);

            // Instantiate and setup food items
            foreach (var foodType in selectedFoods)
            {
                GameObject foodObj = Instantiate(foodItemPrefab, foodTray);
                DraggableFood draggable = foodObj.GetComponent<DraggableFood>();
                if (draggable != null)
                {
                    draggable.SetFood(foodType, allFoodSprites[(int)foodType]);
                }
            }
        }

        public bool CheckFood(FoodType droppedFood)
        {
            if (droppedFood == currentWinningFood)
            {
                score++;
                UpdateScoreUI();
                if (audioSource != null && correctClip != null) audioSource.PlayOneShot(correctClip);
                Invoke(nameof(SetupLevel), 1f);
                return true;
            }
            else
            {
                if (audioSource != null && wrongClip != null) audioSource.PlayOneShot(wrongClip);
                TriggerGameOver();
                return false;
            }
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null) scoreText.text = "Score: " + score;
        }

        private void TriggerGameOver()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
