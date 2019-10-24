using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class PickingUpAsset : PlayableAsset 
{
    public ExposedReference<Animator> anim;
    public AnimationClip clip;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<BlenderPlayableBehaviourZero>.Create(graph);
        var blenderPlayableBehavior = playable.GetBehaviour();

        blenderPlayableBehavior.clip = clip;
        blenderPlayableBehavior.anim = anim.Resolve(graph.GetResolver());
        blenderPlayableBehavior.graph = graph; 

        blenderPlayableBehavior.Start();
        
        return playable; 
    }
}
