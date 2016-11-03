using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class SpaceObjectMngr : MonoBehaviour {

	private Transform _trans;
	public static float height;
	public static float width;
	public event Action OffScreenEvent;

	void Awake ()
	{
		_trans = transform;
		if (height == 0) {
			height = Camera.main.orthographicSize * 2;
			width = height * Screen.width / Screen.height;
		}
	}

	void LateUpdate () {
		bool moved = false;
		if (_trans.position.x > width / 2) {
			_trans.position += Vector3.left * width;
			moved = true;
		} else if (_trans.position.x < -width / 2) {
			_trans.position += Vector3.right * width;
			moved = true;
		} else if (_trans.position.y > height / 2) {
			_trans.position += Vector3.down * height;
			moved = true;
		} else if (_trans.position.y < -height / 2) {
			_trans.position += Vector3.up * height;
			moved = true;
		}
		if (moved && this.OffScreenEvent != null) {
			this.OffScreenEvent ();
		}
	}

	public static Vector3 RandomPos () {
		Vector3 pos = new Vector3 ();
		pos.x = Random.value * width - width / 2;
		pos.y = Random.value * height - height / 2;
		return pos;
	}

	public static Vector3 LateralRandomPos () {
		Vector3 pos = new Vector3 ();
		pos.x = width * (0.5f - Random.Range (0, 2));
		pos.y = Random.value * height - height / 2;
		return pos;
	}
}
