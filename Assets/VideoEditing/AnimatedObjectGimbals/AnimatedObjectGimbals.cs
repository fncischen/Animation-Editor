using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedObjectGimbals : MonoBehaviour
{
    public AnimatedObject animObj;
    public InteractableObject interactableObj;

    public AnimatedGimbal gimbalX;
    public AnimatedGimbal gimbalY;
    public AnimatedGimbal gimbalZ;

    public AnimatedGimbal gimbalRotateX;
    public AnimatedGimbal gimbalRotateY;
    public AnimatedGimbal gimbalRotateZ;

    public Button activateTransformGimbalsButton;
    public Button activateRotationGimbalsButton;
    public Button saveBeizerCurveButton; // for the purposes of saving it on a curve
    public Button switchCurveButton; 

    public AnimatedObjectMenu objMenu;

    public AnimatedGimbal[] gimbals;

    public AnimatedGimbal currentlySelectedGimbal;

    public static AnimatedObjectGimbals _instance; 
    public static AnimatedObjectGimbals Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AnimatedObjectGimbals>();
            }

            return _instance; 
        }
    }


    // keep track of gimbal events
    public delegate void GimbalEvent(Vector3 newPosition);
    public GimbalEvent objectMoved;

    // when a gimbal event takes place, you have to let the animation curve know of the event that is taking place
    // 1st) let gimbal move object
    // 2nd) update animation key frame, delete that specific key frame
    // 3rd) update animation curve, and reset keyframe 
    // 4th) update animation track Keyframe UI (if the timing has been shifted)

    // the animation curve what is happening 
    private void Awake()
    {
        animObj = null;

        gimbals = new AnimatedGimbal[6];
        gimbals[0] = gimbalX;
        gimbals[1] = gimbalY;
        gimbals[2] = gimbalZ;

        gimbals[3] = gimbalRotateX;
        gimbals[4] = gimbalRotateY;
        gimbals[5] = gimbalRotateZ;

        foreach(AnimatedGimbal gimbal in gimbals)
        {
            gimbal.animObjGimbals = this; 
        }
    }

    // Update is called once per frame

    void CheckForSelectingGimbals()
    {
        Ray ray = Camera.main.ViewportPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) & hit.collider.GetComponent<AnimatedGimbal>()){
            if(currentlySelectedGimbal != hit.collider.GetComponent<AnimatedGimbal>())
            {
                currentlySelectedGimbal = hit.collider.GetComponent<AnimatedGimbal>();
            } 
        } 
    }

    public void ActivateGimbalButtons() 
    {
        activateTransformGimbalsButton.gameObject.SetActive(true);
        activateRotationGimbalsButton.gameObject.SetActive(true);
    }

    public void DeactivateGimbalButtons()
    {
        activateTransformGimbalsButton.gameObject.SetActive(false);
        activateRotationGimbalsButton.gameObject.SetActive(false);
    }
    public void ActivateTransformGimbals()
    {
        if(gimbalRotateX.gameObject.activeSelf & gimbalRotateY.gameObject.activeSelf & gimbalRotateZ.gameObject.activeSelf)
        {
            gimbalRotateX.gameObject.SetActive(false);
            gimbalRotateY.gameObject.SetActive(false);
            gimbalRotateZ.gameObject.SetActive(false);

        }

        gimbalX.gameObject.SetActive(true);
        gimbalY.gameObject.SetActive(true);
        gimbalZ.gameObject.SetActive(true);
    }

    public void ActivateRotationGimbals()
    {
        if (gimbalX.gameObject.activeSelf & gimbalY.gameObject.activeSelf & gimbalZ.gameObject.activeSelf)
        {
            gimbalX.gameObject.SetActive(false);
            gimbalY.gameObject.SetActive(false);
            gimbalZ.gameObject.SetActive(false);
        }

        gimbalRotateX.gameObject.SetActive(true);
        gimbalRotateY.gameObject.SetActive(true);
        gimbalRotateZ.gameObject.SetActive(true);
    }

    public void deactivateAllGimbals()
    {
        gimbalX.gameObject.SetActive(false);
        gimbalY.gameObject.SetActive(false);
        gimbalZ.gameObject.SetActive(false);

        gimbalRotateX.gameObject.SetActive(false);
        gimbalRotateY.gameObject.SetActive(false);
        gimbalRotateZ.gameObject.SetActive(false);
    }

    public void ActivateGimbalMenu()
    {
        objMenu.gameObject.SetActive(true);
    }

    public void DeactivateGimbalMenu()
    {
        objMenu.gameObject.SetActive(false);

    }

    public void ActivateBeizerRelatedButtons()
    {
        saveBeizerCurveButton.gameObject.SetActive(true);
        switchCurveButton.gameObject.SetActive(true);
    }

    public void deactivateBeizerRelatedButtons()
    {
        saveBeizerCurveButton.gameObject.SetActive(false);
        switchCurveButton.gameObject.SetActive(false);
    }

    public void saveBeizerCurve()
    {
        animObj.beizerPathGroup.beizerPathData.saveUpdatedBeizerCurve();
    }

    public void toggleSwitchCurve()
    {
        if (animObj.IsBeizer)
        {
            switchCurveToLinear();
            animObj.IsBeizer = false;
        }
        else
        {
            switchCurveToBeizer();
            animObj.IsBeizer = true; 
        }
    }

    public void switchCurveToBeizer()
    {
        animObj.animTrack.SetAnimationCurveToBeizerCurve();
    }

    public void switchCurveToLinear()
    {
        animObj.animTrack.UpdateCurve();
    }
}
