using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        [SerializeField, Tooltip("Time (seconds) it takes to complete one move accross the board")]
        private float MovementSpeed = 0.2f;

        public int score { get; set; }
        public Vector2 boardSize { get; set; }

        private Dictionary<Vector2, Tile> tilePositions = new Dictionary<Vector2, Tile>();

        void Start()
        {
            if(tilePositions.Count <= 0)
            {
                Tile[] tiles = GetComponentsInChildren<Tile>();
                if(tiles.Length == 0)
                {
                    Debug.LogError("No Tiles found!!");
                    return;
                }
                
                foreach(Tile tile in tiles)
                    tilePositions.Add(tile.GridPosition, tile);
            }

            Styles.Sort((TileStyle a, TileStyle b) => a.score.CompareTo(b.score));

            //set the initial tile to a state (If loading save game this doesn't matter)
            if(score == 0)
            {
                //find a tile around the edge of the board
                Tile edge1 = findEmptyEdgeTile();
                SpawnTile(edge1, true);
                Tile edge2 = findEmptyEdgeTile();
                SpawnTile(edge2, true);
            }
        }

        private Tile findEmptyEdgeTile()
        {
            Tile edge = null;
            do
            {
                int x = (int) Random.Range(0, boardSize.x);
                int y = 0;

                if(x > 0 && x < boardSize.x)
                {
                    //X is in the middle of the grid to Y is limited to top or bottom
                    bool top = Random.Range(0, 1) == 1;
                    y = top ? (int) boardSize.x : 0;
                }
                else if(x == 0 || x == boardSize.x)
                    y = (int) Random.Range(0, boardSize.y);
                else Debug.LogError("Unknown generation state! x = " + x);

                if(!tilePositions.ContainsKey(new Vector2(x, y)))
                {
                    Debug.LogWarning("Incorrect tile fetch attempt! x = " + x + ", y = " + y);
                    continue;
                }
                Tile potentialTile = tilePositions[new Vector2(x, y)];
                if(potentialTile.Value != -1) continue;

                edge = potentialTile;
            } while(edge == null);

            return edge;
        }

        public void PlaceNewTile(Direction previousDirection) { }

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
            TileStyle style = forceLow ? Styles[0] : Styles[Random.Range(0, 1)];
            tile.setTile(style.score, style.Color, style.SecondaryTextColour ? SecondaryTextColour : primaryTextColour);
        }

        public void Up() { }

        //shows what pairs would match up via a given swipe
        private void OnDrawGizmos() { }

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