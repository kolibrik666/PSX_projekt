using UnityEngine;

public class GameRunData
{
    private int _saturation = default;
    private int _sanity = default;
    public int Saturation
    {
        get { return _saturation; }
        set { _saturation = Mathf.Clamp(value, 0, 100); }   
    }
    public int Sanity
    {
        get { return _sanity; }
        set { _sanity = Mathf.Clamp(value, 0, 100); }
    }
    public int SurvivedDays = default;
}
