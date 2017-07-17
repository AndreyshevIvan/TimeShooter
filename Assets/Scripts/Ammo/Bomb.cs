using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameUtils;
using System.Collections;

namespace MyGame
{
	// TODO: Create common bomb
	public class Bomb : Body
	{
		public Transform parent { get; set; }

		protected override void OnAwakeEnd()
		{
			m_transform = GetComponent<Transform>();
			target = new Vector3(TARGET_RADIUS, TARGET_RADIUS, TARGET_RADIUS);
			isDemagamble = true;
			touchDemage = DEMAGE;
			gameObject.layer = (int)Layer.UNTOUCH;
		}
		protected override void AfterMatchUpdate()
		{
			OpenAndDemage();
		}
		protected override void PlayingUpdate()
		{
			OpenAndDemage();
		}

		private Transform m_transform;
		private float m_timer;

		private Vector3 scale
		{
			get
			{
				return m_transform.localScale;
			}
			set
			{
				m_transform.localScale = value;
			}
		}
		private Vector3 target { get; set; }
		private float radius { get { return TARGET_RADIUS * m_timer / CAST_DURATION; } }

		private const float CAST_DURATION = 0.8f;
		private const float TARGET_RADIUS = 20;
		private const int DEMAGE = 1;

		private void OpenAndDemage()
		{
			if (!parent)
			{
				world.Remove(this);
				return;
			}

			position = parent.position;

			if (scale.x >= TARGET_RADIUS)
			{
				gameObject.layer = (int)Layer.PLAYER_BULLET;
				StartCoroutine(DestroyAfterBoom());
				return;
			}

			scale = new Vector3(radius, radius, radius);
			m_timer += Time.fixedDeltaTime;
		}
		private IEnumerator DestroyAfterBoom()
		{
			yield return new WaitForFixedUpdate();
			distmantleAllowed = true;
			world.Remove(this);
		}
	}
}
