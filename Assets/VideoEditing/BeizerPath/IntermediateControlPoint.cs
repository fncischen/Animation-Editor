using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateControlPoint : ControlPoint
{
    // this is the hidden control point that is not part of the keyframes in the beizer points
    // unlike this control point, this has no reference to a videokeyframe because 
    // the animated object will not travel to this point

    private void Start()
    {
        // animObj = videoKeyframe.animTrack.animatedObject;

        gimbals = AnimatedObjectGimbals.Instance;
        // objMenu.gameObject.SetActive(false);
    }

    public void setAnimatedObj(AnimatedObject a)
    {
        animObj = a; 
    }
}
