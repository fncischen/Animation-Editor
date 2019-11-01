using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineTicker : MonoBehaviour
{
    public float currentTime;
    public Timeline timeline;

    private Vector3 screenPos; 
    private Vector3 offset;

    public void OnEnable()
    {
        // timeline.CalculateTimelineTickerPosition();
    }
    public void Update()
    {

    }

    public void OnMouseDown()
    {
        // convert position of screen to world position;
        Debug.Log("down!");

        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z));
    }

    public void OnMouseDrag()
    {
        Debug.Log("dragging!");
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        transform.position = cursorPosition;
        
        // convert to local position 
        timeline.transform.InverseTransformPoint(transform.position);
        float timelineTrackDistance = (-timeline.TimelineTrackWidth/timeline.transform.localScale.x + timeline.trackSectionOrigin.localPosition.x);
        float clampedX = Mathf.Clamp(transform.localPosition.x,timelineTrackDistance, timeline.trackSectionOrigin.localPosition.x);
        Debug.Log("localPositionX " + transform.localPosition.x);
        Debug.Log("trackSectionOrigin " + timeline.trackSectionOrigin.localPosition.x + " trackWidth " + timelineTrackDistance);
        transform.localPosition = new Vector3(clampedX, timeline.trackMarkersHeight.localPosition.y, 0.4f/timeline.transform.localScale.z);

        timeline.CalculateTimelineTickerPosition();
    }

    private void OnMouseUp()
    {
        Debug.Log("up!");
        screenPos = Vector3.zero;
        offset = Vector3.zero; 
    }
}

