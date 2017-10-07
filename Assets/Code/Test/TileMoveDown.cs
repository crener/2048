using System.Collections;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Test
{
    public class TileMoveDown : TileMoverTests
    {

        [UnityTest]
        public IEnumerator Down3x3Verticle2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(2, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 2))
            };

            return DownTests(new BoardSize(3, 3), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4x4Verticle2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(2, new BoardPos(0, 3))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4x4Verticle2CombineFloating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(2, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4x4BottomRowFull()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 3)),
                new TileValue(2, new BoardPos(1, 3)),
                new TileValue(2, new BoardPos(2, 3)),
                new TileValue(2, new BoardPos(3, 3)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 3)),
                new TileValue(2, new BoardPos(1, 3)),
                new TileValue(2, new BoardPos(2, 3)),
                new TileValue(2, new BoardPos(3, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Down4And2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(4, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4With2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 3))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(4, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4SingleStay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 3))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Down4SingleMove1()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4SingleMove3()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 3))
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Down4And2Stay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(4, new BoardPos(0, 3)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(4, new BoardPos(0, 3)),
            };

            return DownTests(new BoardSize(4, 4), positions, outcome, false);
        }
    }
}
