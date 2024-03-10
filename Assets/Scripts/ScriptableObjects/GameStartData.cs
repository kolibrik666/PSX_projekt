using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameStartData", order = 1)]
public class GameStartData : ScriptableObject
{
    public int FoodValue = 10;
    public int BeerBalue = 8;
    public int MagazineValue = 10;
    public const int ConsumablePOIS = 3;
}
