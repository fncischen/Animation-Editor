using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineScrollBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Timeline timeline;

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
        float clampedZ = Mathf.Clamp(transform.localPosition.z, -timeline.TimelineHalfWidth + timeline.TimelineOwnerWidth, timeline.TimelineHalfWidth);

        transform.localPosition = new Vector3(2f, 0.15f, clampedZ);
        currPos = transform.localPosition;

        if (currPos.z > prevPos.z)
        {
            Debug.Log("scroll right");
            float delta = Mathf.Abs(currPos.z - prevPos.z); 
            timeline.trackSectionData.ScrollRight(delta);
        }
        else if (currPos.z < prevPos.z)
        {
            Debug.Log("scroll left");
            float delta = Mathf.Abs(currPos.z - prevPos.z);
            timeline.trackSectionData.ScrollLeft(delta);

        }
    }
}
