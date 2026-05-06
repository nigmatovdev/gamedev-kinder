using UnityEngine;

namespace BridgeBuilder
{
    public class BridgeManager : MonoBehaviour
    {
        public static BridgeManager Instance { get; private set; }

        public GameObject[] levelLayouts;
        public Transform carStartPoint;
        public Transform finishPoint;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            SetupLevel(0);
        }

        public void SetupLevel(int index)
        {
            for (int i = 0; i < levelLayouts.Length; i++)
            {
                levelLayouts[i].SetActive(i == index);
            }

            // Reposition car start and finish based on level if needed
            // For simplicity, we can have them as part of the level layouts
            
            if (UIManager.Instance) UIManager.Instance.UpdateLevelText(index);
        }
    }
}
