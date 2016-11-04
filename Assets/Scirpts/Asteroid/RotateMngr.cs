using UnityEngine;
using System.Collections;

/// <summary>
/// Rotate a game object continuously.
/// </summary>
public class RotateMngr : MonoBehaviour {

	/// <summary>
	/// Cached transform component.
	/// </summary>
	private Transform _trans;

	/// <summary>
	/// Angular speed for rotation.
	/// </summary>
	private float _angSpeed;

	void Awake () {
		// Cache component.
		_trans = transform;
		// Get random angular speed.
		_angSpeed = Random.Range (30, 60) * Mathf.Sign (Random.Range (0, 2) - 0.5f);
	}
	
	void Update () {
		// Rotate
		_trans.Rotate (0, 0, _angSpeed * Time.deltaTime);
	}
}
