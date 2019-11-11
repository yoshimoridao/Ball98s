using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZDepth
{
    public enum Layer { TILEBG, TILE, BALL, BALLONTOP, COUNT };
    public static float GetDepth(Layer _layer)
    {
        return (int)Layer.COUNT - (int)_layer;
    }
}
