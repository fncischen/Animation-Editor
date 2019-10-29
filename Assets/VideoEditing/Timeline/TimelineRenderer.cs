using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

/// <summary>
/// Purpose:
/// 
/// To manage addition, removal, and placement of Animation Timeline Tracks
/// 
/// To manage timeline track UI configurations based on zoom in / zoom out / scroll left / scroll right methods
/// 
/// </summary>
public class TimelineTrackSectionRenderer : ScriptableObject
{
    public Timeline timeline;
    public List<AnimationTrack> animationTracks;
    public TimelineTrackMarker[] timelineTrackMarkers;
    // time range, irrespective of timeline track space 

    /// <summary>
    /// The current from Time that is clamped; 
    /// </summary>
    public float currentFromTimeClamp;
    /// <summary>
    /// The current to Time that is clamped; 
    /// </summary>
    public float currentToTimeClamp;

    private float trackHeight;
    private float timelineTrackWidth;
    private float timelineTrackSectionHeight;
    private float timelineTrackSeperators;

    private float trackSectionOriginX;
    private float trackSectionOriginY;

    private int amountOfTimelineMarkers;
    private float timelineDelta;
    private float timelineTrackUVwidth;
    public Vector2 trackSectionOrigin
    {
        get { return new Vector2(trackSectionOriginX, trackSectionOriginY); }
    }

    // the tracks are here

    #region timeline intialization

    // for the timeline tracks, the coord system for 0,0 is at (0-timelineHalfWidth+timelineOwnersWidth,   timelineHalfHeight - menuHeight)
    public void SetUpTimelineTrackSectionParameters(float trackSecOriginX, float trackSecOriginY)
    {
        trackSectionOriginX = trackSecOriginX;
        trackSectionOriginY = trackSecOriginY;

        trackHeight = timeline.t_preFab.GetComponent<Collider>().bounds.size.y / timeline.transform.localScale.y;

        GameObject g = Instantiate(new GameObject("trackSectionOrigin"), timeline.transform);
        g.transform.localPosition = new Vector3(2.0f, trackSecOriginY, trackSectionOriginX);

        animationTracks = new List<AnimationTrack>();
    }

    // use only for generic timeline markers
    public void timelineMarkerGeneration()
    {
        float equalDim = timelineTrackWidth / (timeline.GetComponent<Collider>().bounds.size.z);

        for (float i = 0; i <= timelineTrackWidth; i += equalDim)
        {
            // create a ticker and place under the child 
            GameObject p = Instantiate(timeline.timelineMarker, timeline.transform.parent);
            p.transform.parent = timeline.transform;
            // convert from timeline coordinate space to localposition of the timeline 
            p.transform.localPosition = new Vector3(1, 0.25f, i - timeline.TimelineHalfWidth);
        }
    }

    public void initializeTimelineTrackMarkers()
    {
        // few factors

        // What % of timelineMaxTime is clamped
        // what is the difference between the left and right uv coordinates?
        Debug.Log("track face 0 " + timeline.trackFaceUVs[0]);
        Debug.Log("track face 1 " + timeline.trackFaceUVs[1]);


        timelineTrackUVwidth = timeline.trackFaceUVs[0].x - timeline.trackFaceUVs[1].x;
        Debug.Log("timelineDiff " + timelineTrackUVwidth);
        // that difference is X units of time clamped inside the timeline (at current scale) --- this is our INITIAL clamp
        currentFromTimeClamp = 0;
        currentToTimeClamp = timeline.timelineMaximumTime / 2;
        Debug.Log("toclamptime " + currentToTimeClamp);
        // timelineDiff = maxTime/2


        // 20 Unity Units, 5 is max time. 
        // we get to determine how much unity units from the UV coord represents the time//
        amountOfTimelineMarkers = Mathf.RoundToInt(currentToTimeClamp - currentFromTimeClamp) + 1; //
        timelineDelta = (timelineTrackUVwidth) / (timeline.timelineMaximumTime / 2);


        // make sure all the material textures are scaled at 1,1;
        foreach (AnimationTrack track in animationTracks)
        {
            Material mat = track.GetComponent<Renderer>().material;
            mat.SetTextureScale("_MainTex", new Vector2(1, 1));
        }

        Debug.Log("TimelineDiff " + timelineTrackUVwidth);

        // set up timeline track markers
        timelineTrackMarkers = new TimelineTrackMarker[Mathf.FloorToInt(amountOfTimelineMarkers)];

        // set up positions for these timeline track markers
        for (int i = 0; i < timelineTrackMarkers.Length; i++)
        {
            TimelineTrackMarker t = Instantiate(timeline.timelineTrackMarkerPrefab, timeline.transform).GetComponent<TimelineTrackMarker>();
            timelineTrackMarkers[i] = t;

            // center coord of timeline track + amount of y needed to get to top of timeline renderer + amount o
            t.transform.localPosition = new Vector3(0.2f, trackSectionOriginY, trackSectionOriginX + i * timelineDelta);
            t.transform.localScale = new Vector3(1 / t.transform.parent.localScale.x, 1 / t.transform.parent.localScale.y, 1 / t.transform.parent.localScale.z);
        }
    }

    #endregion

    // some of these variables aren't used for the coordinate system, but they are used in determining time spacing for things like 
    // timeline track, zoom in, zoom out, etc. 

    // setting up the coordinate systems to intialize objects

    // this should be a separate helper method 

    #region track add / remove / update methods
    public void AddAnimationTrack(AnimatedObject obj)
    {
        // first -> check how many tracks are in the listOfAnimationTracks;
        int TrackCount = animationTracks.Count;
        Debug.Log("trackCount" + TrackCount);
        // get the width of each track 
        // calculate the trackPosition after calculating 
        float newTrackPositionY = trackSectionOriginY - trackHeight * TrackCount - timelineTrackSeperators * TrackCount;
        Debug.Log("GameObj", timeline.t_preFab);
        float newTrackPositionX = trackSectionOriginX + timeline.t_preFab.GetComponent<Collider>().bounds.size.x;

        // what is the local position x of this track, 
        Vector3 newTrackPosition = new Vector3(newTrackPositionX, newTrackPositionY, 0.078f);

        GameObject g = Instantiate(timeline.t_preFab, timeline.transform.parent);

        AnimationTrack newTrack = g.GetComponent<AnimationTrack>();
        newTrack.SetUpTrackProperties(obj, newTrackPosition, timeline, TrackCount + 1, timeline.trackPrefab.unselectedColor, timeline.trackPrefab.selectedColor);
        obj.animTrack = newTrack;

        animationTracks.Add(newTrack);

    }

    public void RemoveAnimationTrack(AnimationTrack track)
    {
        // first -> search for animation track on list 
        if (animationTracks.Contains(track))
        {
            // second - > get index of track 
            int trackIndex = animationTracks.IndexOf(track);
            // remove -> remove that track at that point
            animationTracks.RemoveAt(trackIndex);
            Destroy(track.gameObject);

            // recalculate track positions accordingly
            int tracksAfterTrackIndex = trackIndex + 1;
            float newTrackPositionY = trackSectionOriginY + trackHeight - trackHeight * tracksAfterTrackIndex - timelineTrackSeperators * tracksAfterTrackIndex;
            float newTrackPositionX = trackSectionOriginX + timeline.t_preFab.GetComponent<Collider>().bounds.size.x;

            float currentHeight = newTrackPositionY;

            for (int i = tracksAfterTrackIndex; i <= animationTracks.Count; i++)
            {
                Vector3 newTrackPosition = new Vector3(newTrackPositionX, currentHeight, 0.078f);
                animationTracks[i].transform.localPosition = newTrackPosition;

                // note add animations to move up tracks? 

                currentHeight += timelineTrackSeperators;
            }

        }
    }

    public void ConnectTracksToPlayableGraph(AnimationClip clip, Animator anim)
    {

        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(timeline.pg, "Animation", anim);

        var clipPlayable = AnimationClipPlayable.Create(timeline.pg, clip);

        playableOutput.SetSourcePlayable(clipPlayable);
        playableOutput.SetSourceInputPort(0);

    }
    #endregion
    // RULES

    #region track render methods (for previously saved tracks)
    // to instantiate the Animation track, call this button after a game object has been selected and a UI has been clicked
    public void RenderAllTracks()
    {
        foreach (AnimationTrack track in timeline.tracks)
        {
            RenderKeyframesPerTrack(track);
        }
    }

    // may need to use when opening a saved file / track that is an asset
    private void RenderKeyframesPerTrack(AnimationTrack track)
    {
        AnimCurve curvesToCheck = track.animCurve;

        Keyframe[] xKeys = curvesToCheck.curveX.keys;
        Keyframe[] yKeys = curvesToCheck.curveY.keys;
        Keyframe[] zKeys = curvesToCheck.curveZ.keys;

        Keyframe[] rotationXkeys = curvesToCheck.rotationXcurve.keys;
        Keyframe[] rotationYkeys = curvesToCheck.rotationYcurve.keys;
        Keyframe[] rotationZkeys = curvesToCheck.rotationZcurve.keys;

        List<float> times = new List<float>();
        // how to check which keyframes are on the same time? 

        // return an array of times per keyframe curves?

        // generate a list of keyframe UIs that you can use 
        // store on animation track? 

        // we just need at least 2 keyframe that have the same time to avoid other keyframe UIs overriding 

        // a keyframe checker? ---> keyframe performance data structures / algorithims
        // it first checks the xkey time and reads all other keyframes 

        // for loop on first curve?
        // before checking business logic, make sure that this time is not inside the Times List

        // if it is, break and go to next time 
        // if not, add the time onto both list of times

        // and generate a keyframe UI, which references the INDEX in the xcurve that the keyframeUI for the x curve 

        // for loop on second curve?

        // for loop on third curve? 

        // for loop on forth curve?

        // for loop on fifth curve?

        // if the same time is found for at least one array ==> go to the next time in curveX. 

        // make sure that there isnt the same in the list of times

        // when creating keyframeUI, make sure there is a reference to which CURVES the keyframe is referencing


    }


    #endregion

    // look up render texture for animation curves so we can create vertical curve

    #region timeline zoom and scroll methods

    // all zoom methods increase / decrease the amount of 
    public void ZoomIn(float zoomDelta)
    {
        if (timeline.isTimelineActivated)
        {
            currentFromTimeClamp *= (1 + zoomDelta);
            currentToTimeClamp *= (1 - zoomDelta);

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();
        }
    }

    public void ZoomOut(float zoomDelta)
    {
        if (timeline.isTimelineActivated)
        {
            currentFromTimeClamp *= (1 - zoomDelta);
            currentToTimeClamp *= (1 + zoomDelta);

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();

        }
    }

    public void ScrollLeft(float scrollDelta)
    {
        if (timeline.isTimelineActivated && currentFromTimeClamp > 0)
        {

            // shiftUVsLeft(scrollDelta);

            foreach (AnimationTrack track in animationTracks)
            {
                Mesh animationTrackMesh = track.GetComponent<MeshFilter>().mesh;
                animationTrackMesh.uv = timeline.trackFaceUVs;

            }

            currentFromTimeClamp -= scrollDelta;
            currentToTimeClamp -= scrollDelta;

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();

        }
    }

    public void ScrollRight(float scrollDelta)
    {
        // first re-render texture and shift UV Coordinates
        if (timeline.isTimelineActivated && currentToTimeClamp < timeline.timelineMaximumTime)
        {

            // shiftUVsRight(scrollDelta);

            foreach (AnimationTrack track in animationTracks)
            {
                Mesh animationTrackMesh = track.GetComponent<MeshFilter>().mesh;
                animationTrackMesh.uv = timeline.trackFaceUVs;

            }

            // second, recalculate timeclamped 

            currentFromTimeClamp += scrollDelta;
            currentToTimeClamp += scrollDelta;

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();
        }

    }

    private void shiftUVsLeft(float scrollDelta)
    {
        timeline.trackPrefabUVs[0].x -= scrollDelta;
        timeline.trackPrefabUVs[1].x -= scrollDelta;
        timeline.trackPrefabUVs[2].x -= scrollDelta;
        timeline.trackPrefabUVs[3].x -= scrollDelta;

        timeline.trackFaceUVs[timeline.trackUVindices[0]] = timeline.trackPrefabUVs[0];
        timeline.trackFaceUVs[timeline.trackUVindices[1]] = timeline.trackPrefabUVs[1];
        timeline.trackFaceUVs[timeline.trackUVindices[2]] = timeline.trackPrefabUVs[2];
        timeline.trackFaceUVs[timeline.trackUVindices[3]] = timeline.trackPrefabUVs[3];
    }

    private void shiftUVsRight(float scrollDelta)
    {
            timeline.trackPrefabUVs[0].x += scrollDelta;
            timeline.trackPrefabUVs[1].x += scrollDelta;
            timeline.trackPrefabUVs[2].x += scrollDelta;
            timeline.trackPrefabUVs[3].x += scrollDelta;

            timeline.trackFaceUVs[timeline.trackUVindices[0]] = timeline.trackPrefabUVs[0];
            timeline.trackFaceUVs[timeline.trackUVindices[1]] = timeline.trackPrefabUVs[1];
            timeline.trackFaceUVs[timeline.trackUVindices[2]] = timeline.trackPrefabUVs[2];
            timeline.trackFaceUVs[timeline.trackUVindices[3]] = timeline.trackPrefabUVs[3];
    }

    // do some planning
    private void recalculateTimelineTrackMarkers()
    {
        // calculate amount of integers between currentToTimeClamp and c
        amountOfTimelineMarkers = 0;
        Debug.Log("currFromTime " + currentFromTimeClamp);
        Debug.Log("currToTime " + currentToTimeClamp);
        float currentNumber = currentFromTimeClamp;
        // difference is a bad idea / better to count how many integers are here
        timelineDelta = (timelineTrackUVwidth) / (timeline.timelineMaximumTime / 2);


        float timelineStartCoordinate;
        if (currentNumber != Mathf.FloorToInt(currentNumber)) {
            // convert currentNumber to timelineCoordinate
            int nextNumber = Mathf.FloorToInt(currentNumber) + 1;

            float nextIntegerTimelineMarkerCoordinate = timeline.ConvertFromTimeToTimelineZPosition(nextNumber);
            Debug.Log("next Integer Timeline marker " + nextIntegerTimelineMarkerCoordinate);
            timelineStartCoordinate = nextIntegerTimelineMarkerCoordinate;
            Debug.Log("Our start coordinate is not at clamp" + timelineStartCoordinate);
        }
        else
        {
            timelineStartCoordinate = trackSectionOriginX;
            Debug.Log("Our start coordinate is at clamp" + timelineStartCoordinate);
            amountOfTimelineMarkers += 1;
            currentNumber += 1;
        }

        while (currentNumber <= currentToTimeClamp)
        {
            Debug.Log("currNumber " + currentNumber + "amountofTimelineMarkers " + amountOfTimelineMarkers);
            if (currentNumber == Mathf.RoundToInt(currentNumber))
            {
                amountOfTimelineMarkers += 1;
                currentNumber += 1;
            }
            else
            {
                Debug.Log("ok time to add");
                currentNumber = (Mathf.RoundToInt(currentNumber) + 1);
                Debug.Log("added" + currentNumber);
            }
        }


        Debug.Log("timelineMarkerCount " + amountOfTimelineMarkers);


        foreach (TimelineTrackMarker t in timelineTrackMarkers)
        {
            Destroy(t.gameObject);
        }

        timelineTrackMarkers = new TimelineTrackMarker[Mathf.FloorToInt(amountOfTimelineMarkers)];

        for (int i = 0; i < timelineTrackMarkers.Length; i++)
        {
            TimelineTrackMarker t = Instantiate(timeline.timelineTrackMarkerPrefab, timeline.transform).GetComponent<TimelineTrackMarker>();
            timelineTrackMarkers[i] = t;

            // center coord of timeline track + amount of y needed to get to top of timeline renderer + amount o
            t.transform.localPosition = new Vector3(0.2f, trackSectionOriginY, timelineStartCoordinate + i * timelineDelta);
            t.transform.localScale = new Vector3(1 / t.transform.parent.localScale.x, 1 / t.transform.parent.localScale.y, 1 / t.transform.parent.localScale.z);
        }

    }

    public void recalculateTimelineTickerPosition()
    {
        // first - check if the current time is within the clamp
        if (timeline.timelineTicker.currentTime < currentFromTimeClamp | timeline.timelineTicker.currentTime > currentToTimeClamp)
        {
            timeline.timelineTicker.gameObject.SetActive(false);
        }
        // if no, make the timeline ticker inactve
        else
        {
            timeline.timelineTicker.gameObject.SetActive(true);
            // first get the transform local position of the current local time
            float newTimelinePosZ = timeline.timelineTicker.currentTime - (timeline.TimelineHalfWidth - timeline.TimelineOwnerWidth) * timeline.ratioBtwnMaxWidthAndMaxTime - currentFromTimeClamp;
            Vector3 newTimelinePos = new Vector3(timeline.timelineTicker.transform.localPosition.x, timeline.timelineTicker.transform.localPosition.y, newTimelinePosZ);
            timeline.timelineTicker.transform.localPosition = newTimelinePos;
        }
        // if yes, generate new position 

    }

    #endregion


    #region  


    #endregion
}
