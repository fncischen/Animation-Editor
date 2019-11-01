using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimelineTrackMarker : MonoBehaviour
{
    public Text text;
    public void ChangeText(string str)
    {
        text.text = str; 
    }
}
