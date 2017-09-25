using Code.Menu;
using UnityEngine;

namespace Code.Gameplay {
    public class GameOption : MonoBehaviour
    {
        [SerializeField]
        public SizeSelector.GameSize Option;
        private static bool preInit = false;

        void Start()
        {
            GameOption[] options = GetComponents<GameOption>();

            if(preInit)
            {
                Destroy(this);
            }
        }

        public void SetOption(SizeSelector.GameSize newVal)
        {
            Option = newVal;
            preInit = true;
        }
    }
}