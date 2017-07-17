using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;

namespace MyGame
{
	public abstract class UIBar<T> : MonoBehaviour
	{
		public Vector3 worldPosition { set { SetPosition(value); } }
		public T value { get; protected set; }
		public bool isActive { get; protected set; }

		public const float HP_BAR_FADE_DUR = 0.4f;

		public void SetValue(T newValue)
		{
			if (!isFirstSetComplete)
			{
				value = newValue;
				OnFirstSet();
				isFirstSetComplete = true;
				OnSetValue();
				lastUpdateTimer = 0;
				return;
			}

			if (checkOnEqual && newValue.Equals(value))
			{
				return;
			}

			value = newValue;
			OnSetValue();
			lastUpdateTimer = 0;
		}
		public void Fade(float fade, float duration)
		{
			Utils.FadeList(m_fadeElements, fade, duration);
		}
		public void Close()
		{
			Destroy(gameObject);
		}

		protected Camera mainCamera { get; private set; }
		protected RectTransform rect { get; set; }
		protected float lastUpdateTimer { get; private set; }
		protected bool isFirstSetComplete { get; private set; }
		protected bool isTimerWork { get; set; }
		protected bool checkOnEqual { get; set; }

		protected void Awake()
		{
			ResetFadeElements();
			mainCamera = Camera.main;
			rect = GetComponent<RectTransform>();
			isFirstSetComplete = false;
			lastUpdateTimer = 0;
			isTimerWork = false;
			checkOnEqual = true;
			OnAwakeEnd();
		}
		protected virtual void OnAwakeEnd() { }
		protected virtual void OnFirstSet() { }

		protected abstract void OnSetValue();

		protected void FixedUpdate()
		{
			OnUpdate();
			if (isTimerWork) lastUpdateTimer += Time.fixedDeltaTime;
		}
		protected virtual void OnUpdate() { }
		protected virtual void SetPosition(Vector3 worldPosition) { }

		protected void ResetFadeElements()
		{
			m_fadeElements = Utils.GetAllComponents<Graphic>(gameObject.transform);
		}

		private List<Graphic> m_fadeElements;
	}

	public abstract class HealthBar : UIBar<int> { }
}
