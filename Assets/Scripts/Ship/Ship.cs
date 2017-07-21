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

		public void RotateBy(float zRotationDirection)
		{
			zRotationDirection = Mathf.Clamp(zRotationDirection, -1, 1);
			float zAngle = zRotationDirection * MAX_ROTATE_ANGLE;
			m_targetRotation = Quaternion.Euler(0, SHIP_ANGLE_Y, zAngle);
		}
		public void StopRotate()
		{
			m_targetRotation = physicsBody.rotation;
		}

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
		protected override void OnPlaying()
		{
			physicsBody.rotation = Quaternion.Euler(0, SHIP_ANGLE_Y, 0);
		}
		protected override void OnEndGameplay()
		{
			gameObject.layer = (int)Layer.UNTOUCH;
			if (healthBar) healthBar.Close();
		}
		protected override void PlayingUpdate()
		{
			Quaternion current = physicsBody.rotation;
			float rotateSpeed = GTime.timeStep * ROTATE_SPEED;
			physicsBody.rotation = Quaternion.Lerp(current, m_targetRotation, rotateSpeed);
		}
		protected override void DoAfterDemaged()
		{
			healthBar.Fade(1, PlayerHealthBar.HP_BAR_FADE_DUR);
			if (!isLive && onDeath != null) onDeath();
		}
		protected override void OnChangeHealth(ref int valueToAdd)
		{
			if (isFull) healthBar.Fade(0, PlayerHealthBar.HP_BAR_FADE_DUR);
		}

		private Quaternion m_targetRotation;

		private const float SHIP_ANGLE_Y = 180;
		private const float ROTATE_SPEED = 2;
		private const float MAX_ROTATE_ANGLE = 60;
	}
}
