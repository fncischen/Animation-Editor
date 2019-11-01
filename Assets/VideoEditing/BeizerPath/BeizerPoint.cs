using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeizerPoint : InteractableObject
{
    public VideoKeyFrame videoKeyframe;
    public float beizerTime; 
    public float beizerIntermediateTime;

    public virtual void SetBeizerPoint(VideoKeyFrame vKeyframe)
    {
        videoKeyframe = vKeyframe;

        float p1x = videoKeyframe.animTrack.animatedObject.beizerPathGroup.beizerPathData.curveXbeizer.keys[videoKeyframe.keyframeXindex].value;
        float p1y = videoKeyframe.animTrack.animatedObject.beizerPathGroup.beizerPathData.curveYbeizer.keys[videoKeyframe.keyframeYindex].value;
        float p1z = videoKeyframe.animTrack.animatedObject.beizerPathGroup.beizerPathData.curveZbeizer.keys[videoKeyframe.keyframeZindex].value;

        transform.position = new Vector3(p1x, p1y, p1z);
    }

    // this does not have a keyframe -> 
    public virtual void SetBeizerPoint()
    {

    }


}
