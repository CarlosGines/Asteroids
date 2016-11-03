using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class WeaponPowerupMngr : MonoBehaviour {

		public float speed;
		public WeaponType type;
		public int ammo;
		private Transform trans;
		private SpriteRenderer rend;

		void Awake ()
		{
			this.trans = transform;
			this.rend = GetComponent<SpriteRenderer> ();
		}

		void OnEnable ()
		{
			StartCoroutine (this.TimedDisable ());
		}

		private IEnumerator TimedDisable ()
		{
			yield return new WaitForSeconds (10);
			gameObject.SetActive (false);
		}

		void Update ()
		{
			this.trans.Translate (this.speed * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("Ship")) {
				other.GetComponent<ShipView> ().NewWeapon (this.type, this.ammo);
				gameObject.SetActive (false);
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}

		public void Init (WeaponType type)
		{
			this.type = type;
			switch (this.type) {
			case WeaponType.BLUE:
				this.ammo = 50;
				this.rend.color = Color.blue;
				break;
			case WeaponType.YELLOW:
				this.ammo = 5;
				this.rend.color = Color.yellow;
				break;
			case WeaponType.RED:
				this.ammo = 15;
				this.rend.color = Color.red;
				break;
			}
		}
	}
}
