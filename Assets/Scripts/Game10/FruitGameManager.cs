using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class FruitGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI instructionText;
    public Button submitButton;
    public TextMeshProUGUI levelText;

    [Header("Fruit Data")]
    public List<FruitData> fruitDataList; // Assign Apple, Orange, Banana, Grapes here
    public GameObject fruitPrefab; // Base prefab with FruitItem script
    public Transform basketPosition;
    public Transform spawnAreaMin;
    public Transform spawnAreaMax;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int fruitsToSpawn = 8;

    private Dictionary<FruitType, int> targetRequirements = new Dictionary<FruitType, int>();
    private List<FruitItem> currentCollection = new List<FruitItem>();
    private List<GameObject> spawnedFruits = new List<GameObject>();

    private void Start()
    {
        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCollection);
            
        StartLevel();
    }

    public void StartLevel()
    {
        ClearLevel();
        GenerateTarget();
        UpdateUI();
        SpawnFruits();
    }

    private void ClearLevel()
    {
        foreach (var fruit in spawnedFruits) if (fruit != null) Destroy(fruit);
        spawnedFruits.Clear();
        currentCollection.Clear();
        targetRequirements.Clear();
    }

    private void GenerateTarget()
    {
        // Randomly pick 1-2 fruit types for the target
        int typesCount = Random.Range(1, 3);
        List<FruitData> availableTypes = new List<FruitData>(fruitDataList);

        for (int i = 0; i < typesCount; i++)
        {
            if (availableTypes.Count == 0) break;
            int randomIndex = Random.Range(0, availableTypes.Count);
            FruitData picked = availableTypes[randomIndex];
            
            // Target: Collect 1 to 3 of this type
            int count = Random.Range(1, 4); 
            targetRequirements[picked.fruitType] = count;
            availableTypes.RemoveAt(randomIndex);
        }
    }

    private void UpdateUI()
    {
        if (levelText != null) levelText.text = $"Level: {currentLevel}";

        string instruction = "Collect: ";
        List<string> parts = new List<string>();
        foreach (var req in targetRequirements)
            parts.Add($"{req.Value} {req.Key}(s)");
        
        instruction += string.Join(" and ", parts);
        if (instructionText != null) instructionText.text = instruction;
    }

    private void SpawnFruits()
    {
        for (int i = 0; i < fruitsToSpawn; i++)
        {
            FruitData data = fruitDataList[Random.Range(0, fruitDataList.Count)];
            Vector3 randomPos = new Vector3(
                Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x),
                Random.Range(spawnAreaMin.position.y, spawnAreaMax.position.y), 0);

            GameObject fruitObj = Instantiate(fruitPrefab, randomPos, Quaternion.identity);
            FruitItem item = fruitObj.GetComponent<FruitItem>();
            item.Setup(data.fruitType, this);
            
            // Set appearance from FruitData
            fruitObj.GetComponent<SpriteRenderer>().sprite = data.fruitSprite;
            spawnedFruits.Add(fruitObj);
        }
    }

    public void CollectFruit(FruitItem fruit)
    {
        currentCollection.Add(fruit);
        fruit.MoveToBasket(basketPosition.position);
    }

    public void SubmitCollection()
    {
        if (ValidateCollection()) WinLevel();
        else FailLevel();
    }

    private bool ValidateCollection()
    {
        Dictionary<FruitType, int> collectedCounts = new Dictionary<FruitType, int>();
        foreach (var item in currentCollection)
        {
            if (collectedCounts.ContainsKey(item.fruitType)) collectedCounts[item.fruitType]++;
            else collectedCounts[item.fruitType] = 1;
        }

        // Exact Match Check
        foreach (var req in targetRequirements)
        {
            int collected = collectedCounts.ContainsKey(req.Key) ? collectedCounts[req.Key] : 0;
            if (collected != req.Value) return false;
        }

        // Check for any extra/wrong fruits
        foreach (var coll in collectedCounts)
            if (!targetRequirements.ContainsKey(coll.Key)) return false;

        return true;
    }

    private void WinLevel()
    {
        Debug.Log("Success! Basket filled.");
        currentLevel++;
        StartLevel(); // In a real game, you might add a delay or animation here
    }

    private void FailLevel()
    {
        Debug.Log("Wrong amount! Resetting to Level 1.");
        currentLevel = 1;
        StartLevel();
    }
}