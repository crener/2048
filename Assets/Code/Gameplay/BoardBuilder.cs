using Code.Gameplay;
using Code.Menu;
using Code.Reusable;
using UnityEngine;

public class BoardBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject SquarePrefab;
    [SerializeField, Range(0, 1)]
    private float boardSpacer = 0.9f;

    private RectTransform trans;
    private TileMover mover;
    public static ObjectPool<Tile> spareTiles;
    private Vector2 squareSize;

    void Start()
    {
        GameObject optionObject = GameObject.Find("GameSize");
        if(optionObject == null)
        {
            Debug.LogError("GameOption component could be found therefore no Board settings can be extracted");
            return;
        }

        GameOption opt = optionObject.GetComponent<GameOption>();
        Destroy(optionObject);//remove it so that there will be no issues if the game mode is changed before the game finishes

        trans = gameObject.GetComponent<RectTransform>();
        SizeSelector.GameSize option = opt.Option;
        int boardSize = option.X * option.Y;
        spareTiles = new ObjectPool<Tile>(() => CreateTile(), boardSize * 2, g => g.gameObject.activeInHierarchy, true);

        mover = GetComponent<TileMover>();

        if (false)
        {
            //load the previous state
        }
        else
        {
            //create a new game board
            BuildBoard(option.X, option.Y);
            mover.boardSize = new Vector2(option.X, option.Y);
        }

        Destroy(opt);
    }

    private void BuildBoard(int boardX, int boardY)
    {
        squareSize = new Vector2
        {
            x = trans.rect.size.x / boardX,
            y = trans.rect.size.y / boardY
        };

        Vector2 topLeft = new Vector2(trans.position.x, trans.position.y);
        topLeft += new Vector2(-Mathf.Abs(trans.rect.size.x / 2), Mathf.Abs(trans.rect.size.y / 2));
        topLeft += squareSize / 2; //add half square size here once rather than X*Y times within the loop

        for (int y = 0; y < boardY; y++)
        {
            for (int x = 0; x < boardX; x++)
            {
                Vector3 squarePos = new Vector3
                {
                    x = topLeft.x + (squareSize.x * x),
                    y = topLeft.y - (squareSize.y * (y + 1)),
                    z = -1f
                };

                Tile tile = spareTiles.GetObject();
                tile.GridPosition = new Vector2(x, y);

                GameObject newSquare = tile.gameObject;
                newSquare.name = x + " " + y;
                newSquare.transform.position = squarePos;

                mover.AddNewTile(tile);
            }
        }
    }

    private Tile CreateTile()
    {
        GameObject newSquare = Instantiate(SquarePrefab, Vector3.zero, new Quaternion(), trans);
        newSquare.name = "newTile";

        Tile tile = newSquare.GetComponent<Tile>();

        RectTransform squareTrans = newSquare.GetComponent<RectTransform>();
        squareTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, squareSize.x * boardSpacer);
        squareTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, squareSize.y * boardSpacer);

        return tile;
    }

#if ENABLE_PLAYMODE_TESTS_RUNNER
    public void setPrefab(GameObject pre)
    {
        SquarePrefab = pre;
    }
#endif
}