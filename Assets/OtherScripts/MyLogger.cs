using System;
using UnityEngine;

public static class MyLogger
{
    public static void LogInfo(string message)
    {
        Debug.Log("Info: " + message);
    }

    internal static void LogTrace(string v)
    {
        //Debug.Log("Trace: " + message);
    }
}