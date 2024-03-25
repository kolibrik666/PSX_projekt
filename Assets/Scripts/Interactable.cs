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
    public virtual void OnInteract()
    {
    }
    public virtual void OnFocus()
    {
    }
    public virtual void OnLoseFocus()
    {
    }

}
