using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(PlayableDirector))]
[ExecuteInEditMode]
public class PlayableDirectorModifier : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private PlayableGraph pg;

    public AnimationClip clipA;
    public AnimationClip clipB; 
    public void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        PlayableAsset p = playableDirector.playableAsset;

        SetParametersTwo();
        Debug.Log("This function ran on edit mode!");
    }

    public void Start()
    {
    }

    public void SetGraphParameters()
    {
        
        var blenderPlayable = ScriptPlayable<BlenderPlayableBehaviourZero>.Create(pg, 1);
        if (pg.IsValid())
        {
            Debug.Log("There is a player graph");
        }
        else
        {
            Debug.Log("There is no player graph!");
        }
        
        BlenderPlayableBehaviourZero g = blenderPlayable.GetBehaviour();

        var animOutput = AnimationPlayableOutput.Create(pg, "AnimationOutput", GetComponent<Animator>());
        Debug.Log(animOutput + " Hi!");
        var mixerPlayable = AnimationMixerPlayable.Create(pg, 2, false);
        Debug.Log(mixerPlayable + " Nope!");
        blenderPlayable.GetBehaviour().mixerPlayable = mixerPlayable;

        pg.Connect(mixerPlayable, 0, blenderPlayable, 0);

        animOutput.SetSourcePlayable(blenderPlayable);
        animOutput.SetSourceInputPort(0);

        g.animOutput = animOutput;
        g.mixerPlayable = mixerPlayable;

    }

    public void SetParametersTwo()
    {
        // Add an AnimationPlayableOutput to the graph.
        if (pg.IsValid())
        {
            Debug.Log("There is a player graph");
        }
        else
        {
            Debug.Log("There is no player graph!");
        }

        var animOutput = AnimationPlayableOutput.Create(pg, "AnimationOutput", GetComponent<Animator>());

        // Add an AnimationMixerPlayable to the graph.
        var mixerPlayable = AnimationMixerPlayable.Create(pg, 2, false);

        // Add a custom PlayableBehaviour to the graph.
        // This behavior will change the weights of the mixer dynamically.
        var blenderPlayable = ScriptPlayable<BlenderPlayableBehaviourZero>.Create(pg, 1);
        blenderPlayable.GetBehaviour().mixerPlayable = mixerPlayable;

        var clipPlayableA = AnimationClipPlayable.Create(pg, clipA);
        var clipPlayableB = AnimationClipPlayable.Create(pg, clipB);

        pg.Connect(clipPlayableA, 0, mixerPlayable, 0);
        pg.Connect(clipPlayableB, 0, mixerPlayable, 1);
        pg.Connect(mixerPlayable, 0, blenderPlayable, 0);

        // Use the ScriptPlayable as the source for the AnimationPlayableOutput.
        // Since it's a ScriptPlayable, also set the source input port to make the
        // passthrough to the AnimationMixerPlayable.
        animOutput.SetSourcePlayable(blenderPlayable);
        animOutput.SetSourceInputPort(0);

        // Play the graph.
    }
   
}
