using UnityEngine;
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
        public Vector2 GridPosition = new Vector2(-1, -1);
        public Vector2 UiPosition
        {
            get { return moving ? end : (Vector2)transform.position; }
        }

        public int Value
        {
            private set { value = value; }
            get { return moving && merging ? value * 2 : value; }
        }
        private int value = -1;


        private Transform trans;
        private bool moving = false;
        private Vector2 start, end;
        private float speed, passed;
        private Tile hidden;

        private bool merging = false;
        private Color imgColour, textColour;

        private void Start()
        {
            trans = transform;
        }

        private void Update()
        {
            if (moving)
            {
                passed += Time.deltaTime;
                Vector2 loc = Vector2.Lerp(start, end, passed / speed);
                transform.position = loc;

                if(passed >= speed)
                {
                    moving = false;
                    ApplyMerge();
                }
            }
        }

        public void setEmpty(Color colour)
        {
            background.color = colour;
            scoreText.text = "";
            value = -1;
        }

        public void setEmpty(Vector2 gridPos, Vector2 worldPos, Color colour)
        {
            setEmpty(colour);
            GridPosition = gridPos;
            transform.position = new Vector3(worldPos.x, worldPos.y, -1f);
        }

        public void setTile(int score, Color colour, Color fontColour)
        {
            background.color = colour;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            scoreText.text = score.ToString();
            scoreText.color = fontColour;
            value = score;
        }

        public void moveTile(Vector2 gridLocation, Vector2 uiLocation, float speed, Tile hide)
        {
            GridPosition = gridLocation;

            if(moving)
            {
                trans.position = end;
                ApplyMerge();
            }

            start = trans.position;
            end = uiLocation;
            this.speed = speed;
            passed = 0f;
            hidden = hide;
            moving = true;
        }

        private void OnDrawGizmosSelected()
        {
            {
                Vector3 loc = UiPosition;
                Debug.DrawLine(loc + Vector3.left, loc + Vector3.right, Color.yellow);
                Debug.DrawLine(loc + Vector3.up, loc + Vector3.down, Color.yellow);
            }
        }

        internal void MergeTile(Vector2 moveGridLocation, Vector2 moveUiLocation, float movementSpeed, Tile replace, Color styleColor, Color textColor)
        {
            GridPosition = moveGridLocation;

            if(moving)
            {
                trans.position = end;
                ApplyMerge();
            }

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

        private void ApplyMerge()
        {
            hidden.gameObject.SetActive(false);
            if (!merging) return;

            merging = false;
            value *= 2;
            background.color = imgColour;
            scoreText.color = textColour;
            scoreText.text = value.ToString();
        }
    }
}
