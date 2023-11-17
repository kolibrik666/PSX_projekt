using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{
    public bool opened = false;
    public Animation animator;

    public override void OnInteract()
    {
        if (!animator.isPlaying)
        {
            if (opened == false)
            {
                animator.Play("OpenDoor");
                opened = true;
            }
           else
            {
                animator.Play("CloseDoor");
                opened = false;
            }
        }
        
        print("INTERACTED WITH " + gameObject.name) ;
        
    }
    public override void OnFocus()
    {
        //print("LOOKING AT " + gameObject.name);
    }
    public override void OnLoseFocus()
    {
        print("STOP LOOKING AT " + gameObject.name);
    }
}
