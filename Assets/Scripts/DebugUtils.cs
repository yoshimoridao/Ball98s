using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUtils
{
    private static bool isDebugEnable = false;
    private static Color curTrackColor = Color.red;
    private static float redDropPerStep = 1;

    public static bool IsDebugEnable()
    {
        return isDebugEnable;
    }

    public static Color GetColorTrack()
    {
        curTrackColor.r -= 1 / 255.0f;
        if (curTrackColor.r <= 0)
        {
            curTrackColor.r = 0;
        }

        return curTrackColor;
    }

    public static void ResetColorTrack()
    {
        curTrackColor = Color.red;
    }
}
