using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameStartData", order = 1)]
public class GameStartData : ScriptableObject
{
    public bool IsRelease;
    public bool IsDevDebug;

    [Header("Data")]
    public Color _textColor;
    public Color _bloodRedColor;
    public Color _communistRedColor;

    public int FoodValue = 10;
    public int BeerBalue = 8;
    public int MagazineValue = 10;
    public const int ConsumablePOIS = 3;

    [Header("Initial Save File")]
    public bool FirstLaunch = true;
    public int Saturation = 40;
    public int Sanity = 80;
    public int SurvivedDays = 0;
    public Difficulty Difficulty = Difficulty.Normal;
    public bool Headbob = true;
}
