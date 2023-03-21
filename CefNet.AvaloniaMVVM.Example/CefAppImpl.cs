using System;
using System.Runtime.InteropServices;
using CefNet;

namespace CefTest;

public class CefAppImpl : CefNetApplication
{
    protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
    {
        base.OnBeforeCommandLineProcessing(processType, commandLine);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            commandLine.AppendSwitch("no-zygote");
            commandLine.AppendSwitch("no-sandbox");
        }
    }
    public Action<long> ScheduleMessagePumpWorkCallback { get; set; }

    protected override void OnScheduleMessagePumpWork(long delayMs)
    {
        ScheduleMessagePumpWorkCallback(delayMs);
    }
}