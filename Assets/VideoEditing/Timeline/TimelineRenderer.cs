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
    public float timelineTrackSeperators = 0.02f;

    private float trackSectionOriginX;
    private float trackSectionOriginY;
    private float trackMarkersHeightY; 

    private int amountOfTimelineMarkers;
    private float timelineDelta;
    private float timelineTrackUVwidth;
    private float timeDelta; 
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

        trackHeight = timeline.trackPrefab.GetComponent<Collider>().bounds.size.y / timeline.transform.localScale.y;
        animationTracks = new List<AnimationTrack>();
    }

    public void initializeTimelineTrackMarkers(float trackMarkersOriginY)
    {
        // few factors

        // What % of timelineMaxTime is clamped
        // what is the difference between the left and right uv coordinates?
        trackMarkersHeightY = trackMarkersOriginY;


        timelineTrackUVwidth = timeline.TrackFaceUVs[0].x - timeline.TrackFaceUVs[1].x;
        // Debug.Log("timelineDiff " + timelineTrackUVwidth);
        // that difference is X units of time clamped inside the timeline (at current scale) --- this is our INITIAL clamp
        currentFromTimeClamp = 0;
        currentToTimeClamp = timeline.timelineMaximumTime / 2;
        // Debug.Log("toclamptime " + currentToTimeClamp);

        amountOfTimelineMarkers = Mathf.FloorToInt(currentToTimeClamp - currentFromTimeClamp) + 1; //
        timelineDelta = (timelineTrackUVwidth/timeline.transform.localScale.x) / (timeline.timelineMaximumTime / 2); // adjust for timeline scale
        

        // make sure all the material textures are scaled at 1,1;
        foreach (AnimationTrack track in animationTracks)
        {
            Material mat = track.GetComponent<Renderer>().material;
            mat.SetTextureScale("_MainTex", new Vector2(1, 1));
        }

        // Debug.Log("TimelineDiff " + timelineTrackUVwidth);

        // set up timeline track markers
        timelineTrackMarkers = new TimelineTrackMarker[Mathf.FloorToInt(amountOfTimelineMarkers)];
        timeDelta = (timeline.timelineMaximumTime / 2) / amountOfTimelineMarkers;

        // set up positions for these timeline track markers
        for (int i = 0; i < timelineTrackMarkers.Length; i++)
        {
            TimelineTrackMarker t = Instantiate(timeline.timelineTrackMarkerPrefab, timeline.transform).GetComponent<TimelineTrackMarker>();
            timelineTrackMarkers[i] = t;

            int time = Mathf.RoundToInt(i * timeDelta + currentFromTimeClamp);
            string timeText = time.ToString();

            t.ChangeText(timeText);

            // center coord of timeline track + amount of y needed to get to top of timeline renderer + amount o
            t.transform.localPosition = new Vector3(trackSectionOriginX - i * timelineDelta, trackMarkersHeightY, 0.2f/timeline.transform.localScale.z);
            // t.transform.localScale = new Vector3(1 / t.transform.parent.localScale.x, 1 / t.transform.parent.localScale.y, 1 / t.transform.parent.localScale.z);
        }
    }

    #endregion

    #region track add / remove / update methods
    public void AddAnimationTrack(AnimatedObject obj)
    {

        GameObject g = Instantiate(timeline.trackPrefab.gameObject, timeline.transform.parent);
        g.transform.rotation = timeline.trackPrefab.transform.rotation;
        g.transform.localScale = timeline.trackPrefab.transform.localScale;

        AnimationTrack newTrack = g.GetComponent<AnimationTrack>();       
        animationTracks.Add(newTrack);

        int trackIndex = animationTracks.IndexOf(newTrack);
        Vector3 newTrackPosition = TimelineTrackPosition(trackIndex);

        newTrack.SetUpTrackProperties(obj, newTrackPosition, timeline, trackIndex, timeline.trackPrefab.unselectedColor, timeline.trackPrefab.selectedColor);
        obj.animTrack = newTrack;

        /// add animationOwner
        GameObject trackOwner = Instantiate(timeline.timelineTrackOwnerPrefab.gameObject, timeline.transform);
        Debug.Log("created trackOwner " + trackOwner);
        trackOwner.transform.rotation = timeline.timelineTrackOwnerPrefab.transform.rotation;
        trackOwner.transform.localScale = new Vector3(1,1,1);

        trackOwner.transform.localPosition = TimelineTrackOwnerPosition(trackIndex);
        TimelineTrackOwner newTrackOwner = trackOwner.GetComponent<TimelineTrackOwner>();

        newTrack.timelineTrackOwner = newTrackOwner;
        newTrackOwner.track = newTrack; 

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
            track.animatedObject.beizerPathGroup.ResetBeizerPathCurves();
            
            Destroy(track.gameObject);
            Destroy(track.timelineTrackOwner.gameObject);

            // recalculate track positions accordingly
            int tracksAfterTrackIndex = trackIndex + 1;

            for (int i = tracksAfterTrackIndex; i <= animationTracks.Count; i++)
            {
                Vector3 newTrackPosition = TimelineTrackPosition(i);
                Vector3 newTrackOwnerPosition = TimelineTrackOwnerPosition(i);

                animationTracks[i].transform.localPosition = newTrackPosition;
                animationTracks[i].timelineTrackOwner.transform.localPosition = newTrackOwnerPosition;
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
                animationTrackMesh.uv = timeline.TrackFaceUVs;

            }

            currentFromTimeClamp -= scrollDelta;
            currentToTimeClamp -= scrollDelta;

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();
            recalcuateAnimationTrackKeyframePositions();
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
                animationTrackMesh.uv = timeline.TrackFaceUVs;

            }

            // second, recalculate timeclamped 

            currentFromTimeClamp += scrollDelta;
            currentToTimeClamp += scrollDelta;

            recalculateTimelineTrackMarkers();
            recalculateTimelineTickerPosition();
            recalcuateAnimationTrackKeyframePositions();
        }

    }

    private void shiftUVsLeft(float scrollDelta)
    {
        timeline.TrackPrefabUVs[0].x -= scrollDelta;
        timeline.TrackPrefabUVs[1].x -= scrollDelta;
        timeline.TrackPrefabUVs[2].x -= scrollDelta;
        timeline.TrackPrefabUVs[3].x -= scrollDelta;

        timeline.TrackFaceUVs[timeline.trackUVindices[0]] = timeline.TrackPrefabUVs[0];
        timeline.TrackFaceUVs[timeline.trackUVindices[1]] = timeline.TrackPrefabUVs[1];
        timeline.TrackFaceUVs[timeline.trackUVindices[2]] = timeline.TrackPrefabUVs[2];
        timeline.TrackFaceUVs[timeline.trackUVindices[3]] = timeline.TrackPrefabUVs[3];
    }

    private void shiftUVsRight(float scrollDelta)
    {
            timeline.TrackPrefabUVs[0].x += scrollDelta;
            timeline.TrackPrefabUVs[1].x += scrollDelta;
            timeline.TrackPrefabUVs[2].x += scrollDelta;
            timeline.TrackPrefabUVs[3].x += scrollDelta;

            timeline.TrackFaceUVs[timeline.trackUVindices[0]] = timeline.TrackPrefabUVs[0];
            timeline.TrackFaceUVs[timeline.trackUVindices[1]] = timeline.TrackPrefabUVs[1];
            timeline.TrackFaceUVs[timeline.trackUVindices[2]] = timeline.TrackPrefabUVs[2];
            timeline.TrackFaceUVs[timeline.trackUVindices[3]] = timeline.TrackPrefabUVs[3];
    }

    #endregion
    // do some planning

    #region timelineTrackMarker, keyFrame, and ticker calculations
    private void recalculateTimelineTrackMarkers()
    {
        // calculate amount of integers between currentToTimeClamp and c
        amountOfTimelineMarkers = 0;
        float currentNumber = currentFromTimeClamp;

        Debug.Log("currentFromTimeClamp " + currentFromTimeClamp);
        Debug.Log("currentToTimeClamp " + currentToTimeClamp);


        float timelineStartCoordinate;
        if (currentNumber != Mathf.FloorToInt(currentNumber)) {
            // convert currentNumber to timelineCoordinate
            int nextNumber = Mathf.FloorToInt(currentNumber) + 1;

            float nextIntegerTimelineMarkerCoordinate = timeline.ConvertFromTimeToTimelineZPosition(nextNumber);
            timelineStartCoordinate = nextIntegerTimelineMarkerCoordinate;
        }
        else
        {
            timelineStartCoordinate = trackSectionOriginX;
            amountOfTimelineMarkers += 1;
            currentNumber += 1;
        }

        while (currentNumber <= currentToTimeClamp)
        {
            if (currentNumber == Mathf.RoundToInt(currentNumber))
            {
                amountOfTimelineMarkers += 1;
                currentNumber += 1;
            }
            else
            {
                currentNumber = (Mathf.RoundToInt(currentNumber) + 1);
            }
        }

        foreach (TimelineTrackMarker t in timelineTrackMarkers)
        {
            Destroy(t.gameObject);
        }

        timelineTrackMarkers = new TimelineTrackMarker[Mathf.FloorToInt(amountOfTimelineMarkers)];

        for (int i = 0; i < timelineTrackMarkers.Length; i++)
        {
            TimelineTrackMarker t = Instantiate(timeline.timelineTrackMarkerPrefab, timeline.transform).GetComponent<TimelineTrackMarker>();
            timelineTrackMarkers[i] = t;

            int time = Mathf.RoundToInt(i * timeDelta + currentFromTimeClamp);
            string timeText = time.ToString();

            t.ChangeText(timeText);

            // center coord of timeline track + amount of y needed to get to top of timeline renderer + amount o
            t.transform.localPosition = new Vector3(timelineStartCoordinate - i * timelineDelta, trackMarkersHeightY, 0.2f / timeline.transform.localScale.z);
            // t.transform.localScale = new Vector3(1 / t.transform.parent.localScale.x, 1 / t.transform.parent.localScale.y, 1 / t.transform.parent.localScale.z);
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
            float newTimelinePosX = timeline.ConvertFromTimeToTimelineZPosition(timeline.timelineTicker.currentTime);
            Vector3 newTimelinePos = new Vector3(newTimelinePosX, timeline.timelineTicker.transform.localPosition.y,timeline.timelineTicker.transform.localPosition.z);
            timeline.timelineTicker.transform.localPosition = newTimelinePos;
        }
        // if yes, generate new position 

    }

    public void recalcuateAnimationTrackKeyframePositions()
    {
        foreach(AnimationTrack animTrack in animationTracks)
        {
            animTrack.UpdateKeyframeUI();
        }
    }

    #endregion

    #region  helper functions 

    public Vector3 TimelineTrackOwnerPosition(int timelineTrackIndex)
    {
        float x = timeline.trackOwnerOrigin.localPosition.x;
        float y = trackSectionOriginY - (trackHeight / 2 * (timelineTrackIndex + 1) + timelineTrackSeperators * (timelineTrackIndex));
        float z = 0.078f / timeline.transform.localScale.z;

        Vector3 timelineTrackOwnerPos = new Vector3(x, y, z);
        return timelineTrackOwnerPos;
    }

    public Vector3 TimelineTrackPosition(int timelineTrackIndex)
    {
        float newTrackPositionY = trackSectionOriginY - (trackHeight / 2 * (timelineTrackIndex + 1) + timelineTrackSeperators * (timelineTrackIndex));
        float newTrackPositionX = trackSectionOriginX - (timeline.trackPrefab.GetComponent<Collider>().bounds.size.z / 2) / timeline.transform.localScale.x;
        float z = 0.078f / timeline.transform.localScale.z;

        Vector3 timelineTrackPos = new Vector3(newTrackPositionX, newTrackPositionY, z);
        return timelineTrackPos;
    }

    #endregion
}
