using Holoville.HOTween;
using UnityEngine;

/// <summary>
/// Some example of how to create tweens with HOTween.
/// To learn more and find more examples, go to http://hotween.holoville.com
/// </summary>
public class HOTweenDemoBrain : MonoBehaviour
{
    public Transform CubeTrans1;
    public Transform CubeTrans2;
    public Transform CubeTrans3;
    public string SampleString;
    public float SampleFloat = 0;

    // ===================================================================================
    // UNITY METHODS ---------------------------------------------------------------------

    void Start()
    {
        // HOTWEEN INITIALIZATION
        // Must be done only once, before the creation of your first tween
        // (you can skip this if you want, and HOTween will be initialized automatically
        // when you create your first tween - using default values)
        HOTween.Init(true, true, true);

        // TWEEN CREATION
        // With each one is shown a different way you can create a tween,
        // so that in the end you can choose what you prefer

        // Tween the first transform using fast writing mode,
        // and applying an animation that will last 4 seconds
        HOTween.To(CubeTrans1, 4, "position", new Vector3(-3, 6, 0));

        // Tween the second transform using full mode with parameters and linebreaks,
        // applying a yoyo loop with custom ease,
        // and assigning an onStepComplete callback
        // which will be fired each time a loop iteration is complete.
        // Also note that we are tweening 2 proporties of the same target
        // (a transform, in this case)
        HOTween.To(CubeTrans2, 3, new TweenParms()
            .Prop("position", new Vector3(0, 6, 0), true) // Position tween (set as relative)
            .Prop("rotation", new Vector3(0, 1024, 0), true) // Relative rotation tween (this way rotations higher than 360 can be applied)
            .Loops(-1, LoopType.Yoyo) // Infinite yoyo loops
            .Ease(EaseType.EaseInOutQuad) // Ease
            .OnStepComplete(Cube2StepComplete) // OnComplete callback
        );

        // Tween the sample string using full mode with parameters and without linebreaks.
        // The result will be shown using OnGUI
        HOTween.To(this, 3, new TweenParms().Prop("SampleString", "Hello I'm a sample tweened string").Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo));

        // Tween the sample floating point number while creating TweenParms first,
        // and then assigning it to HOTween.
        TweenParms tweenParms = new TweenParms().Prop("SampleFloat", 27.5f).Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo);
        HOTween.To(this, 3, tweenParms);

        // SEQUENCE CREATION
        // Here you'll see how to create a Sequence,
        // which stores your tweens in a timeline-like way

        // Create a Sequence with cube3, which will rotate it,
        // then move it upwards, the rotate it again,
        // all the while changing the alpha of its material's color
        // (which can be done since the cube uses a transparent material).
        // The Sequence will also be set to loop.
        // Note that Sequences don't start automatically, so you'll have to call Play().
        Color colorTo = CubeTrans3.renderer.material.color;
        colorTo.a = 0;
        Sequence sequence = new Sequence(new SequenceParms().Loops(-1, LoopType.Yoyo));
        // "Append" will add a tween after the previous one/s have completed
        sequence.Append(HOTween.To(CubeTrans3, 1, new TweenParms().Prop("rotation", new Vector3(360, 0, 0))));
        sequence.Append(HOTween.To(CubeTrans3, 1, new TweenParms().Prop("position", new Vector3(0, 6, 0), true)));
        sequence.Append(HOTween.To(CubeTrans3, 1, new TweenParms().Prop("rotation", new Vector3(0, 360, 0))));
        // "Insert" lets you insert a tween where you want
        // (in this case we're having it start at half the sequence and last until the end)
        sequence.Insert(sequence.duration * 0.5f, HOTween.To(CubeTrans3.renderer.material, sequence.duration * 0.5f, new TweenParms().Prop("color", colorTo)));
        // Start the sequence animation
        sequence.Play();
    }

    void OnGUI()
    {
        // Here we show the sample string and float being tweened
        GUILayout.Label("String tween: " + SampleString);
        GUILayout.Label("Float tween: " + SampleFloat);
    }

    // ===================================================================================
    // METHODS ---------------------------------------------------------------------------

    void Cube2StepComplete()
    {
        Debug.Log("HOTween: Cube 2 Step Complete");
    }
}
