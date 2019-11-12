using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMgr : Singleton<BallMgr>
{
    // prefab
    public GameObject prefBall;

    // properties
    public int ballsGetPoint = 3;
    public int ballSpawnPerTurn = 3;
    public float ballScale = 0.8f;  // ball's scale (ratio vs tile's size)

    private List<Ball> lBalls = new List<Ball>();           // list contains all of balls entity on board
    [SerializeField]
    private List<int> lEmptyTiles = new List<int>();        // list contains all of id of empty slots on board
    [SerializeField]
    private List<int> lSmallBalls = new List<int>();        // list contains balls scal full size next turn
    private int smallBallOverlayed = -1;
    [SerializeField]
    private int movingBallId = -1;
    private float ballSize;
    private int touchedBallId = -1;
    private List<Ball.Type> lnextTypes = new List<Ball.Type>();

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
            compBall.SetPosition(new Vector3(tile.transform.position.x, tile.transform.position.y, ZDepth.GetDepth(ZDepth.Layer.BALL)));
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
           
            // touch available slot (invisible ball || small ball)
            if (touchBall.GetState() == Ball.State.INVISIBLE || 
                (touchBall.GetState() == Ball.State.IDLE && touchBall.GetSize() == Ball.Size.SMALL))
            {
                // if ball has moving path (-> swap index of 2 ball in storage)
                List<int> cloneListAvailNodes = new List<int>(lEmptyTiles);
                cloneListAvailNodes.AddRange(lSmallBalls);
                if (touchedBall.FindPath(cloneListAvailNodes, _tileId))
                {
                    OnMovingBall(touchedBall, _tileId);
                }
                // if ball's path stucked
                else
                {
                    touchedBall.SetActiveState(Ball.State.IDLE, true);
                }
            }
            // touch same ball || another activated ball
            else if (_tileId == touchedBallId || touchBall.GetState() != Ball.State.INVISIBLE)
            {
                touchedBall.SetActiveState(Ball.State.IDLE, true);
            }

            touchedBallId = -1;
        }
    }

    public void OnEndOneTurn()
    {
        // invisible spending small ball
        if (smallBallOverlayed != -1 && smallBallOverlayed < lBalls.Count)
        {
            Ball smallBall = lBalls[smallBallOverlayed];
            smallBall.OnOverlayed();
            smallBallOverlayed = -1;
        }

        CheckBallsMatchType();  // check all balls matching type
        GrowAllSmallBalls();    // grow all small balls
        SpawnRandomBalls(Ball.State.GROWSMALL);     // spawn random balls

        movingBallId = -1;
    }

    // ========================================================== PRIVATE FUNC ==========================================================
    private void OnMovingBall(Ball _moveBall, int _desTileId)
    {
        Ball desBall = lBalls[_desTileId];

        // small ball (which overlayed later)
        if (desBall.GetState() == Ball.State.IDLE && desBall.GetSize() == Ball.Size.SMALL)
            UpdateListSmallBalls(false, desBall.GetTileId());   // remove this id from list small balls
        // change position for invisible ball
        else
            desBall.SetPosition(_moveBall.GetPosition());

        // update empty slot
        UpdateListEmptyTiles(true, _moveBall.GetTileId());      // add new empty slot (where ball start moving)
        UpdateListEmptyTiles(false, desBall.GetTileId());       // remove empty slot (where ball move to)

        // SWAP Tile ID of 2 balls
        int tmpId = _moveBall.GetTileId();
        _moveBall.SetTileId(desBall.GetTileId());
        desBall.SetTileId(tmpId);

        // also swap in storage
        lBalls[desBall.GetTileId()] = desBall;
        lBalls[_moveBall.GetTileId()] = _moveBall;

        // spending process small overlayed later
        if (desBall.GetState() == Ball.State.IDLE && desBall.GetSize() == Ball.Size.SMALL)
            smallBallOverlayed = desBall.GetTileId();
        movingBallId = _moveBall.GetTileId();   // store id of moving ball
    }

    // === spawn ball ===
    private void SpawnRandomBalls(Ball.State _state)
    {
        // gen balls
        List<int> spawnIds = new List<int>();
        int turn = Mathf.Min(ballSpawnPerTurn, lEmptyTiles.Count);
        // Random Position (not match prev pos)
        for (int i = 0; i < turn; i++)
        {
            int rdId = -1;
            do
            {
                rdId = (turn == ballSpawnPerTurn) ? Random.Range(0, lEmptyTiles.Count) : i;
            } while (spawnIds.Contains(rdId) || rdId >= lEmptyTiles.Count);
            spawnIds.Add(lEmptyTiles[rdId]);
        }

        // Random Type
        List<Ball.Type> rdTypes = new List<Ball.Type>();
        for (int i = 0; i < 3; i++)
        {
            Ball.Type type = Ball.Type.BLUE;    // default color
            // get type from random list 
            if (i < lnextTypes.Count)
            {
                rdTypes.Add(lnextTypes[i]);
            }
            else
            {
                // random type for this turn
                type = (Ball.Type)Random.Range((int)Ball.Type.BLUE, (int)Ball.Type.GHOST + 1);
                rdTypes.Add(type);
            }

            // random for next turn
            type = (Ball.Type)Random.Range((int)Ball.Type.BLUE, (int)Ball.Type.GHOST + 1);
            if (i < lnextTypes.Count)
                lnextTypes[i] = (type);
            else
                lnextTypes.Add(type);
        }

        for (int i = 0; i < spawnIds.Count; i++)
            SpawnBalls(spawnIds[i], rdTypes[i], _state);

        // Show Panel
        if (lnextTypes.Count >= 3)
            TopPanelMgr.instance.RefreshBallPanel(Ball.GetSprite(lnextTypes[0]), Ball.GetSprite(lnextTypes[1]), Ball.GetSprite(lnextTypes[2]));
        // GAME OVER
        if (turn < ballSpawnPerTurn)
        {
            Debug.Log("GAME OVER");
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
        //if (DebugUtils.IsDebugEnable())
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

    private void CheckBallsMatchType()
    {
        List<int> bigBalls = new List<int>();
        // Filter all ball has big size
        for (int i = 0; i < lBalls.Count; i++)
        {
            if (!lEmptyTiles.Contains(i) && !lSmallBalls.Contains(i) && i < lBalls.Count)
                bigBalls.Add(i);
        }

        // Search all balls matching type
        int boardDimension = BoardMgr.Instance.boardDimension;
        List<int> matchedBallIds = new List<int>();
        for (int i = 0; i < bigBalls.Count; i++)
        {
            for (int dir = 0; dir < 8; dir++)
            {
                List<int> path = new List<int>();
                path.Add(bigBalls[i]);

                Utils.FindPathMathColor(new List<Ball>(lBalls), boardDimension, bigBalls, dir, ref path);

                // add matched id 
                if (path.Count >= ballsGetPoint && path.Contains(movingBallId))
                {
                    foreach (int id in path)
                        if (!matchedBallIds.Contains(id))
                            matchedBallIds.Add(id);
                }
            }
        }

        // Explode all matched color
        for (int i = 0; i < matchedBallIds.Count; i++)
        {
            if (matchedBallIds[i] < lBalls.Count)
            {
                Ball ball = lBalls[matchedBallIds[i]];
                ball.OnExplode();
                UpdateListEmptyTiles(true, ball.GetTileId());   // add empty slot
            }
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
