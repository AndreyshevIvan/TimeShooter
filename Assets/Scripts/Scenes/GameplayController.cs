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
				if (m_state != value)
				{
					m_state = value;
					NotifyObjects();
				}
			}
		}
		public World world { get { return m_world; } }
		public Vector2 direction
		{
			get { return m_direction; }
			set
			{
				m_direction = value;
				float sqrSum = Mathf.Pow(value.x, 2) + Mathf.Pow(value.y, 2);
				GTime.timeScale = Mathf.Sqrt(sqrSum);
			}
		}
		public float clearProgress
		{
			get { return m_clearPart; }
			private set { m_clearPart = Mathf.Clamp01(value); }
		}

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
		private float m_clearPosition = WorldConst.CLEAR_MAX_OFFSET;
		private float m_clearPart = 0;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			GTime.Create();
			Factory.Create(m_factory, m_world, m_interface);

			state = GameplayState.INITIALIZATION;
			clearProgress = 0;
		}
		private void Start()
		{
			InitShip();
			InitWorld();
			InitInterface();

			state = GameplayState.BEFORE_PLAYING;
			StopMove();
		}
		private void InitShip()
		{
			m_ship = Factory.GetShip(ShipType.STANDART);
			m_ship.properties = m_shipProperties;
			m_ship.position = WorldConst.ZERO_POINT;
			m_ship.onDeath += EndGame;
		}
		private void InitWorld()
		{
			m_world.Init(this);
			m_world.ship = m_ship;
			m_world.onStart = StartGame;
		}
		private void InitInterface()
		{
			m_interface.Init(this);
			m_interface.onPause += Pause;
			m_interface.joystick.joystickCloseEvent = StopMove;
			m_interface.joystick.joystickListener = delegate (Vector2 newDirection) {
				this.direction = newDirection;
			};
		}

		private void StartGame()
		{
			state = GameplayState.PLAYING;
		}
		private void Pause(bool isPause)
		{
			if (isPause)
			{
				Time.timeScale = 0;
				m_prepauseState = state;
				state = GameplayState.PAUSE;
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

			m_clearPosition += WorldConst.CLEAR_SPEED * Time.fixedDeltaTime;
			m_clearPosition += m_world.offset.y;
			clearProgress = m_clearPosition / WorldConst.CLEAR_MAX_OFFSET;
			if (clearProgress >= 1) EndGame();
		}

		private void StopMove()
		{
			direction = Vector2.zero;
		}
		private void NotifyObjects()
		{
			if (m_world) m_world.GameplayChange(state);
			if (m_interface) m_interface.GameplayChange(state);
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
		Vector2 direction { get; }
		float clearProgress { get; }
	}
}
