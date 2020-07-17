using System;

namespace KlangRageAudioLibrary
{
    [Flags]
    public enum AudioFlags
    {
        None = 0,
        Loop = 1,
        InteriorSound = 2,
        ExteriorSound = 4,
        FadeIn = 8,
        FadeOut = 16
    }
}