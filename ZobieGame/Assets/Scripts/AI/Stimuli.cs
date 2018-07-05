using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base class for all stimuli types
public class Stimuli
{
    public Vector3 position;
    public float intensity;
    public int type;
}

// When an actor hears a sound
public class SoundStimuli : Stimuli
{
    public enum _Type { Player, Gunshot, Battle, Misc};
    public _Type type;
}

// When an actor sees something interesting
public class VisualStimuli : Stimuli
{
    public enum _Type { Player, Bait, Light, Misc };
    public _Type type;
}
