using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Singleton<PlayerInfo>
{
    private int highScore;
    private int score;

    // ========================================================== GET/ SET ==========================================================
    public int GetHighScore() { return highScore; }
    public void SetHighScore(int _pnt) { highScore = _pnt; }
    public int GetScore() { return score; ; }
    public void SetScore(int _pnt)
    {
        score = _pnt;
    }
    public void AddScore(int _plusPnt)
    {
        score += _plusPnt;
    }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
        Debug.Log(instance);
    }

    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void Init()
    {
        score = 0;
        highScore = 0;

        TopPanelMgr.Instance.RefreshScorePanel(score);
        TopPanelMgr.Instance.RefreshScorePanel(highScore);
    }
}
