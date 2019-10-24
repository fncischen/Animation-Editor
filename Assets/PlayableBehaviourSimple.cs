using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class BlenderPlayableBehaviour : PlayableBehaviour
{
    public AnimationMixerPlayable mixerPlayable;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        float blend = Mathf.PingPong((float)playable.GetTime(), 1.0f);

        mixerPlayable.SetInputWeight(0, blend);
        mixerPlayable.SetInputWeight(1, 1.0f - blend);

        base.PrepareFrame(playable, info);
    }
}

public class PlayableBehaviourSimple : MonoBehaviour
{
    PlayableGraph m_Graph;
    public AnimationClip clipA;
    public AnimationClip clipB;

    // Use this for initialization
    void Start()
    {
        // Create the PlayableGraph.
        m_Graph = PlayableGraph.Create();

        // Add an AnimationPlayableOutput to the graph.
        var animOutput = AnimationPlayableOutput.Create(m_Graph, "AnimationOutput", GetComponent<Animator>());

        // Add an AnimationMixerPlayable to the graph.
        var mixerPlayable = AnimationMixerPlayable.Create(m_Graph, 2, false);

        // Add two AnimationClipPlayable to the graph.
        var clipPlayableA = AnimationClipPlayable.Create(m_Graph, clipA);
        var clipPlayableB = AnimationClipPlayable.Create(m_Graph, clipB);

        // Add a custom PlayableBehaviour to the graph.
        // This behavior will change the weights of the mixer dynamically.
        var blenderPlayable = ScriptPlayable<BlenderPlayableBehaviour>.Create(m_Graph, 1);
        blenderPlayable.GetBehaviour().mixerPlayable = mixerPlayable;

        // Create the topology, connect the AnimationClipPlayable to the
        // AnimationMixerPlayable.  Also add the BlenderPlayableBehaviour.
        m_Graph.Connect(clipPlayableA, 0, mixerPlayable, 0);
        m_Graph.Connect(clipPlayableB, 0, mixerPlayable, 1);
        m_Graph.Connect(mixerPlayable, 0, blenderPlayable, 0);

        // Use the ScriptPlayable as the source for the AnimationPlayableOutput.
        // Since it's a ScriptPlayable, also set the source input port to make the
        // passthrough to the AnimationMixerPlayable.
        animOutput.SetSourcePlayable(blenderPlayable);
        animOutput.SetSourceInputPort(0);

        // Play the graph.
        m_Graph.Play();
    }

    private void OnDestroy()
    {
        // Destroy the graph once done with it.
        m_Graph.Destroy();
    }
}