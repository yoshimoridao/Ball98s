using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr : Singleton<InputMgr>
{
    public enum MouseState { HOVER, HOLD, DOWN, UP };

    private MouseState mouseState = MouseState.HOVER;

    public MouseState GetMouseState()
    {
        return mouseState;
    }

    private void Awake()
    {
        instance = this;
    }
    void Start ()
    {
		
	}
	
	void Update ()
    {
		if (Input.GetMouseButtonDown(0))
        {
            mouseState = MouseState.DOWN;
        }
        else if (mouseState != MouseState.HOLD && Input.GetMouseButton(0))
        {
            mouseState = MouseState.HOLD;
        }
        else if ((mouseState == MouseState.HOLD || mouseState == MouseState.DOWN) && Input.GetMouseButtonUp(0))
        {
            mouseState = MouseState.UP;
        }
        else if (mouseState != MouseState.HOVER)
        {
            mouseState = MouseState.HOVER;
        }
	}

    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
