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
		public ParticleSystem engine { get { return m_engineParticles; } }
		public ShipProperties properties
		{
			set
			{
				mind.properties = value;
				health = maxHealth = 100;// value.health;
			}
		}
		public Animator animator { get; private set; }

		public const float ENDING_CONTROLL_DURATION = 2;

		public void MoveBy(Vector2 direction)
		{
			direction.y = 0;
			m_smoothDir = Vector3.MoveTowards(m_smoothDir, direction, CONTROLL_SMOOTHING);
			direction = m_smoothDir;
			Vector3 movement = new Vector3(direction.x, 0, direction.y);
			velocity = movement * CONTROLL_SPEED;
			m_isMoved = true;
		}

		protected override void OnAwakeEnd()
		{
			mind = GetComponent<ShipMind>();
			ParticleSystem.MainModule engineMain = m_engineParticles.main;
			engineMain.simulationSpace = ParticleSystemSimulationSpace.Local;
			animator = GetComponent<Animator>();
			animator.Play(ROTATION_CLIP);
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
			ParticleSystem.MainModule engineMain = m_engineParticles.main;
			engineMain.simulationSpace = ParticleSystemSimulationSpace.World;
			animator.SetTrigger(ROTATION_TRIGGER);
			animator.enabled = false;
			physicsBody.rotation = Quaternion.Euler(0, SHIP_ANGLE_Y, 0);
		}
		protected override void OnEndGameplay()
		{
			gameObject.layer = (int)Layer.UNTOUCH;
			if (healthBar) healthBar.Close();
		}
		protected override void PlayingUpdate()
		{
			position += velocity * GTime.timeStep;

			UpdateRotation();
			UpdateMoveingSpeed();
		}

		protected override void DoAfterDemaged()
		{
			world.player.BeDemaged();
			healthBar.Fade(1, PlayerHealthBar.HP_BAR_FADE_DUR);
		}
		protected override void OnChangeHealth(ref int valueToAdd)
		{
			if (isFull) healthBar.Fade(0, PlayerHealthBar.HP_BAR_FADE_DUR);
		}

		[SerializeField]
		private ParticleSystem m_engineParticles;
		private Vector3 m_smoothDir;
		private bool m_isMoved = false;
		private bool m_startEndAnimation = false;

		private Vector3 velocity { get; set; }

		private const float SHIP_ANGLE_Y = 180;

		private const float CONTROLL_SPEED = 40;
		private const float CONTROLL_SMOOTHING = 15;
		private const float CONTROLL_TILT = 1;
		private const float CONTROLL_MAX_ANGLE = 60;

		private const float ENDING_ANIM_DURATION = 5;
		private const float ENDING_ESCAPE_SPEED = 10;
		private const float ENDING_ANGLE_SPEED = 1;

		private const string ROTATION_TRIGGER = "EndRotate";
		private const string ROTATION_CLIP = "ShipStartRotation";

		private void UpdateRotation()
		{
			float zEuler = velocity.x * -CONTROLL_TILT;
			float zAngle = Mathf.Clamp(-zEuler, -CONTROLL_MAX_ANGLE, CONTROLL_MAX_ANGLE);
			physicsBody.rotation = Quaternion.Euler(0, SHIP_ANGLE_Y, zAngle);
		}
		private void UpdateMoveingSpeed()
		{
			Vector3 currVelocity = velocity;
			velocity = (m_isMoved) ? currVelocity : Vector3.zero;
			m_isMoved = false;
		}
		private void EndingAnimation()
		{
			float step = GTime.timeStep * ENDING_ANGLE_SPEED;
			float zAngle = Mathf.MoveTowards(physicsBody.rotation.z, 0, step);
			physicsBody.rotation = Quaternion.Euler(0, SHIP_ANGLE_Y, zAngle);
			position += new Vector3(0, 0, GTime.timeStep * ENDING_ESCAPE_SPEED);
		}
	}
}
