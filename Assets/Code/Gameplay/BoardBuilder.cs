using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Code.Gameplay;
using Code.Menu;
using UnityEngine;

public class BoardBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject SquarePrefab;
    [SerializeField, Range(0, 1)]
    private float boardSpacer = 0.9f;

    private RectTransform trans;
    private TileMover mover;

    void Start()
    {
        trans = gameObject.GetComponent<RectTransform>();
        GameOption opt = GetComponentInParent<GameOption>();
        SizeSelector.GameSize option = opt.Option;

        mover = GetComponent<TileMover>();

        if(false)
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
        Vector2 squareSize = new Vector2
        {
            x = trans.rect.size.x / boardX,
            y = trans.rect.size.y / boardY
        };

        Vector2 topLeft = new Vector2(trans.position.x, trans.position.y) - (trans.rect.size / 2);
        topLeft += squareSize / 2; //add half square size here once rather than X*Y times within the loop

        for(int y = 0; y < boardY; y++)
        {
            for(int x = 0; x < boardX; x++)
            {
                Vector2 squarePos = new Vector2
                {
                    x = topLeft.x + (squareSize.x * x),
                    y = topLeft.y + (squareSize.y * y)
                };

                GameObject newSquare = Instantiate(SquarePrefab, squarePos, new Quaternion(), trans);
                newSquare.name = x + " " + y;

                Tile tile = newSquare.GetComponent<Tile>();
                tile.GridPosition = new Vector2(x, y);
                
                RectTransform squareTrans = newSquare.GetComponent<RectTransform>();
                squareTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, squareSize.x * boardSpacer);
                squareTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, squareSize.y * boardSpacer);

                mover.addNewTile(tile);
            }
        }
    }

//    private void OnDrawGizmos()
//    {
//        {
//            Vector2 topLeft = new Vector2(trans.position.x, trans.position.y);
//
//            Debug.DrawLine(topLeft + (Vector2.down * 4), topLeft + (Vector2.up * 4), Color.red);
//            Debug.DrawLine(topLeft + (Vector2.left * 4), topLeft + (Vector2.right * 4), Color.red);
//        }
//        {
//            Vector2 topLeft = new Vector2(trans.position.x, trans.position.y);
//            topLeft += new Vector2(-Mathf.Abs(trans.rect.size.x / 2), Mathf.Abs(trans.rect.size.y / 2));
//
//            Debug.DrawLine(topLeft + (Vector2.down * 4), topLeft + (Vector2.up * 4), Color.blue);
//            Debug.DrawLine(topLeft + (Vector2.left * 4), topLeft + (Vector2.right * 4), Color.blue);
//        }
//    }
}