using UnityEngine;

namespace Game1
{
    public enum FoodType
    {
        Carrot,
        Grass,
        Worm,
        Banana,
        Bamboo,
        Apple
    }

    [System.Serializable]
    public struct AnimalData
    {
        public string animalName;
        public Sprite animalSprite;
        public FoodType correctFood;
    }
}
