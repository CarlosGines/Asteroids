using UnityEngine;

namespace CgfGames
{
	public class InputMngr : MonoBehaviour {

		public ShipCtrl shipCtrl;

		// Update is called once per frame
		void Update () 
		{
			if (Input.GetButtonDown ("Fire")) {
				shipCtrl.Fire ();
			}
			if (Input.GetButtonDown ("Teleport")) {
				shipCtrl.Teleport ();
			}
			float h = Input.GetAxis ("Horizontal");
			if (h != 0) {
				shipCtrl.Rotate (h);
			}
		}

		void FixedUpdate ()
		{
			if (Input.GetButton ("Thrust")) {
				shipCtrl.Thrust ();
			}
		}
	}
}
