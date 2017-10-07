using System.Collections;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Test
{
    public class TileMoveLeft : TileMoverTests
    {
        [UnityTest]
        public IEnumerator Left3x3Hotizontal2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(2, new BoardPos(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(3, 3), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4x4Hotizontal2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(1, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4x4Hotizontal2CombineFloating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(2, new BoardPos(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4x4LeftRowFull()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(2, new BoardPos(0, 3)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(2, new BoardPos(0, 3))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Left4And2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(2, 0)),
                new TileValue(4, new BoardPos(1, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(4, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4With2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(2, 0)),
                new TileValue(4, new BoardPos(0, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(4, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4SingleStay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Left4SingleMove1()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4SingleMove3()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(3, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Left4And2Stay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 0)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 0)),
            };

            return LeftTests(new BoardSize(4, 4), positions, outcome, false);
        }
    }
}
