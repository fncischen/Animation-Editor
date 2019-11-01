using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Purpose: to identify ownership of each track, and allow for the user to select track
/// </summary>
public class TimelineTrackOwner : MonoBehaviour
{
    public Timeline timeline;
    public AnimationTrack track; 
    public Text text;

    public void ChangeText(string str)
    {
        text.text = str; 
    }

}
