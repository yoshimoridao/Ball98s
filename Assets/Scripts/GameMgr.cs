using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    public int timeForRound = 600;

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
	}
	
	void Update ()
    {
		
	}
}
