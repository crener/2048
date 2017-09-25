using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Code.Reusable;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.Gameplay
{
    public class TileMover : MonoBehaviour
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField]
        private Text scoreText;
        [Header("Tile Colour Settings")]
        [SerializeField]
        private Color primaryTextColour = Color.black;
        [SerializeField]
        private Color SecondaryTextColour = Color.white;
        [SerializeField]
        private List<TileStyle> Styles;
        [SerializeField]
        private TileStyle UnknownTileStyle;
        [SerializeField]
        private Color emptyTileColor = Color.grey;
        [Header("Tile Movement Settings")]
        [SerializeField, Tooltip("Time (seconds) it takes to complete one move across the board")]
        private float MovementSpeed = 0.2f;
        [Header("Game Balance")]
        [SerializeField, Range(0, 1), Tooltip("Change that a tile will spawn around the edge")]
        private float EdgeSpawn = 0.7f;
        [SerializeField, Range(0, 1), Tooltip("Chance of getting a high point value to spawn")]
        private float highScoreSpawn = 0.1f;

        public int score { get; set; }
        public Vector2 boardSize { get; set; }

        private Dictionary<Vector2, Tile> tilePositions = new Dictionary<Vector2, Tile>();

        void Start()
        {
            if (tilePositions.Count <= 0)
            {
                Tile[] tiles = GetComponentsInChildren<Tile>();
                if (tiles.Length == 0)
                {
                    Debug.LogError("No Tiles found!!");
                    return;
                }

                foreach (Tile tile in tiles)
                    tilePositions.Add(tile.GridPosition, tile);
            }

            Styles.Sort((a, b) => a.score.CompareTo(b.score));

            //set the initial tile to a state (If loading save game this doesn't matter)
            if (score == 0)
            {
                //find a tile around the edge of the board
                Tile edge1 = FindEmptyEdgeTile();
                SpawnTile(edge1, true);
                Tile edge2 = FindEmptyEdgeTile();
                SpawnTile(edge2, true);
            }
        }

        private Tile FindEmptyEdgeTile()
        {
            Tile edgeTile = null;
            do
            {
                int x = (int)Random.Range(0, boardSize.x);
                int y = 0;

                if (x > 0 && x < boardSize.x)
                {
                    //X is in the middle of the grid to Y is limited to top or bottom
                    bool top = Random.Range(0, 1) == 1;
                    y = top ? (int)boardSize.x : 0;
                }
                else if (x == 0 || x == boardSize.x)
                    y = (int)Random.Range(0, boardSize.y);
                else Debug.LogError("Unknown generation state! x = " + x);

                if (!tilePositions.ContainsKey(new Vector2(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }

                Tile potentialTile = tilePositions[new Vector2(x, y)];
                if (potentialTile.Value != -1) continue;

                edgeTile = potentialTile;
            } while (edgeTile == null);

            return edgeTile;
        }

        private Tile FindEmptyEdgeTile(Direction edge)
        {
            Tile edgeTile = null;
            do
            {
                int x = -1;
                int y = -1;

                if (edge == Direction.Up || edge == Direction.Down)
                {
                    x = (int)Random.Range(0, boardSize.x);

                    if (edge == Direction.Up) y = 0;
                    else if (edge == Direction.Down) y = (int)boardSize.y;
                }
                else if (edge == Direction.Left || edge == Direction.Right)
                {
                    y = (int)Random.Range(0, boardSize.y);

                    if (edge == Direction.Left) x = 0;
                    else if (edge == Direction.Right) x = (int)boardSize.x;
                }

                if (!tilePositions.ContainsKey(new Vector2(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }

                Tile potentialTile = tilePositions[new Vector2(x, y)];
                if (potentialTile.Value != -1) continue;

                edgeTile = potentialTile;
            } while (edgeTile == null);

            return edgeTile;
        }

        private Tile FindEmptyTile()
        {
            Tile edgeTile = null;
            do
            {
                int x = (int)Random.Range(0, boardSize.x);
                int y = (int)Random.Range(0, boardSize.y);

                if (!tilePositions.ContainsKey(new Vector2(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }

                Tile potentialTile = tilePositions[new Vector2(x, y)];
                if (potentialTile.Value != -1) continue;

                edgeTile = potentialTile;
            } while (edgeTile == null);

            return edgeTile;
        }

        public void PlaceNewTile(Direction previousDirection)
        {
            bool directionSpawn = EdgeSpawn > Random.Range(0f, 1f);

            Tile change;
            if (directionSpawn)
                change = FindEmptyEdgeTile(OppositeSide(previousDirection));
            else
                change = FindEmptyTile();

            SpawnTile(change);
        }

        private Direction OppositeSide(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    throw new ArgumentOutOfRangeException("dir", dir, null);
            }
        }

        private void SpawnTile(int x, int y)
        {
            SpawnTile(tilePositions[new Vector2(x, y)]);
        }

        /// <summary>
        /// Sets a specific tile to a either the lowest or second lowest value
        /// </summary>
        /// <param name="tile">tile that will be changed</param>
        /// <param name="forceLow">force the score to be the lowest</param>
        private void SpawnTile(Tile tile, bool forceLow = false)
        {
            TileStyle style = forceLow ? Styles[0] : highScoreSpawn > Random.Range(0, 1) ? Styles[0] : Styles[1];
            tile.setTile(style.score, style.Color, style.SecondaryTextColour ? SecondaryTextColour : primaryTextColour);
        }

        public void Up()
        {
            bool valid = false;
            List<Vector2> moveables = new List<Vector2>();

            //ensure that this is a valid move
            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 1; y < boardSize.y; y++)
                {
                    //check tile above
                    Tile testTile = tilePositions[new Vector2(x, y)];
                    if (testTile.Value == -1) continue;
                    else
                    {
                        if (tilePositions[new Vector2(x, y - 1)].Value == -1 || //can move due to empty tile
                           tilePositions[new Vector2(x, y - 1)].Value == testTile.Value) //can move due to merdge  
                        {
                            valid = true;
                            moveables.Add(new Vector2(x, y));
                        }
                    }
                }
            }

            if (valid) Debug.Log("valid up move");
            else Debug.Log("invalid up move");

            //move the tiles up
            foreach (Vector2 moveable in moveables)
            {
                Tile instance = tilePositions[moveable], replace = null;
                Vector2 moveGridLocation = new Vector2(-1f, -1f);
                Vector2 moveUiLocation = new Vector2(-1f, -1f);
                bool move = true;

                //find the next move location
                for (int y = (int)moveable.y - 1; y >= 0; y--)
                {
                    Tile compare = tilePositions[new Vector2(instance.GridPosition.x, y)];

                    if (compare.Value == -1)
                    {
                        //move to empty field
                        moveGridLocation = compare.GridPosition;
                        moveUiLocation = compare.UiPosition;
                        replace = compare;
                        move = true;
                    }
                    else if (compare.Value == instance.Value)
                    {
                        //move to merge with another tile
                        moveGridLocation = compare.GridPosition;
                        moveUiLocation = compare.UiPosition;
                        replace = compare;
                        move = false;
                    }
                    else
                        //stop looking since anything after this would involve going through a tile
                        break;
                }

                if(moveGridLocation == new Vector2(-1f, -1f)) continue;

                //actual move the tile and update internal location
                tilePositions.Remove(instance.GridPosition);
                tilePositions.Remove(moveGridLocation);
                tilePositions.Add(moveGridLocation, instance);
                PlaceEmptyTile(instance.GridPosition, instance.UiPosition);

                if(move)
                    instance.moveTile(moveGridLocation, moveUiLocation, MovementSpeed, replace);
                else
                {
                    TileStyle style = getStyle(instance.Value * 2);
                    instance.MergeTile(moveGridLocation, moveUiLocation, MovementSpeed, replace, style.Color, style.SecondaryTextColour ? SecondaryTextColour : primaryTextColour);
                }
            }
        }

        private TileStyle getStyle(int i)
        {
            foreach(TileStyle style in Styles)
            {
                if(style.score == i) return style;
            }

            return UnknownTileStyle;
        }

        private void PlaceEmptyTile(Vector2 grid, Vector2 ui)
        {
            Tile tile = BoardBuilder.spareTiles.GetObject();
            tile.setEmpty(grid, ui, emptyTileColor);
            tile.gameObject.SetActive(true);

            addNewTile(tile);
        }

        public void addNewTile(Tile tile)
        {
            tilePositions.Add(tile.GridPosition, tile);
        }
    }

    [Serializable]
    internal class TileStyle
    {
        public int score = -1;
        public Color Color = Color.black;
        public bool SecondaryTextColour = false;
    }
}