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
using UnityEngine.UI;

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
		public World world { get { return m_world; } }
		public Player player { get; private set; }
		public float clearProgress
		{
			get { return m_clearProgress; }
			private set { m_clearProgress = Mathf.Clamp01(value); }
		}

		public const float PLAYER_DEPTH = -6;

		[SerializeField]
		private World m_world;
		[SerializeField]
		private GameplayUI m_interface;
		[SerializeField]
		private ScenesController m_scenesController;
		[SerializeField]
		private Factory m_factory;

		private Ship m_ship;
		private ShipProperties m_shipProperties = new ShipProperties();
		private GameplayState m_state;
		private GameplayState m_prepauseState;
		private Vector2 m_direction;
		private float m_clearProgress = 0;

		private Vector2 direction
		{
			get { return m_direction; }
			set
			{
				m_direction = value;
				float sqrSum = Mathf.Pow(value.x, 2) + Mathf.Pow(value.y, 2);
				GTime.timeScale = Mathf.Sqrt(sqrSum);
			}
		}

		private const float CLEAR_DURATION = 10;
		private const float LEAVE_DURATION = 4;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			GTime.Create();
		}
		private void Start()
		{
			m_world.Init(this);
			Factory.Create(m_factory, m_world, m_interface);

			InitShip();
			InitWorld();
			InitInterface();

			player = new Player(m_interface, m_ship);
			StartGame();
			StopMove();
		}
		private void InitShip()
		{
			m_ship = Factory.GetShip(ShipType.STANDART);
			m_ship.properties = m_shipProperties;
			m_ship.position = new Vector3(0, 0, PLAYER_DEPTH);
			m_ship.onDeath += EndGame;
		}
		private void InitWorld()
		{
			m_world.ship = m_ship;
		}
		private void InitInterface()
		{
			m_interface.Init(this);
			m_interface.joystick.joystickListener = UpdateMove;
			m_interface.joystick.joystickCloseEvent = StopMove;
			m_interface.onPause += Pause;
		}

		private void StartGame()
		{
			state = GameplayState.PLAYING;
			clearProgress = 0;
		}
		private void Pause(bool isPause)
		{
			if (isPause)
			{
				Time.timeScale = 0;
				m_prepauseState = state;
				return;
			}

			Time.timeScale = 1;
			state = m_prepauseState;
		}
		private void EndGame()
		{
			state = GameplayState.END;
		}

		private void FixedUpdate()
		{
			UpdateClearProgress();
		}
		private void UpdateClearProgress()
		{
			if (state != GameplayState.PLAYING)
			{
				return;
			}

			float duration = clearProgress * CLEAR_DURATION + Time.fixedDeltaTime;
			duration += -direction.y * Time.fixedDeltaTime * LEAVE_DURATION;
			clearProgress = duration / CLEAR_DURATION;
			if (clearProgress >= 1) EndGame();
		}

		private void UpdateMove(Vector2 direction)
		{
			this.direction = direction;
			m_ship.RotateBy(direction.x);
		}
		private void StopMove()
		{
			direction = Vector2.zero;
		}
		private void NotifyObjects()
		{
			m_world.GameplayChange(state);
			m_interface.GameplayChange(state);
		}
	}

	public class GTime
	{
		public static float timeScale
		{
			get { return m_timeScale; }
			set { m_timeScale = Mathf.Clamp01(value); }
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
		World world { get; }
		Player player { get; }
		float clearProgress { get; }
	}
}
