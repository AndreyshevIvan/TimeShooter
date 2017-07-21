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
		public Transform groundObjects { get { return m_groundObjects; } }
		public Transform skyObjects { get { return m_skyObjects; } }
		public float offset { get; private set; }

		protected override void PlayingUpdate()
		{
			MoveGround();
		}

		[SerializeField]
		private float m_finalTime;
		[SerializeField]
		private Transform m_groundObjects;
		[SerializeField]
		private Transform m_skyObjects;
		[SerializeField]
		private Transform m_ground;
		[SerializeField]
		private Transform m_mapBorder;

		private float groundPosition { get { return GROUND_SPAWN_MARGIN + offset; } }

		private const float MOVE_SPEED = 4;
		private const float GROUND_SPAWN_MARGIN = 30;
		private const float BASE_ENEMY_ROAD_OFFSET = 2;
		private const float COPTER_ROAD_OFFSET = 3;

		private void Awake()
		{
			offset = 0;
		}
		private void MoveGround()
		{
		}
	}
}
