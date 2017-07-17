using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameUtils;

namespace MyGame
{
	public class Body : WorldObject
	{
		public int health { get; protected set; }
		public float healthPart { get { return (float)health / (float)maxHealth; } }
		public bool isLive { get { return !isDemagamble || health > 0; } }
		public bool isDemagamble { get; set; }
		public bool isFull { get { return health == maxHealth; } }
		public int touchDemage { get; protected set; }
		public bool isEraseOnDeath { get; set; }

		public virtual void ChangeHealth(int valueToAdd)
		{
			if (valueToAdd == 0)
			{
				return;
			}

			OnChangeHealth(ref valueToAdd);
			health = health + valueToAdd;
			health = Mathf.Clamp(health, 0, maxHealth);
			if (healthBar) healthBar.SetValue(health);
		}

		protected HealthBar healthBar
		{
			get { return m_healthBar; }
			set
			{
				toDestroy.Add(value.gameObject);
				m_healthBar = value;
			}
		}
		protected int maxHealth { get; set; }

		new protected void Awake()
		{
			base.Awake();
			isEraseOnDeath = true;
			isDemagamble = true;
		}

		protected void OnTriggerEnter(Collider other)
		{
			OnColliderEnter(other);

			Body otherBody = Utils.GetOther<Body>(other);
			if (otherBody) OnCollideWithBody(otherBody);
		}
		protected virtual void OnColliderEnter(Collider other) { }
		protected virtual void OnChangeHealth(ref int valueToAdd) { }
		protected virtual void DoAfterDemaged() { }
		protected virtual void OnDemageTaked() { }
		protected virtual void OnDeath() { }

		private HealthBar m_healthBar;
		private bool m_isDead = false;

		private void UpdateBarPosition()
		{
			if (!healthBar)
			{
				return;
			}

			healthBar.worldPosition = position;
		}
		private void OnCollideWithBody(Body other)
		{
			if (!isDemagamble || m_isDead) return;

			ChangeHealth(-1 * other.touchDemage);
			other.OnDemageTaked();
			DoAfterDemaged();

			if (!isLive && isEraseOnDeath)
			{
				OnDeath();
				world.Remove(this);
				m_isDead = true;
				return;
			}

			if (healthBar) healthBar.SetValue(health);
		}
	}
}
