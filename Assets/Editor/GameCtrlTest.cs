using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

namespace CgfGames
{
	[TestFixture]
	[Category("Asteroids Tests")]
	public class GameCtrlTest
	{
		//======================================================================

		[Test]
		public void CanSpawnAsteroids ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);

			game.SpawnAsteroid ();

			Assert.AreEqual (game.AsteroidList.Count, 1);

			game.SpawnAsteroid ();

			Assert.AreEqual (game.AsteroidList.Count, 2);
		}

		//======================================================================

		[Test]
		public void CanSpawnSaucers ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);

			game.SpawnSaucer ();

			Assert.NotNull (game.Saucer);
		}

		//======================================================================

		[Test]
		public void CanEarnPointsDestroyingAsteroids ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			var asteroid = Substitute.For<IAsteroidCtrl> ();
			asteroid.Size.Returns (AsteroidCtrl.MAX_SIZE);

			game.AsteroidDestroyed (asteroid, null);

			Assert.AreEqual (
				game.GameState.Score,
				GameCtrl.ASTEROIDS_POINTS[AsteroidCtrl.MAX_SIZE]
			);
		}

		//======================================================================

		[Test]
		public void CanEarnPointsDestroyingSaucers ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			var saucer = Substitute.For<ISaucerCtrl> ();
			saucer.Size.Returns (SaucerCtrl.SMALL_SAUCER);

			game.SaucerDestroyed (saucer);

			Assert.AreEqual (
				game.GameState.Score, 
				GameCtrl.SAUCERS_POINTS[SaucerCtrl.SMALL_SAUCER]
			);
		}

		//======================================================================

		[Test]
		public void CanEarnLivesWithPoints ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			int initLives = game.GameState.Lives;

			game.ScoreUpdated (0, GameCtrl.LIFE_POINTS);

			Assert.AreEqual (game.GameState.Lives, initLives + 1);
		}

		//======================================================================

		[Test]
		public void CanLoseLives ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			int initLives = game.GameState.Lives;

			game.ShipDestroyed ();

			Assert.AreEqual (game.GameState.Lives, initLives - 1);
		}

		//======================================================================

		[Test]
		public void CanRespawnShip ()
		{
			var gameView = Substitute.For<IGameView> ();
			var ship = Substitute.For<IShipCtrl> ();
			gameView.WaitToRespawnShip (Arg.Invoke ());
			GameCtrl game = new GameCtrl (gameView, ship);

			game.ShipDestroyed ();

			ship.Received (1).Respawn ();
		}

		//======================================================================

		[Test]
		public void CanGameOver ()
		{
			var gameView = Substitute.For<IGameView> ();
			GameCtrl game = new GameCtrl (
				new GameState (), gameView, Substitute.For<IShipCtrl> ()
			);

			game.ShipDestroyed ();

			gameView.Received (1).GameOver ();
		}

		//======================================================================
	}
}
