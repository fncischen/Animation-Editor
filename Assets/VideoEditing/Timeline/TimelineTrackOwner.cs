using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: to identify ownership of each track, and allow for the user to select track
/// </summary>
public class TimelineTrackOwner : ScriptableObject
{
    public Timeline timeline; 
    // for the timeline track owner, the coord system is at (0-timelineHalfWidth, timelineHalfHeight - menuHeight)
    public void SetupTimelineTrackOwnerParameters(float originX, float originY)
    {

    }
}
