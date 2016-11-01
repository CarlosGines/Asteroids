using UnityEngine;

public class SpaceObjectMngr : MonoBehaviour {

	private Transform _trans;
	public static float height;
	public static float width;

	void Awake ()
	{
		_trans = transform;
		if (height == 0) {
			height = Camera.main.orthographicSize * 2;
			width = height * Screen.width / Screen.height;
		}
	}

	void LateUpdate () {
		if (_trans.position.x > width / 2) {
			_trans.position += Vector3.left * width;
		} else if (_trans.position.x < -width / 2) {
			_trans.position += Vector3.right * width;
		} else if (_trans.position.y > height / 2) {
			_trans.position += Vector3.down * height;
		} else if (_trans.position.y < -height / 2) {
			_trans.position += Vector3.up * height;
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
