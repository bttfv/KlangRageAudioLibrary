using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.UI;
using IrrKlang;
using KlangRageAudioLibrary.Utility;

namespace KlangRageAudioLibrary
{
    public partial class AudioPlayer
    {
        private float _originalVolume;

        private readonly List<ISound> _instancesToRemove;
        private readonly ISoundSource _soundSource;
        private readonly ISoundEngine _soundEngine;
        internal bool Disposed;

        private AudioPlayer()
        {
            SoundInstances = new List<ISound>();
        
            _instancesToRemove = new List<ISound>();
        }

        internal AudioPlayer(AudioEngine audioEngine, string name, Stream res, AudioPreset preset) : this()
        {
            _soundEngine = AudioEngine.SoundEngine;

            FilePath = null;

            _soundSource = _soundEngine.AddSoundSourceFromIOStream(res, name);

            Add(preset, audioEngine.DefaultSourceEntity);
        }

        internal AudioPlayer(AudioEngine audioEngine, string name, AudioPreset preset) : this()
        {
            _soundEngine = AudioEngine.SoundEngine;

            var audioInfo = new FileInfo($".\\scripts\\{audioEngine.BaseSoundFolder}/{name}");

            if (audioInfo.Exists == false)
            {
                Screen.ShowSubtitle($"File: {audioInfo.FullName} doesn't exists!");
                return;
            }

            FilePath = audioInfo.FullName;

            _soundEngine.AddSoundSourceFromFile(FilePath);

            Add(preset, audioEngine.DefaultSourceEntity);
        }

        public void Play(bool stopPrevious = false)
        {
            if (SourceEntity == null)
                return;

            if (stopPrevious && SoundInstances.Count > 0)
            {
                SoundInstances.ForEach(x => x.Stop());
            }

            ISound iSound;

            if (Flags.HasFlag(AudioFlags.No3D))
            {
                if (_soundSource == default)
                    iSound = _soundEngine.Play2D(FilePath, Flags.HasFlag(AudioFlags.Loop), true, StreamMode.AutoDetect);
                else
                    iSound = _soundEngine.Play2D(_soundSource, Flags.HasFlag(AudioFlags.Loop), true, false);
            }
            else
            {
                if (_soundSource == default)
                    iSound = _soundEngine.Play3D(FilePath,
                        MathUtils.Vector3ToVector3D(Vector3.Zero), Flags.HasFlag(AudioFlags.Loop), true, StreamMode.AutoDetect);
                else
                    iSound = _soundEngine.Play3D(_soundSource, 0, 0, 0, Flags.HasFlag(AudioFlags.Loop), true, false);
            }
           
            if (iSound == null)
            {
                throw new Exception($"KRAL Engine init failed. File Path: {FilePath}");
            }

            SoundInstances.Add(iSound);
            Last = iSound;

            if (StartFadeIn)
            {
                Last.Volume = 0;

                IsDoingFadeIn = true;
            }

            Last.Paused = false;
        }

        internal void Process()
        {
            if (SourceEntity == null)
                return;

            SoundInstances.ForEach(x =>
            {
                if (x.Finished)
                    _instancesToRemove.Add(x);
            });

            if (_instancesToRemove.Count > 0)
            {
                _instancesToRemove.ForEach(x => SoundInstances.Remove(x));
                _instancesToRemove.Clear();
            }

            InstancesNumber = SoundInstances.Count();
            
            IsAnyInstancePlaying = InstancesNumber > 0;

            if (SoundInstances.All(x => x.Paused is true))
                return;

            SoundInstances.ForEach(x => 
            {
                x.MinDistance = MinimumDistance;
                x.Velocity = MathUtils.Vector3ToVector3D(Velocity);

                // For some reason irrklang doesn't work with speed less than 0.1
                if (!x.Paused)
                    x.PlaybackSpeed = Game.TimeScale < 0.1f ? 0.11f : Game.TimeScale;
            });

            // Update volume in case if it was changed
            _originalVolume = Volume;

            // Do Fade Out/In
            if (IsDoingFadeIn)
            {
                ProcessFadeIn();
            } 
            else if (IsDoingFadeOut)
            {
                ProcessFadeOut();
            }
            else
            {
                // If fade not processing we can update volume
                //  and process interior / exterior sounds

                SoundInstances.ForEach(x => x.Volume = Volume);

                ProcessInteriorSound();
                ProcessExteriorSound();
            }

            // Adjust sound position relatively to world
            var boneIndex = SourceEntity.Bones[SourceBone].Index;

            var soundPos = MathUtils.Vector3ToVector3D(boneIndex == -1 ? SourceEntity.Position : SourceEntity.Bones[boneIndex].Position);

            SoundInstances.ForEach(x => x.Position = soundPos);
        }

        public void Stop(bool instant = false)
        {
            if (SoundInstances.All(x => x.Finished) || 
                SoundInstances.All(x => x.Paused is true))
                return;

            IsDoingFadeIn = false;

            if (StopFadeOut && !instant)
            {
                SoundInstances?.Where(x => x != Last).ToList().ForEach(x => x.Stop());

                IsDoingFadeOut = true;
            }
            else
            {
                SoundInstances?.ForEach(x => x.Stop());
            }
        }

        public void Dispose()
        {
            SoundInstances?.ForEach(x => { x.Stop(); x.Dispose(); });
            SoundInstances?.Clear();
            Disposed = true;
        }
        
        private void Add(AudioPreset preset, Entity entity)
        {
            Flags = preset.Flags;

            Volume = preset.Volume;
            MinimumDistance = preset.MinimumDistance;

            IsInteriorSound = Flags.HasFlag(AudioFlags.InteriorSound);
            IsExteriorSound = Flags.HasFlag(AudioFlags.ExteriorSound);

            StartFadeIn = Flags.HasFlag(AudioFlags.FadeIn);
            StopFadeOut = Flags.HasFlag(AudioFlags.FadeOut);

            SourceEntity = entity;
        }
    }
}