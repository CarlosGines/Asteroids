using UnityEngine;

namespace CgfGames
{
	/// <summary>
	/// Entry point for the controllers logic to start.
	/// </summary>
	public class EntryPoint : MonoBehaviour
	{
		#region External references
		//======================================================================

		/// <summary>
		/// The input manager.
		/// </summary>
		public InputMngr inputMngr;

		/// <summary>
		/// The game view.
		/// </summary>
		public GameView gameView;

		/// <summary>
		/// The ship view.
		/// </summary>
		public ShipView shipView;

		/// <summary>
		/// Game controller made public, so that it is exposed to inspector
		/// and serialized.
		/// </summary>
		public GameCtrl game;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Start () {
			// Instantiate controllers.
			ShipCtrl ship = new ShipCtrl (this.shipView);
			this.inputMngr.ship = ship;
			game = new GameCtrl (this.gameView, ship);

			// Start everything.
			game.StartGame ();
		}

		#endregion
	}
}
