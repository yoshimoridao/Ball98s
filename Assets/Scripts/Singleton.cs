using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Type> : MonoBehaviour
{
    protected static Type instance;

    public static Type Instance
    {
        get
        {
            return instance;
        }
    }
}