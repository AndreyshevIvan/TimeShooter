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
		public PointDelegate moveShip;
		public BoolEventDelegate onChangeMode;
		public BoolEventDelegate onPause;
		public EventDelegate startTouchEvents;
		public EventDelegate onRestart;

		public int points { set { m_points.SetValue(value); } }
		public int modifications { set { m_modsBar.SetValue(value); } }

		public const float ENDING_FADE_DURATION = 0.4f;
		public const float SLOWMO_OPEN_DUR = 0.3f;

		public void StartGame()
		{
			if (startTouchEvents != null) startTouchEvents();
			SetActive(m_shipArea, false);
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
			m_modsBar.Fade(0, 0);
		}
		protected override void OnPlaying()
		{
			CloseInterface();
			SetActive(m_pauseButton, true);
			SetActive(m_points, true);
			SetActive(m_modsBar, true);

			m_points.Fade(1, BARS_FADING_DURATION);
			m_modsBar.Fade(1, BARS_FADING_DURATION);
		}
		protected override void OnEndGameplay()
		{
			CloseInterface();
			SetActive(m_results, true);

			Utils.FadeElement(m_results.transform, 0, 0);
			m_points.Fade(0, BARS_FADING_DURATION);
			m_modsBar.Fade(0, BARS_FADING_DURATION);
		}

		[SerializeField]
		private Transform m_barsParent;
		[SerializeField]
		private PointsBar m_points;
		[SerializeField]
		private ModificationBar m_modsBar;
		[SerializeField]
		private RectTransform m_pauseButton;
		[SerializeField]
		private Component m_pauseInterface;
		[SerializeField]
		private Button m_shipArea;
		private Camera m_camera;

		private IGameplay gameplay { get; set; }
		private BoundingBox box { get; set; }
		private Vector3 shipScreenPosition
		{
			get { return m_camera.WorldToScreenPoint(player.shipPosition); }
		}

		private const float TOUCH_OFFSET_Y = 0.04f;
		private const float CAMERA_MAX_OFFSET = 2;
		private const float CAMERA_SPEED = 7;
		private const float CAMERA_ANGLE_FACTOR = 0.076f;
		private const float PAUSE_BUTTON_SIZE_FACTOR = 0.08f;
		private const float SHIP_AREA_SIZE_FACTOR = 0.35f;
		private const float AREA_POS_FACTOR = 0.02f;

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
			box = GameData.box;

			InitUIElements();
			InitBehaviours();
		}
		private void InitUIElements()
		{
			Utils.SetSize(m_pauseButton, Utils.FromSreen(PAUSE_BUTTON_SIZE_FACTOR));

			float areaSize = Utils.FromSreen(SHIP_AREA_SIZE_FACTOR);
			Utils.SetSize(m_shipArea.GetComponent<RectTransform>(), areaSize);
		}
		private void InitBehaviours()
		{
			playingBehaviour += ControllShip;
		}

		private void ControllShip()
		{
			Vector3 screenPosition = Input.mousePosition;
			screenPosition.y += TOUCH_OFFSET_Y * Screen.height;
			screenPosition.z = m_camera.transform.position.y;
			screenPosition = m_camera.ScreenToWorldPoint(screenPosition);
			screenPosition.x += screenPosition.x * -CAMERA_ANGLE_FACTOR;
			screenPosition.z += screenPosition.z * -CAMERA_ANGLE_FACTOR;
			if (moveShip != null) moveShip(screenPosition);
			UpdateCameraPosition();
		}
		private void UpdateCameraPosition()
		{
			float playerXPart = player.shipPosition.x / box.xMax;
			float camMove = playerXPart * playerXPart * CAMERA_MAX_OFFSET;
			camMove = (player.shipPosition.x < 0) ? -camMove : camMove;
			Vector3 newCameraPos = m_camera.transform.position;
			newCameraPos.x = Mathf.MoveTowards(newCameraPos.x, camMove, CAMERA_SPEED * Time.fixedDeltaTime);
			m_camera.transform.position = newCameraPos;
		}
		private void CloseInterface()
		{
			SetActive(m_results, false);
			SetActive(m_pauseButton, false);
			SetActive(m_pauseInterface, false);
			SetActive(m_points, false);
			SetActive(m_modsBar, false);
		}
		private void SetActive(Component element, bool isOpen)
		{
			element.gameObject.SetActive(isOpen);
		}

		private EventDelegate currentBehaviour;
		private EventDelegate prePlayingBehaviour;
		private EventDelegate playingBehaviour;
		private EventDelegate pauseBehaviour;
		private EventDelegate winBehaviour;
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
			InitStrings();
			InitAchievements();

			Utils.FadeElement(m_results.transform, 1, RESULTS_FADE_TIME);
		}
		private void InitStrings()
		{
			int fontSize = Utils.FromSreen(ACHIEVEMENTS_SIZE_FACTOR);
		}
		private void InitAchievements()
		{
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

	public delegate void PointDelegate(Vector3 point);
	public delegate void BoolEventDelegate(bool isTrue);
	public delegate void EventDelegate();
}
