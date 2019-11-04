# Mobile Animation Editor
A 3D animation editor that allows for content creation natively on mobile devices. (developed on Unity)

<img src="https://github.com/fncischen/Animation-Editor/blob/master/Documentation/AnimationEditorGIF.gif"></img>

## Summary ##

With the rise of spatial computing (VR/AR) and 3D as the next iteration of how media & commerce flow, there will be an increased demand of <a href="https://twitter.com/fncischen/status/1181158122636754945">native 3D authoring tools for consumers</a>. 

Inspired by 3D computer graphics tools such as Blender, and the rise of media & distribution platforms such as Snap & TikTok / 抖音, I decided to create a lightweight animation editor that could handle 3D content creation natively on mobile devices. 

## How it works ##

All animation clips are created and sent to the <a href="https://docs.unity3d.com/Manual/Playables-Graph.html">Unity Playables Graph</a>, which allows audiences to play, pause, & replay their creations.

The editor comes with a built-in timeline dashboard that takes advantage of the <a href="https://blogs.unity3d.com/2017/08/02/unity-2017-1-feature-spotlight-playable-api/">Unity Playables API</a>. 

To make an animation, the player creates an <a href="https://docs.unity3d.com/ScriptReference/AnimationClip.html">animation clip</a> on the Timeline Dashboard. All <a href="https://docs.unity3d.com/ScriptReference/Keyframe.html">animation keyframes</a> are added, removed, or updated on the animation track through the Timeline Dashboard or the Animated Object's Built-In UI. 

The animation track stores all <a href="https://docs.unity3d.com/ScriptReference/AnimationCurve.html">animation curves</a>, which stores data on the <a href="https://docs.unity3d.com/Manual/animeditor-AnimationCurves.html">object's position & rotation</a> at different points in time on the animation clip. These animation curves are sent to the animation clip each time a keyframe is added, removed or updated.  

## How to set up ##

Download the Unity package <a href="https://github.com/fncischen/Animation-Editor/archive/master.zip">zip file here</a>. The project is currently in development, as I am adding save / open features and 3D asset search to allow for project reusability. 

## Next Steps ##

1. Make this an augmented realiy app that allows the audience to make short video clips that one can share on Snap or TikTok. 
2. Create an 3D interaction design system that leverages VFX (i.e. dynamic shaders) that create a seamless and user-friendly feedback loop throughout the animation editing experience. 
3. Add 3D onion skinning, improved linear / beizer path designs to keep track of the real-time positions and rotations of each object. 




