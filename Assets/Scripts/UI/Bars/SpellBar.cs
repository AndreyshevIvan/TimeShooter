using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
	public class SpellBar : UIBar<float>
	{
		protected override void OnAwakeEnd()
		{
		}
		protected override void OnSetValue()
		{
			m_progress.fillAmount = value / MAX_PERCENTS;
			m_animator.SetBool(m_readyTrigger, isReady);
		}

		[SerializeField]
		private Image m_progress;
		[SerializeField]
		private Image m_icon;
		[SerializeField]
		private Animator m_animator;
		[SerializeField]
		private string m_readyTrigger;

		private bool isReady { get { return value == MAX_PERCENTS; } }

		private const int MAX_PERCENTS = 100;
	}
}
