using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObjectMenu : MonoBehaviour
{
    // note --> set up menu animations to make interaction design more interesting ;
    public Timeline timeline;
    public AnimatedObjectGimbals gimbals;

    public GameObject createTrackButton;
    public GameObject removeTrackButton;
    public GameObject addKeyframeButton;

    public void OnEnable()
    {
        if (gimbals.animObj & !gimbals.animObj.animTrack)
        {
            createTrackButton.SetActive(true);
            removeTrackButton.SetActive(false);
        }
    }

    public void AddOrUpdateKeyframe()
    {
        gimbals.animObj.SetOrUpdateKeyframes();
    }
    public void AddTrackToTimeline()
    {
        timeline.trackSectionData.AddAnimationTrack(gimbals.animObj);
        createTrackButton.SetActive(false);
        removeTrackButton.SetActive(true);
    }

    public void removeTrackFromTimeline()
    {
        timeline.trackSectionData.RemoveAnimationTrack(gimbals.animObj.animTrack);
        createTrackButton.SetActive(true);
        removeTrackButton.SetActive(false);
    }
}
