using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;


/// <summary>
/// Purpose:
/// 
/// To handle Timeline UI configuration
/// To organize Timeline Component coordinates
///
/// </summary>
[System.Serializable]
public class Timeline : MonoBehaviour
{
    // have system to cache / save items
    #region public variables
    public PlayableGraph pg;

    public TimelineTrackSectionRenderer trackSectionData;
    public TimelineScrollBar timelineScrollBar; 

    public bool isTimelineActivated = false;

    public Transform trackSectionOrigin;
    public Transform trackOwnerOrigin;
    public Transform trackSectionHeight;
    public Transform trackMarkersHeight; 

    public delegate void BeizerCurveEvent();
    public TimelineTicker timelineTicker;

    public List<AnimationTrack> tracks;

    public AnimationTrack trackPrefab;
    public VideoKeyFrame keyframePrefab;
    public TimelineTrackMarker timelineTrackMarkerPrefab;
    public TimelineTrackOwner timelineTrackOwnerPrefab; 

    public AnimationTrack currentlySelectedTrack;

    // assuming 0,0 is in the top left coordinate system
    private float menuHeight;
    private float timelineTrackSectionHeight;
    private float timelineOwnersWidth;
    private float timelineTrackWidth;

    // UV coordinates for 
    private Mesh timelineTrackMesh;
    private Vector2[] trackPrefabUVs;
    private Vector2[] trackFaceUVs;
    public int[] trackUVindices = new int[4]; 

    public Vector2[] TrackPrefabUVs
    {
        get { return trackPrefabUVs; }
    }

    public Vector2[] TrackFaceUVs
    {
        get { return trackFaceUVs; }
    }

    /// <summary>
    /// Maximum length of animation clip;
    /// </summary>
    public float timelineMaximumTime;
    private float ratioBtwnMaxWidthAndMaxTime;

    public float RatioBtwnMaxWdithAndMaxTime
    {
        get { return ratioBtwnMaxWidthAndMaxTime; }
    }

    public float TimelineTrackWidth
    {
        get { return timelineTrackWidth; }
    }
    #endregion

    // click on Timeline and tap twice in order to activate the timeline 

    #region turn on / off timeline
    public void TurnOnTimeline()
    {
        // after tapping twice, the UIs will be activated, and these methods will be 
        // available to use 
    }

    public void TurnOffTimeline()
    {


    }

    #endregion

    #region setupTimeline
    public void Awake()
    {
        // might change this and use new Playable Graph instead due to debugging issues

        SetUpPlayableGraph();
        SetupTimelineScriptableObjs();
        SetUpTimelineUVCoordinates();
        SetUpTimelineTrackCoordinates();
        SetUpTimelineTrackMarkers();
        CalculateTimelineTickerPosition();
    }

    // are these values sent to each respective TimelineComponent? 
    public void SetUpPlayableGraph()
    {
        pg = PlayableGraph.Create();
    }

    public void SetupTimelineScriptableObjs()
    {
        trackSectionData = ScriptableObject.CreateInstance<TimelineTrackSectionRenderer>();
        trackSectionData.timeline = this;

    }

    public void SetUpTimelineTrackCoordinates()
    {
        // heights established;
        menuHeight = ( trackSectionOrigin.localPosition.y - trackSectionHeight.localPosition.y);
        timelineTrackSectionHeight = (trackSectionOrigin.localPosition.y - trackSectionHeight.localPosition.y);

        // widths established;
        timelineTrackWidth = trackFaceUVs[0].x - trackFaceUVs[1].x;
        ratioBtwnMaxWidthAndMaxTime = ((timelineMaximumTime / 2) / timelineTrackWidth) * transform.localScale.x;

        // instantiate coordiante system here
        trackSectionData.SetUpTimelineTrackSectionParameters(trackSectionOrigin.localPosition.x, trackSectionOrigin.localPosition.y);
    }

    public void SetUpTimelineUVCoordinates()
    {
        timelineTrackMesh = trackPrefab.GetComponent<MeshFilter>().mesh;
        trackPrefabUVs = timelineTrackMesh.uv;

        trackFaceUVs = new Vector2[4];

        trackFaceUVs[0] = trackPrefabUVs[trackUVindices[0]];
        trackFaceUVs[1] = trackPrefabUVs[trackUVindices[1]];
        trackFaceUVs[2] = trackPrefabUVs[trackUVindices[2]];
        trackFaceUVs[3] = trackPrefabUVs[trackUVindices[3]];

    }

    public void SetUpTimelineTrackMarkers()
    {
        float trackMarkersPosY = trackMarkersHeight.localPosition.y; 
        trackSectionData.initializeTimelineTrackMarkers(trackMarkersPosY);
    }

    public void CalculateTimelineTickerPosition()
    {
        timelineTicker.currentTime = (- timelineTicker.transform.localPosition.x + (trackSectionOrigin.localPosition.x)) * ratioBtwnMaxWidthAndMaxTime + trackSectionData.currentFromTimeClamp;
        timelineTicker.currentTime = Mathf.Clamp(timelineTicker.currentTime, trackSectionData.currentFromTimeClamp, trackSectionData.currentToTimeClamp);
    }

    public float ConvertFromTimeToTimelineZPosition(float time)
    {
        float z;
        z = -1 * (time - trackSectionData.currentFromTimeClamp) / ratioBtwnMaxWidthAndMaxTime + (trackSectionOrigin.localPosition.x);
        return z; 
    }

    #endregion

}
