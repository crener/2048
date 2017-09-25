using System;
using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Menu
{
    public class SizeSelector : MonoBehaviour
    {
        [SerializeField]
        private List<GameSize> boards = new List<GameSize>();
        [SerializeField]
        private Image boardImg;
        [SerializeField]
        private Text boardText;

        private int boardPos = -1;

        void Start()
        {
            boards.Sort(new GameSize());

            for(int i = 0; i < boards.Count; i++)
            {
                GameSize board = boards[i];
                if(board.first)
                {
                    boardPos = i;
                    return;
                }
            }
        }

        public void MoveLeft()
        {
            if(boardPos <= 0) return;
            ShowNextBoard(boards[--boardPos]);
        }


        public void MoveRight()
        {
            if(boardPos + 1 >= boards.Count) return;
            ShowNextBoard(boards[++boardPos]);
        }

        public void Play()
        {
            //create a new gameobject to tell the game scene how it should play
            GameObject info = new GameObject();
            DontDestroyOnLoad(info);

            info.name = "GameSize";
            GameOption option = info.AddComponent<GameOption>();
            option.SetOption(boards[boardPos]);
            
            SceneManager.LoadScene("Game");
        }

        private void ShowNextBoard(GameSize newSize)
        {
            String newText = "";
            if(String.IsNullOrEmpty(newSize.Name))
                newText = newSize.Name + " (" + newSize.X + "x" + newSize.Y + ")";
            else newText = newSize.X + " x " + newSize.Y;

            boardText.text = newText;
            boardImg.sprite = newSize.SourceImage;
        }

        [Serializable]
        public class GameSize : IComparer<GameSize>
        {
            public Sprite SourceImage;
            public String Name = "";
            public int X = 0;
            public int Y = 0;
            public bool first = false; //makes this the first to show up when game is loaded

            public int Compare(GameSize x, GameSize y)
            {
                int xSize = x.X * x.Y;
                int ySize = y.X * y.Y;

                return xSize.CompareTo(ySize);
            }
        }
    }
}