using UnityEngine;
using System.Collections;

public class RotateMngr : MonoBehaviour {

	private Transform _trans;
	private float _angSpeed;

	// Use this for initialization
	void Awake () {
		_trans = transform;
		_angSpeed = Random.Range (30, 60) * Mathf.Sign (Random.Range (0, 2) - 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		_trans.Rotate (0, 0, _angSpeed * Time.deltaTime);
	}
}
