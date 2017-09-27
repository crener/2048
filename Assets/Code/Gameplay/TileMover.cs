using System;
using System.Collections.Generic;
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
        private Color secondaryTextColour = Color.white;
        [SerializeField]
        private List<TileStyle> Styles = new List<TileStyle>(14);
        [SerializeField]
        private TileStyle unknownTileStyle;
        [SerializeField]
        private Color emptyTileColor = Color.grey;
        [Header("Tile Movement Settings")]
        [SerializeField, Tooltip("Time (seconds) it takes to complete one move across the board")]
        private float movementSpeed = 0.2f;
        [Header("Game Balance")]
        [SerializeField, Range(0, 1), Tooltip("Change that a tile will spawn around the edge")]
        private float edgeSpawn = 0.7f;
        [SerializeField, Range(0, 1), Tooltip("Chance of getting a high point value to spawn")]
        private float highScoreSpawn = 0.1f;

        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                if (scoreText != null) scoreText.text = score.ToString("N0");
            }
        }

        public Vector2 boardSize { get; set; }

        private Dictionary<Vector2, Tile> tilePositions = new Dictionary<Vector2, Tile>();
        private int score;

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

            if (Styles.Count == 0) Debug.LogError("There need to be at least 2 Styles set");
            Styles.Sort((a, b) => a.score.CompareTo(b.score));

            //set the initial tile to a state (If loading save game this doesn't matter)
            if (Score == 0)
            {
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
                    x = (int)Random.Range(0, boardSize.x - 1);

                    if (edge == Direction.Up) y = 0;
                    else if (edge == Direction.Down) y = (int)boardSize.y - 1;
                }
                else if (edge == Direction.Left || edge == Direction.Right)
                {
                    y = (int)Random.Range(0, boardSize.y - 1);

                    if (edge == Direction.Left) x = 0;
                    else if (edge == Direction.Right) x = (int)boardSize.x - 1;
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

        private void PlaceNewTile(Direction previousDirection)
        {
            if(!CheckForEmptyTile()) return;

            bool directionSpawn = CheckForEmptyEdgeTile() && edgeSpawn > Random.Range(0f, 1f);

            Tile change;
            if (directionSpawn)
                change = FindEmptyEdgeTile(OppositeSide(previousDirection));
            else
                change = FindEmptyTile();

            SpawnTile(change);
        }

        private bool isEndOfGame()
        {
            if(CheckForEmptyTile()) return false;

            //there are no more empty tiles so check for possible moves

            return true;
        }

        private bool CheckForEmptyTile()
        {
            foreach (Tile tilePosition in tilePositions.Values)
                if (tilePosition.Value != -1) return true;
            return false;
        }

        private bool CheckForEmptyEdgeTile()
        {
            //top
            for (int x = 0; x < boardSize.x; x++)
                if (tilePositions[new Vector2(x, 0)].Value != -1) return true;

            //bottom
            for (int x = 0; x < boardSize.x; x++)
                if (tilePositions[new Vector2(x, boardSize.y - 1)].Value != -1) return true;

            //middle left
            for (int y = 1; y < boardSize.y - 1; y++)
                if (tilePositions[new Vector2(0, y)].Value != -1) return true;

            //middle right
            for (int y = 1; y < boardSize.y - 1; y++)
                if (tilePositions[new Vector2(boardSize.x - 1, y)].Value != -1) return true;

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            //top
            for(int x = 0; x < boardSize.x; x++)
                Draw(tilePositions[new Vector2(x, 0)].UiPosition, Color.blue);

            //bottom
            for (int x = 0; x < boardSize.x; x++)
                Draw(tilePositions[new Vector2(x, boardSize.y - 1)].UiPosition, Color.blue);

            //left middle
            for (int y = 1; y < boardSize.y - 1; y++)
                Draw(tilePositions[new Vector2(0, y)].UiPosition, Color.red);

            //right middle
            for (int y = 1; y < boardSize.y - 1; y++)
                Draw(tilePositions[new Vector2(boardSize.x - 1, y)].UiPosition, Color.red);
        }

        private void Draw(Vector2 pos, Color col)
        {
            Debug.DrawLine(pos - (Vector2.left * 3), pos - (Vector2.right * 3), col);
            Debug.DrawLine(pos - (Vector2.up * 3), pos - (Vector2.down * 3), col);
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

        /// <summary>
        /// Sets a specific tile to a either the lowest or second lowest value
        /// </summary>
        /// <param name="tile">tile that will be changed</param>
        /// <param name="forceLow">force the score to be the lowest</param>
        private void SpawnTile(Tile tile, bool forceLow = false)
        {
            TileStyle style = forceLow ? Styles[0] : highScoreSpawn > Random.Range(0, 1) ? Styles[0] : Styles[1];
            tile.setTile(style.score, style.Color, style.SecondaryTextColour ? secondaryTextColour : primaryTextColour);
        }

        public void Up()
        {
            bool valid = false;

            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 1; y < boardSize.y; y++)
                {
                    Tile testTile = tilePositions[new Vector2(x, y)], replace = null;
                    if (testTile.Value == -1) continue;

                    bool move = true;

                    //find the next move location (go down from this tile until an invalid tile is hit)
                    for (int y2 = y - 1; y2 >= 0; y2--)
                    {
                        Tile compare = tilePositions[new Vector2(x, y2)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                        }
                        else if (compare.Value == testTile.Value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (replace == null) continue;
                    MoveTile(testTile, replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid up move" : "invalid up move");
            if (!valid) return;

            PlaceNewTile(Direction.Up);
        }

        public void Down()
        {
            bool valid = false;

            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = (int)boardSize.y - 2; y >= 0; y--)
                {
                    Tile testTile = tilePositions[new Vector2(x, y)], replace = null;
                    if (testTile.Value == -1) continue;

                    bool move = true;

                    //find the next move location (go down from this tile until an invalid tile is hit)
                    for (int y2 = y + 1; y2 < boardSize.y; y2++)
                    {
                        Tile compare = tilePositions[new Vector2(x, y2)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                        }
                        else if (compare.Value == testTile.Value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (replace == null) continue;
                    MoveTile(testTile, replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid down move" : "invalid down move");
            if (!valid) return;

            PlaceNewTile(Direction.Down);
        }

        public void Left()
        {
            bool valid = false;

            for (int y = 0; y < boardSize.y; y++)
            {
                for (int x = 1; x < boardSize.x; x++)
                {
                    Tile testTile = tilePositions[new Vector2(x, y)], replace = null;
                    if (testTile.Value == -1) continue;

                    bool move = true;

                    //find the next move location (go left from this tile until an invalid tile is hit)
                    for (int x2 = x - 1; x2 >= 0; x2--)
                    {
                        Tile compare = tilePositions[new Vector2(x2, y)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                        }
                        else if (compare.Value == testTile.Value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (replace == null) continue;
                    MoveTile(testTile, replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid left move" : "invalid left move");
            if (!valid) return;

            PlaceNewTile(Direction.Left);
        }

        public void Right()
        {
            bool valid = false;

            //ensure that this is a valid move
            for (int y = 0; y < boardSize.y; y++)
            {
                for (int x = (int)boardSize.x - 2; x >= 0; x--)
                {
                    Tile testTile = tilePositions[new Vector2(x, y)], replace = null;
                    if (testTile.Value == -1) continue;

                    bool move = true;

                    for (int x2 = x + 1; x2 < boardSize.y; x2++)
                    {
                        Tile compare = tilePositions[new Vector2(x2, y)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                        }
                        else if (compare.Value == testTile.Value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (replace == null) continue;
                    MoveTile(testTile, replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid right move" : "invalid right move");
            if (!valid) return;

            PlaceNewTile(Direction.Right);
        }

        private void MoveTile(Tile mover, Tile replace, bool moveNotMerge)
        {
            //update internal board position information
            tilePositions.Remove(mover.GridPosition);
            tilePositions.Remove(replace.GridPosition);
            tilePositions.Add(replace.GridPosition, mover);
            PlaceEmptyTile(mover.GridPosition, mover.UiPosition);

            mover.transform.SetAsLastSibling();

            if (moveNotMerge) mover.MoveTile(replace.GridPosition, replace.UiPosition, movementSpeed, replace);
            else
            {
                Score += mover.Value * 2;
                TileStyle style = getStyle(mover.Value * 2);
                mover.MergeTile(replace.GridPosition, replace.UiPosition, movementSpeed, replace, style.Color, style.SecondaryTextColour ? secondaryTextColour : primaryTextColour);
            }
        }

        private TileStyle getStyle(int i)
        {
            foreach (TileStyle style in Styles)
            {
                if (style.score == i) return style;
            }

            return unknownTileStyle;
        }

        private void PlaceEmptyTile(Vector2 grid, Vector2 ui)
        {
            Tile tile = BoardBuilder.spareTiles.GetObject();
            tile.transform.SetAsFirstSibling();
            tile.setEmpty(grid, ui, emptyTileColor);
            tile.gameObject.SetActive(true);

            AddNewTile(tile);
        }

        public void AddNewTile(Tile tile)
        {
            if (tilePositions.ContainsKey(tile.GridPosition))
                tilePositions.Remove(tile.GridPosition);

            tilePositions.Add(tile.GridPosition, tile);
        }

#if ENABLE_PLAYMODE_TESTS_RUNNER
        internal Dictionary<Vector2, Tile> getBoardRepresentation()
        {
            return tilePositions;
        }
        internal void setBoardRepresentation(Dictionary<Vector2, Tile> newBoard)
        {
            tilePositions = newBoard;
        }

        internal void AddStyle(TileStyle style)
        {
            Styles.Add(style);
        }

        internal void AddUnknownStyle(TileStyle style)
        {
            unknownTileStyle = style;
        }

        public float DirectionChance {set { edgeSpawn = value; } }
#endif
    }

    [Serializable]
    internal class TileStyle
    {
        public int score = -1;
        public Color Color = Color.black;
        public bool SecondaryTextColour = false;
    }
}