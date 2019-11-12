using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartSceneUI : MonoBehaviour
{
	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (InputMgr.Instance.GetMouseState() == InputMgr.MouseState.DOWN)
        {
            GameMgr.Instance.OnStartGame();
            SceneUIMgr.Instance.HideScene(SceneUIMgr.Scene.GAMESTART);
        }
    }
}
