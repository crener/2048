using System.Collections;
using System.Collections.Generic;
using Code.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Test
{
    public class TileMoveDragTests : TileMoverTests
    {

        [UnityTest]
        public IEnumerator DownInvalidTileMove()
        {
            TileMover tile = SetupBasicField(new BoardSize(4, 4));
            tile.DirectionChance = 1f;
            yield return null;

            Dictionary<BoardPos, Tile> board = tile.getBoardRepresentation();
            board[new BoardPos(0, 0)].setTile(2, Color.blue, Color.magenta);
            board[new BoardPos(0, 1)].setTile(2, Color.blue, Color.magenta);

            yield return null;

            List<Vector2> positions = new List<Vector2>(4 * 4 + 1);
            foreach (KeyValuePair<BoardPos, Tile> pair in board)
                positions.Add(pair.Value.UiPosition);

            yield return null;
            tile.Down(); //start moving
            for (int i = 0; i < 10; i++)
            {
                yield return null;
                tile.Down(); //break the play field
                yield return null;
            }
            yield return new WaitForSeconds(0.5f); //wait for all tiles to move were they should go

            board = tile.getBoardRepresentation();
            foreach (Vector2 position in positions)
            {
                bool match = false;
                foreach (KeyValuePair<BoardPos, Tile> pos in board)
                    if (pos.Value.UiPosition == position)
                    {
                        match = true;
                        break;
                    }

                Assert.IsTrue(match, "tile positions have shifted for a tile, X=" + position.x + " Y=" + position.y);
            }
        }

        [UnityTest]
        public IEnumerator PrematureGameover()
        {
            TileMover tile = SetupBasicField(new BoardSize(3, 3));
            tile.DirectionChance = 1f;
            yield return null;

            Dictionary<BoardPos, Tile> board = tile.getBoardRepresentation();
            board[new BoardPos(0, 0)].setTile(4, Color.blue, Color.magenta);
            board[new BoardPos(0, 1)].setTile(32, Color.blue, Color.magenta);
            board[new BoardPos(0, 2)].setTile(2, Color.yellow, Color.magenta); //match
            board[new BoardPos(1, 0)].setTile(2, Color.blue, Color.magenta);
            board[new BoardPos(1, 1)].setTile(16, Color.blue, Color.magenta);
            board[new BoardPos(1, 2)].setTile(2, Color.yellow, Color.magenta); //match
            board[new BoardPos(2, 0)].setTile(16, Color.blue, Color.magenta);
            board[new BoardPos(2, 1)].setTile(4, Color.blue, Color.magenta);
            board[new BoardPos(2, 2)].setTile(8, Color.blue, Color.magenta);

            yield return null;

            Assert.IsFalse(tile.isEndOfGame());
        }
    }
}
