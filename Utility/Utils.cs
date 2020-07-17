using GTA;
using GTA.Math;
using GTA.Native;
using IrrKlang;

namespace KlangRageAudioLibrary.Utility
{
    internal class MathUtils
    {
        public static Vector3D Vector3ToVector3D(Vector3 vector)
        {
            return new Vector3D(vector.X, vector.Z, vector.Y);
        }
        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }
        public static int Lerp(int firstFloat, int secondFloat, float by)
        {
            return (int)(firstFloat + (secondFloat - (float)firstFloat) * by);
        }
    }
    internal class CameraUtils 
    {
        public static bool IsCameraValid(Camera cam)
        {
            return cam != null && cam.Position != Vector3.Zero;
        }
        public static bool IsPlayerUseFirstPerson()
        {
            return Function.Call<int>(Hash.GET_FOLLOW_PED_CAM_VIEW_MODE) == 4 && !GameplayCamera.IsLookingBehind &&
                   !Function.Call<bool>((Hash)0xF5F1E89A970B7796);
        }
    }
    internal class VehicleUtils
    {
        public static bool IsAnyOfFrontDoorsOpen(Vehicle vehicle)
        {
            // TODO: Make it work with any car, 
            //  currently works only with 2 front doors

            var doorOpen = false;
            foreach (var door in vehicle.Doors)
            {
                if (door.IsOpen)
                    doorOpen = true;
            }

            return doorOpen;
        }
    }
}