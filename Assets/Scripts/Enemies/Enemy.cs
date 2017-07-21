using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameUtils;

namespace MyGame.Enemies
{
	using BonusPair = Pair<BonusType, int>;

	public abstract class Enemy : Body
	{
		public UnitType type { get; set; }

		protected bool isTimerWork { get; set; }
		protected float coldown { get; set; }

		protected sealed override void OnInitEnd()
		{
			isTimerWork = false;
			exitAllowed = false;
			openAllowed = true;
			distmantleAllowed = true;

			InitProperties();

			if (healthBar) healthBar.SetValue(health);
			if (roadController) roadController.OnEndReached.AddListener(T =>
			{
				if (gameplay.state != GameplayState.AFTER_MATCH) world.player.LossEnemy();
				Exit();
			});

			playingUpd += Shooting;
		}
		protected abstract void InitProperties();

		protected abstract void Shoot();
		protected void ExtraReady()
		{
			m_timer = coldown;
		}

		protected sealed override void OnPlaying()
		{
			if (roadController) roadController.Play();
		}
		protected sealed override void OnPause()
		{
			if (roadController) roadController.Pause();
		}

		protected sealed override void DoAfterDemaged()
		{
			if (healthBar) healthBar.Fade(1, PlayerHealthBar.HP_BAR_FADE_DUR);
		}
		protected sealed override void OnDeath()
		{
			world.player.KillEnemy(type);

			if (Utils.IsHappen(HEALTH_PROBABLILITY))
			{
				bonuses.Add(BonusPair.Create(BonusType.HEALTH, 1));
			}
			if (Utils.IsHappen(MODIFICATION_PROBABILITY))
			{
				bonuses.Add(BonusPair.Create(BonusType.MODIFICATION, 1));
			}
		}

		private float m_timer = 0;

		private float HEALTH_PROBABLILITY = 0.1f;
		private float MODIFICATION_PROBABILITY = 1;

		private void Shooting()
		{
			if (isTimerWork && Utils.UpdateTimer(ref m_timer, coldown, true))
			{
				Shoot();
				m_timer = 0;
			}
		}
	}
}
