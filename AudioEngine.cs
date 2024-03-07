using GTA;
using IrrKlang;
using KlangRageAudioLibrary.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// FIXME: Script doesn't detect correctly cinematic camera position
// TODO: Disable proccesing if no sound playing

namespace KlangRageAudioLibrary
{
    public class AudioEngine
    {
        /// <summary>
        /// Defines path to base sound folder, e.g. ModFolder/Sounds
        /// </summary>
        public string BaseSoundFolder { get; set; } = string.Empty;

        /// <summary>
        /// Controls state of automatic audio fade in/out system.
        /// </summary>
        public bool EnableFadeSystem { get; set; } = true;

        /// <summary>
        /// Defines default <see cref="Entity"/> for stereo sound.
        ///  You can override it in <see cref="AudioPlayer"/> instance.
        /// </summary>
        public Entity DefaultSourceEntity { get; set; }

        /// <summary>
        /// Returns number of created <see cref="AudioPlayer"/>.
        /// </summary>
        public int SoundsCount => _allSounds.Count;

        /// <summary>
        /// Returns true if any instance of the sounds is still playing.
        /// </summary> 
        public bool IsAnyInstancePlaying => _allSounds.Any(x => x.IsAnyInstancePlaying);

        /// <summary>
        /// Returns <see cref="ISoundEngine"/> object.
        /// </summary>
        public static ISoundEngine SoundEngine { get; }

        internal static bool IsMuted { get; private set; }

        internal static bool IsFadingOut { get; private set; }

        internal static bool IsFadedOut { get; private set; }

        internal static bool IsFadingIn { get; private set; }

        internal static float OriginalSoundVolume;

        private static readonly List<AudioEngine> AllAudioEngines;

        private readonly List<AudioPlayer> _allSounds;

        private readonly List<AudioPlayer> _soundsToRemove;

        static AudioEngine()
        {
            AllAudioEngines = new List<AudioEngine>();
            SoundEngine = new ISoundEngine();
        }

        public AudioEngine()
        {
            _allSounds = new List<AudioPlayer>();

            _soundsToRemove = new List<AudioPlayer>();

            AllAudioEngines.Add(this);
        }

        internal static void TickAll()
        {
            if (SoundEngine == null)
                return;

            // Adjust spectator position for proper stereo calculation

            // Check what camera is in use now -> gameplay (player pespective) or custom camera (like some free camera)
            GTA.Math.Vector3 cameraPos = CameraUtils.IsCameraValid(World.RenderingCamera) ? World.RenderingCamera.Position : GameplayCamera.Position;
            GTA.Math.Vector3 cameraDir = CameraUtils.IsCameraValid(World.RenderingCamera) ? World.RenderingCamera.Direction : GameplayCamera.Direction;

            // GameplayCamera gives incorrect position values when player is in car and using first person mode
            // so we use player head as sound listener
            if (CameraUtils.IsPlayerUseFirstPerson() && !CameraUtils.IsCameraValid(World.RenderingCamera))
            {
                cameraPos = Main.PlayerPed.Bones[Bone.SkelHead].Position;
                cameraDir = Main.PlayerPed.Bones[Bone.SkelHead].ForwardVector;
            }
            SoundEngine.SetListenerPosition(MathUtils.Vector3ToVector3D(cameraPos), MathUtils.Vector3ToVector3D(cameraDir));

            AllAudioEngines.ForEach(x => x.Tick());
        }

        internal static void DisposeAll()
        {
            AllAudioEngines?.ForEach(x => x.Dispose());
            SoundEngine.Dispose();
        }

        internal static void PauseAll(bool pause)
        {
            SoundEngine.SetAllSoundsPaused(pause);
        }

        internal static void MuteAll(bool mute)
        {
            if (mute == IsMuted)
                return;

            if (mute && !(IsFadingOut || IsFadedOut || IsFadingIn))
                OriginalSoundVolume = SoundEngine.SoundVolume;

            SoundEngine.SoundVolume = mute ? 0 : OriginalSoundVolume;
            IsMuted = mute;
        }

        internal static void FadeOutAll()
        {
            if (IsFadedOut)
                return;

            if (!IsFadingOut)
                OriginalSoundVolume = SoundEngine.SoundVolume;

            IsFadingOut = true;

            SoundEngine.SoundVolume =
                MathUtils.Lerp(SoundEngine.SoundVolume, 0f, Game.LastFrameTime);

            if (!(SoundEngine.SoundVolume <= 0.05f))
                return;

            SoundEngine.SoundVolume = 0;

            IsFadedOut = true;
            IsFadingOut = false;
        }

        internal static void FadeInAll()
        {
            if (IsMuted)
                return;

            IsFadingIn = true;
            IsFadingOut = false;
            IsFadedOut = false;

            SoundEngine.SoundVolume =
                MathUtils.Lerp(SoundEngine.SoundVolume, OriginalSoundVolume, Game.LastFrameTime);

            if (!(SoundEngine.SoundVolume >= OriginalSoundVolume))
                return;

            SoundEngine.SoundVolume = OriginalSoundVolume;
            IsFadingIn = false;
        }

        internal void Tick()
        {
            _allSounds.ForEach(x =>
            {
                if (x.Disposed)
                    _soundsToRemove.Add(x);
                else
                    x.Tick();
            });

            if (_soundsToRemove.Count > 0)
            {
                _soundsToRemove.ForEach(x => _allSounds.Remove(x));
                _soundsToRemove.Clear();
            }

            if ((GTA.UI.Screen.IsFadingOut || GTA.UI.Screen.IsFadedOut) && EnableFadeSystem)
            {
                FadeOutAll();
            }
            else if (GTA.UI.Screen.IsFadingIn && EnableFadeSystem)
            {
                FadeInAll();
            }
        }

        public void Dispose()
        {
            _allSounds?.ForEach(x => x.Dispose());
            _allSounds?.Clear();
        }

        /// <summary>
        /// Creates object of AudioPlayer that can be played on <see cref="DefaultSourceEntity"/>.
        /// </summary>
        /// <param name="name">Path to sound file.</param>
        /// <param name="preset"><see cref="AudioPreset"/> that defines different sound properties.</param>
        /// <returns>Object of <see cref="AudioPlayer"/> object.</returns>
        public AudioPlayer Create(string name, AudioPreset preset)
        {
            AudioPlayer audio = new AudioPlayer(this, name, preset);

            _allSounds.Add(audio);

            return audio;
        }

        /// <summary>
        /// Creates object of streamed AudioPlayer that can be played on <see cref="DefaultSourceEntity"/>.
        /// </summary>
        /// <param name="name">Name of audio.</param>
        /// <param name="res"><see cref="Stream"/> that contains audio.</param>
        /// <param name="preset"><see cref="AudioPreset"/> that defines different sound properties.</param>
        /// <returns>Object of <see cref="AudioPlayer"/>.</returns>
        public AudioPlayer Create(string name, Stream res, AudioPreset preset)
        {
            AudioPlayer audio = new AudioPlayer(this, name, res, preset);

            _allSounds.Add(audio);

            return audio;
        }
    }
}
