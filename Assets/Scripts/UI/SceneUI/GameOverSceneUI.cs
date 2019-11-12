using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverSceneUI : MonoBehaviour
{
    public Text txtHighScore;
	void Start ()
    {
        txtHighScore.text = PlayerInfo.Instance.GetHighScore().ToString();
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
