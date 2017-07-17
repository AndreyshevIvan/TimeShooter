using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyGame.Hero;
using GameFactory;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Controllers;
using GameUtils;

namespace MyGame
{
	public sealed class GameplayController : MonoBehaviour, IGameplay
	{
		public GameplayState state
		{
			get { return m_state; }
			private set
			{
				m_state = value;
				NotifyObjects();
			}
		}
		public GameWorld world { get { return m_world; } }
		public Player player { get; private set; }

		private Ship ship { get; set; }
		private Map map { get; set; }

		[SerializeField]
		private GameWorld m_world;
		[SerializeField]
		private GameplayUI m_interface;
		[SerializeField]
		private ScenesController m_scenesController;
		[SerializeField]
		private Factories m_factory;

		private ShipProperties m_shipProperties = new ShipProperties();
		private GameplayState m_state;
		private GameplayState m_prepauseState;
		private float m_prePauseTimeScale;
		private EventDelegate m_update;

		private const float SHIP_PRE_START_SPEED = 4;
		private const float SHIP_START_SPEED = 11;
		public const float ENDING_WAITING_TIME = 3;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			player = new Player(m_interface, ship);
		}
		private void Start()
		{
			m_world.Init(this);

			InitFactory();
			InitMap();
			InitShip();
			InitWorld();
			InitInterface();

			state = GameplayState.BEFORE_START;
		}
		private void InitFactory()
		{
			m_factory.world = m_world;
			m_world.factory = m_factory;
		}
		private void InitMap()
		{
			map = m_factory.GetMap(MapType.FIRST);
			map.Init(this);
			map.gameplay = this;
			map.factory = m_factory;
		}
		private void InitShip()
		{
			ship = m_factory.GetShip(ShipType.STANDART);
			ship.properties = m_shipProperties;

			ship.roadController.Spline = m_factory.GetRoad(RoadType.PLAYER_START);
			ship.roadController.Clamping = CurvyClamping.Loop;
			ship.roadController.PlayAutomatically = true;
			ship.roadController.Speed = SHIP_PRE_START_SPEED;
		}
		private void InitWorld()
		{
			m_world.map = map;
			m_world.ship = ship;
		}
		private void InitInterface()
		{
			m_interface.Init(this);
			m_interface.onPause += Pause;
			m_interface.moveShip += MoveShip;
			m_interface.onChangeMode += isTrue => m_world.SetSlowMode(isTrue);
			m_interface.startTouchEvents += () =>
			{
				ship.roadController.Spline = null;
				m_update += MoveShipToStart;
			};
		}

		private void FixedUpdate()
		{
			if (m_update != null) m_update();
			WaitEndOfGame();
		}
		private void WaitEndOfGame()
		{
			if (state == GameplayState.PLAYING &&
				map.isReached &&
				world.isAllEnemiesKilled &&
				ship.isLive)
			{
				GameOver();
			}
		}

		private void StartGame()
		{
			map.Play();
			state = GameplayState.PLAYING;
			Destroy(ship.roadController);
		}
		private void GameOver()
		{
		}
		private void Pause(bool isPause)
		{
			if (isPause)
			{
				m_prePauseTimeScale = Time.timeScale;
				Time.timeScale = 0;
				m_prepauseState = state;
				return;
			}

			Time.timeScale = m_prePauseTimeScale;
			state = m_prepauseState;
		}

		private void SaveUserData()
		{
		}
		private void MoveShip(Vector3 targetPosition)
		{
			if (ship) ship.MoveTo(targetPosition);
		}
		private void MoveShipToStart()
		{
			Vector3 target = new Vector3(0, GameWorld.FLY_HEIGHT, 0);
			float movement = SHIP_START_SPEED * Time.fixedDeltaTime;
			ship.position = Vector3.MoveTowards(ship.position, target, movement);
			if (ship.position == target)
			{
				StartGame();
				m_update -= MoveShipToStart;
			}
		}
		private void NotifyObjects()
		{
			m_world.GameplayChange(state);
			m_interface.GameplayChange(state);
			map.GameplayChange(state);
			Debug.Log(state);
		}
	}

	public interface IGameplay
	{
		GameplayState state { get; }
		GameWorld world { get; }
		Player player { get; }
	}
}
