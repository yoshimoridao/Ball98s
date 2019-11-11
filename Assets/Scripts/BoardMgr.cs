using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMgr : Singleton<BoardMgr>
{
    public GameObject prefTile;

    public int boardDimension = 9;
    // scale ratio vs camera height
    public float boardScale = 0.8f;
    public float tileOffset = 0.1f;

    private List<GameObject> lTiles = new List<GameObject>();
    [SerializeField]
    private Rect rectBoardSize = Rect.zero;
    private float tileSize;

    // ========================================================== GETTER/ SETTER ==========================================================
    public List<GameObject> GetListTiles()
    {
        return lTiles;
    }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
    }
    void Start ()
    {
        
    }
	
	void Update ()
    {
		if (InputMgr.instance.GetMouseState() == InputMgr.MouseState.DOWN)
        {
            Vector3 mousePos = InputMgr.instance.GetMousePosition();

            // if mouse touch on board
            if (mousePos.x >= rectBoardSize.x && mousePos.x <= rectBoardSize.x + rectBoardSize.width &&
                mousePos.y <= rectBoardSize.y && mousePos.y >= rectBoardSize.y - rectBoardSize.height)
            {
                mousePos.x = Mathf.Abs(mousePos.x - rectBoardSize.x);
                mousePos.y = Mathf.Abs(mousePos.y - rectBoardSize.y);
                int touchedBallId = ((int)(mousePos.y / tileSize) * boardDimension) + (int)(mousePos.x / tileSize);

                // call event response ball
                BallMgr.instance.OnTouchBall(touchedBallId);
            }
        }
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void Init()
    {
        CreateBoard();
    }

    // ========================================================== PRIVATE FUNC ==========================================================
    private void CreateBoard()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // set width, height of board following ratio vs height
        rectBoardSize.width = rectBoardSize.height = cam.orthographicSize * 2 * boardScale;

        transform.localScale = new Vector3(rectBoardSize.width / sr.bounds.size.x, rectBoardSize.height / sr.bounds.size.y, 1.0f);
        // set position for board
        Vector3 pos = transform.position;
        pos.x = cam.transform.position.x;
        pos.y = (cam.transform.position.y - cam.orthographicSize) + rectBoardSize.height / 2.0f;
        transform.position = pos;

        // get top-left position of board
        rectBoardSize.x = sr.bounds.min.x;
        rectBoardSize.y = sr.bounds.max.y;

        // generate tiles
        tileSize = rectBoardSize.width / (float)boardDimension;
        for (int i = 0; i < Mathf.Pow(boardDimension, 2); i++)
        {
            GameObject genTile = GameObject.Instantiate(prefTile);
            SpriteRenderer tileSr = genTile.GetComponent<SpriteRenderer>();
            genTile.name = "tile_" + i.ToString();
            // set size
            Vector3 tileScale = new Vector3((tileSize - tileOffset) / tileSr.bounds.size.x, (tileSize - tileOffset) / tileSr.bounds.size.y, 1.0f);
            genTile.transform.localScale = tileScale;
            // set position
            int r = (i / boardDimension);
            int c = (i % boardDimension);
            Vector3 tilePos = new Vector3(rectBoardSize.x + (c + 0.5f) * tileSize, rectBoardSize.y - (r + 0.5f) * tileSize, 0.0f);
            genTile.transform.position = tilePos;
            // set parent
            genTile.transform.parent = transform;
            // store tiles
            lTiles.Add(genTile);
        }
    }

    // ========================================================== DEBUG ==========================================================
    public void HighlightTile(Color _color, int _id)
    {
        if (_id < lTiles.Count)
        {
            lTiles[_id].GetComponent<SpriteRenderer>().color = _color;
        }
    }
}
