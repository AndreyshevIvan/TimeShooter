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

		public int health;
		public float magnetDistance;
		public float magnetFactor;
	}
}
