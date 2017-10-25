using Assets.Code.Gameplay;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Menu
{
    public class BackToMenu : MonoBehaviour
    {
        [SerializeField]
        private TileMover mover;

        void Update()
        {
            if (Input.GetButtonDown("back"))
            {
                if (mover.isEndOfGame()) SaveSystem.RemoveBoard(mover.boardSize.X, mover.boardSize.Y);
                else mover.Save();

                SceneManager.LoadScene("Main menu");
            }
        }

    }
}
