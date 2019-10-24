using System;
using System.Linq; 
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEditor; 

/// <summary>
/// Purpose:
/// 
/// To handle all Animation KeyFrame UI events
/// To act as a bridge between Animation Object and Curve
/// 
/// This is the interfact for the animation curve that the animation object interacts with and the timeline menu invokes.
/// 
/// </summary>
public class AnimationTrack : MonoBehaviour
{

    #region public variables
    // the game object that the animation track the object is attached to 
    public AnimatedObject animatedObject;

    public AnimationPlayableOutput animationPlayableOutput;
    public AnimationMixerPlayable animMixerPlayable;
    public AnimationClipPlayable animClipPlayable; 
    public AnimationClip clip;
    public Timeline timeline;

    public VideoKeyFrame currentlySelectedVideoKeyframe; 
    public Keyframe currentlySelectedKeyframe; 

    // might be easier to use the videoKeyFrames asset 
    public List<VideoKeyFrame> videoKeyFrames;
    public AnimCurve animCurve;

    // call this when the path or timeline track has been modified -> we only need the index of they keyframe 

    // this event is sent here AFTER the curve has been updated
    public delegate void KeyFrameUIEvent(int keyFrameIndex, float keyFrameTime, AnimationCurveToUpdate curve);
    public KeyFrameUIEvent onKeyframeAdded; 
    public KeyFrameUIEvent onKeyframeSelected; 
    public KeyFrameUIEvent onKeyFrameUpdated;
    public KeyFrameUIEvent onKeyFrameRemoved;

    public Material unselectedColor;
    public Material selectedColor;
    
    // call this when your timeline is moving the event 
    public KeyFrameUIEvent onKeyFrameMoved; 

    // purpose // manage all Keyframe UI related events 
    // handle any changes due to zoom in, zoom out, scroll left or scroll right 
    // render all Keyframe UI 

    #endregion
    private void Awake()
    {
    }

    public void SubscribeToAnimationEvents()
    {
        // two ways to add key frame -> invoking it near the game object 
        animatedObject.onObjectAdded += animCurve.addKeyFrameToCurve;
        animatedObject.onObjectUpated += animCurve.updateKeyFrameOnCurve;

        // or adjust on timeline UI by walking to the timeline and invoking the timeline buttons


        // begin adding, updating, or removing Keyframe UIs --> implement check functions to ensure
        // UI consistency 

    }

    // determine invokers 
    public void IncreaseTrackWidth()
    {

    }

    public void DecreaseTrackWidth()
    {

    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("selecting!");
            Debug.Log(gameObject.name);
            if (timeline.currentlySelectedTrack != gameObject)
            {
                Debug.Log("Select this!");
                timeline.currentlySelectedTrack = gameObject;
                ToggleAnimationTrackMaterial();
                foreach (AnimatedGimbal gimbal in animatedObject.gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(true);

                }
            }
            else
            {
                Debug.Log("Unselect this!");
                timeline.currentlySelectedTrack = null;
                ToggleAnimationTrackMaterial();
                foreach (AnimatedGimbal gimbal in animatedObject.gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(false);
                }
            }

        }
        
    }

    public void OnMouseUp()
    {
        
    }

    public void ToggleAnimationTrackMaterial()
    {
        if(timeline.currentlySelectedTrack != gameObject)
        {
            GetComponent<MeshRenderer>().material = unselectedColor;

        }
        else
        {
            GetComponent<MeshRenderer>().material = selectedColor;

        }
    }

    #region track configuration methods
    public AnimationTrack(AnimatedObject aObj, Vector3 trackPosition, Timeline t, int trackNumber)
    {
        animatedObject = aObj;
        timeline = t;

        transform.parent = timeline.transform;
        transform.localPosition = trackPosition;

        // SetUpTrackProperties(trackNumber);
    }

    public void SetUpTrackProperties(AnimatedObject aObj, Vector3 trackPosition, Timeline t, int i, Material unselectedCol, Material selectedCol)
    { 
        animatedObject = aObj;
        timeline = t;
        aObj.beizerPathGroup.beizerPathData.animTrack = this; 

        transform.parent = timeline.transform;
        transform.localPosition = trackPosition;

        clip = new AnimationClip();
        clip.name = $"FunClip {i}";
        clip.wrapMode = WrapMode.Loop;
        Debug.Log("wrapMode " + clip.wrapMode);

        unselectedColor = unselectedCol;
        selectedColor = selectedCol;

        AnimationClipSettings tSettings = AnimationUtility.GetAnimationClipSettings(clip);
        Debug.Log(tSettings.loopTime);
        tSettings.loopTime = true;
        Debug.Log(tSettings.loopTime);

        AnimationUtility.SetAnimationClipSettings(clip, tSettings);


        AssetDatabase.CreateAsset(clip, $"Assets/clip_{i}.anim");
        animCurve = ScriptableObject.CreateInstance<AnimCurve>();
        animCurve.clip = clip;
        animCurve.animTrack = this;
        animCurve.SetupAnimationCurves();
        timeline.trackSectionData.ConnectTracksToPlayableGraph(clip, animatedObject.GetComponent<Animator>());
        SubscribeToAnimationEvents();

        Debug.Log(animCurve);
    }

    public AnimationTrack(AnimationPlayableOutput apo, AnimationMixerPlayable amp, AnimationClipPlayable acp, AnimationClip c, GameObject objToAnimate)
    {
        animationPlayableOutput = apo;
        animMixerPlayable = amp;
        animClipPlayable = acp;
        animCurve = new AnimCurve();
        animCurve.animTrack = this;

        animatedObject =  objToAnimate.AddComponent<AnimatedObject>();
        clip = c; 
    }

    #endregion
    public void UpdateCurve(AnimCurve animCurve)
    {
        clip.SetCurve("", typeof(Transform), "localPosition.x", animCurve.curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", animCurve.curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", animCurve.curveZ);

        clip.SetCurve("", typeof(Transform), "localRotation.x", animCurve.rotationXcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", animCurve.rotationYcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", animCurve.rotationZcurve);
    }
    public void SelectKeyframeUI(Keyframe keyframe, AnimationCurveToUpdate curve)
    {
        // first --> check if this keyframe exists 
        foreach (VideoKeyFrame videoKeyframe in videoKeyFrames)
        {
            // first check the time to see if the keyframe time matches the video keyframe
            if (keyframe.time == videoKeyframe.keyframeTime)
            {
                currentlySelectedKeyframe = keyframe;
                currentlySelectedVideoKeyframe = videoKeyframe;
                break;
            }
        }
    }

    public void DeselectKeyframeUI()
    {
        currentlySelectedKeyframe = default(Keyframe);
        currentlySelectedVideoKeyframe = null; 
    }
    // these are timeline methods

    #region keyframe UI Manipulation methods

    // invoke when zooming in, zooming out, or retrieving a timeline with saved keyframes
    public void GenerateKeyframeUI()
    {

        // think of the coordinate system
        float zPosOfTrack = transform.localPosition.z;
        
        // there is a chance that this is right of 0,0


        // float keyframePositionInTimelineTrackSpace = (timeline.timelineTicker.transform.localPosition.z + (timeline.TimelineHalfWidth - timeline.TimelineOwnerWidth)) * timeline.ratioBtwnMaxWidthAndMaxTime;
        // timeline space
        foreach (VideoKeyFrame vk in videoKeyFrames)
        {
            // this is accessible due to the properties already set by videokeyframe
            VideoKeyFrame keyframe = Instantiate(vk, transform);

            float currTime = keyframe.keyframeTime;
            // check what the time is from the keyframe

            // if the keyframe time is within the clamp, we can instantiate it
            if (isTimeWithinTimelineClampedTime(currTime))
            {
                keyframe.transform.localPosition = timelinePositionAtKeyframeTime(currTime);
            }
        }
        
    }

    /// <summary>
    /// Invoke when the timeline is zoomed in or zoom out
    /// </summary>
    public void ZoomInZoomOutKeyframeUI() 
     {
        DestoryExistingKeyframesUI();
        GenerateKeyframeUI();
     } 


    public void DestoryExistingKeyframesUI()
    {
        foreach(VideoKeyFrame vk in videoKeyFrames)
        {
            Destroy(vk.gameObject);
        }
    }

    // question - should this be invoked after the keyframe has been added to the curve? - answer, YES, after anim Curve has completed curve data update for all 3 positions
    
    public void AddKeyFrameUI(float time, out VideoKeyFrame newK)
    {
        int keyframeIndex; 
        if (isVideoKeyFrameHere(time, out keyframeIndex))
        {
            Destroy(videoKeyFrames[keyframeIndex].gameObject);
           
        } 
            GameObject keyframeObj = Instantiate(timeline.keyframePrefab, transform.parent);
            newK = keyframeObj.GetComponent<VideoKeyFrame>();
            newK.keyframeTime = time; 

            videoKeyFrames.Add(newK);

            sortVideoKeyframesByTime(); // ensure the correct indices in the track for proper retrival of keyframes

            newK.animTrack = this;

            // get keyframe indices and insert them onto videokeyframe
            int keyframePosIndexX = animCurve.getKeyframeIndicesFromCurve(timeline.timelineTicker.currentTime, AnimationCurveToUpdate.x);
            int keyframePosIndexY = animCurve.getKeyframeIndicesFromCurve(timeline.timelineTicker.currentTime, AnimationCurveToUpdate.y);
            int keyframePosIndexZ = animCurve.getKeyframeIndicesFromCurve(timeline.timelineTicker.currentTime, AnimationCurveToUpdate.z);

        Debug.Log("keyframeIndexX " + keyframePosIndexX + " keyframePosIndexY " + keyframePosIndexY + " keyframeIndexZ " + keyframePosIndexZ);

            newK.keyframeXindex = keyframePosIndexX;
            newK.keyframeYindex = keyframePosIndexY;
            newK.keyframeZindex = keyframePosIndexZ;

         
        if (isTimeWithinTimelineClampedTime(time))
         {
            newK.transform.localPosition = timelinePositionAtKeyframeTime(time);
         }
        else
        {
            keyframeObj.SetActive(false);
        }

    }

    // invoked when the user is dragging the keyframe
    // when OnDrag is called on the Keyframe, then this will be called 

    // whereas the update feature is called when 

    // update keyframe is pressed (when you're NOT in real time edit mode)
    // or when the animObj has been dragged by the gimbals and real time edit mode has been enabled) 
    
    // in this situation, we'd get the keyframeIndex from the Keyframe and call this function 

    // call the beizer paths to change the values of the times on the animation curve
    public void MoveKeyFrameUI(int keyframeIndex, float newTime, AnimationCurveToUpdate curve)
    {
        Keyframe oldKey;
        Keyframe updatedKey; 
        // get the selected keyframe       
        // set the selected keyframe time to equal the keyframe time on the event
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                oldKey = animCurve.curveX.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.curveX.MoveKey(keyframeIndex, updatedKey);
                clip.SetCurve("", typeof(Transform), "localPosition.x", animCurve.curveX);

                break;
            case AnimationCurveToUpdate.y:
                oldKey = animCurve.curveY.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.curveY.MoveKey(keyframeIndex, updatedKey);
                clip.SetCurve("", typeof(Transform), "localPosition.y", animCurve.curveY);

                break;
            case AnimationCurveToUpdate.z:
                oldKey = animCurve.curveZ.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.curveZ.MoveKey(keyframeIndex, updatedKey);
                clip.SetCurve("", typeof(Transform), "localPosition.z", animCurve.curveZ);


                break;
            case AnimationCurveToUpdate.rotationX:
                oldKey = animCurve.rotationXcurve.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.rotationXcurve.MoveKey(keyframeIndex, updatedKey);

                clip.SetCurve("", typeof(Transform), "localRotation.x", animCurve.rotationXcurve);
                break;
            case AnimationCurveToUpdate.rotationY:
                oldKey = animCurve.rotationYcurve.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.rotationYcurve.MoveKey(keyframeIndex, updatedKey);

                clip.SetCurve("", typeof(Transform), "localRotation.y", animCurve.rotationYcurve);
                break;
            case AnimationCurveToUpdate.rotationZ:
                oldKey = animCurve.rotationZcurve.keys[keyframeIndex];
                updatedKey = new Keyframe(newTime, oldKey.value);
                animCurve.rotationZcurve.MoveKey(keyframeIndex, updatedKey);

                clip.SetCurve("", typeof(Transform), "localRotation.z", animCurve.rotationZcurve);
                break;
        }

        // call the animationCurves of the BeizerCurve -> use the time difference between the nearest left or right control point to change the time it takes from the intermediate points
        // to change the motion
    }

    public void RemoveKeyFrameUI(int keyframeIndex, float newTime, AnimationCurveToUpdate curve)
    {
        sortVideoKeyframesByTime();
    }

    #endregion


    #region helper functions
    public bool isVideoKeyFrameHere(float time, out int keyframeIndex)
    {
        keyframeIndex = 0; 
        foreach(VideoKeyFrame k in videoKeyFrames)
        {
            if(k.keyframeTime == time)
            {
                keyframeIndex = videoKeyFrames.IndexOf(k);
                return true;
            }
        } 
        return false; 
    }
    public bool isKeyFrameHere(float time, AnimationCurveToUpdate curve)
    {
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                Keyframe[] xkeys = animCurve.curveX.keys;
                foreach(Keyframe k in xkeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false; 
            case AnimationCurveToUpdate.y:
                Keyframe[] ykeys = animCurve.curveY.keys;
                foreach (Keyframe k in ykeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false;
            case AnimationCurveToUpdate.z:
                Keyframe[] zkeys = animCurve.curveZ.keys;
                foreach (Keyframe k in zkeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false;
            case AnimationCurveToUpdate.rotationX:
                Keyframe[] xRotateKeys = animCurve.rotationXcurve.keys;
                foreach (Keyframe k in xRotateKeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false;
            case AnimationCurveToUpdate.rotationY:
                Keyframe[] yRotateKeys = animCurve.rotationYcurve.keys;
                foreach (Keyframe k in yRotateKeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false;
            case AnimationCurveToUpdate.rotationZ:
                Keyframe[] zRotateKeys = animCurve.rotationZcurve.keys;
                foreach (Keyframe k in zRotateKeys)
                {
                    if (k.time == time)
                    {
                        return true;
                    }
                }
                return false;
        }
        return false; 
    }

    public Vector3 timelinePositionAtKeyframeTime(float currTime)
    {
       float xPos = 2f; // extrude outwards 
       float yPos = 0f; // because we want this to be at center of the animation track
       float zPos = currTime - (timeline.TimelineHalfWidth - timeline.TimelineOwnerWidth) * timeline.ratioBtwnMaxWidthAndMaxTime;// we are going backwards 

       return new Vector3(xPos, yPos, zPos);
    }

    private void sortVideoKeyframesByTime()
    {
        Dictionary<float, VideoKeyFrame> orderedVideoKeyframes = new Dictionary<float, VideoKeyFrame>();
        foreach(VideoKeyFrame vk in videoKeyFrames)
        {
            orderedVideoKeyframes.Add(vk.keyframeTime, vk);
        }

       List<float> times =  orderedVideoKeyframes.Keys.ToList<float>();
       times.Sort();

       videoKeyFrames = new List<VideoKeyFrame>();

        // this is sorted now
        foreach(float time in times)
        {
            videoKeyFrames.Add(orderedVideoKeyframes[time]);
        }

    }

    public bool isTimeWithinTimelineClampedTime(float currTime)
    {
        if (currTime >= timeline.trackSectionData.currentFromTimeClamp && currTime <= timeline.trackSectionData.currentToTimeClamp)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    #endregion
}
