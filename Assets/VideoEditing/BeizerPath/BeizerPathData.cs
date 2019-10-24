using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct keyframeRefs
{
 
    public int keyframeXindex;
    public int keyframeYindex;
    public int keyframeZindex;

    public float keyframeTime;
}

public class BeizerPathData : ScriptableObject
{
    public AnimationTrack animTrack; 
    public List<BeizerPoint> beizerPoints;
    public List<BeizerPoint> realTimeBeizerPoints;
    public List<GameObject> intermediatePoints;

    public BeizerPathGroup beizerPathGroup;

    #region AnimationCurves for Beizer Curve

    public AnimationCurve curveXbeizer;
    public AnimationCurve curveYbeizer;
    public AnimationCurve curveZbeizer; 

    #endregion

    #region public methods 

    /// <summary>
    /// This is called after a new keyframe has been added. Takes the previous position keyframe nad the new position keyframe, and generates keyframe postions as such 
    /// </summary>
    /// <param name="k1"></param>
    /// <param name="k2"></param>    
    public void generateBeizerCurve(VideoKeyFrame k1, VideoKeyFrame k2)
    {
        int controlPointIndexOne;
        int controlPointIndexTwo;
        generateBeizerControlPoints(k1, k2, out controlPointIndexOne, out controlPointIndexTwo);
        generateIntermediatePoints(controlPointIndexOne, controlPointIndexTwo);

        /// change the realtime beizer points to this saved beizer points
        realTimeBeizerPoints = beizerPoints;
    }

    /// <summary>
    /// This is called after a control point has been moved, but not saved as key frame 
    /// </summary>
    public void updateBeizerCurve()
    {


        
    }
    /// <summary>
    /// This is invoked when the UI menu button has been pressed and 
    /// 
    /// We are basically sending a new path (w/ the customized control points), with lots of manipulated control points and intermediate points, to the animation curve.
    /// 
    /// Have customized 
    /// 
    /// make sure the control points have reference to the keyframe index number
    /// 
    /// Basically, this is a more customized version of how to 
    /// </summary>
    /// 

    // we should keep the beizer curve animationcurves Sepearate from the Regular linear path curves (toggle) 

    // case 1) complete linear path ---- save that curve in curveX, curveY, curveZ 

    // case 2) complete beizer path ---- save those curves as beizerCurveX, beizerCurveY, beizerCurveZ, but make sure you have reference / distinguish between control points and beizer points

    // case 3) have a button that toggles back and forth that chooses between which curve to set 

    // saving this beizer curve arrangement deserves a separate button 
    public void saveUpdatedBeizerCurve()
    {
        // we're going to create a new animation curve, from these positions, for x,y,z
        // and then send them to the clip for playing
        AnimationCurve curveX = new AnimationCurve();
        AnimationCurve curveY = new AnimationCurve();
        AnimationCurve curveZ = new AnimationCurve();

        // get keyframes ready to set to curve
        Keyframe[] xKeys = new Keyframe[realTimeBeizerPoints.Count];
        Keyframe[] yKeys = new Keyframe[realTimeBeizerPoints.Count];
        Keyframe[] zKeys = new Keyframe[realTimeBeizerPoints.Count];


        // generate keyframe per position and time
        for(int i = 0; i < realTimeBeizerPoints.Count; i++)
        {
            Vector3 pos = realTimeBeizerPoints[i].transform.position;
            xKeys[i] = new Keyframe(realTimeBeizerPoints[i].videoKeyframe.keyframeTime, pos.x);
            yKeys[i] = new Keyframe(realTimeBeizerPoints[i].videoKeyframe.keyframeTime, pos.y);
            zKeys[i] = new Keyframe(realTimeBeizerPoints[i].videoKeyframe.keyframeTime, pos.z);

        }

        // set a new list of VideoKeyframes 
        animTrack.DestoryExistingKeyframesUI();

        List<VideoKeyFrame> videoKeyFrames = new List<VideoKeyFrame>();

        for(int i = 0; i < realTimeBeizerPoints.Count; i++)
        {
            int curveXindex = curveX.AddKey(xKeys[i]);
            int curveYindex = curveY.AddKey(yKeys[i]);
            int curveZindex = curveZ.AddKey(zKeys[i]);

            VideoKeyFrame vk = new VideoKeyFrame();
            vk.animTrack = animTrack;

            vk.keyframeXindex = curveXindex;
            vk.keyframeYindex = curveYindex;
            vk.keyframeZindex = curveZindex;

            videoKeyFrames.Add(vk);
        }

        /// include place holder and ? -- do we want to have a sepearate method for this -> because it seems much easier just to generate new curve, adjust key frames, etc. 
        /// assess time it takes to create other methods, versus time saved here. 

        animTrack.clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        animTrack.clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        animTrack.clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

        // call animTrack to recreate a new list of VideoKeyframes, which are sent from here
        animTrack.videoKeyFrames = videoKeyFrames;

        // we need to save these video key frame positions
        animTrack.GenerateKeyframeUI();
        
        

    }

    public void sendBeizerCurvesToAnimClip() { }
    public void addControlPoint(VideoKeyFrame vk)
    {
        // first instantiante a beizer point in the real world  
        ControlPoint b = Instantiate(beizerPathGroup.controlPointPrefab, beizerPathGroup.transform).GetComponent<ControlPoint>();
        b.transform.localScale = new Vector3(1, 1, 1);
        b.videoKeyframe = vk;
        vk.controlPoint = b; 


        // set the transform positions here
        b.transform.position = vk.retrieveTransformPositionFromVideoKeyframe();

        // this has no info on the left control point or right control point 
        beizerPoints.Add(b);
        sortBeizerPointsByTime();

        // add this control point inside the beizer Curve
        Keyframe xControl = new Keyframe(vk.keyframeTime, b.transform.position.x);
        Keyframe yControl = new Keyframe(vk.keyframeTime, b.transform.position.y);
        Keyframe zControl = new Keyframe(vk.keyframeTime, b.transform.position.z);

        // add the keyframes in the curve specific to this beizer point
        addKeyFrameToBeizerCurve(xControl, AnimationCurveToUpdate.x);
        addKeyFrameToBeizerCurve(yControl, AnimationCurveToUpdate.y);
        addKeyFrameToBeizerCurve(zControl, AnimationCurveToUpdate.z);

    }
    #endregion

    #region private beizer Animation Curve methods 

    private void addKeyFrameToBeizerCurve(Keyframe keyframe, AnimationCurveToUpdate curve)
    {
        switch (curve)
        {
            case AnimationCurveToUpdate.x:
                curveXbeizer.AddKey(keyframe);
                break;
            case AnimationCurveToUpdate.y:
                curveYbeizer.AddKey(keyframe);
                break; 
            case AnimationCurveToUpdate.z:
                curveZbeizer.AddKey(keyframe);
                break;
        }
    }

    #endregion

    #region private methods

    // don't use this because this is assuming we generate control points two at a time. doesn't work like this
    private void generateBeizerControlPoints(VideoKeyFrame k1, VideoKeyFrame k2, out int beizerPointIndexOne, out int beizerPointIndexTwo)
    {
        ControlPoint c1 = new ControlPoint();
        ControlPoint c2 = new ControlPoint();

        c1.SetBeizerPoint(k1);
        c2.SetBeizerPoint(k2);

        beizerPoints.Add(c1);

        beizerPointIndexOne = beizerPoints.IndexOf(c1);
        beizerPointIndexTwo = beizerPoints.IndexOf(c2);

        c1.rightControlPoint = c2;
        c2.leftControlPoint = c1; 

    }

    
    public void generateIntermediatePoints(int keyframeIndexPointOne, int keyframeIndexPointTwo)
    {
        Debug.Log("intermediate keyframe control pt index " + keyframeIndexPointOne + " index2 " + keyframeIndexPointTwo);
        // first, retrieve the times and vector positions at these beizer points --> assume the beizer points have been made already
        ControlPoint p0 = animTrack.videoKeyFrames[keyframeIndexPointOne].controlPoint;
        ControlPoint p2 = animTrack.videoKeyFrames[keyframeIndexPointTwo].controlPoint;

        float t1 = p0.videoKeyframe.keyframeTime;
        float t2 = p2.videoKeyframe.keyframeTime;

        // get vector3 from these key frame positons, p0
        float p0x = animTrack.animCurve.curveX.keys[p0.videoKeyframe.keyframeXindex].value;
        float p0y = animTrack.animCurve.curveY.keys[p0.videoKeyframe.keyframeYindex].value;
        float p0z = animTrack.animCurve.curveZ.keys[p0.videoKeyframe.keyframeZindex].value;

        // get vector3 from these key frame positions, p2
        float p2x = animTrack.animCurve.curveX.keys[p2.videoKeyframe.keyframeXindex].value;
        float p2y = animTrack.animCurve.curveY.keys[p2.videoKeyframe.keyframeYindex].value;
        float p2z = animTrack.animCurve.curveZ.keys[p2.videoKeyframe.keyframeZindex].value;

        Vector3 pointZero = new Vector3(p0x, p0y, p0z);
        Vector3 pointTwo = new Vector3(p2x, p2y, p2z);
        Vector3 pointOne = Vector3.Lerp(pointZero, pointTwo, 0.5f);

        Vector3[] intermediatePositions = BeizerPointIntermediatePoints(pointZero, pointOne, pointTwo);
        Debug.Log("IntermediatePositions " + intermediatePositions);
        foreach(Vector3 pos in intermediatePositions)
        {
            Debug.Log("Position in intermediatePos " + pos);
        }

        keyframeRefs[] intermediateKeyFramesRefs;
        /// everytime there is an update to the keyframe, you have to change the intermediate points
        /// // and thus, the set curve methods are different -- there's going to be a lot of remove curve

        // start with a new set of VideoKeyframes

        // fill this up using this method 
        addIntermediateKeyframes(intermediatePositions, t1, t2, out intermediateKeyFramesRefs);
        int amountOfIntermediatePoints = intermediateKeyFramesRefs.Length; 

        int controlPointIndexOne = beizerPoints.IndexOf(p0);
        int controlPointIndexTwo = beizerPoints.IndexOf(p2);

        IntermediateControlPoint middleControlPoint; 
        if (!areThereIntermediatePoints(controlPointIndexOne,controlPointIndexTwo))
        {
            // generate beizer intermediate points from these keyframes 
            Debug.Log("add intermediate keyframes");
            addIntermediateBeizerPoints(controlPointIndexOne, amountOfIntermediatePoints, intermediateKeyFramesRefs, out middleControlPoint);
        }
        else
        {
            // update existing beizer curves
            Debug.Log("update intermediate keyframes");

            updateIntermediatePoints(controlPointIndexOne, controlPointIndexTwo, intermediateKeyFramesRefs, out middleControlPoint);
        }

        p0.rightControlPoint = middleControlPoint;

        middleControlPoint.rightControlPoint = p2;
        middleControlPoint.leftControlPoint = p0;

        p2.leftControlPoint = middleControlPoint;
    }

    public void addIntermediateKeyframes(Vector3[] intermediatePositions, float t1, float t2, out keyframeRefs[] intermediateKeyframeRefs)
    {
        float dt = (t2 - t1) / 10 ;
        float currTime = t1 + dt;
        int i = 0;
        Debug.Log("This is dt " + dt);
        intermediateKeyframeRefs = new keyframeRefs[intermediatePositions.Length];
        foreach (Vector3 intermediatePoint in intermediatePositions)
        {
            Debug.Log("this is currTime" + currTime);


            Keyframe pointX = new Keyframe(currTime, intermediatePoint.x);
            Keyframe pointY = new Keyframe(currTime, intermediatePoint.y);
            Keyframe pointZ = new Keyframe(currTime, intermediatePoint.z);

            addKeyFrameToBeizerCurve(pointX, AnimationCurveToUpdate.x);
            addKeyFrameToBeizerCurve(pointY, AnimationCurveToUpdate.y);
            addKeyFrameToBeizerCurve(pointZ, AnimationCurveToUpdate.z);

            int keyFrameXindex = retrieveKeyframeIndexFromBeizerAnimationCurves(currTime, AnimationCurveToUpdate.x);
            int keyFrameYindex = retrieveKeyframeIndexFromBeizerAnimationCurves(currTime, AnimationCurveToUpdate.y);
            int keyFrameZindex = retrieveKeyframeIndexFromBeizerAnimationCurves(currTime, AnimationCurveToUpdate.z);

            keyframeRefs k = new keyframeRefs();
            k.keyframeXindex = keyFrameXindex;
            k.keyframeYindex = keyFrameYindex;
            k.keyframeZindex = keyFrameZindex;
            k.keyframeTime = currTime;

            intermediateKeyframeRefs[i] = k;

            i += 1;
            currTime += dt;


        }
    }

    // these should not be accessible to the client 
    // these should be inside the beizerPointData
    private Vector3[] BeizerPointIntermediatePoints(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float initialTime = 0.1f;
        int index = 0; 
        float currTime = initialTime;
        Vector3[] intermediatePoints = new Vector3[8];

        while(currTime < 0.9)
        {
            float pX = Mathf.Pow((1f - currTime), 2.0f) * p0.x + 2 * (1 - currTime) * currTime * p1.x + Mathf.Pow(currTime,2) * p2.x ;
            float pY = Mathf.Pow((1f - currTime), 2.0f) * p0.y + 2 * (1 - currTime) * currTime * p1.y + Mathf.Pow(currTime, 2) * p2.y;
            float pZ = Mathf.Pow((1f - currTime), 2.0f) * p0.z + 2 * (1 - currTime) * currTime * p1.z + Mathf.Pow(currTime, 2) * p2.z;

            intermediatePoints[index] = new Vector3(pX, pY, pZ);

            index += 1; 
            currTime += 0.1f; 
        }
        return intermediatePoints;
    }

    // if there are intermediate points, return the beizerpoints to remove and replace

    private bool areThereIntermediatePoints(int beizerPointIndexOne, int beizerPointIndexTwo)
    {
        // loop from p1, p2, 
        if (beizerPoints[beizerPointIndexOne+1].GetType() == typeof(IntermediatePoint) || beizerPoints[beizerPointIndexTwo - 1].GetType() == typeof(IntermediatePoint))
        {
            return true; 
        }
        else
        {
            return false; 
        }

        // return the intermediate points and their indices, so we can check which points to remove
    }
    // if the control points 

    private VideoKeyFrame[] retrieveVideoKeyFramesFromBeizerPoint(int beizerPointIndexOne, int beizerPointIndexTwo)
    {
        int countDiff = beizerPointIndexTwo - (beizerPointIndexOne + 1);
        VideoKeyFrame[] videoKeyframes = new VideoKeyFrame[countDiff];

        for(int i = beizerPointIndexOne+1; i < countDiff; i++)
        {
            videoKeyframes[i] = beizerPoints[i].videoKeyframe;
        }

        return videoKeyframes;

    }

    private void addIntermediateBeizerPoints(int beizerPointIndexOne, int amountOfIntermediatePointsToAdd, keyframeRefs[] keyframeRefData, out IntermediateControlPoint middleControlPoint)
    {
        int countDiff = (amountOfIntermediatePointsToAdd);
        Debug.Log("check countDiff " + countDiff);
        List<BeizerPoint> intermediatePoints = new List<BeizerPoint>();
        
        for(int i = 0; i < countDiff; i++)
        {

            GameObject g = Instantiate(beizerPathGroup.intermediatePointPrefab, beizerPathGroup.transform);
            g.transform.localScale = new Vector3(1, 1, 1);
            IntermediatePoint newIntermediatePoint = g.GetComponent<IntermediatePoint>();
            newIntermediatePoint.transform.position = beizerPointPositionFromVideoKeyframeOfBeizerCurves(keyframeRefData[i]);
            newIntermediatePoint.beizerIntermediateTime = keyframeRefData[i].keyframeTime;
            intermediatePoints.Add(newIntermediatePoint);
        }

        middleControlPoint = Instantiate(beizerPathGroup.intermediateControlPointPrefab, beizerPathGroup.transform).GetComponent<IntermediateControlPoint>();
        middleControlPoint.transform.position = beizerPointPositionFromVideoKeyframeOfBeizerCurves(keyframeRefData[3]); 
        middleControlPoint.beizerIntermediateTime = keyframeRefData[3].keyframeTime;
        middleControlPoint.setAnimatedObj(animTrack.animatedObject);

        beizerPoints.InsertRange(beizerPointIndexOne, intermediatePoints);
        sortBeizerPointsByTime();
    }
    // use only when updating the timeline
    private void updateIntermediatePoints(int beizerPointIndexPointOne, int beizerPointIndexPointTwo, keyframeRefs[] newKeyframes, out IntermediateControlPoint middleControlPoint)
    {
        int newBeizerPointIndex;
        removeIntermediateBeizerPoints(beizerPointIndexPointOne, beizerPointIndexPointTwo, out newBeizerPointIndex);
        addIntermediateBeizerPoints(beizerPointIndexPointOne, newKeyframes.Length, newKeyframes, out middleControlPoint);

    }

    private int retrieveKeyframeIndexFromBeizerAnimationCurves(float time, AnimationCurveToUpdate curve)
    {
        int keyframeIndex = 0;
        switch (curve)
        {
            case AnimationCurveToUpdate.x:

                Keyframe[] keysX = curveXbeizer.keys;
                for (int i = 0; i < keysX.Length; i++)
                {
                    if (keysX[i].time == time)
                    {
                        return i;
                    }
                }
                break;
            case AnimationCurveToUpdate.y:
                Keyframe[] keysY = curveYbeizer.keys;
                for (int i = 0; i < keysY.Length; i++)
                {
                    if (keysY[i].time == time)
                    {
                        return i;
                    }
                }
                break;
            case AnimationCurveToUpdate.z:
                Keyframe[] keysZ = curveZbeizer.keys;
                for (int i = 0; i < keysZ.Length; i++)
                {
                    if (keysZ[i].time == time)
                    {
                        return i;
                    }
                }
                break;
        }

        return keyframeIndex;
    }

    private void removeIntermediateBeizerPoints(int beizerPointIndexOne, int beizerPointIndexTwo, out int newBeizerPointIndexTwo)
    {
        int countDiff = beizerPointIndexTwo - (beizerPointIndexOne + 1);
        beizerPoints.RemoveRange(beizerPointIndexOne + 1, countDiff);
        newBeizerPointIndexTwo = beizerPointIndexOne + 1; 
     }

    // calculate the new beizer curve section path, depending on which beizer curve is selected
    public void updateBeizerCurveSection(ControlPoint selectedControlPoint)
    {
        if(selectedControlPoint.leftControlPoint == null)
        {
            Debug.Log("go right");
            ControlPoint cp2 = selectedControlPoint.rightControlPoint;
            Vector3 posTwo = cp2.transform.position;
            
            if(cp2.rightControlPoint != null)
            {
                Debug.Log("move right control points");
                ControlPoint cp3 = cp2.rightControlPoint;
                Vector3 posThree = cp3.transform.position;

                Vector3[] newIntermediatePoints = BeizerPointIntermediatePoints(selectedControlPoint.transform.position, posTwo, posThree);

                int selectedControlPointIndex = beizerPoints.IndexOf(selectedControlPoint);
                int cp2Index = beizerPoints.IndexOf(cp2);
                int cp3Index = beizerPoints.IndexOf(cp3);

                // update intermediate positons thru for loop
                int intermediatePointIndex = 0;

                for (int i = selectedControlPointIndex + 1; i < cp3Index; i++)
                {

                    beizerPoints[i].transform.position = newIntermediatePoints[intermediatePointIndex];
                    intermediatePointIndex += 1;

                }

            }
        }
        else if (selectedControlPoint.rightControlPoint == null)
        {
            Debug.Log("go left");

            ControlPoint cp2 = selectedControlPoint.leftControlPoint;
            Vector3 posTwo = cp2.transform.position;

            if (cp2.leftControlPoint != null)
            {
                Debug.Log("move left control points");

                ControlPoint cp1 = cp2.leftControlPoint;
                Vector3 posOne = cp1.transform.position;

                Vector3[] newIntermediatePoints = BeizerPointIntermediatePoints(posOne, posTwo, selectedControlPoint.transform.position);

                int selectedControlPointIndex = beizerPoints.IndexOf(selectedControlPoint);
                int cp2Index = beizerPoints.IndexOf(cp2);
                int cp1Index = beizerPoints.IndexOf(cp1);

                Debug.Log("indexes; cp1index: " + cp1Index + " cp2index: " + cp2Index + " cp3index: " + selectedControlPointIndex);
                // update intermediate positons thru for loop\
                int intermediatePointIndex = 0;

                for (int i = cp1Index + 1; i < selectedControlPointIndex; i++)
                {

                    beizerPoints[i].transform.position = newIntermediatePoints[intermediatePointIndex];

                    intermediatePointIndex += 1;

                }


            }
        }
        else
        {
            ControlPoint cp1 = selectedControlPoint.leftControlPoint;
            ControlPoint cp3 = selectedControlPoint.rightControlPoint;

            Debug.Log("move middle control points");
            Vector3 posOne = cp1.transform.position;
            Vector3 posTwo = selectedControlPoint.transform.position;
            Vector3 posThree = cp3.transform.position;

            Vector3[] newIntermediatePoints = BeizerPointIntermediatePoints(posOne, posTwo, posThree);

            int cp1Index = beizerPoints.IndexOf(cp1);
            int cp2Index = beizerPoints.IndexOf(selectedControlPoint);
            int cp3Index = beizerPoints.IndexOf(cp3);

            int intermediatePointIndex = 0;
            for (int i = cp1Index + 1; i < cp3Index; i++)
            {

                    Debug.Log("set points transforms");
                    Debug.Log("beizerPointIndex" + i + " intermediatePointIndex " + intermediatePointIndex);
                    beizerPoints[i].transform.position = newIntermediatePoints[intermediatePointIndex];
                
                
                intermediatePointIndex += 1; 

            }

        }
    }

    private Vector3 beizerPointPositionFromVideoKeyframeOfBeizerCurves(VideoKeyFrame vk)
    {
        Vector3 animObjTransformPos;

        float x = curveXbeizer.keys[vk.keyframeXindex].value;
        float y = curveYbeizer.keys[vk.keyframeYindex].value;
        float z = curveZbeizer.keys[vk.keyframeZindex].value;

        animObjTransformPos = new Vector3(x, y, z);
        return animObjTransformPos;
    }

    private Vector3 beizerPointPositionFromVideoKeyframeOfBeizerCurves(keyframeRefs k)
    {
        Vector3 animObjTransformPos;

        float x = curveXbeizer.keys[k.keyframeXindex].value;
        float y = curveYbeizer.keys[k.keyframeYindex].value;
        float z = curveZbeizer.keys[k.keyframeZindex].value;

        animObjTransformPos = new Vector3(x, y, z);
        return animObjTransformPos;
    }
    private void sortBeizerPointsByTime()
    {
        Dictionary<float, BeizerPoint> orderedBeizerPoints = new Dictionary<float, BeizerPoint>();
        foreach (BeizerPoint bp in beizerPoints)
        {
            if (bp.GetType() == typeof(ControlPoint))
            {
                orderedBeizerPoints.Add(bp.videoKeyframe.keyframeTime, bp);
            }
            else if (bp.GetType() == typeof(IntermediatePoint) || bp.GetType() == typeof(IntermediateControlPoint))
            {
                orderedBeizerPoints.Add(bp.beizerIntermediateTime, bp);

            }
        }

        List<float> times = orderedBeizerPoints.Keys.ToList<float>();
        times.Sort();
        foreach(float time in times)
        {
            Debug.Log("times from time" + time);
        }


        Debug.Log("times from beizer keyframes" + times);

        beizerPoints = new List<BeizerPoint>();

        // this is sorted now
        foreach (float time in times)
        {
            beizerPoints.Add(orderedBeizerPoints[time]);
        }

    }

    #endregion
}
