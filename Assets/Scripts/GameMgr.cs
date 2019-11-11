using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public BoardMgr boardMgr;
    public BallMgr ballMgr;

	void Start ()
    {
        boardMgr.Init();
        ballMgr.Init();
	}
	
	void Update ()
    {
		
	}
}
