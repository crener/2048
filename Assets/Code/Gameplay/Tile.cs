using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Code.Gameplay
{
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Text scoreText;
        [SerializeField]
        private Image background;

        [HideInInspector]
        public BoardPos GridPosition = new BoardPos(-1, -1);
        public Vector2 UiPosition
        {
            get { return moving ? end : (Vector2)transform.position; }
        }

        public int Value
        {
            private set { this.value = value; }
            get
            {
                if (moving && merging)
                {
                    if (value == -1) return -1;
                    else return value * 2;
                }
                else return value;
            }
        }
        private int value = -1;


        private Transform trans;
        private bool moving = false;
        private Vector2 start, end;
        private float speed, passed;
        private Tile hidden;

        private bool merging = false;
        private Color imgColour, textColour;

        private bool spawning = false;

        private void Start()
        {
            trans = transform;
        }

        private void Update()
        {
            if (moving)
            {
                if (spawning) FinishSpawning();

                passed += Time.deltaTime;
                Vector2 loc = Vector2.Lerp(start, end, passed / speed);
                trans.position = loc;

                if (passed >= speed) FinishMoving();
            }
            else if (spawning)
            {
                passed += Time.deltaTime;
                Vector3 size = Vector3.Lerp(Vector3.zero, Vector3.one, passed / speed);
                trans.localScale = size;

                if (passed >= speed) FinishSpawning();
            }
        }

        public void setEmpty(Color colour)
        {
            background.color = colour;
            scoreText.text = "";
            value = -1;
        }

        public void setEmpty(BoardPos gridPos, Vector2 worldPos, Color colour)
        {
            setEmpty(colour);
            GridPosition = gridPos;
            transform.position = new Vector3(worldPos.x, worldPos.y, -1f);

            moving = false;
            merging = false;
            FinishSpawning();
        }

        public void setTile(int score, Color colour, Color fontColour, float spawnSpeed = 0.2f, bool spawn = true)
        {
            background.color = colour;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            scoreText.text = score.ToString();
            scoreText.color = fontColour;
            value = score;

            moving = false;
            merging = false;
            if (spawn)
            {
                spawning = true;
                passed = 0f;
                speed = spawnSpeed;
            }
            else FinishSpawning();
        }

        public void MoveTile(BoardPos gridLocation, Vector2 uiLocation, float speed, Tile hide)
        {
            GridPosition = gridLocation;

            FinishMoving();
            FinishSpawning();

            start = trans.position;
            end = uiLocation;
            this.speed = speed;
            passed = 0f;
            hidden = hide;
            moving = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (moving)
                Debug.DrawLine(start, end, merging ? Color.magenta : Color.green);
        }

        internal void MergeTile(BoardPos moveGridLocation, Vector2 moveUiLocation, float movementSpeed, Tile replace, Color styleColor, Color textColor)
        {
            GridPosition = moveGridLocation;

            FinishMoving();
            FinishSpawning();

            start = trans.position;
            end = moveUiLocation;
            speed = movementSpeed;
            passed = 0f;
            hidden = replace;
            imgColour = styleColor;
            textColour = textColor;

            moving = true;
            merging = true;
        }

        private void FinishMoving()
        {
            if (moving)
            {
                trans.position = end;
                passed = 0f;
                moving = false;

                if (hidden != null) hidden.PreShutDown();
                hidden = null;

                if (merging)
                {
                    merging = false;
                    if (value == -1)
                    {
                        background.color = Color.red;
                        Debug.LogError("-1 Value!!!");
                        return;
                    }

                    value *= 2;
                    background.color = imgColour;
                    scoreText.color = textColour;
                    scoreText.text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Call to make sure all tiles properly hide themselves
        /// </summary>
        private void PreShutDown()
        {
            if (hidden != null) hidden.PreShutDown();
            hidden = null;

            gameObject.SetActive(false);
        }

        private void FinishSpawning()
        {
            if (!spawning) return;

            trans.localScale = Vector3.one;
            spawning = false;
            passed = 0f;
        }

#if ENABLE_PLAYMODE_TESTS_RUNNER
        /// <summary>
        /// TEST ONLY sets the values which are usually set by Unity
        /// </summary>
        /// <param name="img"></param>
        /// <param name="txt"></param>
        internal void setSerials(Image img, Text txt)
        {
            background = img;
            scoreText = txt;
        }
#endif
    }
}
