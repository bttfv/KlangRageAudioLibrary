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
            return Function.Call<bool>(Hash.IS_CINEMATIC_FIRST_PERSON_VEHICLE_INTERIOR_CAM_RENDERING) || Function.Call<bool>(Hash.IS_BONNET_CINEMATIC_CAM_RENDERING);
        }
    }
    internal class VehicleUtils
    {
        public static bool IsAnyPassengerDoorOpen(Vehicle vehicle)
        {
            foreach (VehicleDoor door in vehicle.Doors)
            {
                if (door.Index != VehicleDoorIndex.Hood && door.Index != VehicleDoorIndex.Trunk && door.IsOpen)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
