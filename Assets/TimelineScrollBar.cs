using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineScrollBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Timeline timeline;
    public GameObject scrollBox; 

    private Vector3 prevPos;
    private Vector3 currPos;



    Vector3 offset;
    Vector3 screenPos;

    private void OnMouseDown()
    {
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z));
    }

    private void OnMouseDrag()
    {
        /// how to clamp?
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        prevPos = transform.localPosition;
        transform.position = cursorPosition;

        // convert to local position 
        timeline.transform.InverseTransformPoint(transform.position);
        float timelineTrackDistance = (-timeline.TimelineTrackWidth / timeline.transform.localScale.x + timeline.trackSectionOrigin.localPosition.x);
        float clampedX = Mathf.Clamp(transform.localPosition.x, timelineTrackDistance, timeline.trackSectionOrigin.localPosition.x);

        transform.localPosition = new Vector3(clampedX, scrollBox.transform.localPosition.y, 0.001f);
        currPos = transform.localPosition;

        if (currPos.x < prevPos.x)
        {
            Debug.Log("scroll right");
            float delta = Mathf.Abs(currPos.x - prevPos.x)*100;
            timeline.trackSectionData.ScrollRight(delta);
        }
        else if (currPos.x > prevPos.x)
        {
            Debug.Log("scroll left");
            float delta = Mathf.Abs(currPos.x - prevPos.x)*100;

            timeline.trackSectionData.ScrollLeft(delta);

        }
    }
}
