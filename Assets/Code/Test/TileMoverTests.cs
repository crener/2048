using System;
using System.Collections;
using System.Collections.Generic;
using Code.Gameplay;
using Code.Menu;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Code.Gameplay
{
    public class TileMoverTests
    {

        public struct BoardSize
        {
            public int X;
            public int Y;

            public BoardSize(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        public struct TileValue
        {
            public int Val;
            public Vector2 Grid;

            public TileValue(int val, Vector2 grid)
            {
                Val = val;
                Grid = grid;
            }
        }

        protected IEnumerator LeftTests(BoardSize size, TileValue[] positions, TileValue[] expected, bool valid)
        {
            return MoveTests(size, positions, expected, valid, TileMover.Direction.Left);
        }

        protected IEnumerator RightTests(BoardSize size, TileValue[] positions, TileValue[] expected, bool valid)
        {
            return MoveTests(size, positions, expected, valid, TileMover.Direction.Right);
        }

        protected IEnumerator DownTests(BoardSize size, TileValue[] positions, TileValue[] expected, bool valid)
        {
            return MoveTests(size, positions, expected, valid, TileMover.Direction.Down);
        }

        protected IEnumerator UpTests(BoardSize size, TileValue[] positions, TileValue[] expected, bool valid)
        {
            return MoveTests(size, positions, expected, valid, TileMover.Direction.Up);
        }

        private IEnumerator MoveTests(BoardSize size, TileValue[] positions, TileValue[] expected, bool valid, TileMover.Direction dir)
        {
            TileMover mover = SetupBasicField(size);

            yield return null;
            yield return null;

            Dictionary<Vector2, Tile> board = mover.getBoardRepresentation();
            foreach (TileValue position in positions)
                board[position.Grid].setTile(position.Val, Color.blue, Color.black);

            yield return null;
            yield return null;

            LogAssert.Expect(LogType.Log, (valid ? "valid " : "invalid ") + dir.ToString().ToLower() + " move");

            switch (dir)
            {
                case TileMover.Direction.Up:
                    mover.Up();
                    break;
                case TileMover.Direction.Down:
                    mover.Down();
                    break;
                case TileMover.Direction.Left:
                    mover.Left();
                    break;
                case TileMover.Direction.Right:
                    mover.Right();
                    break;
                default:
                    Assert.Fail("Direction was unknown");
                    break;
            }

            yield return null;
            board = mover.getBoardRepresentation();

            foreach (TileValue tileValue in expected)
            {
                Assert.IsTrue(board.ContainsKey(tileValue.Grid), "Expected value could not be found");
                Assert.AreEqual(tileValue.Val, board[tileValue.Grid].Value);
            }
        }

        protected TileMover SetupBasicField(BoardSize size)
        {
            GameObject gameSize = new GameObject("GameSize");
            {
                GameOption settings = gameSize.AddComponent<GameOption>();
                settings.Option = new SizeSelector.GameSize() { X = size.X, Y = size.Y, Name = "test" };
            }

            GameObject root = new GameObject("root");
            {
                root.AddComponent<Canvas>();
            }

            GameObject tile = new GameObject("Tile", new[] { typeof(Tile), typeof(Image) });
            {
                GameObject text = new GameObject("text", new[] { typeof(Text) });
                text.transform.SetParent(tile.transform);

                Tile t = tile.GetComponent<Tile>();
                Image img = tile.GetComponent<Image>();
                Text txt = text.GetComponent<Text>();

                t.setSerials(img, txt);
            }

            GameObject builder = new GameObject("board", new[] { typeof(RectTransform) });
            builder.transform.SetParent(root.transform);
            BoardBuilder bld = builder.AddComponent<BoardBuilder>();
            bld.setPrefab(tile);

            TileMover mover = builder.AddComponent<TileMover>();
            mover.AddStyle(new TileStyle { Color = Color.yellow, score = 2, SecondaryTextColour = false });
            mover.AddStyle(new TileStyle { Color = Color.red, score = 4, SecondaryTextColour = false });
            mover.Score = 1;

            mover.AddUnknownStyle(new TileStyle { Color = Color.black, score = 4, SecondaryTextColour = true});

            return mover;
        }
    }
}
