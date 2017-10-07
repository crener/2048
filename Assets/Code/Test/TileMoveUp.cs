using System.Collections;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Test {
    public class TileMoveUp : TileMoverTests
    {

        [UnityTest]
        public IEnumerator Up3x3Verticle2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(0, 1))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(3, 3), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4x4Verticle2Combine()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(2, new BoardPos(0, 3))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4x4Verticle2CombineFloating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(2, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(4, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4x4TopRowFull()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(2, new BoardPos(2, 0)),
                new TileValue(2, new BoardPos(3, 0)),
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(2, new BoardPos(1, 0)),
                new TileValue(2, new BoardPos(2, 0)),
                new TileValue(2, new BoardPos(3, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Up4And2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 2))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0)),
                new TileValue(4, new BoardPos(0, 1))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4With2Floating()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 2)),
                new TileValue(4, new BoardPos(0, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 1)),
                new TileValue(4, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4SingleStay()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, false);
        }

        [UnityTest]
        public IEnumerator Up4SingleMove1()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 1))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4SingleMove3()
        {
            TileValue[] positions = new[]
            {
                new TileValue(2, new BoardPos(0, 3))
            };

            TileValue[] outcome = new[]
            {
                new TileValue(2, new BoardPos(0, 0))
            };

            return UpTests(new BoardSize(4, 4), positions, outcome, true);
        }

        [UnityTest]
        public IEnumerator Up4And2Stay()
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

            return UpTests(new BoardSize(4, 4), positions, outcome, false);
        }
    }
}
