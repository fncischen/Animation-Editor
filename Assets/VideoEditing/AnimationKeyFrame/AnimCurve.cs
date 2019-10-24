using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimationCurveToUpdate
{
    x,
    y,
    z,
    rotationX,
    rotationY,
    rotationZ,
}

/// <summary>
/// Purpose
/// 
/// To handle all back-end Animation curve updates which are sent to the animation clip for the Unity Playable Graph to play
/// 
/// </summary>
[CreateAssetMenu(fileName = "AnimCurve", menuName = "ScriptableObjects/AnimCurve", order = 1)]
public class AnimCurve : ScriptableObject
{
    public List<Keyframe> curveX_keyFrames;
    public List<Keyframe> curveY_keyFrames;
    public List<Keyframe> curveZ_keyFrames;

    public List<Keyframe> rotationX_keyFrames;
    public List<Keyframe> rotationY_keyFrames;
    public List<Keyframe> rotationZ_keyFrames;

    public AnimationClip clip;

    public AnimationCurve curveX;
    public AnimationCurve curveY;
    public AnimationCurve curveZ;

    public AnimationCurve rotationXcurve;
    public AnimationCurve rotationYcurve;
    public AnimationCurve rotationZcurve;

    public AnimationTrack animTrack;

    // there needs to be way to make sure 
    // we don't make duplicate KeyFrame UIs 

    // use a check function?
    // problem is check function makes it hard to match times

    // Keyframe [] a -> no way to sync the for loop to iterate simultaneously 
    // Keyframe [] b -> no way to synce the for loop to iterate simultaneously

    // solution have a Time Data Structure that keeps track of each key frame that is placed at each time position?
    // place the keyframes at the time data structure, which is placed at the animationTrack 

    // assume key frames have all rotations and positions ready

    // from gimbal to object to curve to track 

    public void Awake()
    {
       // SetupAnimationCurves();
    }

    public void SetupAnimationCurves()
    {
        curveX = new AnimationCurve();
        curveY = new AnimationCurve();
        curveZ = new AnimationCurve();

        rotationXcurve = new AnimationCurve();
        rotationYcurve = new AnimationCurve();
        rotationZcurve = new AnimationCurve();

        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

        clip.SetCurve("", typeof(Transform), "localRotation.x", rotationXcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", rotationYcurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", rotationZcurve);
    }

    #region keyframe adding/updating 

    // these are non timeline methods // they come from the UI menu next to gimbals
    public void addKeyFrameToCurve(Keyframe keyframe, AnimationCurveToUpdate curve)
    {
        int keyFrameIndex; 
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                keyFrameIndex = curveX.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }

                break;
            case AnimationCurveToUpdate.y:
                keyFrameIndex = curveY.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }
                break;
            case AnimationCurveToUpdate.z:
                keyFrameIndex = curveZ.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }
                break;
            case AnimationCurveToUpdate.rotationX:
                keyFrameIndex = rotationXcurve.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localRotation.x", rotationXcurve);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }
                break;
            case AnimationCurveToUpdate.rotationY:
                keyFrameIndex = rotationYcurve.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localRotation.y", rotationYcurve);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }
                break;
            case AnimationCurveToUpdate.rotationZ:
                keyFrameIndex = rotationZcurve.AddKey(keyframe);
                clip.SetCurve("", typeof(Transform), "localRotation.z", rotationZcurve);

                if (animTrack.onKeyframeAdded != null)
                {
                    animTrack.onKeyframeAdded(keyFrameIndex, keyframe.time, curve);
                }
                break;
        }

        // if(animTrack.onKeyFrameAdded != null)
        //    {
        //        animTrack.onKeyFrameAdded(keyframe,curve);
        //    }

    }
 
    public void updateKeyFrameOnCurve(Keyframe keyframe, AnimationCurveToUpdate curve)
    {
        // find which keyframe value was updated
        int keyFrameIndex;
        Debug.Log("updating key frame!");
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                for (int i = 0; i < curveX.keys.Length; i++)
                {
                    // step 2) check if this keyframe time is the same 
                    if (curveX.keys[i].time == keyframe.time)
                    {
                        Debug.Log("updating key frame x!");

                        keyFrameIndex = curveX.MoveKey(i, keyframe);
                        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);

                        if (animTrack.onKeyFrameUpdated != null)
                        {
                            animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                        }
                        break;
                    }
                }
                break;
            case AnimationCurveToUpdate.y:
                for (int i = 0; i < curveY.keys.Length; i++)
                {
                    // step 2) check if this keyframe time is the same 
                    if (curveY.keys[i].time == keyframe.time)
                    {
                        Debug.Log("updating key frame y!");

                        keyFrameIndex = curveY.MoveKey(i, keyframe);
                        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
                        
                        if (animTrack.onKeyFrameUpdated != null)
                        {
                            animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                        }
                        break;
                    }
                }
                break;
            case AnimationCurveToUpdate.z:
                for (int i = 0; i < curveZ.keys.Length; i++)
                {
                    // step 2) check if this keyframe time is the same 
                    if (curveZ.keys[i].time == keyframe.time)
                    {
                        Debug.Log("updating key frame z!");

                        keyFrameIndex = curveZ.MoveKey(i, keyframe);
                        clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

                        if (animTrack.onKeyFrameUpdated != null)
                        {
                            animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                        }
                        break;
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationX:
                {
                    for (int i = 0; i < rotationXcurve.keys.Length; i++)
                    {
                        // step 2) check if this keyframe time is the same 
                        if (rotationXcurve.keys[i].time == keyframe.time)
                        {
                            keyFrameIndex = rotationXcurve.MoveKey(i, keyframe);
                            clip.SetCurve("", typeof(Transform), "localRotation.x", rotationXcurve);

                            if (animTrack.onKeyFrameUpdated != null)
                            {
                                animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                            }
                            break;
                        }
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationY:
                for (int i = 0; i < rotationYcurve.keys.Length; i++)
                {
                    // step 2) check if this keyframe time is the same 
                    if (rotationYcurve.keys[i].time == keyframe.time)
                    {
                        keyFrameIndex = rotationYcurve.MoveKey(i, keyframe);
                        clip.SetCurve("", typeof(Transform), "localRotation.y", rotationYcurve);

                        if (animTrack.onKeyFrameUpdated != null)
                        {
                            animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                        }
                        break;
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationZ:
                for (int i = 0; i < rotationZcurve.keys.Length; i++)
                {
                    // step 2) check if this keyframe time is the same 
                    if (rotationZcurve.keys[i].time == keyframe.time)
                    {
                        keyFrameIndex = rotationZcurve.MoveKey(i, keyframe);
                        clip.SetCurve("", typeof(Transform), "localRotation.z", rotationZcurve);

                        if (animTrack.onKeyFrameUpdated != null)
                        {
                            animTrack.onKeyFrameUpdated(keyFrameIndex, keyframe.time, curve);
                        }
                        break;
                    }
                }
                break; 
            }

    }

    // for removing key frame
    // what would you use ? // how would it be invoked?

    // the only two ways to be invoked 

    // press remove keyframe button on OBJ menu (only enabled if the timeline ticker time equals the keyframe time, after checking keyframes, and this obj or the timeline is selected)
    // or press remove keyframe button on timeline button (only enabled if the timeline ticker time equals the keyframe time, after checking keyframes, and this obj or the timeline is selected)
    public void RemoveKeyFrameFromCurve(int keyFrameIndexToRemove, AnimationCurveToUpdate curve)
    {
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                curveX.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
                break;
            case AnimationCurveToUpdate.y:
                curveY.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
                break;
            case AnimationCurveToUpdate.z:
                curveZ.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);
                break;
            case AnimationCurveToUpdate.rotationX:
                rotationXcurve.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localRotation.x", rotationXcurve);
                break;
            case AnimationCurveToUpdate.rotationY:
                rotationYcurve.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localRotation.y", rotationYcurve);
                break;
            case AnimationCurveToUpdate.rotationZ:
                rotationZcurve.RemoveKey(keyFrameIndexToRemove);
                clip.SetCurve("", typeof(Transform), "localRotation.z", rotationZcurve);
                break;
        }
    }

    #endregion

    #region beizercurvekeyframemethods 
    // for beizer curves , use beizer curve calculator 
    public void addIntermediateKeyframes(Vector3[] intermediatePositions, float t1, float t2, out VideoKeyFrame[] intermediateVideoKeyframes)
    {
        float dt = intermediatePositions.Length / (t2 - t1);
        float currTime = t1;
        intermediateVideoKeyframes = new VideoKeyFrame[intermediatePositions.Length];

        int i = 0;
        foreach (Vector3 intermediatePoint in intermediatePositions)
        {

            Keyframe pointX = new Keyframe(currTime, intermediatePoint.x);
            Keyframe pointY = new Keyframe(currTime, intermediatePoint.y);
            Keyframe pointZ = new Keyframe(currTime, intermediatePoint.z);

            animTrack.animCurve.addKeyFrameToCurve(pointX, AnimationCurveToUpdate.x);
            animTrack.animCurve.addKeyFrameToCurve(pointY, AnimationCurveToUpdate.y);
            animTrack.animCurve.addKeyFrameToCurve(pointZ, AnimationCurveToUpdate.z);

            int keyFrameXindex = retrieveKeyframeIndex(pointX, AnimationCurveToUpdate.x);
            int keyFrameYindex = retrieveKeyframeIndex(pointY, AnimationCurveToUpdate.x);
            int keyFrameZindex = retrieveKeyframeIndex(pointY, AnimationCurveToUpdate.x);

            VideoKeyFrame intermediateKeyframe = new VideoKeyFrame();
            intermediateKeyframe.keyframeXindex = keyFrameXindex;
            intermediateKeyframe.keyframeYindex = keyFrameYindex;
            intermediateKeyframe.keyframeZindex = keyFrameZindex;

            intermediateVideoKeyframes[i] = intermediateKeyframe;

            i += 1; 
            currTime += dt;


        }
    }

    public int retrieveKeyframeIndex(Keyframe k, AnimationCurveToUpdate curve)
    {
        int keyFrameIndex = 0;
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                keyFrameIndex = System.Array.IndexOf(curveX.keys, k);
                return keyFrameIndex;
            case AnimationCurveToUpdate.y:
                keyFrameIndex = System.Array.IndexOf(curveY.keys, k);
                return keyFrameIndex;
            case AnimationCurveToUpdate.z:
                keyFrameIndex = System.Array.IndexOf(curveZ.keys, k);
                return keyFrameIndex;
            case AnimationCurveToUpdate.rotationX:
                keyFrameIndex = System.Array.IndexOf(rotationXcurve.keys, k);
                return keyFrameIndex;
            case AnimationCurveToUpdate.rotationY:
                keyFrameIndex = System.Array.IndexOf(rotationYcurve.keys, k);
                return keyFrameIndex;
            case AnimationCurveToUpdate.rotationZ:
                keyFrameIndex = System.Array.IndexOf(rotationZcurve.keys, k);
                return keyFrameIndex;
        }
        return keyFrameIndex;

    }
    
    public void removeIntermediateKeyframes(int[] indices, AnimationCurveToUpdate curve)
    {

    }

    public void updateBeizerKeyframes()
    {

    }

    #endregion

    #region helper functions 

    public int getKeyframeIndicesFromCurve(float time, AnimationCurveToUpdate curve)
    {
        int keyframeIndex = 0;
        switch (curve)
        {
            case AnimationCurveToUpdate.x:

                Keyframe[] keysX = curveX.keys;
                for(int i = 0; i < keysX.Length; i++)
                {
                    if (keysX[i].time == time)
                    {
                        return i; 
                    }
                }
                break; 
            case AnimationCurveToUpdate.y:
                Keyframe[] keysY = curveY.keys;
                for (int i = 0; i < keysY.Length; i++)
                {
                    if (keysY[i].time == time)
                    {
                        return i;
                    }
                }
                break; 
            case AnimationCurveToUpdate.z:
                Keyframe[] keysZ = curveZ.keys;
                for (int i = 0; i < keysZ.Length; i++)
                {
                    if (keysZ[i].time == time)
                    {
                        return i;
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationX:
                Keyframe[] keysRotationX = rotationXcurve.keys;
                for (int i = 0; i < keysRotationX.Length; i++)
                {
                    if (keysRotationX[i].time == time)
                    {
                        return i;
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationY:
                Keyframe[] keysRotationY = rotationYcurve.keys;
                for (int i = 0; i < keysRotationY.Length; i++)
                {
                    if (keysRotationY[i].time == time)
                    {
                        return i;
                    }
                }
                break;
            case AnimationCurveToUpdate.rotationZ:
                Keyframe[] keysRotationZ = rotationZcurve.keys;
                for (int i = 0; i < keysRotationZ.Length; i++)
                {
                    if (keysRotationZ[i].time == time)
                    {
                        return i;
                    }
                }
                break; 

        }

        return keyframeIndex;

    }

    #endregion
}
