using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : BeizerPoint
{
    public AnimatedObjectGimbals gimbals;
    public AnimatedObjectMenu objMenu;
    protected AnimatedObject animObj;

    public ControlPoint leftControlPoint;
    public ControlPoint rightControlPoint;
    private void Start()
    {
        animObj = videoKeyframe.animTrack.animatedObject;

        gimbals = AnimatedObjectGimbals.Instance;
        Debug.Log("creating " + gimbals);

        // objMenu.gameObject.SetActive(false);
    }

    // activate the 1st gimbals + menus

    // second if the control point has been moved, notify the intermediate points they need to be moved 
    public void OnMouseDown()
    {
        Debug.Log("touching " + gimbals);
          if (gimbals.interactableObj != this)
              {
                if (!gimbals.gameObject.active)
                {
                    gimbals.gameObject.SetActive(true);
                }
                gimbals.interactableObj = this;
                gimbals.animObj = null; 
                gimbals.transform.position = transform.position;
                foreach (AnimatedGimbal gimbal in gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(true);
                }
                SubscribeToGimbalEvents();
             }
            else
             {
                gimbals.gameObject.SetActive(false);
                gimbals.animObj = null;
                gimbals.interactableObj = null; 
                foreach (AnimatedGimbal gimbal in gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(false);
                    objMenu.gameObject.SetActive(false);
                }

                UnsubscribeToGimbalEvents();
            }
     }


    public override void SubscribeToGimbalEvents()
    {
        gimbals.gimbalX.onGimbalMoving += MoveIntermediatePoints;
        gimbals.gimbalY.onGimbalMoving += MoveIntermediatePoints;
        gimbals.gimbalZ.onGimbalMoving += MoveIntermediatePoints;

    }

    public override void UnsubscribeToGimbalEvents()
    {
        gimbals.gimbalX.onGimbalMoving -= MoveIntermediatePoints;
        gimbals.gimbalY.onGimbalMoving -= MoveIntermediatePoints;
        gimbals.gimbalZ.onGimbalMoving -= MoveIntermediatePoints;
    }

    // invoke this function when gimbal moves a control point - it will get the control points and move 
    public void MoveIntermediatePoints()
    {
        animObj.beizerPathGroup.beizerPathData.updateBeizerCurveSection(this);
    }
}
