using System.Collections;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Test
{
    public class TileMoveRight : TileMoverTests
    {
        [UnityTest]
        public IEnumerator Right3x3Hotizontal2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(1, 0)),
                new TileValue(2, new Vector2(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new Vector2(2, 0))
            };

            return RightTests(new BoardSize(3, 3), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4x4Hotizontal2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(2, 0)),
                new TileValue(2, new Vector2(3, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4x4Hotizontal2CombineFloating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(1, 0)),
                new TileValue(2, new Vector2(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4x4RightRowFull()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(3, 0)),
                new TileValue(2, new Vector2(3, 1)),
                new TileValue(2, new Vector2(3, 2)),
                new TileValue(2, new Vector2(3, 3)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(3, 0)),
                new TileValue(2, new Vector2(3, 1)),
                new TileValue(2, new Vector2(3, 2)),
                new TileValue(2, new Vector2(3, 3))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Right4And2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(1, 0)),
                new TileValue(4, new Vector2(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(2, 0)),
                new TileValue(4, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4With2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(1, 0)),
                new TileValue(4, new Vector2(3, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(2, 0)),
                new TileValue(4, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4SingleStay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(3, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Right4SingleMove1()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(2, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4SingleMove3()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(1, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(3, 0))
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Right4And2Stay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new Vector2(2, 0)),
                new TileValue(4, new Vector2(3, 0)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new Vector2(2, 0)),
                new TileValue(4, new Vector2(3, 0)),
            };

            return RightTests(new BoardSize(4, 4), positions, outcome, false);
        }
    }
}
