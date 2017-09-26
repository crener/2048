using Code.Menu;
using UnityEngine;

namespace Code.Gameplay
{
    public class GameOption : MonoBehaviour
    {
        [SerializeField]
        public SizeSelector.GameSize Option;
        private static bool preInit = false;
        private static bool selfDestruct = false;

        void Start()
        {
            //preInit will be true by the time that start is called so flip self destruct instead
            if(preInit && selfDestruct) Destroy(gameObject);
            else selfDestruct = true;
        }

        public void SetOption(SizeSelector.GameSize newVal)
        {
            Option = newVal;
            preInit = true;
        }
    }
}