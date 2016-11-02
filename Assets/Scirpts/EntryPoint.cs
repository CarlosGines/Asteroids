using UnityEngine;

namespace CgfGames
{
	public class EntryPoint : MonoBehaviour
	{
		public GameView gameView;
		public ShipView shipView;
		public InputMngr inputMngr;

		public GameCtrl game;

		// Use this for initialization
		void Start () {
			ShipCtrl ship = new ShipCtrl (this.shipView);
			this.inputMngr.ship = ship;
			game = new GameCtrl (this.gameView, ship);
			game.StartGame ();
		}
	}
}
