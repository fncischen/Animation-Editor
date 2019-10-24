using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The purpose of the video key frame is to have access to the indices on the animation curve to access the keys, such that 
/// the curve can directly modify the positions at this time. 
/// 
/// You can retrieve the indices and then use that index to call the curve directly 
/// </summary>
public class VideoKeyFrame : MonoBehaviour
{

    public AnimationTrack animTrack;
    public ControlPoint controlPoint; 

    public int keyframeXindex;
    public int keyframeYindex;
    public int keyframeZindex;
    public int keyframeRotateXindex;
    public int keyframeRotateYindex;
    public int keyframeRotateZindex; 

    public float keyframeTime;

    // this represents all 6 keyframes to be moved
    public void moveKeyframe()
    {
        animTrack.MoveKeyFrameUI(keyframeXindex,keyframeTime,AnimationCurveToUpdate.x);
        animTrack.MoveKeyFrameUI(keyframeYindex, keyframeTime, AnimationCurveToUpdate.y);
        animTrack.MoveKeyFrameUI(keyframeZindex, keyframeTime, AnimationCurveToUpdate.z);
        animTrack.MoveKeyFrameUI(keyframeRotateXindex, keyframeTime, AnimationCurveToUpdate.rotationX);
        animTrack.MoveKeyFrameUI(keyframeRotateYindex, keyframeTime, AnimationCurveToUpdate.rotationY);
        animTrack.MoveKeyFrameUI(keyframeRotateZindex, keyframeTime, AnimationCurveToUpdate.rotationZ);

    }

    public Vector3 retrieveTransformPositionFromVideoKeyframe()
    {
        Vector3 animObjTransformPos;

        float x = animTrack.animCurve.curveX.keys[keyframeXindex].value;
        float y = animTrack.animCurve.curveY.keys[keyframeYindex].value;
        float z = animTrack.animCurve.curveZ.keys[keyframeZindex].value;

        animObjTransformPos = new Vector3(x, y, z);
        return animObjTransformPos;



    }

    public void onMouseDown()
    {
        
    }

    public void OnMouseDrag()
    {
        
    }

}
