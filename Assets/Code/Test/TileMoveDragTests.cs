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

            Dictionary<Vector2, Tile> board = tile.getBoardRepresentation();
            board[new Vector2(0, 0)].setTile(2, Color.blue, Color.magenta);
            board[new Vector2(0, 1)].setTile(2, Color.blue, Color.magenta);

            yield return null;

            List<Vector2> positions = new List<Vector2>(4 * 4 + 1);
            foreach (KeyValuePair<Vector2, Tile> pair in board)
                positions.Add(pair.Value.UiPosition);

            yield return null;
            tile.Down(); //start moving
            for (int i = 0; i < 10; i++)
            {
                yield return null;
                tile.Down(); //break the play field
            }
            yield return new WaitForSeconds(0.5f); //wait for all tiles to move were they should go

            board = tile.getBoardRepresentation();
            foreach (Vector2 position in positions)
            {
                bool match = false;
                foreach (KeyValuePair<Vector2, Tile> pos in board)
                    if (pos.Value.UiPosition == position)
                    {
                        match = true;
                        break;
                    }

                Assert.IsTrue(match, "tile positions have shifted for a tile, X=" + position.x + " Y=" + position.y);
            }
        }
    }
}
