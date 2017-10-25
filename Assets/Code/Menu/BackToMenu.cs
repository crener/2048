using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Menu {
    public class BackToMenu : MonoBehaviour {

        void Update()
        {
            if(Input.GetButtonDown("back"))
            {
                SceneManager.LoadScene("Main menu");
            }
        }

    }
}
