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
		public bool explosionStart { get; set; }
		public bool rotateOnStart { get; set; }
		public bool isMagnetic { get; protected set; }

		protected sealed override void OnAwakeEnd()
		{
			explosionStart = true;
			rotateOnStart = true;
			isMagnetic = true;
		}
		protected override void OnInitEnd()
		{
			exitAllowed = true;
			MoveToGround();

			if (explosionStart) SetExplosionForce();
			if (rotateOnStart) SetRandomRotation();
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == (int)Layer.PLAYER)
			{
				if (realization != null) realization();
				Exit();
			}
		}
		protected sealed override void PlayingUpdate()
		{
			if (isMagnetic) world.MoveToShip(this);
			UpdatePositionOnField();
		}
		protected sealed override void AfterMatchUpdate()
		{
			if (isMagnetic) world.MoveToShip(this);
			UpdatePositionOnField();
		}

		private BonusType m_type;
		private EventDelegate realization;

		private const int HEAL_COUNT = 50;
		private const float DELTA_FORCE = 200;
		private const float DELTA_ROTATION = 100;

		private void SetExplosionForce()
		{
			Vector3 force = Utils.RandomVect(-DELTA_FORCE, DELTA_FORCE);
			physicsBody.AddForce(force);
		}
		private void SetRandomRotation()
		{
			Vector3 rotation = Utils.RandomVect(-DELTA_ROTATION, DELTA_ROTATION);
			physicsBody.AddTorque(rotation);
		}

		private void InitRealization()
		{
			realization = () => {;};

			switch (m_type)
			{
				case BonusType.HEALTH:
					InitHealth();
					break;

				case BonusType.MODIFICATION:
					InitModification();
					break;

				case BonusType.STAR:
					InitStar();
					break;
			}
		}
		private void InitHealth()
		{
			realization = () => world.player.Heal(HEAL_COUNT);
		}
		private void InitModification()
		{
			realization = () => world.player.Modify();
		}
		private void InitStar()
		{
			realization = () => world.player.AddStars(1);
		}
	}
}
