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
	public partial class GameplayUI
		: GameplayObject
		, IPointerDownHandler
		, IPointerUpHandler
		, IDragHandler
	{
		public Transform barsParent { get { return m_barsParent; } }
		public TouchDelegate joystickListener { get; set; }
		public EventDelegate joystickCloseEvent { get; set; }
		public BoolEventDelegate onPause { get; set; }
		public int points { set { m_points.SetValue(value); } }

		public void OnPointerDown(PointerEventData eventData)
		{
			m_joystick.Open(eventData);
			if (joystickListener != null) joystickListener(m_joystick.direction);
		}
		public void OnDrag(PointerEventData eventData)
		{
			m_joystick.MoveTouch(eventData);
			if (joystickListener != null) joystickListener(m_joystick.direction);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			m_joystick.Close();
			if (joystickCloseEvent != null) joystickCloseEvent();
		}
		public void Pause(bool isPause)
		{
			onPause(isPause);
			SetActive(m_pauseInterface, isPause);
		}

		protected override void OnPreStart()
		{
			CloseInterface();

			m_points.Fade(0, 0);
		}
		protected override void OnPlaying()
		{
			CloseInterface();
			SetActive(m_pauseButton, true);
			SetActive(m_points, true);
			SetActive(m_joystick, true);

			m_points.Fade(1, BARS_FADING_DURATION);
		}
		protected override void OnEndGameplay()
		{
			CloseInterface();
			SetActive(m_results, true);

			Utils.FadeElement(m_results.transform, 0, 0);
			m_points.Fade(0, BARS_FADING_DURATION);
		}

		protected override void PlayingUpdate()
		{
			UpdateCameraPosition();
		}
		protected override void AfterMatchUpdate()
		{
			UpdateCameraPosition();
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
		private Camera m_camera;

		private const float TOUCH_OFFSET_Y = 0.04f;
		private const float CAMERA_MAX_OFFSET = 4;
		private const float CAMERA_SPEED = 7;
		private const float CAMERA_ANGLE_FACTOR = 0.076f;
		private const float PAUSE_BUTTON_SIZE_FACTOR = 0.07f;
		private const float SHIP_AREA_SIZE_FACTOR = 0.35f;
		private const float AREA_POS_FACTOR = 0.02f;
		private const float JOYSTICK_SIZE_FACTOR = 0.4f;

		private const float MAX_CURTAIN_TRANSPARENCY = 0.5f;
		private const float BARS_FADING_DURATION = 1.2f;

		private const string AREA_EXIT_TRIGGER = "AreaExit";
		private const string OPEN_LEVEL_INFO = "OpenLevelInfo";
		private const string CLOSE_LEVEL_INFO = "CloseLevelInfo";
		private const string CLOSE_BARS = "CloseBars";

		private void Awake()
		{
			m_camera = Camera.main;
			Input.multiTouchEnabled = false;

			InitUIElements();
		}
		private void InitUIElements()
		{
			Utils.SetSize(m_pauseButton, Utils.FromSreen(PAUSE_BUTTON_SIZE_FACTOR));

			//float areaSize = Utils.FromSreen(SHIP_AREA_SIZE_FACTOR);
			//Utils.SetSize(m_shipArea.GetComponent<RectTransform>(), areaSize);
		}
		private void UpdateCameraPosition()
		{
			Vector3 position = m_camera.transform.position;
			position.x = player.shipPosition.x;
			m_camera.transform.position = position;
			/*
			float playerXPart = player.shipPosition.x / box.xMax;
			float camMove = playerXPart * playerXPart * CAMERA_MAX_OFFSET;
			camMove = (player.shipPosition.x < 0) ? -camMove : camMove;
			Vector3 newCameraPos = m_camera.transform.position;
			newCameraPos.x = Mathf.MoveTowards(newCameraPos.x, camMove, CAMERA_SPEED * Time.fixedDeltaTime);
			m_camera.transform.position = newCameraPos;
			*/
		}
		private void CloseInterface()
		{
			SetActive(m_results, false);
			SetActive(m_pauseButton, false);
			SetActive(m_pauseInterface, false);
			SetActive(m_points, false);
			SetActive(m_joystick, false);
		}
		private void SetActive(Component element, bool isOpen)
		{
			element.gameObject.SetActive(isOpen);
		}
	}

	public partial class GameplayUI
	{
		public void ViewResults()
		{
			CalcData();
			InitResultsInterface();
		}

		[SerializeField]
		private Component m_results;
		[SerializeField]
		private Button m_continue;

		private Player player { get { return gameplay.player; } }

		private const float ACHIEVEMENTS_SIZE_FACTOR = 0.05f;
		private const float RESULTS_FADE_TIME = 0.2f;

		private const string OPEN_RESULTS = "OpenResults";

		private void CalcData()
		{
		}
		private void InitResultsInterface()
		{
			Utils.FadeElement(m_results.transform, 1, RESULTS_FADE_TIME);
		}
	}

	public partial class GameplayUI
	{
		public static void SetShipBulletColor(float modsPart, TrailRenderer tail)
		{
			tail.startColor = new Color(1, 1 - modsPart, 1 - modsPart);
		}

		private const float BULLET_TAIL_FIRST_KEY_TIME = 0.25f;
	}

	public delegate void BoolEventDelegate(bool isTrue);
	public delegate void EventDelegate();
}
