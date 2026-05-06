using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game1 { // This starts the namespace
    public class GameManager1 : MonoBehaviour {
         [Header("UI References")]
    public Image animalDisplay; 
    public DraggableFood[] foodTraySlots; 

    [Header("Game Data")]
    public List<AnimalData> allAnimals; 
    public Sprite[] allFoodSprites; 

    private FoodType correctFoodForThisRound;

    void Start() {
        StartNewRandomRound();
    }

    public void StartNewRandomRound() {
        int randomAnimalIndex = Random.Range(0, allAnimals.Count);
        AnimalData chosenAnimal = allAnimals[randomAnimalIndex];

        animalDisplay.sprite = chosenAnimal.animalSprite;
        correctFoodForThisRound = chosenAnimal.correctFood;

        List<FoodType> foodsToDisplay = new List<FoodType>();
        foodsToDisplay.Add(correctFoodForThisRound);

        while (foodsToDisplay.Count < 4) {
            FoodType randomFood = (FoodType)Random.Range(0, 6);
            if (!foodsToDisplay.Contains(randomFood)) {
                foodsToDisplay.Add(randomFood);
            }
        }

        for (int i = 0; i < foodsToDisplay.Count; i++) {
            FoodType temp = foodsToDisplay[i];
            int randomIndex = Random.Range(i, foodsToDisplay.Count);
            foodsToDisplay[i] = foodsToDisplay[randomIndex];
            foodsToDisplay[randomIndex] = temp;
        }

        for (int i = 0; i < foodTraySlots.Length; i++) {
            FoodType assignedType = foodsToDisplay[i];
            foodTraySlots[i].foodType = assignedType;
            foodTraySlots[i].GetComponent<Image>().sprite = allFoodSprites[(int)assignedType];
            foodTraySlots[i].gameObject.SetActive(true);
            foodTraySlots[i].ReturnToTray();
            Image slotImage = foodTraySlots[i].GetComponent<Image>();
            slotImage.sprite = allFoodSprites[(int)type];
        }
    }
    }
} // This ends the namespace