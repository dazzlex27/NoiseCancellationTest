using System.Runtime.InteropServices;

namespace SoundLib
{
    public static class BeepSampler
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Beep(uint dwFreq, uint dwDuration);

        public static void MakeBeep(uint frequency, uint length)
        {
            Beep(frequency, length);
        }
    }
}