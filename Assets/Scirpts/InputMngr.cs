using UnityEngine;

namespace CgfGames
{
	public class InputMngr : MonoBehaviour {

		public ShipCtrl ship;

		// Update is called once per frame
		void Update () 
		{
			if (Input.GetButtonDown ("Fire")) {
				ship.Fire ();
			} else if (Input.GetButton ("Fire")) {
				ship.FireHeld ();
			}
			if (Input.GetButtonDown ("Teleport")) {
				ship.Teleport ();
			}
			float h = Input.GetAxis ("Horizontal");
			if (h != 0) {
				ship.Rotate (h);
			}
		}

		void FixedUpdate ()
		{
			if (Input.GetButton ("Thrust")) {
				ship.Thrust ();
			}
		}
	}
}
