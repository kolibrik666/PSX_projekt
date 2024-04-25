using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake()
    {
        if(gameObject.layer == 0) gameObject.layer = 9;
    }
    public virtual void OnInteract()
    {
    }
    public virtual void OnHold()
    {
    }
    public virtual void OnFocus()
    {
    }
    public virtual void OnLoseFocus()
    {
    }

}
