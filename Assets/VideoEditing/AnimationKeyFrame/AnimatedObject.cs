using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Purpose:
/// 
/// To manage all position and rotation data of the GameObject, 
///
/// manipulated from the gimbals
/// </summary>
public class AnimatedObject : InteractableObject
{
    public AnimationTrack animTrack;
    public AnimatedObjectGimbals gimbals;
    public bool objectSelected;
    public Timeline timeline; 

    public delegate void AnimatedObjectUpdatedEvent(Keyframe keyframe, AnimationCurveToUpdate curve);

    public AnimatedObjectUpdatedEvent onObjectAdded;
    public AnimatedObjectUpdatedEvent onObjectUpated;
    public AnimatedObjectUpdatedEvent onObjectRemoved;

    // is this going to be sent to the 

    // toggle on and off on click to add keyframe 
    public AnimatedObjectMenu objMenu;

    public BeizerPathGroup beizerPathGroup;
    private void Awake()
    {
        // gimbals.enabled = false;
        gimbals = AnimatedObjectGimbals.Instance;
        objMenu.gameObject.SetActive(false);

    }

    public void Update()
    {
        if(timeline.pg.IsPlaying() && gimbals.gameObject.active && gimbals.animObj ==  this)
        {
            gimbals.transform.position = transform.position;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("selecting object!");
        if (timeline.isTimelineActivated)
        {
            if(gimbals.animObj != this)
            {
                if (!gimbals.gameObject.active)
                {
                    gimbals.gameObject.SetActive(true);
                }
                gimbals.animObj = this;
                gimbals.interactableObj = null; 
                gimbals.transform.position = transform.position;
                foreach(AnimatedGimbal gimbal in gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(true);
                    objMenu.gameObject.SetActive(true);
                    if(animTrack != null)
                    {
                        animTrack.timeline.currentlySelectedTrack = animTrack.gameObject;
                        animTrack.ToggleAnimationTrackMaterial();
                        objectSelected = true; 
                    }

                }
                if (animTrack != null)
                {
                    timeline.currentlySelectedTrack = animTrack.gameObject;
                }
                
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
                    if(animTrack != null)
                    {
                        animTrack.timeline.currentlySelectedTrack = null;
                        animTrack.ToggleAnimationTrackMaterial();
                        objectSelected = false; 
                    }

                }

                timeline.currentlySelectedTrack = null;

            }
        }
    }


    // that means the invoker must have info on the animObj you're setting they keyframe 2

    // in the keyframes, there are several things going on 

    // we need to invoke this function each time that 
    // a keyframe is generated from these times

    // so that we can place keyframes, and reference the
    // beizer curve key frame structues // control points

    // these are all position curves

    // 1st -> during initialization of the clip / track, generate a Beizer Path (quadratic)
    // 2nd -> during intialization of the clip / track, generate a Linear Path (linear)

    // keyframe generator 

    // 1st --> create a keyframe from the curve
    // 2nd --> keyframe should have a reference to the next control point
    // 3rd --> invoke generate points from t0 to t1 
    // 4th --> add those points on the path 
    // 5th --> if a beizer control point is generated between those keyframes
    // 6th --> change the intermediate keyframes between k1 and k2. 

    // [k1] ----- [k2] ---- [k3] ----- [k4] ----- [k5]

    // this checks to see if there are control points in between 
    // this position

    //  but when you put a intermediate keyframe, that keyframe has  no information
    //  on whether or not that there's keyframes on the left or the right of this keyframe

    //  do this step before adding into path 

    //  the way to check is to return the keys for all 3 position curves
    //  find the index of this new keyframe 

    //  check to see if there are indices
    //  if there is on the left, like a linked list, change the next control point inside the video keyframe on the left
    //  if there is on the right, like a linked list, change the next control point inside the video keyframe on the right 

    //  one strategy is to have a reference to the next position keyframe in each video keyframe
    //  by doing this, you can invoke the generatePoints method to

    //  we need to distingiush between intermediate keyframes and control points
    //  but the animation curves won't give us this information 

    //  one thing you could do is have a videokeyframe that flags between intermediate point and control point 
    // 

    // question --> should we use a videoKeyframe array and change each time? []
    // or should we use a list<>
    public void SetOrUpdateKeyframes()
    {
        // first, set the position curve pos at the current timeline marker
        onObjectMoved(AnimationCurveToUpdate.x);
        onObjectMoved(AnimationCurveToUpdate.y);
        onObjectMoved(AnimationCurveToUpdate.z);

        // second, set the rotation curve at the current timeline marker 
        onObjectedRotated(AnimationCurveToUpdate.rotationX);
        onObjectedRotated(AnimationCurveToUpdate.rotationY);
        onObjectedRotated(AnimationCurveToUpdate.rotationZ);

        // create a video keyframe at this time
        VideoKeyFrame vk;
        animTrack.AddKeyFrameUI(timeline.timelineTicker.currentTime, out vk);
        int vkIndex = animTrack.videoKeyFrames.IndexOf(vk);
        
        int vkLeft = vkIndex - 1;
        int vkRight = vkIndex + 1;
        
        // add control point w/ this video keyframe -> beizer control points and intermediate points are in charge of the beizer animation curves
        beizerPathGroup.beizerPathData.addControlPoint(vk);

        // generate intermediate points, depending on where the control point was created
        if (vkLeft >= 0)
        {
            beizerPathGroup.beizerPathData.generateIntermediatePoints(vkLeft, vkIndex);
            
            // set the control point defintions here
            // vk.controlPoint.leftControlPoint = animTrack.videoKeyFrames[vkLeft].controlPoint;
        }

        if (vkRight < animTrack.videoKeyFrames.Count)
        {
            beizerPathGroup.beizerPathData.generateIntermediatePoints(vkIndex, vkRight);

            // vk.controlPoint.rightControlPoint = animTrack.videoKeyFrames[vkRight].controlPoint;
        }

    }

    private void onObjectMoved(AnimationCurveToUpdate curve)
    {        
        float currentTime = animTrack.timeline.timelineTicker.currentTime;
        bool keyFrameExistsAlready = false;

        Debug.Log("on obj moved");
        // check if this key frame doesnt exist on the timeline
        // to check -> do a for loop for all the keyframes 

        // depending on the curve type 
        Keyframe[] curveKeyframes = null;
        float modifiedDimension = 0f; 
        
        if (curve == AnimationCurveToUpdate.x)
        {
            curveKeyframes = animTrack.animCurve.curveX.keys;
            modifiedDimension = transform.position.x;
        }

        else if (curve == AnimationCurveToUpdate.y)
        {
            curveKeyframes = animTrack.animCurve.curveY.keys;
            modifiedDimension = transform.position.y;

        }

        else if (curve == AnimationCurveToUpdate.z)
        {
            curveKeyframes = animTrack.animCurve.curveZ.keys;
            modifiedDimension = transform.position.z;

        }

        foreach (Keyframe keyframe in curveKeyframes)
        {
            // if it does exist, send keyFrameUpdated 
            if (keyframe.time == currentTime)
            {
                // update keyframe
                Keyframe keyframeTomodify = keyframe;
                keyframeTomodify.value = modifiedDimension;

                // send keyframe to curve and timeline 
                if(onObjectUpated != null)
                {
                    onObjectUpated(keyframeTomodify, curve);
                }

                keyFrameExistsAlready = true;
                break;
            }
        }

        if(!keyFrameExistsAlready)
        {
            Keyframe newKeyframe = new Keyframe(currentTime, modifiedDimension);

            if (onObjectAdded != null)
            {
                onObjectAdded(newKeyframe, curve);
            }
        }
        // if it doesnt exist, send keyFrameAdded
        // and generate a new Keyframe 
        
    }

    private void onObjectedRotated(AnimationCurveToUpdate curve)
    {
        float currentTime = animTrack.timeline.timelineTicker.currentTime;
        bool keyFrameExistsAlready = false;

        // check if this key frame doesnt exist on the timeline
        // to check -> do a for loop for all the keyframes 

        Keyframe[] curveKeyframes = null;
        float modifiedDimension = 0f;

        if (curve == AnimationCurveToUpdate.rotationX)
        {
            curveKeyframes = animTrack.animCurve.rotationXcurve.keys;
            modifiedDimension = transform.rotation.x;
        }

        else if (curve == AnimationCurveToUpdate.rotationY)
        {
            curveKeyframes = animTrack.animCurve.rotationYcurve.keys;
            modifiedDimension = transform.rotation.y;
        }

        else if (curve == AnimationCurveToUpdate.rotationZ)
        {
            curveKeyframes = animTrack.animCurve.rotationZcurve.keys;
            modifiedDimension = transform.rotation.z;
        }

        foreach (Keyframe keyframe in curveKeyframes)
        {
            // if it does exist, send keyFrameUpdated 
            if (keyframe.time == currentTime)
            {
                // update keyframe 
                Keyframe keyframeTomodify = keyframe;
                keyframeTomodify.value = modifiedDimension;

                // send keyframe to curve and timeline 
                if (onObjectUpated != null)
                {
                    onObjectUpated(keyframe, curve);
                }

                keyFrameExistsAlready = true;
                break;
            }
        }

        // question: should we use Keyframe or VideoKeyFrame

        // we're not storing VideoKeyFrame anymore 

        if (!keyFrameExistsAlready)
        {
            Keyframe newKeyframe = new Keyframe(currentTime, modifiedDimension);

            if (onObjectAdded != null)
            {
                onObjectAdded(newKeyframe, curve);
            }
        }
        // if it doesnt exist, send keyFrameAdded
        // and generate a new Keyframe 

    }

}

