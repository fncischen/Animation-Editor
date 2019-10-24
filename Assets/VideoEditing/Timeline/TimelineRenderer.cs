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

        trackHeight = timeline.t_preFab.GetComponent<Collider>().bounds.size.y/timeline.transform.localScale.y;

        animationTracks = new List<AnimationTrack>();
    }


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
                Vector3 newTrackPosition = new Vector3(newTrackPositionX , currentHeight, 0.078f);
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
        foreach(AnimationTrack track in timeline.tracks)
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

    #region timeline configuration
    public void ZoomIn()
    {
        if (timeline.isTimelineActivated)
        {

        }
    }

    public void ZoomOut()
    {
        if (timeline.isTimelineActivated)
        {

        }
    }

    public void ScrollLeft()
    {
        if (timeline.isTimelineActivated)
        {

        }
    }

    public void ScrollRight()
    {
        if (timeline.isTimelineActivated)
        {

        }
    }

    #endregion

}
