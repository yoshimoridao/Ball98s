using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelMgr : Singleton<TopPanelMgr>
{
    public RectTransform rtBallPanel;
    public RectTransform rtScoreZone;
    public RectTransform rtHighScore;
    public Image ball1;
    public Image ball2;
    public Image ball3;
    public Text txtTimer;
    public Text txtScore;
    public Text txtHighScore;

    private float timer;

    // ========================================================== GET/ SET ==========================================================

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void Update ()
    {
        if (GameMgr.Instance.GetGameState() != GameMgr.GameState.PLAYING)
            return;

        // count time
        timer -= Time.deltaTime;
        int minute = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        txtTimer.text = (minute < 10 ? "0" : "") + minute.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();

        // time end
        if (minute == 0 && seconds == 0)
            GameMgr.Instance.OnGameOver();
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void Init()
    {
        timer = GameMgr.Instance.timeForRound;
    }

    public void SetActive(bool _isActive)
    {
        gameObject.SetActive(_isActive);
    }

    public void SetPositionBallPanel(Vector3 _val)
    {
        rtBallPanel.position = _val;
    }
    public void SetPositionScoreZone(Vector3 _val)
    {
        rtScoreZone.position = _val;
    }
    public void SetPositionHighScoreZone(Vector3 _val)
    {
        rtHighScore.position = _val;
    }

    public void RefreshBallPanel(Sprite _sprite1, Sprite _sprite2, Sprite _sprite3)
    {
        ball1.sprite = _sprite1;
        ball2.sprite = _sprite2;
        ball3.sprite = _sprite3;
    }

    public void RefreshScorePanel(int _score)
    {
        txtScore.text = _score.ToString();
    }
    public void RefreshHighscorePanel(int _score)
    {
        txtHighScore.text = _score.ToString();
    }
}
