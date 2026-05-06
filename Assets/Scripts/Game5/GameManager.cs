using UnityEngine;
using UnityEngine.SceneManagement;

namespace BridgeBuilder
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsTesting { get; private set; }
        public int CurrentLevel { get; private set; } = 0;

        public GameObject carPrefab;
        public Transform carStartPoint;
        private GameObject currentCar;

        public DragObject[] bridgePieces;

        void Awake()
        {
            Instance = this;
        }

        public void StartTest()
        {
            if (IsTesting) return;
            IsTesting = true;

            bridgePieces = Object.FindObjectsByType<DragObject>(FindObjectsSortMode.None);

            foreach (var piece in bridgePieces)
            {
                piece.SetPhysics(true);
            }

            if (currentCar) Destroy(currentCar);
            currentCar = Instantiate(carPrefab, carStartPoint.position, Quaternion.identity);
            currentCar.GetComponent<CarController>().StartDriving();
        }

        public void ResetLevel()
        {
            IsTesting = false;
            if (currentCar) Destroy(currentCar);
            
            bridgePieces = Object.FindObjectsByType<DragObject>(FindObjectsSortMode.None);
            foreach (var piece in bridgePieces)
            {
                piece.ResetPosition();
                piece.SetPhysics(false);
            }
            
            if (UIManager.Instance) UIManager.Instance.HidePopups();
        }

        public void Win()
        {
            IsTesting = false;
            if (UIManager.Instance) UIManager.Instance.ShowWin();
        }

        public void Fail()
        {
            IsTesting = false;
            if (UIManager.Instance) UIManager.Instance.ShowRetry();
        }

        public void NextLevel()
        {
            CurrentLevel++;
            if (CurrentLevel >= 3)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                ResetLevel();
                if (BridgeManager.Instance) BridgeManager.Instance.SetupLevel(CurrentLevel);
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
