using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneUI : MonoBehaviour
{
	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (InputMgr.Instance.GetMouseState() == InputMgr.MouseState.DOWN)
        {
            GameMgr.Instance.OnRestartGame();
            SceneUIMgr.Instance.HideScene(SceneUIMgr.Scene.GAMEOVER);
        }
    }
}
