using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyExit : Interactable
{
    [SerializeField] GameObject _key;
    public static event Action OnKeyPickedUp;
    public override void OnFocus()
    {
        //UI + zvyrazniť pickable
    }

    public override void OnInteract()
    {       
        OnKeyPickedUp?.Invoke();
        Destroy(_key);
    }

    public override void OnHold()
    {
        
    }
    public override void OnLoseFocus()
    {
        //UI + zvyrazniť pickable
    }

}
