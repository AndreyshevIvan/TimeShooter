using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFactory;
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

		protected override void OnInitEnd()
		{
			m_shield.gameObject.SetActive(false);
		}
		protected override void PlayingUpdate()
		{
			ShootByBaseGun();
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
		private float m_gunTimer = 0;

		private bool isGunReady
		{
			get { return Utils.UpdateTimer(ref m_gunTimer, m_properties.gunColdown, true); }
		}

		private const float SCATTER_STEP = 0.5f;
		private const float GUN_COLDOWN_STEP = 0.03f;

		private Bullet CreateBullet(AmmoType type, BulletData data, Transform spawn)
		{
			Bullet bullet = Factory.GetAmmo(type) as Bullet;
			bullet.Shoot(data, spawn.position);
			return bullet;
		}
		private void SetNewProperties()
		{
			m_properties.gunColdown = 0.48f;
			m_properties.gunData = new BulletData();
			m_properties.gunData.speed = 30;
			m_properties.gunData.demage = 1;
		}
		private void ShootByBaseGun()
		{
			/*
			if (!isGunReady)
			{
				return;
			}

			m_properties.gunData.direction = Vector3.forward;
			AmmoType type = AmmoType.PLAYER_BULLET;
			Bullet bullet = CreateBullet(type, m_properties.gunData, m_gunSpawn);
			m_gunTimer = 0;
			*/
		}
	}
}
