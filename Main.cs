using System;
using GTA;
using IrrKlang;
using KlangRageAudioLibrary.Utility;

namespace KlangRageAudioLibrary
{
    public class Main : Script
    {
        public static Ped PlayerPed => Game.Player.Character;
        public static Vehicle PlayerVehicle => PlayerPed.CurrentVehicle;

        private static ExternalThread externalThread;

        public static bool GamePaused = false;
                
        public Main()
        {
            Tick += OnTick;
            Aborted += OnAbort;

            externalThread = new ExternalThread();

            externalThread.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            GamePaused = false;

            AudioEngine.ProcessAll();
        }
        private static void OnAbort(object sender, EventArgs e)
        {
            externalThread?.Stop();

            AudioEngine.DisposeAll();
        }
    }
}