using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

namespace CgfGames
{
	[TestFixture]
	[Category("Asteroids Tests")]
	public class ShipCtrlTest
	{
		//======================================================================

		[Test]
		public void CanBeDestoyedAndRespawned ()
		{
			ShipCtrl ship = new ShipCtrl (Substitute.For<IShipView> ());

			ship.Destroyed ();

			Assert.False (ship.IsAlive);
			Assert.False (ship.IsActive);

			ship.Respawn ();

			Assert.True (ship.IsAlive);
			Assert.True (ship.IsActive);
		}

		//======================================================================

		[Test]
		public void CanBeInactiveWhileTeleporting ()
		{
			var view = Substitute.For<IShipView> ();
			Action teleportDone = null;
			view.Teleport (Arg.Do<Action> (x => teleportDone = x));
			ShipCtrl ship = new ShipCtrl (view);

			ship.Teleport ();

			Assert.True (ship.IsAlive);
			Assert.False (ship.IsActive);

			teleportDone ();

			Assert.True (ship.IsAlive);
			Assert.True (ship.IsActive);
		}

		//======================================================================

		[Test]
		public void CanRaiseDestroyedEvent ()
		{
			ShipCtrl ship = new ShipCtrl (Substitute.For<IShipView> ());
			bool raised = false;
			ship.DestroyedEvent += () => raised = true;

			ship.Destroyed ();

			Assert.IsTrue (raised);
		}

		//======================================================================

		[Test]
		public void CannotReactWhileInactive ()
		{
			var view = Substitute.For<IShipView> ();
			ShipCtrl ship = new ShipCtrl (view);

			ship.Destroyed ();
			ship.Fire ();
			ship.Thrust ();
			ship.Rotate (0);
			ship.Teleport ();

			ship.CurrentWeapon.DidNotReceive ().Fire ();
			view.DidNotReceive ().Thrust ();
			view.DidNotReceive ().Rotate (0);
			view.DidNotReceive ().Teleport (Arg.Any<Action> ());

			ship.Respawn ();
			ship.Thrust ();
			ship.Rotate (0);
			ship.Fire ();
			ship.Teleport ();

			ship.CurrentWeapon.Received (1).Fire ();
			view.Received (1).Thrust ();
			view.Received (1).Rotate (0);
			view.Received (1).Teleport (Arg.Any<Action> ());
		}

		//======================================================================
	}
}
