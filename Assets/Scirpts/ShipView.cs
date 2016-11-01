using UnityEngine;
using System;
using System.Collections;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class ShipView : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const float TELEPORT_TIME = 2f;

		#endregion

		#region Public fields & properties
		//======================================================================

		public float angSpeed;
		public float thrustForce;
		public Vector2 Pos { get { return trans.position; } }

		#endregion

		#region Events
		//======================================================================

		public event Action HitEvent;

		#endregion

		#region External references
		//======================================================================
	
		public GameObject shotPrefab;

		#endregion

		#region Cached components
		//======================================================================

		private Transform trans;
		private Rigidbody2D rb;
		private Renderer rend;
		private Collider2D col;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			this.trans = transform;
			this.rb = GetComponent<Rigidbody2D> ();
			this.rend = GetComponent<Renderer> ();
			this.col = GetComponent<Collider2D> ();
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("Asteroid") || other.CompareTag ("Saucer") 
//					|| other.CompareTag ("SaucerShot")
			) {
				if (this.HitEvent != null) {
					this.HitEvent ();
				}
			}
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Rotate (float direction)
		{
			this.trans.Rotate (0, 0, angSpeed * Time.deltaTime * -direction);
		}

		public void Thrust ()
		{
			this.rb.AddForce (trans.right * thrustForce);
		}

		public void Fire ()
		{
			Instantiate (shotPrefab, trans.position, trans.rotation);
		}

		public void Teleport (Action teleportDone)
		{
			StartCoroutine (Teleport2 (teleportDone));
		}

		public IEnumerator Teleport2 (Action teleportDone)
		{
			this.rb.Sleep ();
			this.rend.enabled = false;
			this.col.enabled = false;
			yield return new WaitForSeconds (TELEPORT_TIME);
			this.trans.position = SpaceObjectMngr.RandomPos ();
			this.rend.enabled = true;
			this.col.enabled = true;
			teleportDone.Invoke ();
		}

		public void Destroyed ()
		{
			gameObject.SetActive (false);
		}

		public void Respawn ()
		{
			this.trans.position = Vector3.zero;
			gameObject.SetActive (true);
		}

		#endregion
	}
}