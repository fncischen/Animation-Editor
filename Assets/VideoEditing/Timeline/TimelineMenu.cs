using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// Purpose: to invoke all keyframe related events to the specific animation track on state
/// </summary>
public class TimelineMenu : MonoBehaviour
{
    public Timeline timeline; 

    public void SetupTimelineMenuParameters(float originX, float originY) { }
    public void PlayTracks()
    {
        timeline.pg.Play();

        AnimatedObjectGimbals.Instance.gameObject.SetActive(false);

        foreach (AnimationTrack track in timeline.trackSectionData.animationTracks)
        {
            AnimationClip clip = track.clip;

            AnimationClipSettings tSettings = AnimationUtility.GetAnimationClipSettings(clip);
            Debug.Log(tSettings.loopTime);
            tSettings.loopTime = true;
            Debug.Log(tSettings.loopTime);

            AnimationUtility.SetAnimationClipSettings(clip, tSettings);
        }
    }

    public void StopTracks()
    {
        timeline.pg.Stop();
        // resetClipsToBeginning();
    }

    public void resetClipsToBeginning()
    {
        foreach (AnimationTrack track in timeline.trackSectionData.animationTracks)
        {
            AnimationClip clip = track.clip;

            AnimationClipSettings tSettings = AnimationUtility.GetAnimationClipSettings(clip);
            Debug.Log(tSettings.loopTime);
            tSettings.loopTime = false;
            tSettings.startTime = 0;
            Debug.Log(tSettings.loopTime);

            AnimationUtility.SetAnimationClipSettings(clip, tSettings);
        }
    }

}
