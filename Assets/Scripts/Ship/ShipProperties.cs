using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MyGame
{
	[System.Serializable]
	public struct ShipProperties
	{
		public BulletData gunData;
		public float gunColdown;

		public BulletData rocketData;
		public float rocketColdown;

		public float bombColdown;

		public float shieldColdown;
		public float shieldDuration;

		public int health;
		public int rocketsDemage;
		public float magnetDistance;
		public float magnetFactor;
	}
}
