using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static int timeForRound = 600;

    public static int boardDimension = 4;
    // scale ratio vs camera height
    public static float boardScale = 0.8f;
    public static float tileOffset = 0.1f;

    public static int ballsGetPoint = 3;
    public static int ballSpawnPerTurn = 3;
    public static float ballScale = 0.8f;  // ball's scale (ratio vs tile's size)

    public static string scenePrefabPath = "SceneUI/{0}";
    public static string ballImgPath = "Image/{0}_ball";

    // enable debug
    public static bool isDebugEnable = false;
}
