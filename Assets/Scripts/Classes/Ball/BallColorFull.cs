using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallColorFull : Ball
{
    public float dtChangeSpriteColor = 0.1f;

    private float curDt = 0;
    private int spriteId = 0;

    // ========================================================== UNITY FUNC ==========================================================
    public int GetSpriteId()
    {
        return spriteId;
    }

    public void Start()
    {
        base.Start();

        // random color
        spriteId = Random.Range((int)Ball.Type.BLUE, (int)Ball.Type.RED + 1);
        srBall.sprite = GetSprite((Ball.Type)spriteId);
        // rd time
        curDt = Random.Range(0, dtChangeSpriteColor);
    }

    public void Update()
    {
        base.Update();

        // update change sprite
        curDt += Time.deltaTime;
        if (curDt >= dtChangeSpriteColor)
        {
            curDt = 0;
            spriteId++;
            if (spriteId >= (int)Ball.Type.RED)
                spriteId = (int)Ball.Type.BLUE;
            srBall.sprite = GetSprite((Ball.Type)spriteId);
        }
    }

    public override void Init(int _tileId, Type _type, State _state = State.INVISIBLE)
    {
        base.Init(_tileId, _type, _state);
    }

    public override void SetActiveState(State _state, bool _isActive)
    {
        //if (_state == State.INVISIBLE && _isActive)
        //{
        //    base.SetActiveState(_state, _isActive);
        //    Destroy(this);
        //}

        base.SetActiveState(_state, _isActive);
    }
}
