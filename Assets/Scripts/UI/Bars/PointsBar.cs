using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
	public class PointsBar : UIBar<int>
	{
		protected override void OnAwakeEnd()
		{
			m_field = GetComponent<Text>();
			OnSetValue();
		}
		protected override void OnSetValue()
		{
			box += value - currentValue;
		}
		protected override void OnUpdate()
		{
			if (box == 0)
			{
				return;
			}

			int toAdd = (box > MIN_ADD_STEP) ? MIN_ADD_STEP : box;
			box -= toAdd;
			currentValue += toAdd;
			m_field.text = currentValue.ToString(PATTERN);
		}

		private Text m_field;
		private int box = 0;
		private int currentValue = 0;

		private const int MIN_ADD_STEP = 25;
		private const string PATTERN = "000-000-000";
	}
}
