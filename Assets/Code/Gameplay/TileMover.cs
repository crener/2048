using System;
using System.Collections.Generic;
using Assets.Code.Gameplay;
using Assets.Code.Reusable;
using UnityEngine;
using UnityEngine.Profiling;
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
        private Text scoreText, GameOver;
        [SerializeField]
        private int historyLength = 3;
        [Header("Tile Colour Settings")]
        [SerializeField]
        private Color primaryTextColour = Color.black,
            secondaryTextColour = Color.white,
            emptyTileColor = Color.grey;
        [SerializeField]
        private List<TileStyle> Styles = new List<TileStyle>(14);
        [SerializeField]
        private TileStyle unknownTileStyle;
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

        public BoardPos boardSize { get; set; }

        private Dictionary<BoardPos, Tile> tilePositions = new Dictionary<BoardPos, Tile>();
        private DropOutStack<BoardState> history;
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

            if (GameOver != null) GameOver.gameObject.SetActive(false);

            history = new DropOutStack<BoardState>(historyLength);
            UpdateHistory(); //save initial state
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Save();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveSystem.SaveBoard(BuildSaveState());
        }

        private Tile FindEmptyEdgeTile()
        {
            Tile edgeTile = null;
            do
            {
                int x = Random.Range(0, boardSize.X);
                int y = 0;

                if (x > 0 && x < boardSize.X)
                {
                    //X is in the middle of the grid to Y is limited to top or bottom
                    bool top = Random.Range(0, 1) == 1;
                    y = top ? boardSize.X : 0;
                }
                else if (x == 0 || x == boardSize.X)
                    y = Random.Range(0, boardSize.Y);
                else Debug.LogError("Unknown generation state! x = " + x);

                if (!tilePositions.ContainsKey(new BoardPos(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }

                Tile potentialTile = tilePositions[new BoardPos(x, y)];
                if (potentialTile.Value != -1) continue;

                edgeTile = potentialTile;
            } while (edgeTile == null);

            return edgeTile;
        }

        private Tile FindEmptyEdgeTile(Direction edge)
        {
            Profiler.BeginSample("Find Edge Tile");

            List<Tile> validPositions = new List<Tile>();
            int returnTile = 0;

            if (edge == Direction.Up || edge == Direction.Down)
            {
                int y = -1;
                Profiler.BeginSample("X Search");

                if (edge == Direction.Up) y = 0;
                else if (edge == Direction.Down) y = boardSize.Y - 1;

                //figure out which tiles are free
                BoardPos pos = new BoardPos(-1, y);
                for (int i = 0; i < boardSize.X; i++)
                {
                    pos.X = i;
                    if (tilePositions[pos].Value == -1)
                        validPositions.Add(tilePositions[pos]);
                }

                returnTile = Random.Range(0, validPositions.Count - 1);
                Profiler.EndSample();
            }

            if (edge == Direction.Left || edge == Direction.Right)
            {
                int x = -1;
                Profiler.BeginSample("Y Search");

                if (edge == Direction.Left) x = 0;
                else if (edge == Direction.Right) x = boardSize.X - 1;

                //figure out which tiles are free
                BoardPos pos = new BoardPos(x, -1);
                for (int i = 0; i < boardSize.Y; i++)
                {
                    pos.Y = i;
                    if (tilePositions[pos].Value == -1)
                        validPositions.Add(tilePositions[pos]);
                }

                returnTile = Random.Range(0, validPositions.Count - 1);
                Profiler.EndSample();
            }

            Profiler.EndSample();
            return validPositions[returnTile];
        }

        private Tile FindEmptyTile()
        {
            Profiler.BeginSample("Find Any Tile");
            Tile edgeTile = null;
            do
            {
                int x = Random.Range(0, boardSize.X);
                int y = Random.Range(0, boardSize.Y);

                if (!tilePositions.ContainsKey(new BoardPos(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }

                Tile potentialTile = tilePositions[new BoardPos(x, y)];
                if (potentialTile.Value != -1) continue;

                edgeTile = potentialTile;
            } while (edgeTile == null);

            Profiler.EndSample();
            return edgeTile;
        }

        private void PlaceNewTile(Direction previousDirection)
        {
            Profiler.BeginSample("Place Tile");
            if (!CheckForEmptyTile())
            {
                Profiler.EndSample();
                return;
            }

            bool directionSpawn = CheckForEmptyEdgeTile() && edgeSpawn > Random.Range(0f, 1f);
            SpawnTile(directionSpawn ?
                FindEmptyEdgeTile(OppositeSide(previousDirection)) :
                FindEmptyTile());
            Profiler.EndSample();
        }

        public bool isEndOfGame()
        {
            Profiler.BeginSample("isEndOfGame");
            if (CheckForEmptyTile())
            {
                Profiler.EndSample();
                return false;
            }

            //there are no more empty tiles so check for possible moves
            for (int x = 0; x < boardSize.X; x++)
            {
                for (int y = 0; y < boardSize.Y; y++)
                {
                    //go though each tile and check if a tile can move
                    Tile tile = tilePositions[new BoardPos(x, y)];

                    //up
                    if (y != 0)
                    {
                        Tile upTile = tilePositions[new BoardPos(x, y - 1)];
                        if (upTile.Value == tile.Value || upTile.Value == -1)
                        {
                            Profiler.EndSample();
                            return false;
                        }
                    }

                    //down
                    if (y != boardSize.Y - 1)
                    {
                        Tile downTile = tilePositions[new BoardPos(x, y + 1)];
                        if (downTile.Value == tile.Value || downTile.Value == -1)
                        {
                            Profiler.EndSample();
                            return false;
                        }
                    }

                    //left
                    if (x != 0)
                    {
                        Tile leftTile = tilePositions[new BoardPos(x - 1, y)];
                        if (leftTile.Value == tile.Value || leftTile.Value == -1)
                        {

                            Profiler.EndSample();
                            return false;
                        }
                    }

                    //right
                    if (x != boardSize.X - 1)
                    {
                        Tile rightTile = tilePositions[new BoardPos(x + 1, y)];
                        if (rightTile.Value == tile.Value || rightTile.Value == -1)
                        {

                            Profiler.EndSample();
                            return false;
                        }
                    }
                }
            }

            Profiler.EndSample();
            return true;
        }

        /// <summary>
        /// Checks the game board for any empty tiles
        /// </summary>
        /// <returns>true is there are empty tiles</returns>
        private bool CheckForEmptyTile()
        {
            foreach (Tile tilePosition in tilePositions.Values)
                if (tilePosition.Value == -1) return true;
            return false;
        }

        private bool CheckForEmptyEdgeTile()
        {
            //top
            for (int x = 0; x < boardSize.X; x++)
                if (tilePositions[new BoardPos(x, 0)].Value != -1) return true;

            //bottom
            for (int x = 0; x < boardSize.X; x++)
                if (tilePositions[new BoardPos(x, boardSize.Y - 1)].Value != -1) return true;

            //middle left
            for (int y = 1; y < boardSize.Y - 1; y++)
                if (tilePositions[new BoardPos(0, y)].Value != -1) return true;

            //middle right
            for (int y = 1; y < boardSize.Y - 1; y++)
                if (tilePositions[new BoardPos(boardSize.X - 1, y)].Value != -1) return true;

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            //top
            for (int x = 0; x < boardSize.X; x++)
                Draw(tilePositions[new BoardPos(x, 0)].UiPosition, Color.blue);

            //bottom
            for (int x = 0; x < boardSize.X; x++)
                Draw(tilePositions[new BoardPos(x, boardSize.Y - 1)].UiPosition, Color.blue);

            //left middle
            for (int y = 1; y < boardSize.Y - 1; y++)
                Draw(tilePositions[new BoardPos(0, y)].UiPosition, Color.red);

            //right middle
            for (int y = 1; y < boardSize.Y - 1; y++)
                Draw(tilePositions[new BoardPos(boardSize.X - 1, y)].UiPosition, Color.red);
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
            tile.setTile(style.score, style.Color, style.SecondaryTextColour ? secondaryTextColour : primaryTextColour, movementSpeed, true);
        }

        public void Up()
        {
            if (isEndOfGame() && GameOver != null)
            {
                SaveSystem.RemoveBoard(boardSize.X, boardSize.Y);
                GameOver.gameObject.SetActive(true);
                return;
            }

            bool valid = false;
            for (int x = 0; x < boardSize.X; x++)
            {
                for (int y = 1; y < boardSize.Y; y++)
                {
                    int value = tilePositions[new BoardPos(x, y)].Value;
                    if (value == -1) continue;

                    Tile replace = null;
                    bool move = true, updatable = false;

                    //find the next move location (go down from this tile until an invalid tile is hit)
                    for (int y2 = y - 1; y2 >= 0; y2--)
                    {
                        Tile compare = tilePositions[new BoardPos(x, y2)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                            updatable = true;
                        }
                        else if (compare.Value == value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            updatable = true;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (!updatable) continue;
                    if (valid == false) UpdateHistory(); //only updates history once you know changes will happen
                    MoveTile(tilePositions[new BoardPos(x, y)], replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid up move" : "invalid up move");
            PlaceNewTile(Direction.Up);
        }

        public void Down()
        {
            if (isEndOfGame() && GameOver != null)
            {
                SaveSystem.RemoveBoard(boardSize.X, boardSize.Y);
                GameOver.gameObject.SetActive(true);
                return;
            }

            bool valid = false;

            for (int x = 0; x < boardSize.X; x++)
            {
                for (int y = boardSize.Y - 2; y >= 0; y--)
                {
                    int value = tilePositions[new BoardPos(x, y)].Value;
                    if (value == -1) continue;

                    Tile replace = null;
                    bool move = true, updatable = false;

                    //find the next move location (go down from this tile until an invalid tile is hit)
                    for (int y2 = y + 1; y2 < boardSize.Y; y2++)
                    {
                        Tile compare = tilePositions[new BoardPos(x, y2)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                            updatable = true;
                        }
                        else if (compare.Value == value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            updatable = true;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (!updatable) continue;
                    if (valid == false) UpdateHistory(); //only updates history once you know changes will happen
                    MoveTile(tilePositions[new BoardPos(x, y)], replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid down move" : "invalid down move");
            PlaceNewTile(Direction.Down);
        }

        public void Left()
        {
            if (isEndOfGame() && GameOver != null)
            {
                SaveSystem.RemoveBoard(boardSize.X, boardSize.Y);
                GameOver.gameObject.SetActive(true);
                return;
            }

            bool valid = false;

            for (int y = 0; y < boardSize.Y; y++)
            {
                for (int x = 1; x < boardSize.X; x++)
                {
                    int value = tilePositions[new BoardPos(x, y)].Value;
                    if (value == -1) continue;

                    Tile replace = null;
                    bool move = true, updatable = false;

                    //find the next move location (go left from this tile until an invalid tile is hit)
                    for (int x2 = x - 1; x2 >= 0; x2--)
                    {
                        Tile compare = tilePositions[new BoardPos(x2, y)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                            updatable = true;
                        }
                        else if (compare.Value == value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            updatable = true;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (!updatable) continue;
                    if (valid == false) UpdateHistory(); //only updates history once you know changes will happen
                    MoveTile(tilePositions[new BoardPos(x, y)], replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid left move" : "invalid left move");
            PlaceNewTile(Direction.Left);
        }

        public void Right()
        {
            if (isEndOfGame() && GameOver != null)
            {
                SaveSystem.RemoveBoard(boardSize.X, boardSize.Y);
                GameOver.gameObject.SetActive(true);
                return;
            }

            bool valid = false;

            //ensure that this is a valid move
            for (int y = 0; y < boardSize.Y; y++)
            {
                for (int x = boardSize.X - 2; x >= 0; x--)
                {
                    int value = tilePositions[new BoardPos(x, y)].Value;
                    if (value == -1) continue;

                    Tile replace = null;
                    bool move = true, updatable = false;

                    for (int x2 = x + 1; x2 < boardSize.Y; x2++)
                    {
                        Tile compare = tilePositions[new BoardPos(x2, y)];

                        if (compare.Value == -1)
                        {
                            //move to empty field
                            replace = compare;
                            move = true;
                            updatable = true;
                        }
                        else if (compare.Value == value)
                        {
                            //move to merge with another tile
                            replace = compare;
                            move = false;
                            updatable = true;
                            break;
                        }
                        else
                            //stop looking since anything after this would involve going through a tile
                            break;
                    }

                    if (!updatable) continue;
                    if (valid == false) UpdateHistory(); //only updates history once you know changes will happen
                    MoveTile(tilePositions[new BoardPos(x, y)], replace, move);
                    valid = true;
                }
            }

            Debug.Log(valid ? "valid right move" : "invalid right move");
            PlaceNewTile(Direction.Right);
        }

        private void MoveTile(Tile mover, Tile replace, bool moveNotMerge)
        {
            Profiler.BeginSample("Move Tile");

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
                Color font, style;
                getStyle(mover.Value * 2, out style, out font);
                mover.MergeTile(replace.GridPosition, replace.UiPosition, movementSpeed, replace, style, font);
            }
            Profiler.EndSample();
        }

        private void UpdateHistory()
        {
            history.Push(BuildSaveState());
        }

        private BoardState BuildSaveState()
        {
            BoardState state = new BoardState(boardSize.X, boardSize.Y);
            state.Score = score;

            foreach (KeyValuePair<BoardPos, Tile> tile in tilePositions)
            {
                int position = (tile.Key.Y * boardSize.X) + tile.Key.X;

                TileState tileState = new TileState();
                tileState.grid.X = tile.Key.X;
                tileState.grid.Y = tile.Key.Y;
                tileState.Points = tile.Value.Value;

                state.tiles[position] = tileState;
            }

            return state;
        }

        public void Back()
        {
            BoardState state = history.Pop();
            if (state == null) return;

            foreach (TileState tile in state.tiles)
            {
                if (tile.Points == -1)
                {
                    tilePositions[new BoardPos(tile.grid.X, tile.grid.Y)].setEmpty(emptyTileColor);
                }
                else
                {
                    TileStyle style = getStyle(tile.Points);
                    tilePositions[new BoardPos(tile.grid.X, tile.grid.Y)].setTile(tile.Points, style.Color,
                        style.SecondaryTextColour ? secondaryTextColour : primaryTextColour, movementSpeed, false);
                }
            }

            if (isEndOfGame())
            {
                SaveSystem.RemoveBoard(boardSize.X, boardSize.Y);
                SaveSystem.SaveScore(score, boardSize.X, boardSize.Y);
                if (GameOver != null) GameOver.gameObject.SetActive(true);
            }
            else
            {
                SaveSystem.SaveBoard(BuildSaveState());
                if (GameOver != null) GameOver.gameObject.SetActive(false);
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

        internal void getStyle(int i, out Color tile, out Color fontColour)
        {
            TileStyle style = getStyle(i);
            fontColour = style.SecondaryTextColour ? secondaryTextColour : primaryTextColour;
            tile = style.Color;
        }

        private void PlaceEmptyTile(BoardPos grid, Vector2 ui)
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
        internal Dictionary<BoardPos, Tile> getBoardRepresentation()
        {
            return tilePositions;
        }

        internal void setBoardRepresentation(Dictionary<BoardPos, Tile> newBoard)
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

        public float DirectionChance { set { edgeSpawn = value; } }
#endif
    }

    [Serializable]
    internal class TileStyle
    {
        public int score = -1;
        public Color Color = Color.black;
        public bool SecondaryTextColour = false;
    }

    internal class BoardState
    {
        public TileState[] tiles;
        public int BoardWidth, BoardHeight, Score;

        public BoardState(int width, int height)
        {
            BoardHeight = height;
            BoardWidth = width;
            Score = 0;

            tiles = new TileState[width * height];
        }
    }

    [Serializable]
    public struct BoardPos
    {
        public int X;
        public int Y;

        public BoardPos(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [Serializable]
    internal class TileState
    {
        public int Points;
        public BoardPos grid;

        public TileState()
        {
            grid = new BoardPos(-1, -1);
            Points = -1;
        }
    }
}