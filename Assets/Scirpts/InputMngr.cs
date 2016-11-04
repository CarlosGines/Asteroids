using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace CgfGames
{
	/// <summary>
	/// Input manager. Recevies all inputs and sends it to controllers.
	/// </summary>
	public class InputMngr : MonoBehaviour {

		/// <summary>
		/// The ship controller.
		/// </summary>
		public ShipCtrl ship;

		#if UNITY_IOS || UNITY_ANDROID

		void Update () 
		{
			// Send input to ship from mobile controllers.
			if (CrossPlatformInputManager.GetButtonDown ("Fire")) {
				ship.Fire ();
			} else if (CrossPlatformInputManager.GetButton ("Fire")) {
				ship.FireHeld ();
			}
			if (CrossPlatformInputManager.GetButtonDown ("Teleport")) {
				ship.Teleport ();
			}

			Vector2 dir = new Vector2 (
				CrossPlatformInputManager.GetAxis ("Horizontal"),
				CrossPlatformInputManager.GetAxis ("Vertical")
			);
			if (dir != Vector2.zero) {
				ship.Rotate (dir);
			}
		}

		#else

		void Update () 
		{
			// Send input to ship from desktop controllers.
			if (Input.GetButtonDown ("Fire")) {
				this.ship.Fire ();
			} else if (Input.GetButton ("Fire")) {
				this.ship.FireHeld ();
			}
			if (Input.GetButtonDown ("Teleport")) {
				this.ship.Teleport ();
			}
			Input.GetAxis ("Horizontal");
			float h = Input.GetAxis ("Horizontal");
			if (h != 0) {
				this.ship.Rotate (h);
			}
		}

		#endif

		void FixedUpdate ()
		{
			// Send physics related input to ship.
			if (CrossPlatformInputManager.GetButton ("Thrust")) {
				this.ship.Thrust ();
			}
		}
	}
}
