using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    public enum GameState { GAMESTART, PLAYING, PAUSE, GAMEOVER };

    private GameState gameState = GameState.GAMESTART;

    // ========================================================== GET/ SET ==========================================================
    public GameState GetGameState() { return gameState; }
    public void SetGameState(GameState _gameState) { gameState = _gameState; }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        PlayerInfo.Instance.Init();
        BoardMgr.Instance.Init();
        BallMgr.Instance.Init();
        TopPanelMgr.Instance.Init();

        // start game
        SceneUIMgr.Instance.ShowScene(SceneUIMgr.Scene.GAMESTART);
    }
	
	void Update ()
    {
		
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void OnStartGame()
    {
        SetGameState(GameState.PLAYING);
    }

    public void OnRestartGame()
    {
        SetGameState(GameState.PLAYING);

        PlayerInfo.Instance.Init();
        TopPanelMgr.Instance.Init();
        BallMgr.Instance.OnRestart();
    }

    public void OnGameOver()
    {
        PlayerInfo.Instance.UpdateHighScore();

        SetGameState(GameState.GAMEOVER);
        SceneUIMgr.Instance.ShowScene(SceneUIMgr.Scene.GAMEOVER);
    }

    public void PauseGame()
    {
        SetGameState(GameState.PAUSE);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        SetGameState(GameState.PLAYING);
        Time.timeScale = 1;
    }
}
