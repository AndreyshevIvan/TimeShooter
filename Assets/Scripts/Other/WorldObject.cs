using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FluffyUnderware.Curvy.Controllers;
using GameUtils;
using GameFactory;
using FluffyUnderware.Curvy;

namespace MyGame
{
	using System.Collections;
	using BonusCount = Pair<BonusType, int>;

	public abstract class WorldObject : GameplayObject
	{
		public Vector3 position
		{
			get { return transform.position; }
			set { transform.position = value; }
		}
		public ParticleSystem explosion { get { return m_explosion; } }

		public bool exitAllowed { get; protected set; }
		public bool openAllowed { get; protected set; }
		public bool distmantleAllowed { get; protected set; }

		public bool isWorldInit { get { return m_isInit; } }
		public int points { get; protected set; }
		public List<Pair<BonusType, int>> bonuses { get; protected set; }

		public void AddToRoad(CurvySpline road, float position, float speed)
		{
			m_roadSpeed = speed;
			StartCoroutine(SetSpline(road, position));
		}
		public void MoveToGround()
		{
			transform.SetParent(world.map.groundObjects);
		}
		public void MoveToSky()
		{
			transform.SetParent(world.map.skyObjects);
		}
		public void ExitFromWorld()
		{
			OnExitFromWorld();
		}

		protected float m_roadSpeed = 0;

		protected IGameWorld world { get; private set; }
		protected Rigidbody physicsBody { get; private set; }
		protected SplineController roadController { get; set; }
		protected List<ParticleSystem> particles { get; private set; }
		protected List<GameObject> toDestroy { get; private set; }
		protected enum CorrectType
		{
			X_ONLY,
			Z_ONLY,
			X_Z,
		}

		protected void Awake()
		{
			physicsBody = GetComponent<Rigidbody>();
			roadController = GetComponent<SplineController>();
			particles = Utils.GetAllComponents<ParticleSystem>(this);
			bonuses = new List<BonusCount>();
			toDestroy = new List<GameObject>();

			exitAllowed = true;
			openAllowed = false;
			distmantleAllowed = false;

			OnAwakeEnd();
		}
		protected virtual void OnAwakeEnd() { }

		protected sealed override void OnInitGameplayComplete()
		{
			GameWorld newWorld = gameplay as GameWorld;
			if (newWorld == null || newWorld.Equals(world))
			{
				return;
			}

			world = newWorld;
			OnInitEnd();
			m_isInit = true;
		}
		protected virtual void OnInitEnd() { }

		protected virtual void OnExitFromWorld() { }

		protected void UpdateRoadSpeed()
		{
			roadController.Speed = m_roadSpeed * GTime.timeScale;
		}

		protected void Exit()
		{
			openAllowed = false;
			distmantleAllowed = false;
			world.Remove(this);
		}
		protected void Cleanup()
		{
			Utils.DestroyAll(particles);
			points = 0;
		}
		protected void OnDestroy()
		{
			toDestroy.ForEach(element => { if (element) Destroy(element); });
		}

		[SerializeField]
		private ParticleSystem m_explosion;
		private bool m_isStartExit = false;
		private bool m_isInit = false;

		private IEnumerator SetSpline(CurvySpline spline, float position)
		{
			roadController.Spline = spline;
			yield return new WaitForFixedUpdate();
			roadController.Position = position;
		}
	}
}
