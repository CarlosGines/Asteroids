using UnityEngine;

namespace CgfGames
{
	public class EntryPoint : MonoBehaviour
	{
		public GameView gameView;
		public ShipView shipView;
		public InputMngr inputMngr;

		// Use this for initialization
		void Start () {
			ShipCtrl shipCtrl = new ShipCtrl (this.shipView);
			this.inputMngr.shipCtrl = shipCtrl;
			GameCtrl gameCtrl = new GameCtrl (
				this.gameView,
				shipCtrl
			);
			gameCtrl.StartGame ();
		}
	}
}
