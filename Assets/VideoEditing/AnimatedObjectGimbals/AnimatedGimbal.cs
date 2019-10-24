using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// gimbals == input system 

// input system software architecture
/// <summary>
/// Note: Let your input system send events to the different AR components of the game. 
/// </summary>
/// 

// another way to keep gimbal positions consistent is to make them separate game objs from the animationObj
public class AnimatedGimbal : MonoBehaviour
{
    public AnimatedObjectGimbals animObjGimbals;
    public delegate void GimbalMoveEvent(Vector3 newPos, AnimationCurveToUpdate curve);
    public delegate void GimbalMovingEvent();
    public GimbalMoveEvent onGimbalMoved;
    public GimbalMovingEvent onGimbalMoving; 

    public delegate void GimbalRotateEvent(float rotationDelta, AnimationCurveToUpdate curve);
    public GimbalRotateEvent onGimbalRotated;

    public Vector3 screenPos;
    private Vector3 offset;

    private bool isRotateX;
    private bool isRotateY;
    private bool isRotateZ;

    private Vector2 prevPos;
    private Vector2 currPos;

    public Material nonSelectedGimbal;
    public Material selectedGimbal;

    public enum GimbalType
    {
        x,
        y,
        z,
        rotateX,
        rotateY,
        rotateZ
    }

    public GimbalType gimbalType;

    #region gimbalMovements
    private void OnMouseDown()
    {
        Debug.Log("on mouse down");
        Debug.Log(gameObject.name);
        if (gimbalType == GimbalType.x || gimbalType == GimbalType.y || gimbalType == GimbalType.z)
        {
            screenPos = Camera.main.WorldToScreenPoint(transform.position);
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z));
        
            // the question that I have is that is this offset delta going to be the same as the direction this gimbal will move? 
        }
        else if (gimbalType == GimbalType.rotateX)
        {
                Debug.Log("Rotate x");
                prevPos = Input.mousePosition;
                animObjGimbals.gimbalRotateY.GetComponent<Collider>().enabled = false;
                animObjGimbals.gimbalRotateZ.GetComponent<Collider>().enabled = false;

                 GetComponent<MeshRenderer>().material = selectedGimbal;
                isRotateX = true; 
        } 
        else if (gimbalType == GimbalType.rotateY)
        {

                Debug.Log("Rotate y");

                prevPos = Input.mousePosition;
                animObjGimbals.gimbalRotateX.GetComponent<Collider>().enabled = false;
                animObjGimbals.gimbalRotateZ.GetComponent<Collider>().enabled = false;

                GetComponent<MeshRenderer>().material = selectedGimbal;
                isRotateY = true; 


        }
        else if (gimbalType == GimbalType.rotateZ)
        {

                prevPos = Input.mousePosition;
                animObjGimbals.gimbalRotateX.GetComponent<Collider>().enabled = false;
                animObjGimbals.gimbalRotateY.GetComponent<Collider>().enabled = false;

            GetComponent<MeshRenderer>().material = selectedGimbal;

            isRotateZ = true; 

        }

    }

    private void OnMouseDrag()
    {
        // get the z distance of the mouse 

        // camera -> allow for rotation 

        // the new cursot point position
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        currPos = Camera.main.ScreenToWorldPoint(cursorPoint);

        float cursorDelta = Vector2.Angle(prevPos, cursorPoint);

        Debug.Log("on mouse drag");
        Debug.Log("dragging " + gameObject.name);


        // depending on gimbal type, constrain the direction -> seee the transform positions

        switch (gimbalType)
        {
            case GimbalType.x:
                animObjGimbals.transform.position = new Vector3(cursorPosition.x, animObjGimbals.transform.position.y, animObjGimbals.transform.position.z);
                // moveX();
                break;
            case GimbalType.y:
                animObjGimbals.transform.position = new Vector3(animObjGimbals.transform.position.x, cursorPosition.y, animObjGimbals.transform.position.z);
                // moveY();
                break;
            case GimbalType.z:
                animObjGimbals.transform.position = new Vector3(animObjGimbals.transform.position.x, animObjGimbals.transform.position.y, cursorPosition.z);
                // moveZ();
                break;
            case GimbalType.rotateX:
                if (isRotateX)
                {
                    // instead of setting it equal, starting from the axis, we should add this qunanterion 
                    animObjGimbals.animObj.transform.localRotation = Quaternion.AngleAxis(cursorDelta,Vector3.right) * animObjGimbals.animObj.transform.localRotation ;

                    // set up local rotation on gimbal 
                    transform.localRotation = Quaternion.AngleAxis(cursorDelta, Vector3.right) * transform.localRotation;

                    // constrain all other gimbals to their rotations

                    // rotateX(animObjGimbals.animObj.transform.rotation.x);
                    prevPos = currPos;
                }
                break;
            case GimbalType.rotateY:
                if(isRotateY)
                {
                    animObjGimbals.animObj.transform.localRotation = Quaternion.AngleAxis(cursorDelta, Vector3.up) * animObjGimbals.animObj.transform.localRotation;

                    transform.localRotation = Quaternion.AngleAxis(cursorDelta, Vector3.up) * transform.localRotation;

                    // constrain all other gimbals to their rotations

                    // rotateY(animObjGimbals.transform.rotation.y);
                    prevPos = currPos;

                }

                break;
            case GimbalType.rotateZ:
                if (isRotateZ)
                {

                    animObjGimbals.animObj.transform.localRotation = Quaternion.AngleAxis(cursorDelta, Vector3.forward) * animObjGimbals.animObj.transform.localRotation;
                    transform.localRotation = Quaternion.AngleAxis(cursorDelta, Vector3.forward) * transform.localRotation;

                    // constrain all other gimbals to their rotations

                    // rotateZ(animObjGimbals.transform.rotation.z);
                    prevPos = currPos;

                }
                break; 
        }

        if (animObjGimbals.animObj != null)
        {
            animObjGimbals.animObj.transform.position = animObjGimbals.transform.position;



        }
        else if (animObjGimbals.interactableObj != null)
        {
            animObjGimbals.interactableObj.transform.position = animObjGimbals.transform.position;
            if(animObjGimbals.interactableObj.GetType() == typeof(ControlPoint) || animObjGimbals.interactableObj.GetType() == typeof(IntermediateControlPoint))
            {
                if(onGimbalMoving != null)
                {
                    onGimbalMoving();
                }
            }
        
        }
    }

    private void OnMouseUp()
    {
        
        animObjGimbals.gimbalRotateX.GetComponent<Collider>().enabled = true;
        animObjGimbals.gimbalRotateY.GetComponent<Collider>().enabled = true;
        animObjGimbals.gimbalRotateZ.GetComponent<Collider>().enabled = true;

        switch (gimbalType)
        {
             case GimbalType.rotateX:
                isRotateX = false;
                GetComponent<MeshRenderer>().material = nonSelectedGimbal;
                break;
             case GimbalType.rotateY:
                isRotateY = false;
                GetComponent<MeshRenderer>().material = nonSelectedGimbal;

                break;
             case GimbalType.rotateZ:
                isRotateZ = false;
                GetComponent<MeshRenderer>().material = nonSelectedGimbal;

                break;
        }
      
    }

    #endregion

    #region eventinvokers
    private void moveX()
    {

        // Vector3 newPosDiff = offset; 
       if (onGimbalMoved != null)
       {
           // onGimbalMoved(animObjGimbals.transform.position, AnimationCurveToUpdate.x );     
       }
       // animObjGimbals.MoveGimbalSet(newPosDiff);
    }

    private void moveY()
    {
        // Vector3 newPos = animObj.transform.position + offset;
        if (onGimbalMoved != null)
        {
            // onGimbalMoved(animObjGimbals.transform.position, AnimationCurveToUpdate.y);
            
        }
        // animObjGimbals.MoveGimbalSet(newPos);
    }

    private void moveZ()
    {
        if (onGimbalMoved != null)
        {
            // onGimbalMoved(animObjGimbals.transform.position, AnimationCurveToUpdate.z);
        }

        // animObjGimbals.MoveGimbalSet(newPos);
    }


    private void rotateX(float mouseDelta)
    {

        
        if (onGimbalRotated != null)
        {
            // onGimbalRotated(mouseDelta, AnimationCurveToUpdate.rotationX);
        }

        prevPos = currPos;
    }

    private void rotateY(float mouseDelta)
    {

        if (onGimbalRotated != null)
        {
           // onGimbalRotated(mouseDelta, AnimationCurveToUpdate.rotationY);
        }
        
        prevPos = currPos; 
    }

    private void rotateZ(float mouseDelta)
    {

        if (onGimbalRotated != null)
        {
            // onGimbalRotated(mouseDelta, AnimationCurveToUpdate.rotationZ);
        }

        prevPos = currPos;

    }

    #endregion
}