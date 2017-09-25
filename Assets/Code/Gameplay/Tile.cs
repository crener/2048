using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay {
	public class Tile : MonoBehaviour
	{

		[SerializeField]
		private Text scoreText;
		[SerializeField]
		private Image background;
	
		public Vector2 GridPosition = new Vector2(-1, -1);
		public int Value
		{
			private set { value = value; }
			get { return value; }
		}
		private int value = -1;

		public void setEmpty(Color colour)
		{
			background.color = colour;
			scoreText.text = "";
			value = -1;
		}

		public void setTile(int score, Color colour, Color fontColour)
		{
			background.color = colour;
			scoreText.text = score.ToString();
			scoreText.color = fontColour;
			value = score;
		}
	}
}
