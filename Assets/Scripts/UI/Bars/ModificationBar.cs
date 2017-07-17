using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUtils;
using MyGame.Hero;

namespace MyGame
{
	public class ModificationBar : UIBar<int>
	{
		protected override void OnAwakeEnd()
		{
			CreateNewPlanks();
			ResetFadeElements();
		}
		protected override void OnSetValue()
		{
			m_planks.ForEach(plank =>
			{
				plank.color = m_inactive;
				int plankIndex = m_planks.IndexOf(plank);

				if (plankIndex < value)
				{
					plank.color = GetPlankColor(plankIndex);
				}
			});
		}

		[SerializeField]
		public GameObject m_plank;
		[SerializeField]
		private Gradient m_gradient;
		[SerializeField]
		public Color m_inactive;
		private List<Image> m_planks = new List<Image>();

		private void CreateNewPlanks()
		{
			List<Component> oldPlanks = Utils.GetChilds<Component>(transform);
			m_planks.ForEach(element => Destroy(element));
			m_planks.Clear();

			Utils.DoAnyTimes(ShipMind.MODIFICATION_COUNT, () =>
			{
				GameObject plank = Instantiate(m_plank, transform);
				Image image = plank.GetComponentInChildren<Image>();
				m_planks.Add(image);
				image.color = m_inactive;
			});
		}
		private Color GetPlankColor(int plankIndex)
		{
			int maxIndex = m_planks.Count - 1;
			float colorTime = (float)plankIndex / maxIndex;
			return m_gradient.Evaluate(colorTime);
		}
	}
}
