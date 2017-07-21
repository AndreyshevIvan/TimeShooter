using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MyGame
{
	public class Rocket : Body
	{
		public void SetTarget(RocketData newData, Vector3 position)
		{
			if (!newData.target)
			{
				ClearAndExit();
			}

			m_data = newData;
			this.position = position;
			transform.rotation = necessaryRotation;
			touchDemage = m_data.demage;
		}

		protected override void OnDemageTaked()
		{
			ClearAndExit();
		}
		protected override void PlayingUpdate()
		{
			if (!m_data.target || m_data.deltaAngle < angleToTarget)
			{
				ClearAndExit();
			}

			rotation = rotationAfterStep;
			position += movement;
		}
		protected override void OnEndGameplay()
		{
			ClearAndExit();
		}

		[SerializeField]
		private ParticleSystem m_tail;
		private RocketData m_data;

		private Vector3 direction
		{
			get { return m_data.target.position - position; }
		}
		private Vector3 movement
		{
			get { return transform.forward * m_data.speed * GTime.timeStep; }
		}
		private Quaternion necessaryRotation
		{
			get { return Quaternion.LookRotation(direction); }
		}
		private Quaternion rotation
		{
			get { return transform.rotation; }
			set { transform.rotation = value; }
		}
		private Quaternion rotationAfterStep
		{
			get { return Quaternion.Slerp(rotation, necessaryRotation, stepRotation); }
		}
		private float stepRotation
		{
			get { return m_data.rotationSpeed * GTime.timeStep; }
		}
		private float angleToTarget
		{
			get { return Quaternion.Angle(rotation, necessaryRotation); }
		}

		private void ClearAndExit()
		{
			ParticleSystem.MainModule module = m_tail.main;
			module.loop = false;
			Exit();
		}
	}

	public struct RocketData
	{
		public Transform target;
		public float diactivateTime;
		public float speed;
		public int demage;
		public int retargetCount;
		public float rotationSpeed;
		public float deltaAngle;
	}
}
