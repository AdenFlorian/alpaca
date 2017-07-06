using System;
using UnityEngine;

public static class MyLogger
{
    public static void LogInfo(string message)
    {
        if (LogSettings.I.Info)
        {
            Debug.Log("Info: " + message);
        }
    }

    internal static void LogTrace(string message)
    {
        if (LogSettings.I.Trace && message.Contains(LogSettings.I.TraceFilter))
        {
            Debug.Log("Trace: " + message);
        }
    }

    internal static void LogError(string message)
    {
        if (LogSettings.I.Error)
        {
            Debug.LogError(message);
        }
    }

    internal static void LogWarning(string message)
    {
        if (LogSettings.I.Warning)
        {
            Debug.LogWarning(message);
        }
    }
}