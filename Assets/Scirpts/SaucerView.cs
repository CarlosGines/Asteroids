using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

public class SaucerView : MonoBehaviour {

	#region Constants
	//======================================================================

	private const float LIVING_TIME = 8f;

	#endregion

	#region Public fields & properties
	//======================================================================

	public float speed;
	public Vector2 Pos { get { return trans.position; } }

	#endregion

	#region Events
	//======================================================================

	public event Action HitEvent;
	public event Action GoneEvent;

	#endregion

	#region External references
	//======================================================================

	public GameObject shotPrefab;

	#endregion

	#region Cached components
	//======================================================================

	private Transform trans;

	#endregion

	#region Private fields
	//======================================================================

	private int _sense;
	private Vector3 _vTranslate;

	#endregion


	#region Unity callbacks
	//======================================================================

	void Awake ()
	{
		this.trans = transform;
		_vTranslate = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		this.trans.Translate (
			(Vector3.right + _vTranslate) * _sense * speed * Time.deltaTime
		);
		if (Mathf.Abs (this.trans.position.x) > SpaceObjectMngr.width / 2) {
			Destroy (gameObject);
			if (this.GoneEvent != null) {
				this.GoneEvent ();
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag ("ShipShot")) {
			Destroy (other.gameObject);
			this.HitEvent ();
		}
	}

	#endregion

	#region Public methods
	//======================================================================

	public void Init (int size)
	{
		this.trans.position = SpaceObjectMngr.LateralRandomPos ();
		this.trans.localScale *= size * 2 + 1;
		_sense = this.trans.position.x > 0 ? -1 : 1;
		StartCoroutine (SetRandomDirection ());
	}

	public void Destroyed ()
	{
		Destroy (gameObject);
	}

	#endregion

	#region Public methods
	//======================================================================

	private IEnumerator SetRandomDirection ()
	{
		// Change 3 direction 3 times
		yield return new WaitForSeconds (SpaceObjectMngr.width / speed / 3);
		_vTranslate = new Vector3 (0, Random.Range (0, 3) - 1, 0);
		StartCoroutine (SetRandomDirection ());
	}

	public void Fire (float angle)
	{
		GameObject shotGo = Instantiate (
			shotPrefab, 
			trans.position,
			Quaternion.Euler (0, 0, angle)
		) as GameObject;
		shotGo.tag = "SaucerShot";
	}

	#endregion
}
