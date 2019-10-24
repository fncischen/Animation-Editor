using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
public class LightControlAsset : PlayableAsset
{
    public ExposedReference<Light> light;
    public Color color = Color.white;
    public float intensity = 1.0f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<LightControlBehaviour>.Create(graph);

        var lightControlBehaviour = playable.GetBehaviour();
        lightControlBehaviour.light = light.Resolve(graph.GetResolver());
        lightControlBehaviour.color = color;
        lightControlBehaviour.intensity = intensity;

        return playable;
    }
}
