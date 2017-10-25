using Assets.Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay
{
    public class HighScore : MonoBehaviour
    {

        [SerializeField]
        private TileMover tiles = null;
        [SerializeField]
        private Text highScoreText = null;

        private int high;

        void Start()
        {
            int file = SaveSystem.LoadScore(tiles.boardSize.X, tiles.boardSize.Y);
            high = file > tiles.Score ? file : tiles.Score;

            highScoreText.text = high.ToString();
        }

        void Update()
        {
            if (tiles.Score > high)
            {
                high = tiles.Score;
                highScoreText.text = high.ToString("N0");
            }
        }
    }
}
