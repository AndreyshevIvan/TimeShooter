using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace MyGame
{
	public class Bonus : WorldObject
	{
		public BonusType type
		{
			set
			{
				m_type = value;
				InitRealization();
			}
		}
		public bool isMagnetic { get; protected set; }

		protected sealed override void OnAwakeEnd()
		{
			isMagnetic = true;
		}
		protected override void OnInitEnd()
		{
			exitAllowed = true;
			MoveToGround();
			SetExplosionForce();
			SetRandomRotation();
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == (int)Layer.PLAYER)
			{
				if (m_realization != null) m_realization();
				Exit();
			}
		}
		protected sealed override void PlayingUpdate()
		{
			if (isMagnetic) MoveToShip(true);
			transform.Rotate(m_rotation * GTime.timeStep);
		}

		private BonusType m_type;
		private EventDelegate m_realization;
		private Vector3 m_rotation;
		private Vector3 m_startPosition;

		private float moveSpeed { get { return GTime.timeStep * MOVE_SPEED; } }

		private const int HEAL_COUNT = 50;
		private const float DELTA_POSITION = 100;
		private const float DELTA_ROTATION = 100;
		private const float MOVE_SPEED = 10;

		private void SetExplosionForce()
		{
			Vector3 randPosition = Utils.RandomVect(-DELTA_POSITION, DELTA_POSITION);
			position = randPosition;
		}
		private void SetRandomRotation()
		{
			m_rotation = Utils.RandomVect(-DELTA_ROTATION, DELTA_ROTATION);
		}

		private void InitRealization()
		{
			m_realization = () => {;};

			switch (m_type)
			{
				case BonusType.HEALTH:
					InitHealth();
					break;

				case BonusType.STAR:
					InitStar();
					break;
			}
		}
		private void InitHealth()
		{
			m_realization = () => world.player.Heal(HEAL_COUNT);
		}
		private void InitStar()
		{
			m_realization = () => world.player.AddStars(1);
		}
	}
}
