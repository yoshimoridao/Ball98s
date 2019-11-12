using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMgr : Singleton<BoardMgr>
{
    public GameObject prefTile;
    public GameObject topPanelZone;

    public int boardDimension = 9;
    // scale ratio vs camera height
    public float boardScale = 0.8f;
    public float tileOffset = 0.1f;

    private List<Tile> lTiles = new List<Tile>();
    [SerializeField]
    private Rect rectBoardSize = Rect.zero;
    private float tileSize;

    // ========================================================== GETTER/ SETTER ==========================================================
    public Tile GetTile(int _tileId)
    {
        if (_tileId < lTiles.Count)
            return lTiles[_tileId];
        return null;
    }
    public List<Tile> GetListTiles() { return lTiles; }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        instance = this;
    }
    void Start ()
    {
        // set z for bg
        transform.position = new Vector3(transform.position.x, transform.position.y, ZDepth.GetDepth(ZDepth.Layer.TILEBG));
    }
	
	void Update ()
    {
        if (GameMgr.Instance.GetGameState() != GameMgr.GameState.PLAYING)
            return;

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
        // set width, height of Board Background (following ratio vs camera's height)
        rectBoardSize.width = rectBoardSize.height = cam.orthographicSize * 2 * boardScale;
        transform.localScale = new Vector3(rectBoardSize.width / sr.bounds.size.x, rectBoardSize.height / sr.bounds.size.y, 1.0f);
        // set position of Board Background
        Vector3 pos = transform.position;
        pos.x = cam.transform.position.x;
        pos.y = (cam.transform.position.y - cam.orthographicSize) + rectBoardSize.height / 2.0f;
        transform.position = pos;
        // top-left position of Board
        rectBoardSize.x = sr.bounds.min.x;
        rectBoardSize.y = sr.bounds.max.y;

        // set width, height of Top Zone
        SpriteRenderer topPanelSr = topPanelZone.GetComponent<SpriteRenderer>();
        topPanelZone.transform.localScale = new Vector3(rectBoardSize.width / topPanelSr.bounds.size.x, (cam.orthographicSize * 2 - rectBoardSize.height - 0.2f) / topPanelSr.bounds.size.y, 1.0f);
        pos.y = (cam.transform.position.y + cam.orthographicSize) - (cam.orthographicSize * 2 - rectBoardSize.height) / 2.0f;
        // set pos of Top Zone
        topPanelZone.transform.position = pos;
        TopPanelMgr.Instance.SetPositionBallPanel(Camera.main.WorldToScreenPoint(topPanelZone.transform.position));
        TopPanelMgr.Instance.SetPositionScoreZone(Camera.main.WorldToScreenPoint(new Vector3(topPanelSr.bounds.min.x, topPanelSr.bounds.center.y, 0.0f)));
        TopPanelMgr.Instance.SetPositionHighScoreZone(Camera.main.WorldToScreenPoint(new Vector3(topPanelSr.bounds.max.x, topPanelSr.bounds.center.y, 0.0f)));

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
            Vector3 tilePos = new Vector3(rectBoardSize.x + (c + 0.5f) * tileSize, rectBoardSize.y - (r + 0.5f) * tileSize, ZDepth.GetDepth(ZDepth.Layer.TILE));
            genTile.transform.position = tilePos;
            // set parent
            genTile.transform.parent = transform;
            // store tiles
            lTiles.Add(genTile.GetComponent<Tile>());
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
