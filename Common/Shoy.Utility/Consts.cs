namespace Shoy.Utility
{
    internal static class Consts
    {
        internal const string WinRarPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";
        internal const string CompressCommand = " a {0} {1} -r";
        internal const string UnzipCommand = " x {0} {1} -y";
    }
}
