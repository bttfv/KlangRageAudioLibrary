using GTA;
using GTA.Math;
using IrrKlang;
using System.Collections.Generic;

namespace KlangRageAudioLibrary
{
    public partial class AudioPlayer
    {
        /// <summary>
        /// Returns full path to the sound.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Returns true if audio is played from interior, works only with <seealso cref="Vehicle"/>
        /// </summary>
        public bool IsInteriorSound { get; private set; }

        /// <summary>
        /// Returns true if audio is played from outside, works only with <seealso cref="Vehicle"/>
        /// </summary>
        public bool IsExteriorSound { get; private set; }

        /// <summary>
        /// Will be audio starter with fade in.
        /// </summary>
        public bool StartFadeIn { get; set; }

        /// <summary>
        /// Will be audio stopped with fade out.
        /// </summary>
        public bool StopFadeOut { get; set; }

        /// <summary>
        /// Returns true if audio doing fade out sequence.
        /// </summary>
        public bool IsDoingFadeOut { get; private set; }

        /// <summary>
        /// Returns true if audio doing fade in sequence.
        /// </summary>
        public bool IsDoingFadeIn { get; private set; }

        /// <summary>
        /// Defines how long it takes to complete fade out sequence, e.g. 0.1 - slow / 1 - fast
        /// </summary>
        public float FadeOutMultiplier { get; set; } = 1f;

        /// <summary>
        /// Defines how long it takes to complete fade in sequence, e.g. 0.1 - slow / 1 - fast
        /// </summary>
        public float FadeInMultiplier { get; set; } = 1f;

        /// <summary>
        /// Changes the distance at which the 3D sound stops getting louder.
        /// </summary>
        public float MinimumDistance { get; set; } = 5f;

        /// <summary>
        /// Sets the position of the sound in 3d space, needed for Doppler effects.
        /// </summary>
        public Vector3 Velocity { get; set; } = Vector3.Zero;

        /// <summary>
        /// Sets the volume of all playing instances.
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        /// Returns true if any instance of this sound is still playing.
        /// </summary> 
        public bool IsAnyInstancePlaying { get; private set; }

        /// <summary>
        /// Defines source of audio for stereo sound.
        /// </summary>
        public Entity SourceEntity { get; set; }

        /// <summary>
        /// Defines bone for stereo sound.
        /// </summary>
        public string SourceBone { get; set; } = string.Empty;

        /// <summary>
        /// All instances of currently playing sound.
        /// </summary>
        public List<ISound> SoundInstances { get; internal set; }

        /// <summary>
        /// Number of currently playing instances.
        /// Use this one if u want to get number of playing sounds
        ///     because SoundInstances contains finished ones.
        /// </summary>
        public int InstancesNumber { get; private set; }

        /// <summary>
        /// Returns ISound object of latest added sound.
        /// </summary>
        public ISound Last { get; private set; }

        /// <summary>
        /// Contains all audio player flags.
        /// </summary>
        public AudioFlags Flags { get; set; }
    }
}
