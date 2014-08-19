using System;

namespace Shoy.Laboratory.Transmiter
{
    public class CommandReceivedEventArgs : EventArgs
    {
        public string CommandLine { get; set; }

        public CommandReceivedEventArgs(string commandLine)
        {
            CommandLine = commandLine;
        }
    }
}
