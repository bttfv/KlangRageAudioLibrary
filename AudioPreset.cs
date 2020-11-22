namespace KlangRageAudioLibrary
{
    public class AudioPreset
    {
        public float Volume { get; set; }
        public float MinimumDistance { get; set; }
        public AudioFlags Flags { get; set; }
        public AudioPreset(float volume, float minDistance, AudioFlags flags)
        {
            Volume = volume;
            MinimumDistance = minDistance;
            Flags = flags;
        }
    }
    public class Presets
    {
        public static AudioPreset Interior { get; } = new AudioPreset(0.5f, 2f, AudioFlags.InteriorSound);
        public static AudioPreset Exterior { get; } = new AudioPreset(1f, 5f, AudioFlags.ExteriorSound);
        public static AudioPreset InteriorLoop { get; } = new AudioPreset(0.5f, 2f, AudioFlags.InteriorSound |
                                                                                    AudioFlags.FadeIn |
                                                                                    AudioFlags.FadeOut |
                                                                                    AudioFlags.Loop);
        public static AudioPreset ExteriorLoop { get; } = new AudioPreset(1f, 5f, AudioFlags.ExteriorSound |
                                                                                  AudioFlags.FadeIn |
                                                                                  AudioFlags.FadeOut |
                                                                                  AudioFlags.Loop);

        public static AudioPreset ExteriorLoud { get; } = new AudioPreset(4f, 8f, AudioFlags.ExteriorSound);

        public static AudioPreset ExteriorLoudLoop { get; } = new AudioPreset(4f, 8f, AudioFlags.ExteriorSound |
                                                                                  AudioFlags.FadeIn |
                                                                                  AudioFlags.FadeOut |
                                                                                  AudioFlags.Loop);

        public static AudioPreset No3D { get; } = new AudioPreset(1f, 8f, AudioFlags.No3D);

        public static AudioPreset No3DLoop { get; } = new AudioPreset(1f, 8f, AudioFlags.No3D | AudioFlags.Loop);
    }
}