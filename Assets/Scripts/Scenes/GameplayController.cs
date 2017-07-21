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
		public float timeScale { get; private set; }

		public const float ENDING_WAITING_TIME = 3;

		[SerializeField]
		private GameWorld m_world;
		[SerializeField]
		private GameplayUI m_interface;
		[SerializeField]
		private ScenesController m_scenesController;
		[SerializeField]
		private Factory m_factory;
		[SerializeField]
		private Map m_map;

		private ShipProperties m_shipProperties = new ShipProperties();
		private GameplayState m_state;
		private GameplayState m_prepauseState;
		private EventDelegate m_update;
		private float m_prePauseTimeScale;

		private Ship ship { get; set; }

		private const float SHIP_PRE_START_SPEED = 4;
		private const float SHIP_START_SPEED = 11;
		private const float SHIP_START_Z = -5;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			GTime.Create();
		}
		private void Start()
		{
			m_world.Init(this);
			Factory.Create(m_factory, m_world, m_interface);

			InitMap();
			InitShip();
			InitWorld();
			InitInterface();

			player = new Player(m_interface, ship);
			StartGame();
		}
		private void InitMap()
		{
			m_map.Init(this);
		}
		private void InitShip()
		{
			ship = Factory.GetShip(ShipType.STANDART);
			ship.properties = m_shipProperties;
			ship.position = new Vector3(0, GameWorld.FLY_HEIGHT, SHIP_START_Z);
		}
		private void InitWorld()
		{
			m_world.map = m_map;
			m_world.ship = ship;
		}
		private void InitInterface()
		{
			m_interface.Init(this);
			m_interface.joystickListener = Move;
			m_interface.onPause += Pause;
		}

		private void FixedUpdate()
		{
			if (m_update != null) m_update();
		}

		private void StartGame()
		{
			state = GameplayState.PLAYING;
		}
		private void WinGameOver()
		{
			Debug.Log("Win!");
		}
		private void LoseGameOver()
		{
			Debug.Log("Lose!");
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

		private void Move(Vector2 direction)
		{
			float sqrSum = Mathf.Pow(direction.x, 2) + Mathf.Pow(direction.y, 2);
			GTime.timeScale = Mathf.Sqrt(sqrSum);

			ship.MoveBy(direction);
		}
		private void MoveShipToStart()
		{
			Vector3 target = new Vector3(0, GameWorld.FLY_HEIGHT, SHIP_START_Z);
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
			m_map.GameplayChange(state);
		}
	}

	public class GTime
	{
		public static float timeScale
		{
			get { return m_timeScale; }
			set { m_timeScale = Mathf.Clamp(value, 0, 1); }
		}
		public static float timeStep
		{
			get { return Time.fixedDeltaTime * timeScale; }
		}

		public static GTime Create()
		{
			if (m_instance != null) return m_instance;
			return new GTime();
		}

		private static GTime m_instance;
		private static float m_timeScale;

		private GTime()
		{
			timeScale = 1;
		}
	}
	public interface IGameplay
	{
		GameplayState state { get; }
		GameWorld world { get; }
		Player player { get; }
	}
}
