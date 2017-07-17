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
		public SplineController roadController { get; protected set; }
		public ParticleSystem explosion { get { return m_explosion; } }

		public bool exitAllowed { get; protected set; }
		public bool openAllowed { get; protected set; }
		public bool distmantleAllowed { get; protected set; }
		public bool inGameBox { get { return m_box.Contain(position); } }

		public bool isWorldInit { get { return m_isInit; } }
		public int points { get; protected set; }
		public List<Pair<BonusType, int>> bonuses { get; protected set; }

		public void AddToRoad(CurvySpline road, float position)
		{
			StartCoroutine(SetSpline(road, position));
		}
		public void MoveToGround()
		{
			transform.SetParent(world.ground);
		}
		public void MoveToSky()
		{
			transform.SetParent(world.sky);
		}
		public void ExitFromWorld()
		{
			OnExitFromWorld();
		}

		protected BoundingBox m_box;

		protected IGameWorld world { get; private set; }
		protected IFactory factory { get { return world.factory; } }
		protected Rigidbody physicsBody { get; private set; }
		protected List<ParticleSystem> particles { get; private set; }
		protected List<GameObject> toDestroy { get; private set; }

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
			m_box = world.box;
			OnInitEnd();
			m_isInit = true;
		}
		protected virtual void OnInitEnd() { }

		protected virtual void OnExitFromWorld() { }
		protected void UpdatePositionOnField()
		{
			position = new Vector3(
				Mathf.Clamp(position.x, m_box.xMin, m_box.xMax),
				GameWorld.FLY_HEIGHT,
				position.z//Mathf.Clamp(position.z, m_box.zMin, m_box.zMax)
			);
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
