using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base class for all stimuli types
public class Stimuli
{
    public Vector3 postition;
    public float intensity;
}

// When an actor hears a sound
public class SoundStimuli : Stimuli
{
    public enum _Type { Gunshot, Player, Battle, Misc};
    public _Type type;

}
