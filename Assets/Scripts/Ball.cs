using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ball : MonoBehaviour
{
    public string ballImgPath = "Image/{0}_ball";
    public enum Type { BLUE, BROWN, GREEN, LIGHT_BLUE, PINK, RED, GHOST };
    public enum State { INVISIBLE, IDLE, GROWSMALL, GROWBIG, BOUNCE, MOVING };
    public enum Size { SMALL, BIG };

    public Transform transBall;
    public SpriteRenderer srBall;
    public Animator animator;
    public float mvmSpeed = 5;

    [SerializeField]
    private int tileId;
    private Type type;
    [SerializeField]
    private State state = State.IDLE;
    private Size size = Size.SMALL;

    [SerializeField]
    private Path movingPath = null;
    [SerializeField]
    private int curStep = -1;
    [SerializeField]
    private Vector2 mvmVelocity = Vector2.zero;
    [SerializeField]
    private Vector2 oldMvmVelocity = Vector2.zero;

    // ========================================================== GETTER/ SETTER ==========================================================
    public SpriteRenderer GetSpriteRenderer() { return srBall; }

    public void SetTileId(int _tileId) { tileId = _tileId; }
    public int GetTileId() { return tileId; }

    public void SetLocalScale(Vector3 _scale) { transBall.localScale = _scale; }
    public Vector3 GetLocalScale() { return transBall.localScale; }

    public void SetPosition(Vector3 _val) { transform.position = _val; }
    public void SetPositionXY(Vector2 _val)
    {
        transform.position = new Vector3(_val.x, _val.y, transform.position.z);
    }
    public Vector3 GetPosition() { return transform.position; }

    public void SetType(Type _val)
    {
        type = _val;
        ChangeSprite();
    }
    public Type GetType() { return type; }

    public Size GetSize() { return size; }

    public void SetState(State _state) { state = _state; }
    public State GetState() { return state; }

    public void SetActiveState(State _state, bool _isActive)
    {
        if (_isActive)
            SetState(_state);

        if (state != State.INVISIBLE && !gameObject.active)
            gameObject.SetActive(true);

        switch (state)
        {
            case State.INVISIBLE:
                gameObject.SetActive(false);
                break;
            case State.IDLE:
            case State.GROWSMALL:
            case State.GROWBIG:
            case State.BOUNCE:
                size = state == State.GROWBIG ? Size.BIG : Size.SMALL;

                if (_isActive)
                    PlayAnimation(state);
                else
                    SetState(State.IDLE);
                break;
            case State.MOVING:
                curStep = 0;
                mvmVelocity = oldMvmVelocity = Vector2.zero;
                movingPath = _isActive ? movingPath : null;
                // set z depth
                Vector3 pos = GetPosition();
                pos.z = ZDepth.GetDepth(_isActive ? ZDepth.Layer.BALLONTOP : ZDepth.Layer.BALL);
                SetPosition(pos);

                // play anim idle
                if (_isActive)
                    PlayAnimation(State.IDLE);
                else
                    SetActiveState(State.IDLE, true);
                break;
        }
    }
    // ========================================================== UNITY FUNC ==========================================================
    private void Start()
    {
        animator = GetComponent<Animator>();
        movingPath = null;
        // change color following type
        ChangeSprite();
    }

    private void Update()
    {
        UpdateState();
    }

    public void Init(int _tileId, Type _type, State _state = State.INVISIBLE)
    {
        tileId = _tileId;
        SetActiveState(_state, true);
        SetType(_type);
    }

    private void UpdateState()
    {
        switch (state)
        {
            case State.MOVING:
                UpdateMovingBall();
                break;
        }
    }

    private void UpdateMovingBall()
    {
        // update velocity
        oldMvmVelocity = mvmVelocity;
        int boardDimension = BoardMgr.Instance.boardDimension;
        List<GameObject> lTiles = BoardMgr.Instance.GetListTiles();
        int curTileId = movingPath.GetNode(curStep);

        // if next step is available
        if (curStep + 1 < movingPath.GetNodesCount())
        {
            // set velocity
            int nextTileId = movingPath.GetNode(curStep + 1);
            // moving horizontal (move left || right)
            if ((int)(curTileId / boardDimension) == (int)(nextTileId / boardDimension))
            {
                mvmVelocity.x = (nextTileId > curTileId ? 1 : -1) * mvmSpeed;
                mvmVelocity.y = 0;
            }
            // moving vertical (move up || down)
            else if (curTileId % boardDimension == nextTileId % boardDimension)
            {
                mvmVelocity.x = 0;
                mvmVelocity.y = (nextTileId > curTileId ? -1 : 1) * mvmSpeed;
            }

            GameObject curTile = lTiles[curTileId];
            // snap to instersection pos if change velocity
            if ((mvmVelocity.x != 0 && oldMvmVelocity.y != 0) || (mvmVelocity.y != 0 && oldMvmVelocity.x != 0))
                SetPositionXY(new Vector2(curTile.transform.position.x, curTile.transform.position.y));

            // update step
            GameObject nextTile = lTiles[nextTileId];
            if (IsReachNode(nextTile))
                curStep++;
        }

        // update position
        if (mvmVelocity != Vector2.zero)
        {
            Vector2 pos = GetPosition();
            pos.x += mvmVelocity.x * Time.deltaTime;
            pos.y += mvmVelocity.y * Time.deltaTime;
            SetPositionXY(pos);
        }

        // reach end pos
        if (curStep + 1 >= movingPath.GetNodesCount())
        {
            // snap position
            GameObject curTile = lTiles[movingPath.GetNode(curStep)];
            if (IsReachNode(curTile))
                SetPositionXY(new Vector2(curTile.transform.position.x, curTile.transform.position.y));

            // disable MOVING state
            SetActiveState(State.MOVING, false);

            // callback End Turn event
            BallMgr.Instance.OnEndOneTurn();
        }
    }

    // ========================================================== PUBLIC FUNC ==========================================================
    public bool FindPath(List<int> _lAvailSlots, int _desTileId)
    {
        movingPath = Utils.FindShortestPath(_lAvailSlots, BoardMgr.Instance.boardDimension, tileId, _desTileId);
        if (movingPath != null)
            SetActiveState(State.MOVING, true);

        // Debug
        if (DebugUtils.IsDebugEnable())
            Debug_FindPathFunc();
        // end Debug

        return movingPath != null;
    }

    public void OnOverlayed()
    {
        // snap to tile' position
        GameObject tile = BoardMgr.Instance.GetTile(GetTileId());
        if (tile)
            SetPositionXY(tile.transform.position);

        // invisible 
        SetActiveState(Ball.State.INVISIBLE, true);
    }

    // ========================================================== PRIVATE FUNC ==========================================================
    private void ChangeSprite()
    {
        if (!srBall)
            srBall = GetComponent<SpriteRenderer>();

        Sprite sprite = Resources.Load<Sprite>(ballImgPath.Replace("{0}", type.ToString().ToLower()));
        if (sprite)
        {
            srBall.sprite = sprite;
        }
        else
        {
            Debug.LogError("Missing sprite of " + type.ToString() + " ball");
        }
    }

    private void PlayAnimation(State _state)
    {
        animator.Play(_state.ToString().ToLower(), -1, 0);
    }

    private bool IsReachNode(GameObject _node)
    {
        float ballSize = BallMgr.Instance.GetBallSize();
        if ((mvmVelocity.x > 0 && Mathf.Abs(transform.position.x - _node.transform.position.x) <= ballSize / 4.0f) ||
             (mvmVelocity.x < 0 && Mathf.Abs(transform.position.x - _node.transform.position.x) <= ballSize / 4.0f) ||
             (mvmVelocity.y > 0 && Mathf.Abs(transform.position.y - _node.transform.position.y) <= ballSize / 4.0f) ||
             (mvmVelocity.y < 0 && Mathf.Abs(transform.position.y - _node.transform.position.y) <= ballSize / 4.0f))
            return true;
        return false;
    }

    // ========================================================== DEBUG FUNC ==========================================================
    private void Debug_FindPathFunc()
    {
        Debug.Log("=>>>>>>>>>>>>>>> Founding Path = ");
        if (movingPath != null)
        {
            movingPath.ShowDebug();

            DebugUtils.ResetColorTrack();
            movingPath.ShowDebug();
        }
    }
}
