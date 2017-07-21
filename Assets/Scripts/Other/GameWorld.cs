using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FluffyUnderware.Curvy;
using MyGame.Hero;
using GameFactory;
using MyGame.Enemies;
using GameUtils;

namespace MyGame
{
	public class GameWorld : GameplayObject, IGameWorld
	{
		public Ship ship
		{
			get { return m_ship; }
			set
			{
				m_ship = value;
				Add(m_ship);
				Add(m_ship.mind);
			}
		}
		public Map map { get; set; }
		public WorldContainer container { get; private set; }

		public GameplayState state { get { return gameplay.state; } }
		public GameWorld world { get { return this; } }
		public Player player { get { return gameplay.player; } }

		public const float FLY_HEIGHT = 4;
		public const float SPAWN_OFFSET = 1.2f;

		public void Add<T>(T obj) where T : WorldObject
		{
			if (!obj || obj.isWorldInit)
			{
				return;
			}

			container.Add(obj);
		}
		public void Remove<T>(T obj) where T : WorldObject
		{
			if (!obj) return;

			container.Remove(obj);
			if (obj.openAllowed) OpenObject(obj);
			if (obj.distmantleAllowed) Dismantle(obj);
			Destroy(obj.gameObject);
		}

		public void MoveToShip(WorldObject body, bool useShipMagnetic = true)
		{
			float distance = Vector3.Distance(body.position, ship.position);
			if (distance > ship.mind.magnetDistance)
			{
				return;
			}

			float factor = (useShipMagnetic) ? ship.mind.magnetFactor : 1;
			float distanceFactor = ship.mind.magnetDistance / distance;
			float movement = factor * distanceFactor * MAGNETIC_SPEED * GTime.timeStep;
			body.position = Vector3.MoveTowards(body.position, ship.position, movement);
		}
		public void KillPlayer()
		{
		}
		public void CreateExplosion(ParticleSystem explosion, Vector3 position)
		{
			if (explosion == null)
			{
				return;
			}

			ParticleSystem explosionObject = Instantiate(explosion);
			explosionObject.transform.position = position;
			explosionObject.transform.SetParent(map.skyObjects);
		}
		public void Cleanup()
		{
		}

		protected override void OnChangeGameplay(GameplayState newState)
		{
			container.NotifyObjects(newState);
		}

		private Ship m_ship;
		private Camera m_camera;
		private bool m_lastModeType = false;
		private bool m_isDismantle = false;
		private float m_deltaScale = 1 - SLOW_TIMESCALE;
		private float m_targetTimeScale = 1;
		[SerializeField]
		private Material m_garbageMaterial;

		private const float MAGNETIC_SPEED = 2;
		private const float SLOW_TIMESCALE = 0.2f;
		private const float DISMANTLE_FORCE = 200;
		private const float NORMAL_TIME_SCALE = 1;
		private const float NORMAL_DT = 0.02f;
		private const float SLOWMO_DT = 0.01f;
		private const int GARBAGE_LAYER = 12;
		private const int DEATH_STARS_COUNT = 25;

		private void Awake()
		{
			container = new WorldContainer(this);
			m_camera = Camera.main;
		}

		private void OnTriggerExit(Collider other)
		{
			WorldObject obj = other.GetComponent<WorldObject>();

			if (obj && obj.exitAllowed)
			{
				obj.ExitFromWorld();
				Remove(obj);
				return;
			}

			Destroy(other.gameObject);
		}
		private void OpenObject(WorldObject obj)
		{
			if (!obj)
			{
				return;
			}

			player.AddPoints(obj.points);
			OpenBonuses(obj.bonuses, obj.position);
		}
		private void OpenBonuses(List<Pair<BonusType, int>> list, Vector3 spawn)
		{
			if (list == null)
			{
				return;
			}

			list.ForEach(bonus =>
			{
				Utils.DoAnyTimes(bonus.value, () =>
				{
					Bonus newBonus = Factory.GetBonus(bonus.key);
					newBonus.position = spawn;
				});
			});
		}
		private void Dismantle(WorldObject dismantleObject)
		{
			if (!dismantleObject)
			{
				return;
			}

			CreateExplosion(dismantleObject.explosion, dismantleObject.position);
		}
	}

	public interface IGameWorld : IGameplay
	{
		Ship ship { get; }
		Map map { get; }

		void Add<T>(T obj) where T : WorldObject;
		void Remove<T>(T obj) where T : WorldObject;

		void CreateExplosion(ParticleSystem explosion, Vector3 position);
		void MoveToShip(WorldObject body, bool useShipMagnetic = true);
	}

	public class WorldContainer
	{
		public WorldContainer(GameWorld world)
		{
			this.world = world;
			gameplay = world;
		}

		public bool isEnemiesEmpty { get { return m_enemies.Count == 0; } }

		public void Add<T>(T newObject) where T : WorldObject
		{
			if (!newObject) return;

			if (newObject is Enemy)
			{
				AddObject(m_enemies, newObject as Enemy);
			}
			else if (newObject is Bonus)
			{
				AddObject(m_bonuses, newObject as Bonus);
			}
			else
			{
				AddObject(m_other, newObject);
			}
		}
		public void Remove<T>(T obj) where T : WorldObject
		{
			if (obj is Enemy)
			{
				EraseObject(m_enemies, obj as Enemy);
			}
			else if (obj is Bonus)
			{
				EraseObject(m_bonuses, obj as Bonus);
			}
			else
			{
				EraseObject(m_other, obj);
			}
		}

		public override string ToString()
		{
			string result = "";

			result += "Enemies: " + m_enemies.Count + "\n";
			result += "Bonuses: " + m_bonuses.Count + "\n";
			result += "Other: " + m_other.Count + "\n";
			result += "Erase list: " + m_onErasing.Count;

			return result;
		}

		public void NotifyObjects(GameplayState newState)
		{
			m_enemies.ForEach(element => element.GameplayChange(newState));
			m_bonuses.ForEach(element => element.GameplayChange(newState));
			m_other.ForEach(element => element.GameplayChange(newState));
		}

		private List<Enemy> m_enemies = new List<Enemy>();
		private List<Bonus> m_bonuses = new List<Bonus>();
		private List<WorldObject> m_other = new List<WorldObject>();
		private List<object> m_onErasing = new List<object>();

		private IGameWorld world { get; set; }
		private IGameplay gameplay { get; set; }

		private void AddObject<T>(List<T> list, T newObject) where T : WorldObject
		{
			if (list.Exists(obj => obj.Equals(newObject)))
			{
				return;
			}

			newObject.Init(gameplay);
			list.Add(newObject);
		}
		private void EraseObject<T>(List<T> list, T eraseObject) where T : WorldObject
		{
			if (m_onErasing.Exists(element => element.Equals(eraseObject)))
			{
				return;
			}

			m_onErasing.Add(eraseObject);
			list.Remove(eraseObject);
			list.RemoveAll(element => element == null);
			m_onErasing.Remove(eraseObject);
		}
	}
}