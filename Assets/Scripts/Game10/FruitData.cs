using UnityEngine;

public enum FruitType { Apple, Orange, Banana, Grapes }

[CreateAssetMenu(fileName = "New Fruit Data", menuName = "Fruit Game/Fruit Data")]
public class FruitData : ScriptableObject
{
    public FruitType fruitType;
    public string fruitName;
    public Sprite fruitSprite;
}