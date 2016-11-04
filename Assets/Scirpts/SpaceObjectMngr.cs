using UnityEngine;
using System;

using Random = UnityEngine.Random;

/// <summary>
/// Behaviour for an object that moves within our specific space, coupled 
/// to screen space.
/// </summary>
public class SpaceObjectMngr : MonoBehaviour
{


	#region Public fields & properties
	//======================================================================

	/// <summary>
	/// Screen height.
	/// </summary>
	public static float height;

	/// <summary>
	/// Screen width.
	/// </summary>
	public static float width;

	#endregion

	#region Events
	//======================================================================

	/// <summary>
	/// Occurs when this object leaves screen space.
	/// </summary>
	public event Action OffScreenEvent;

	#endregion

	#region Cached components
	//======================================================================

	/// <summary>
	/// Transform component cached.
	/// </summary>
	private Transform _trans;

	#endregion

	#region Unity callbacks
	//======================================================================

	void Awake ()
	{
		// Cache components.
		_trans = transform;
		// Init static fields.
		if (height == 0) {
			height = Camera.main.orthographicSize * 2;
			width = height * Screen.width / Screen.height;
		}
	}

	void LateUpdate () {
		// Move object if it leaves screen space so that it reappears from
		// the opposite side.
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
		// Notify object off the screen.
		if (moved && this.OffScreenEvent != null) {
			this.OffScreenEvent ();
		}
	}

	#endregion

	#region Public methods
	//======================================================================

	/// <summary>
	/// Gets a random position within screen space.
	/// </summary>
	/// <returns>The random position.</returns>
	public static Vector3 RandomPos () {
		Vector3 pos = new Vector3 ();
		pos.x = Random.value * width - width / 2;
		pos.y = Random.value * height - height / 2;
		return pos;
	}

	/// <summary>
	/// Gets a random position within screen horizontal sides.
	/// </summary>
	/// <returns>The random position.</returns>
	public static Vector3 LateralRandomPos () {
		Vector3 pos = new Vector3 ();
		pos.x = width * (0.5f - Random.Range (0, 2));
		pos.y = Random.value * height - height / 2;
		return pos;
	}

	/// <summary>
	/// Get the shortest path between to points taking into account
	/// off screen reappearance.
	/// </summary>
	/// <returns>The shortest path.</returns>
	/// <param name="start">Start point.</param>
	/// <param name="end">End point.</param>
	public static Vector2 SmartPath (Vector2 start, Vector2 end)
	{
		float h = SpaceObjectMngr.height;
		float w = SpaceObjectMngr.width;
		// Realculate target end point as though it were off screen.
		float targetX;
		float targetY;
		if (Mathf.Abs (end.x - start.x) < w / 2) {
			targetX = end.x;
		} else if (start.x > 0) {
			targetX = end.x + w;
		} else {
			targetX = end.x - w;
		}
		if (Mathf.Abs (end.y - start.y) < h / 2) {
			targetY = end.y;
		} else if (start.y > 0) {
			targetY = end.y + h;
		} else {
			targetY = end.y - h;
		}
		// Return smart path from start point to recalculated end point.
		return new Vector2 (targetX, targetY) - start;
	}

	#endregion
}
