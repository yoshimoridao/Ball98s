using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMgr : Singleton<BallMgr>
{
    // prefab
    public GameObject prefBall;

    // properties
    public int ballPerTurn = 3;
    public float ballScale = 0.8f;  // ball's scale (ratio vs tile's size)

    private List<Ball> lBalls = new List<Ball>();           // list contains all of balls entity on board
    [SerializeField]
    private List<int> lEmptyTiles = new List<int>();        // list contains all of id of empty slots on board
    [SerializeField]
    private List<int> lSmallBalls = new List<int>();        // list contains balls scal full size next turn
    private float ballSize;

    private int touchedBallId = -1;

    [SerializeField]
    public Path shortestPath;

    // ========================================================== GET/ SET ==========================================================
    public float GetBallSize() { return ballSize; }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
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
        // get list of tiles on board
        List<GameObject> lTiles = BoardMgr.Instance.GetListTiles();

        // gen ball objects
        for (int i = 0; i < lTiles.Count; i++)
        {
            GameObject tile = lTiles[i];
            GameObject genBall = GameObject.Instantiate(prefBall, transform);
            Ball compBall = genBall.GetComponent<Ball>();
            SpriteRenderer srBall = compBall.GetSpriteRenderer();

            genBall.name = "ball_" + i.ToString();
            // set position
            Vector3 posBall = compBall.GetPosition();
            compBall.SetPosition(new Vector3(tile.transform.position.x, tile.transform.position.y, posBall.z));
            // set scale
            if (ballSize == 0)
            {
                SpriteRenderer tileSr = tile.GetComponent<SpriteRenderer>();
                ballSize = tileSr.bounds.size.x * ballScale;
                ballSize = Mathf.Min(ballSize / srBall.bounds.size.x, ballSize / srBall.bounds.size.y);
            }
            compBall.SetLocalScale(new Vector3(ballSize, ballSize, 1.0f));

            // init Ball component
            compBall.Init(i, Ball.Type.GHOST, Ball.State.INVISIBLE);

            // store ball
            lBalls.Add(compBall);
            lEmptyTiles.Add(i); // default all slots available to spawn
        }

        // default spawn big size
        SpawnRandomBalls(Ball.State.GROWBIG);
    }

    public void OnTouchBall(int _tileId)
    {
        Ball touchBall = lBalls[_tileId];
        // First touch (select ball)
        if (touchedBallId == -1 && _tileId < lBalls.Count)
        {
            if (touchBall.GetState() == Ball.State.IDLE)
            {
                touchedBallId = _tileId;    // store touched ball's id
                touchBall.SetActiveState(Ball.State.BOUNCE, true);
            }
        }
        // Second touch (select path)
        else if (touchedBallId != -1)
        {
            Ball touchedBall = lBalls[touchedBallId];
            // touch same ball || another activated ball
            if (_tileId == touchedBallId || touchBall.GetState() != Ball.State.INVISIBLE)
            {
                touchedBall.SetActiveState(Ball.State.IDLE, true);
            }
            // touch available slot
            else if (touchBall.GetState() == Ball.State.INVISIBLE)
            {
                // if ball has moving path (-> swap index of 2 ball in storage)
                if (touchedBall.FindPath(lEmptyTiles, _tileId))
                {
                    OnMovingBall(touchedBall, _tileId);
                }
                // if ball's path stucked
                else
                {
                    touchedBall.SetActiveState(Ball.State.IDLE, true);
                }
            }
            touchedBallId = -1;
        }
    }

    public void OnEndOneTurn()
    {
        GrowAllSmallBalls();    // grow all small balls
        SpawnRandomBalls(Ball.State.GROWSMALL);     // spawn random balls
    }

    // ========================================================== PRIVATE FUNC ==========================================================
    private void OnMovingBall(Ball _moveBall, int _desTileId)
    {
        Ball desBall = lBalls[_desTileId];
        // swap Tile ID of 2 balls
        int tmpId = _moveBall.GetTileId();
        _moveBall.SetTileId(desBall.GetTileId());
        desBall.SetTileId(tmpId);

        // also swap in storage
        lBalls[desBall.GetTileId()] = desBall;
        lBalls[_moveBall.GetTileId()] = _moveBall;

        // change position of destination ball
        desBall.SetPosition(_moveBall.GetPosition());

        // update empty slot
        UpdateListEmptyTiles(true, desBall.GetTileId());    // add new empty slot
        UpdateListEmptyTiles(false, _moveBall.GetTileId()); // remove empty slot
    }

    // === spawn ball ===
    private void SpawnRandomBalls(Ball.State _state)
    {
        List<GameObject> lTiles = BoardMgr.instance.GetListTiles();

        // gen balls
        List<int> prevIds = new List<int>();
        int turn = Mathf.Min(ballPerTurn, lEmptyTiles.Count);
        for (int i = 0; i < turn; i++)
        {
            int rdSpawnId = -1;
            // Random Position (not match prev pos)
            do
            {
                rdSpawnId = turn < ballPerTurn ? i : Random.Range(0, lEmptyTiles.Count);
            } while (prevIds.Contains(rdSpawnId));
            prevIds.Add(rdSpawnId);

            // Random Type
            Ball.Type rdType = (Ball.Type)Random.Range((int)Ball.Type.BLUE, (int)Ball.Type.GHOST + 1);

            // spawn
            SpawnBalls(rdSpawnId, rdType, _state);
        }

        // GAME OVER
        if (turn < ballPerTurn)
        {

        }
    }

    private void SpawnBalls(int _id, Ball.Type _type, Ball.State _state)
    {
        if (_id >= lBalls.Count)
            return;

        // active ball
        Ball ball = lBalls[_id];
        ball.SetType(_type);
        ball.SetActiveState(_state, true);

        // update list of small balls
        if (_state == Ball.State.GROWSMALL)
            UpdateListSmallBalls(true, _id);

        // update list empty tiles (remove this id)
        UpdateListEmptyTiles(false, _id);

        // DEBUG
        if (DebugUtils.IsDebugEnable())
            Debug.Log("SPAWN BALL = " + _id);
    }

    // === update list info ===
    private void UpdateListEmptyTiles(bool _isAddAction, int _tileId)
    {
        if (_isAddAction)
        {
            if (!lEmptyTiles.Contains(_tileId))
            {
                lEmptyTiles.Add(_tileId);
                // sort list 
                lEmptyTiles.Sort((a, b) => a - b);
            }
        }
        else
        {
            int findId = lEmptyTiles.FindIndex(x => x == _tileId);
            if (findId != -1)
                lEmptyTiles.RemoveAt(findId);
        }
    }

    private void UpdateListSmallBalls(bool _isAddAction, int _tileId)
    {
        if (_isAddAction)
        {
            if (!lSmallBalls.Contains(_tileId))
                lSmallBalls.Add(_tileId);
        }
        else
        {
            int findId = lSmallBalls.FindIndex(x => x == _tileId);
            if (findId != -1)
                lSmallBalls.RemoveAt(findId);
        }
    }

    // ==========
    private void GrowAllSmallBalls()
    {
        for (int i = 0; i < lSmallBalls.Count; i++)
        {
            int id = lSmallBalls[i];
            if (id < lBalls.Count)
                lBalls[id].SetActiveState(Ball.State.GROWBIG, true);
        }
        // clear all list
        lSmallBalls.Clear();
    }
}
