﻿using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface ISaucerCtrl
	{
		int Size { get; }

		event Action<ISaucerCtrl> DestroyedEvent;

		event Action<ISaucerCtrl> GoneEvent;

		void Fire ();

		void ShipDestroyed ();

		void Destroyed ();

		void Gone ();
	}

	[Serializable]
	public class SaucerCtrl : ISaucerCtrl
	{
		#region Constants
		//======================================================================

		public const int SMALL_SAUCER = 0;
		public const int BIG_SAUCER = 1;
		public static readonly float[] SPEED = new float[] {2f, 1f};
		private static readonly float[] SHOT_PERIOD = new float[] {1f, 2f};
		private const float MAX_SHOT_ANGLE_DISTORTION = 45f;
		private const int ACCURACY_LIMIT_POINTS = 40000;

		#endregion

		#region Public fields & properties
		//======================================================================

		public int Size { get; private set; }

		public GameState GameState { get; private set; }
		public SaucerView View { get; private set; }
		public ShipCtrl Ship { get; private set; }

		#endregion

		#region Events
		//======================================================================

		public event Action<ISaucerCtrl> DestroyedEvent;
		public event Action<ISaucerCtrl> GoneEvent;

		#endregion

		#region Init
		//======================================================================

		public SaucerCtrl (GameState gameState, ISaucerView view, 
				IShipCtrl ship, int size)
		{
			this.GameState = gameState;
			this.View = view as SaucerView;
			this.Ship = ship as ShipCtrl;
			this.Size = size;
			this.View.RepeatFire (this.Fire, SHOT_PERIOD [size]);
			this.Ship.DestroyedEvent += this.ShipDestroyed;
			this.View.HitEvent += this.Destroyed;
			this.View.GoneEvent += this.Gone;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Fire ()
		{
			if (this.Ship.IsActive) {
				if (this.Size == BIG_SAUCER) {
					this.View.Fire (Random.Range (0, 360));
				} else {
					this.View.Fire (
						this.DistortShotAngle (this.GetShotAngleSimple ())
					);
				}
			}
		}

		public void ShipDestroyed ()
		{
			this.View.ShipDestroyed ();
		}

		public void Destroyed ()
		{
			this.Ship.DestroyedEvent -= this.ShipDestroyed;
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent (this);
			}
			this.View.Destroyed ();
		}

		public void Gone ()
		{
			this.Ship.DestroyedEvent -= this.ShipDestroyed;
			if (this.GoneEvent != null) {
				this.GoneEvent (this);
			}
		}

		#endregion

		#region Private methods
		//======================================================================

		private float DistortShotAngle (float angle) {
			float maxDistortion = MAX_SHOT_ANGLE_DISTORTION * Mathf.Max (
				(1f -  1f * this.GameState.Score / ACCURACY_LIMIT_POINTS), 0
			);
			float randomDistortion = Random.value * maxDistortion;
			float randomSign = Random.Range (0, 2) * 2 - 1;
			return angle + randomDistortion * randomSign;
		}

		private float GetShotAngleSimple ()
		{
			Vector2 dir = this.Ship.Pos - this.View.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		private float GetShotAngleSmart ()
		{
			float h = SpaceObjectMngr.height;
			float w = SpaceObjectMngr.width;
			float targetX;
			float targetY;
			if (Mathf.Abs (this.Ship.Pos.x - this.View.Pos.x) < w / 2) {
				targetX = this.Ship.Pos.x;
			} else if (this.View.Pos.x > 0) {
				targetX = this.Ship.Pos.x + w;
			} else {
				targetX = this.Ship.Pos.x - w;
			}
			if (Mathf.Abs (this.Ship.Pos.y - this.View.Pos.y) < h / 2) {
				targetY = this.Ship.Pos.y;
			} else if (this.View.Pos.y > 0) {
				targetY = this.Ship.Pos.y + h;
			} else {
				targetY = this.Ship.Pos.y - h;
			}
			Vector2 dir = new Vector2 (targetX, targetY) - this.View.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		#endregion
	}
}
