using System.Runtime.InteropServices;
using System.Threading;

namespace KlangRageAudioLibrary.Utility
{
    internal class ExternalThread
    {
        public bool PauseAll { get; set; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetForegroundWindow();

        private Thread _backgroundThread;

        public void Start()
        {
            _backgroundThread = new Thread(Process)
            {
                IsBackground = true
            };

            _backgroundThread?.Start();
        }

        public void Stop()
        {
            _backgroundThread?.Abort();
        }

        private static System.Diagnostics.Process GetActiveAppProcess()
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out var activeProcessId);

            return System.Diagnostics.Process.GetProcessById(activeProcessId);
        }

        private void Process()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (!Main.GamePaused)
                {
                    Main.GamePaused = true;

                    if (PauseAll)
                    {
                        PauseAll = false;
                        AudioEngine.PauseAll(PauseAll);
                    }
                }
                else
                {
                    if (!PauseAll)
                    {
                        PauseAll = true;
                        AudioEngine.PauseAll(PauseAll);
                    }
                }

                AudioEngine.MuteAll(GetActiveAppProcess().ProcessName != "GTA5");
            }
        }
    }
}