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
    public TimelineTrackOwner timelineOwner;
    public TimelineMenu timelineMenu;
    public TimelineScrollBar timelineScrollBar; 

    public bool isTimelineActivated = false;

    public delegate void TimelineTrackEvents();
    public TimelineTrackEvents onZoomIn;
    public TimelineTrackEvents onZoomOut;
    public TimelineTrackEvents onAnimationPlay;

    public delegate void BeizerCurveEvent();
    public TimelineTicker timelineTicker;

    public delegate void TimelineMenuEvent();
    public TimelineMenuEvent OnSave;
    public TimelineMenuEvent OnUndo;
    public TimelineMenuEvent OnRedo;

    public List<AnimationTrack> tracks;

    public GameObject timelineMarker;
    public Canvas timelineCanvas;
    public AnimationTrack trackPrefab;
    public GameObject t_preFab;
    public GameObject keyframePrefab;
    public GameObject timelineTrackMarkerPrefab;

    private float timelineHalfWidth;
    private float timelineFullWidth;
    private float timelineHalfHeight;
    private float timelineFullHeight;
    private float trackHeight;

    public GameObject currentlySelectedTrack;
    private float timelineTrackHeight;
    private float timelineTrackSeperators; 

    // allow for manipulation during editor
    [Range(0, 1)]
    public float timelineTracksWidthRatio;
    [Range(0, 1)]
    public float timelineTrackOwnerWidthRatio;
    [Range(0, 1)]
    public float timelineTrackSectionHeightRatio;
    [Range(0, 1)]
    public float timelineOptionsSectionHeightRatio;

    // assuming 0,0 is in the top left coordinate system
    private float menuHeight;
    private float timelineTrackSectionHeight;
    private float timelineOwnersWidth;
    private float timelineTrackWidth;

    // make 0,0 at the timeline space
    private float menuHeightInTimelineSpace;
    private float timelineTrackSectionHeightInTimelineSpace;
    private float timelineOwnersHeightInTimelineSpace;
    private float timelineTrackWidthInTimelineSpace;

    // UV coordinates for 
    private Mesh timelineTrackMesh;
    public Vector2[] trackPrefabUVs;
    public Vector2[] trackFaceUVs;
    public int[] trackUVindices = new int[4]; 

    public float TimelineHalfWidth
    {
        get { return timelineHalfWidth; }
    }

    public float TimelineFullWidth
    {
        get { return timelineFullWidth;  }
    }

    public float TimelineOwnerWidth
    {
        get { return timelineOwnersWidth; }
    }

    /// <summary>
    /// Maximum length of animation clip;
    /// </summary>
    public float timelineMaximumTime;
    public float ratioBtwnMaxWidthAndMaxTime;

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
    }

    // are these values sent to each respective TimelineComponent? 
    public void SetUpPlayableGraph()
    {
        pg = PlayableGraph.Create();
    }

    public void SetupTimelineScriptableObjs()
    {
        timelineMenu = GetComponent<TimelineMenu>();
        trackSectionData = ScriptableObject.CreateInstance<TimelineTrackSectionRenderer>();
        timelineOwner = ScriptableObject.CreateInstance<TimelineTrackOwner>();

        timelineMenu.timeline = this;
        trackSectionData.timeline = this;
        timelineOwner.timeline = this; 
    }

    public void SetUpTimelineTrackCoordinates()
    {
         // this is for clamping and positioning purposes 
        timelineHalfWidth = GetComponent<Collider>().bounds.size.z / 2 / transform.localScale.z;
        timelineFullWidth = timelineHalfWidth * 2;

        timelineHalfHeight = GetComponent<Collider>().bounds.size.y / 2 / transform.localScale.y;
        timelineFullHeight = timelineHalfHeight * 2;

        // heights established;

        menuHeight = timelineFullHeight * timelineOptionsSectionHeightRatio;
        timelineTrackSectionHeight = timelineFullHeight * timelineTrackSectionHeightRatio;

        // widths established;

        timelineTrackWidth = trackFaceUVs[0].x - trackFaceUVs[1].x;
        timelineOwnersWidth = TimelineFullWidth * timelineTrackOwnerWidthRatio;

        ratioBtwnMaxWidthAndMaxTime = (timelineMaximumTime / 2) / timelineTrackWidth;

        float timelineTrackSectionOriginX = transform.localPosition.z - TimelineHalfWidth + timelineOwnersWidth;
        float timelineTrackSectionOriginY = transform.localPosition.y + timelineHalfHeight - menuHeight;

        float timelineMenuSectionOriginX = transform.localPosition.z - TimelineHalfWidth;
        float timelineMenuSectionOriginY = transform.localPosition.y + timelineHalfHeight;

        float timelineOwnerOriginX = transform.localPosition.z - TimelineHalfWidth;
        float timelineOwnerOriginY = transform.localPosition.y + timelineHalfHeight - menuHeight;

        timelineTrackHeight = (GetComponent<Collider>().bounds.size.y / 2 / transform.localScale.y);

        // instantiate coordiante system here
        trackSectionData.SetUpTimelineTrackSectionParameters(timelineTrackSectionOriginX, timelineTrackSectionOriginY);
        timelineOwner.SetupTimelineTrackOwnerParameters(timelineOwnerOriginX, timelineOwnerOriginY);
        timelineMenu.SetupTimelineMenuParameters(timelineMenuSectionOriginX, timelineMenuSectionOriginY);


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
        trackSectionData.initializeTimelineTrackMarkers();
    }

    public void CalculateTimelineTickerPosition()
    {
        // we need to convert this to timeline track space, not timeline space

        ///  0 ----- 3 
        /// -3 ----- 0 
        /// -6 -3 0 --> -3 0 3 
        /// shift the 0 position -  

        timelineTicker.currentTime = (timelineTicker.transform.localPosition.z + (TimelineHalfWidth - TimelineOwnerWidth)) * ratioBtwnMaxWidthAndMaxTime + trackSectionData.currentFromTimeClamp;
        timelineTicker.currentTime = Mathf.Clamp(timelineTicker.currentTime, trackSectionData.currentFromTimeClamp, trackSectionData.currentToTimeClamp);
    }

    public float ConvertFromTimeToTimelineZPosition(float time)
    {
        float z;
        z = (time - trackSectionData.currentFromTimeClamp) / ratioBtwnMaxWidthAndMaxTime - (TimelineHalfWidth - timelineOwnersWidth);
        return z; 
    }

    #endregion

}
