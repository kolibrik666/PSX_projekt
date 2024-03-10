using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class Interactable : MonoBehaviour
{    
    public virtual void Awake()
    {
        gameObject.layer = 9;
    }
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();

}
