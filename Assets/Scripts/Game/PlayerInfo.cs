﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : Singleton<PlayerInfo>
{
    private int highScore = 0;
    private int score = 0;

    // ========================================================== GET/ SET ==========================================================
    public int GetHighScore() { return highScore; }
    public int GetScore() { return score; ; }
    public void SetScore(int _pnt)
    {
        score = _pnt;
        TopPanelMgr.Instance.RefreshScorePanel(score);
    }
    public void AddScore(int _plusPnt)
    {
        score += _plusPnt;
        TopPanelMgr.Instance.RefreshScorePanel(score);
    }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
        highScore = 0;
        score = 0;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================================== PUBLIC FUNC ==========================================================
    public void Init()
    {
        TopPanelMgr.Instance.RefreshScorePanel(score);
        TopPanelMgr.Instance.RefreshHighscorePanel(highScore);
    }

    public void UpdateHighScore()
    {
        highScore = score;
        TopPanelMgr.Instance.RefreshHighscorePanel(highScore);
    }
}
