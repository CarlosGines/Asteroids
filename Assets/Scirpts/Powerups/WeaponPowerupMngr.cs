using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class WeaponPowerupMngr : MonoBehaviour {

		public float speed;
		public WeaponType type;
		public int ammo;

		public SpriteRenderer halo;
		public Sprite blueSprite;
		public Sprite yellowSprite;
		public Sprite redSprite;

		private Transform _trans;
		private SpriteRenderer _rend;

		private Vector3 _dir;

		void Awake ()
		{
			_trans = transform;
			_rend = GetComponent<SpriteRenderer> ();
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
			_trans.Translate (_dir  * this.speed * Time.deltaTime);
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
				_rend.sprite = blueSprite;
				this.halo.color = BlueWeaponView.BLUE;
				break;
			case WeaponType.YELLOW:
				this.ammo = 5;
				_rend.sprite = yellowSprite;
				this.halo.color = YellowWeaponView.YELLOW;
				break;
			case WeaponType.RED:
				this.ammo = 15;
				_rend.sprite = redSprite;
				this.halo.color = RedWeaponView.RED;
				break;
			}
			_dir = _trans.right;
			_trans.rotation = Quaternion.identity;
		}
	}
}
