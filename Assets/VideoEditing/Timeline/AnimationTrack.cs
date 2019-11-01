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
    public VideoKeyFrame[] currentlyRenderedKeyframes; 

    public AnimCurve animCurve;
    public TimelineTrackOwner timelineTrackOwner; 
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
            if (timeline.currentlySelectedTrack != this)
            {
                timeline.currentlySelectedTrack = this;
                ToggleAnimationTrackMaterial();
                foreach (AnimatedGimbal gimbal in animatedObject.gimbals.gimbals)
                {
                    gimbal.gameObject.SetActive(true);

                }
            }
            else
            {
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

 
    public void UpdateCurve()
    {
        clip.SetCurve("", typeof(Transform), "localPosition.x", animCurve.curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", animCurve.curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", animCurve.curveZ);

        clip.SetCurve("", typeof(Transform), "localRotation.x", animCurve.rotationXcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", animCurve.rotationYcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", animCurve.rotationZcurve);
    }   

    // invoke from buttons
    public void SetAnimationCurveToBeizerCurve()
    {
        clip.SetCurve("", typeof(Transform), "localPosition.x", animatedObject.beizerPathGroup.beizerPathData.curveXbeizer);
        clip.SetCurve("", typeof(Transform), "localPosition.y", animatedObject.beizerPathGroup.beizerPathData.curveYbeizer);
        clip.SetCurve("", typeof(Transform), "localPosition.z", animatedObject.beizerPathGroup.beizerPathData.curveZbeizer);

        clip.SetCurve("", typeof(Transform), "localRotation.x", animCurve.rotationXcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", animCurve.rotationYcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", animCurve.rotationZcurve);
    }
    
    #endregion

    // these are timeline methods

    #region keyframe UI Manipulation methods

    // invoke when zooming in, zooming out, or retrieving a timeline with saved keyframes
    public void UpdateKeyframeUI()
    {
        float fromTimeClamp = timeline.trackSectionData.currentFromTimeClamp;
        float toTimeClamp = timeline.trackSectionData.currentToTimeClamp;

        List<VideoKeyFrame> videoKeyFramesInClampedTime = new List<VideoKeyFrame>();
        foreach(VideoKeyFrame vk in videoKeyFrames)
        {
            if(isTimeWithinTimelineClampedTime(vk.keyframeTime))
            {
                vk.gameObject.SetActive(true);
                videoKeyFramesInClampedTime.Add(vk);
            }
            else
            {
                vk.gameObject.SetActive(false);
            }
        }

        currentlyRenderedKeyframes = new VideoKeyFrame[videoKeyFramesInClampedTime.Count];

        for(int i = 0; i < currentlyRenderedKeyframes.Length; i++)
        {
            currentlyRenderedKeyframes[i] = videoKeyFramesInClampedTime[i];
        }

        for(int i =0; i < currentlyRenderedKeyframes.Length; i++)
        {
            // float timeCoordinate = timeline.ConvertFromTimeToTimelineZPosition(currentlyRenderedKeyframes[i].keyframeTime);
            currentlyRenderedKeyframes[i].transform.localPosition = trackPositionAtKeyframeTime(currentlyRenderedKeyframes[i].keyframeTime);
            currentlyRenderedKeyframes[i].transform.localScale = timeline.keyframePrefab.transform.localScale;
        }
                
    }

    /// <summary>
    /// Invoke when the timeline is zoomed in or zoom out
    /// </summary>
    public void ZoomInZoomOutKeyframeUI() 
     {
        UpdateKeyframeUI();
     } 

    // question - should this be invoked after the keyframe has been added to the curve? - answer, YES, after anim Curve has completed curve data update for all 3 positions
    
    public void AddKeyFrameUI(float time, out VideoKeyFrame newK)
    {
        int keyframeIndex; 
        if (isVideoKeyFrameHere(time, out keyframeIndex))
        {
            Destroy(videoKeyFrames[keyframeIndex].gameObject);
           
        } 
            GameObject keyframeObj = Instantiate(timeline.keyframePrefab.gameObject, transform);
            newK = keyframeObj.GetComponent<VideoKeyFrame>();
            newK.keyframeTime = time; 
            videoKeyFrames.Add(newK);

            if(!isTimeWithinTimelineClampedTime(newK.keyframeTime))
            {
                newK.gameObject.SetActive(false);    
            }
            else
            {
                UpdateKeyframeUI();
            }


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

    public void RemoveKeyFrameUI(VideoKeyFrame vk)
    {

        videoKeyFrames.Remove(vk);
        
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeXindex,AnimationCurveToUpdate.x);
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeYindex, AnimationCurveToUpdate.y);
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeZindex, AnimationCurveToUpdate.z);
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeRotateXindex, AnimationCurveToUpdate.rotationX);
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeRotateYindex, AnimationCurveToUpdate.rotationY);
        animCurve.RemoveKeyFrameFromCurve(vk.keyframeRotateZindex, AnimationCurveToUpdate.rotationZ);

        // remove from beizer curve as well 
        animatedObject.beizerPathGroup.beizerPathData.removeControlPoint(vk);

        Destroy(vk.gameObject);

        UpdateCurve();
        UpdateKeyframeUI();
        sortVideoKeyframesByTime();
        
        // remove the beizerPoint
    
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

    public Vector3 trackPositionAtKeyframeTime(float currTime)
    {
       float xPos = 0.002f/transform.localScale.x; // extrude outwards 
       float yPos = 0f; // because we want this to be at center of the animation track
       float zPos = currTime - ((GetComponent<Collider>().bounds.size.z) / 2) - timeline.trackSectionData.currentFromTimeClamp;// assume pivot point at center of animation track  

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
