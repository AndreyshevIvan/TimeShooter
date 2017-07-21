using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
	public sealed class Bullet : Body
	{
		public Vector3 direction { get; set; }
		public TrailRenderer trailRenderer { get; private set; }

		public void Shoot(BulletData data, Vector3 position)
		{
			this.data = data;
			touchDemage = data.demage;
			this.position = position;
			data.direction.y = 0;
			direction = Vector3.Normalize(data.direction);
		}

		protected override void OnInitEnd()
		{
			trailRenderer = GetComponent<TrailRenderer>();
			MoveToSky();
		}
		protected override void PlayingUpdate()
		{
			Move();
		}
		protected override void AfterMatchUpdate()
		{
			Move();
		}

		private BulletData data { get; set; }

		private void Move()
		{
			position += direction * data.speed * GTime.timeStep;
		} 
	}

	public struct BulletData
	{
		public Vector3 direction;
		public float speed;
		public int demage;
	}
}
