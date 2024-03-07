using GTA;
using KlangRageAudioLibrary.Utility;

namespace KlangRageAudioLibrary
{
    public partial class AudioPlayer
    {
        private void ProcessFadeIn()
        {
            Last.Volume =
                MathUtils.Lerp(Last.Volume, _originalVolume, Game.LastFrameTime * FadeInMultiplier);

            if (!(Last.Volume >= _originalVolume - 0.05f))
                return;

            Last.Volume = _originalVolume;
            IsDoingFadeIn = false;
        }

        private void ProcessFadeOut()
        {
            Last.Volume =
                MathUtils.Lerp(Last.Volume, 0f, Game.LastFrameTime * FadeOutMultiplier);

            if (!(Last.Volume <= 0.05f))
                return;

            IsDoingFadeOut = false;

            Last.Stop();
        }

        private void ProcessInteriorSound()
        {
            if (SourceEntity is Vehicle is false)
                return;

            if (IsInteriorSound is false)
                return;

            // If player is in the source vehicle we use original sound volume
            if (Main.PlayerVehicle == SourceEntity)
            {
                SoundInstances.ForEach(x => x.Volume = _originalVolume);
                return;
            }

            // Define volume based on is car doors open / closed
            SoundInstances.ForEach(x => x.Volume =
                VehicleUtils.IsAnyPassengerDoorOpen((Vehicle)SourceEntity) ? _originalVolume : _originalVolume / 4);
        }

        private void ProcessExteriorSound()
        {
            if (SourceEntity is Vehicle is false)
                return;

            if (IsExteriorSound is false)
                return;

            // If player is in the source vehicle and use 3rd person mode we use original sound volume
            if (Main.PlayerVehicle == SourceEntity == false || CameraUtils.IsPlayerUseFirstPerson() == false)
            {
                SoundInstances.ForEach(x => x.Volume = _originalVolume);
                return;
            }

            // Define volume based on is car doors open / closed
            SoundInstances.ForEach(x => x.Volume =
                VehicleUtils.IsAnyPassengerDoorOpen((Vehicle)SourceEntity) ? _originalVolume : _originalVolume / 2);
        }
    }
}
