using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using MyGame.Hero;
using GameUtils;

namespace MyGame
{
	public partial class GameplayUI : GameplayObject
	{
		public BoolEventDelegate onPause { get; set; }
		public VirtualJoyStick joystick { get { return m_joystick; } }
		public Transform barsParent { get { return m_barsParent; } }
		public int points { set { m_points.SetValue(value); } }
		public bool isJoystickWork
		{
			set { joystick.gameObject.SetActive(value); }
		}
		new public static Camera camera { get; private set; }

		public void Pause(bool isPause)
		{
			if (onPause != null) onPause(isPause);
		}

		protected override void OnPreStart()
		{
			CloseInterface();
			isJoystickWork = true;

			m_points.Fade(0, 0);
		}
		protected override void OnPlaying()
		{
			CloseInterface();
			SetActive(m_pauseButton, true);
			SetActive(m_points, true);
			SetActive(m_progressSlider, true);
			isJoystickWork = true;

			m_points.Fade(1, BARS_FADING_DURATION);
			Utils.FadeElement(m_progressSlider, 0, 0);
			Utils.FadeElement(m_progressSlider, 1, BARS_FADING_DURATION);
		}
		protected override void OnPause()
		{
			CloseInterface();
			SetActive(m_pauseInterface, true);
			isJoystickWork = false;
		}
		protected override void OnEndGameplay()
		{
			CloseInterface();
			SetActive(m_results, true);
			isJoystickWork = false;

			m_points.Fade(0, BARS_FADING_DURATION);
		}

		protected override void PlayingUpdate()
		{
			m_progressSlider.value = gameplay.clearProgress;
		}

		[SerializeField]
		private Transform m_barsParent;
		[SerializeField]
		private PointsBar m_points;
		[SerializeField]
		private RectTransform m_pauseButton;
		[SerializeField]
		private Component m_pauseInterface;
		[SerializeField]
		private VirtualJoyStick m_joystick;
		[SerializeField]
		private Slider m_progressSlider;

		private const float PAUSE_BUTTON_SIZE_FACTOR = 0.07f;
		private const float BARS_FADING_DURATION = 1.2f;

		private void Awake()
		{
			camera = Camera.main;
			Input.multiTouchEnabled = false;

			InitUIElements();
		}
		private void InitUIElements()
		{
			Utils.SetSize(m_pauseButton, Utils.FromSreen(PAUSE_BUTTON_SIZE_FACTOR));

			//float areaSize = Utils.FromSreen(SHIP_AREA_SIZE_FACTOR);
			//Utils.SetSize(m_shipArea.GetComponent<RectTransform>(), areaSize);
		}
		private void CloseInterface()
		{
			SetActive(m_results, false);
			SetActive(m_pauseButton, false);
			SetActive(m_pauseInterface, false);
			SetActive(m_points, false);
			SetActive(m_progressSlider, false);
		}
	}

	public partial class GameplayUI
	{
		[SerializeField]
		private Component m_results;

		private const float RESULTS_FADE_TIME = 0.2f;
	}

	public partial class GameplayUI
	{
		public static void SetShipBulletColor(float modsPart, TrailRenderer tail)
		{
			tail.startColor = new Color(1, 1 - modsPart, 1 - modsPart);
		}
		public static void SetActive(Component component, bool isOpen)
		{
			component.gameObject.SetActive(isOpen);
		}
	}

	public delegate void BoolEventDelegate(bool isTrue);
	public delegate void EventDelegate();
}
