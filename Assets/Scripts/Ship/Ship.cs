using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFactory;
using GameUtils;

namespace MyGame.Hero
{
	public sealed class Ship : Body
	{
		public ShipMind mind { get; set; }
		public ShipProperties properties
		{
			set
			{
				mind.properties = value;
				health = maxHealth = 100;// value.health;
			}
		}
		public EventDelegate onDeath { get; set; }

		protected override void OnAwakeEnd()
		{
			mind = GetComponent<ShipMind>();
		}
		protected override void OnInitEnd()
		{
			healthBar = Factory.GetPlayerHealthBar();
			healthBar.SetValue(health);
			touchDemage = int.MaxValue;
			isEraseOnDeath = false;
			distmantleAllowed = true;
		}
		protected override void OnEndGameplay()
		{
			gameObject.layer = (int)Layer.UNTOUCH;
			if (healthBar) healthBar.Close();
		}

		protected override void BeforePlayingUpdate()
		{
			UpdateRotation();
		}
		protected override void PlayingUpdate()
		{
			UpdateRotation();
		}

		protected override void OnChangeHealth(ref int valueToAdd)
		{
			if (isFull) healthBar.Fade(0, PlayerHealthBar.HP_BAR_FADE_DUR);
		}
		protected override void DoAfterDemaged()
		{
			healthBar.Fade(1, PlayerHealthBar.HP_BAR_FADE_DUR);
			if (!isLive && onDeath != null) onDeath();
		}

		private const float ROTATE_SPEED = 2;
		private const float MAX_ROTATE_ANGLE = 60;

		private void UpdateRotation()
		{
			float rotateSpeed = GTime.timeStep * ROTATE_SPEED;
			float zAngle = gameplay.direction.x * MAX_ROTATE_ANGLE;
			Quaternion targetRotation = Quaternion.Euler(0, 0, -zAngle);
			rotation = Quaternion.Lerp(rotation, targetRotation, rotateSpeed);
		}
	}
}
