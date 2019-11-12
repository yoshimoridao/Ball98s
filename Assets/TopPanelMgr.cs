using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelMgr : Singleton<TopPanelMgr>
{
    public RectTransform rtBallPanel;
    public Image ball1;
    public Image ball2;
    public Image ball3;
    public Text txtTimer;
    private float timer;

    // ========================================================== GET/ SET ==========================================================
    public void SetPositionBallPanel(Vector3 _val)
    {
        rtBallPanel.position = _val;
    }

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
        timer -= Time.deltaTime;
        int minute = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        txtTimer.text = (minute < 10 ? "0" : "") + minute.ToString() + ":" + (seconds < 10 ? "0" : "") + seconds.ToString();
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void Init()
    {
        timer = GameMgr.Instance.timeForRound;
    }

    public void RefreshBallPanel(Sprite _sprite1, Sprite _sprite2, Sprite _sprite3)
    {
        ball1.sprite = _sprite1;
        ball2.sprite = _sprite2;
        ball3.sprite = _sprite3;
    }
}
