using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using MyGame.Enemies;
using GameSpawns;
using GameFactory;
using Malee;

namespace MyGame
{
	public sealed class Map : GameplayObject
	{
		public IFactory factory { get; set; }
		public Transform groundObjects { get { return m_groundObjects; } }
		public Transform skyObjects { get { return m_skyObjects; } }
		public float time { get; private set; }
		public float offset { get; private set; }
		public bool isTimeEnd { get { return m_finalTime <= time; } }
		public bool isReached
		{
			get
			{
				return isTimeEnd &&
				m_tmpSkySpawns.Count == 0 &&
				m_tmpGroundSpawns.Count == 0;
			}
		}
		public bool isMoveing { get; private set; }

		public void Play()
		{
		}

		protected override void PlayingUpdate()
		{
			MoveGround();
			CheckSkyUnits();
			CheckGroundUnits();
		}

		[SerializeField]
		private float m_finalTime;
		[SerializeField]
		private Transform m_groundObjects;
		[SerializeField]
		private Transform m_skyObjects;
		[SerializeField]
		private Transform m_ground;
		[SerializeField][Reorderable]
		private SkySpawnList m_skySpawns;
		[SerializeField][Reorderable]
		private GroundSpawnList m_groundSpawns;

		private List<FlySpawn> m_tmpSkySpawns;
		private List<GroundSpawn> m_tmpGroundSpawns;

		private float groundPosition { get { return GROUND_SPAWN_MARGIN + offset; } }

		private const float MOVE_SPEED = 1.6f;
		private const float GROUND_SPAWN_MARGIN = 30;
		private const float BASE_ENEMY_ROAD_OFFSET = 2;
		private const float COPTER_ROAD_OFFSET = 3;

		private void Awake()
		{
			time = 0;
			offset = 0;

			m_tmpSkySpawns = new List<FlySpawn>(m_skySpawns);
			m_tmpGroundSpawns = new List<GroundSpawn>(m_groundSpawns);
		}
		private void MoveGround()
		{
			if (time >= m_finalTime)
			{
				return;
			}

			float movement = MOVE_SPEED * Time.fixedDeltaTime;
			m_ground.transform.Translate(new Vector3(0, 0, -movement));

			time += Time.fixedDeltaTime;
			offset += movement;
		}
		private void CheckSkyUnits()
		{
			List<FlySpawn> ready = m_tmpSkySpawns.FindAll(x => x.time <= time);
			if (ready != null) ready.ForEach(spawn =>
			{
				m_tmpSkySpawns.Remove(spawn);
				SpawnSkyUnit(spawn);
			});
		}
		private void SpawnSkyUnit(FlySpawn spawn)
		{
			CurvySpline road = factory.GetRoad(spawn.road);

			for (int i = 0; i < spawn.count; i++)
			{
				Enemy enemy = factory.GetEnemy(spawn.enemy);
				enemy.transform.SetParent(m_skyObjects);
				enemy.roadController.Speed = spawn.speed;
				enemy.AddToRoad(road, GetRoadOffset(spawn.enemy) * i);
			}
		}

		private void CheckGroundUnits()
		{
			List<GroundSpawn> ready = m_tmpGroundSpawns.FindAll(x =>
				x.position.z <= groundPosition);
			if (ready != null) ready.ForEach(spawn =>
			{
				m_tmpGroundSpawns.Remove(spawn);
				SpawnGroundUnit(spawn);
			});
		}
		private void SpawnGroundUnit(GroundSpawn spawn)
		{
			Enemy newEnemy = factory.GetEnemy(spawn.enemy);
			newEnemy.position = spawn.position - new Vector3(0, 0, offset);
			newEnemy.MoveToGround();
		}

		private float GetRoadOffset(UnitType type)
		{
			switch (type)
			{
				case UnitType.BASE_ENEMY:
					return BASE_ENEMY_ROAD_OFFSET;
				case UnitType.ROCKET_COPTER:
					return COPTER_ROAD_OFFSET;
			}

			return 1;
		}
	}
}
