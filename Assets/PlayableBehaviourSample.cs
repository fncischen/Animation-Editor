using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class BlenderPlayableBehaviourZero : PlayableBehaviour
{
    public AnimationMixerPlayable mixerPlayable;
    public AnimationPlayableOutput animOutput; 

    public Animator anim;
    public Animation animation; 

    public AnimationClip clip;

    public PlayableGraph graph;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        float blend = Mathf.PingPong((float)playable.GetTime(), 1.0f);

        mixerPlayable.SetInputWeight(0, blend);
        mixerPlayable.SetInputWeight(1, 1.0f - blend);

        base.PrepareFrame(playable, info);
    }

    public void Start()
    {
        var clipPlayableA = AnimationClipPlayable.Create(graph, clip);
        graph.Connect(clipPlayableA, 0, mixerPlayable, 0);
    }

    // use this for Animation curves
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

    }
}
