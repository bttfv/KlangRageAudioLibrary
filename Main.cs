using GTA;
using KlangRageAudioLibrary.Utility;
using System;

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
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1)
                    .AddDays(version.Build).AddSeconds(version.Revision * 2);

            System.IO.File.AppendAllText($"./ScriptHookVDotNet.log", $"KlangRageAudioLibrary - {version} ({buildDate})" + Environment.NewLine);

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