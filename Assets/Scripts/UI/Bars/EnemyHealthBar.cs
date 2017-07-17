using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;

namespace MyGame
{
	public class EnemyHealthBar : HealthBar
	{
		protected override void OnAwakeEnd()
		{
			layout = GetComponent<HorizontalLayoutGroup>();

			float barWidth = Utils.FromSreen(WIDTH_FACTOR);
			Utils.SetWidth(rect, barWidth);
			Utils.SetHeight(rect, barWidth * HEIGHT_FROM_WIDTH);

			layout.spacing = 1;
			layout.padding = new RectOffset(1, 1, 1, 1);
		}
		protected override void OnFirstSet()
		{
			Utils.DoAnyTimes(value, () =>
			{
				GameObject unit = Instantiate(m_healthUnit);
				unit.transform.SetParent(transform);
				m_units.Add(unit.GetComponent<Image>());
			});
		}
		protected override void OnSetValue()
		{
			for (int i = value; i < m_units.Count; i++)
			{
				m_units[i].color = Color.black;
			}
		}
		protected override void SetPosition(Vector3 worldPosition)
		{
			Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
			transform.position = screenPosition;
		}

		[SerializeField]
		private GameObject m_healthUnit;
		[SerializeField]
		private List<Image> m_units = new List<Image>();

		private HorizontalLayoutGroup layout { get; set; }

		private const float WIDTH_FACTOR = 0.075f;
		private const float HEIGHT_FROM_WIDTH = 0.28f;
	}
}
