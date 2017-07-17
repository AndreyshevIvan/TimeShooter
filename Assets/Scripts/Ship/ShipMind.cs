using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace MyGame.Hero
{
	public class ShipMind : WorldObject
	{
		public ShipType type { get; set; }
		public ShipProperties properties
		{
			set
			{
				m_properties = value;
				SetNewProperties();
			}
		}

		public float magnetFactor { get { return 1; } }
		public float magnetDistance { get { return 5; } }
		public float bombProcess { get { return m_bombTimer / m_properties.bombColdown; } }
		public float shieldProcess { get { return m_shieldTimer / m_properties.shieldColdown; } }
		public float shieldFactor { get { return SHIELD_DEMAGE_FACTOR; } }
		public float modsPart { get { return (float)modsCount / MODIFICATION_COUNT; } }
		public int modsCount { get; protected set; }
		public bool isShieldActive { get; private set; }

		public const int MODIFICATION_COUNT = 12;

		public void ModificateByOne()
		{
			if (modsCount >= MODIFICATION_COUNT)
			{
				return;
			}

			modsCount++;
			m_properties.gunColdown -= GUN_COLDOWN_STEP;
			m_gunScatter += SCATTER_STEP;
		}
		public bool Bomb()
		{
			if (!isBombReady)
			{
				return false;
			}

			Bomb bomb = world.factory.GetAmmo(AmmoType.PLAYER_BOMB) as Bomb;
			bomb.parent = transform;
			m_bombTimer = 0;
			return true;
		}
		public bool Shield()
		{
			if (!isShieldReady || isShieldActive)
			{
				return false;
			}

			SetShield(true);
			return true;
		}

		protected override void OnInitEnd()
		{
			m_shield.gameObject.SetActive(false);
		}
		protected override void PlayingUpdate()
		{
			ShootByBaseGun();
			ShootByRockets();

			UpdateTimers();
		}
		protected override void OnExitFromWorld()
		{
		}

		[SerializeField]
		private Transform m_gunSpawn;
		[SerializeField]
		private GameObject m_shield;
		[SerializeField]
		private Transform m_leftRocketSpawn;
		[SerializeField]
		private Transform m_rightRocketSpawn;
		private ShipProperties m_properties;
		private float m_gunScatter = 0;
		private float m_gunTimer = 0;

		private float m_rocketTimer = 0;

		private float m_bombTimer = 0;

		private float m_shieldTimer = 0;
		private float m_shieldDuration = 0;

		private Vector3 gunDirection
		{
			get { return Utils.RndDirBetween(90 - m_gunScatter, 90 + m_gunScatter); }
		}
		private bool isGunReady
		{
			get { return Utils.UpdateTimer(ref m_gunTimer, m_properties.gunColdown); }
		}
		private bool isRocketsReady
		{
			get { return Utils.UpdateTimer(ref m_rocketTimer, m_properties.rocketColdown); }
		}
		private bool isBombReady
		{
			get { return m_bombTimer >= m_properties.bombColdown; }
		}
		private bool isShieldReady
		{
			get { return m_shieldTimer >= m_properties.shieldColdown; }
		}

		private const float SCATTER_STEP = 0.5f;
		private const float GUN_COLDOWN_STEP = 0.04f;
		private const float SHIELD_DEMAGE_FACTOR = 0.5f;
		private const string SHIELD_OPEN_ANIM = "OpenShield";
		private const string SHIELD_CLOSE_ANIM = "CloseShield";

		private void UpdateTimers()
		{
			m_gunTimer = 0;
			Utils.UpdateTimer(ref m_bombTimer, m_properties.bombColdown);
			Utils.UpdateTimer(ref m_shieldTimer, m_properties.shieldColdown);
		}

		private Bullet CreateBullet(AmmoType type, BulletData data, Transform spawn)
		{
			Bullet bullet = factory.GetAmmo(type) as Bullet;
			bullet.Shoot(data, spawn.position);
			return bullet;
		}
		private void SetNewProperties()
		{
			m_properties.bombColdown = 1;

			m_properties.shieldColdown = 2;
			m_properties.shieldDuration = 4;

			m_properties.gunColdown = 0.48f;
			m_properties.gunData = new BulletData();
			m_properties.gunData.speed = 30;
			m_properties.gunData.demage = 1;

			m_properties.rocketColdown = 0.2f;
			m_properties.rocketData = new BulletData();
			m_properties.rocketData.speed = 40;
			m_properties.rocketData.demage = int.MaxValue;
		}
		private void ShootByBaseGun()
		{
			if (!isGunReady)
			{
				return;
			}

			m_properties.gunData.direction = gunDirection;
			AmmoType type = AmmoType.PLAYER_BULLET;
			Bullet bullet = CreateBullet(type, m_properties.gunData, m_gunSpawn);
			GameplayUI.SetShipBulletColor(modsPart, bullet.trailRenderer);
			m_gunTimer = 0;
		}
		private void ShootByRockets()
		{
			if (!isRocketsReady)
			{
				return;
			}

			m_properties.rocketData.direction = Vector3.forward;
			AmmoType type = AmmoType.PLAYER_ROCKET;
			BulletData data = m_properties.rocketData;
			Bullet left = CreateBullet(type, data, m_leftRocketSpawn);
			Bullet right = CreateBullet(type, data, m_rightRocketSpawn);
			left.isDemagamble = false;
			right.isDemagamble = false;
			m_rocketTimer = 0;
		}
		private void SetShield(bool isOpen)
		{
			isShieldActive = isOpen;

			if (!isOpen)
			{
				m_shield.gameObject.SetActive(false);
				//world.ship.animator.Play(SHIELD_CLOSE_ANIM, 1);
				return;
			}

			m_shield.gameObject.SetActive(true);
			//world.ship.animator.Play(SHIELD_OPEN_ANIM, 1);
			Utils.DoAfterTime(this, m_properties.shieldDuration, () => SetShield(false));
			m_shieldTimer = 0;
		}

		private enum Mods
		{
			BASIC = 1,
			DOUBLE,
			TRIPLE,
			QUADRO,
		}
	}
}
