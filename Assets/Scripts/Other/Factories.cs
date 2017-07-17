using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy;
using MyGame.Hero;
using MyGame.Enemies;
using MyGame;
using Malee;

namespace GameFactory
{
	public class Factories : MonoBehaviour, IFactory
	{
		public GameWorld world { get; set; }

		public Map GetMap(MapType key)
		{
			Map map = m_maps.Find(pair => pair.key.Equals(key)).value;
			return Instantiate(map);
		}

		public Enemy GetEnemy(UnitType type)
		{
			Enemy origin = m_enemies.Find(pair => pair.key == type).value;
			Enemy enemy = Instantiate(origin, INVISIBLE_SPAWN, Quaternion.identity);
			world.Add(enemy);
			enemy.type = type;
			return enemy;
		}

		public CurvySpline GetRoad(RoadType type)
		{
			return m_roads.Find(pair => pair.key == type).value;
		}

		public Ship GetShip(ShipType type)
		{
			Ship original = m_ships.Find(pair => pair.key == type).value;
			Ship ship = Instantiate(original, INVISIBLE_SPAWN, Quaternion.identity);
			return ship;
		}

		public Bonus GetBonus(BonusType type)
		{
			Bonus bonus = Instantiate(m_bonuses. Find(pair => pair.key == type).value);
			bonus.type = type;
			world.Add(bonus);
			return bonus;
		}

		public HealthBar GetPlayerHealthBar()
		{
			HealthBarPair pair = m_healthBars.Find(x => x.key == BarType.PLAYER_HEALTH);
			HealthBar bar = Instantiate(pair.value);
			return bar;
		}

		public HealthBar GetEnemyHealthBar()
		{
			HealthBarPair pair = m_healthBars.Find(x => x.key == BarType.ENEMY_HEALTH);
			HealthBar bar = Instantiate(pair.value);
			return bar;
		}

		public Body GetAmmo(AmmoType type)
		{
			Body ammo = Instantiate(m_ammo.Find(pair => pair.key == type).value);
			world.Add(ammo);
			return ammo;
		}

		[SerializeField]
		private List<MapPair> m_maps;
		[SerializeField][Reorderable]
		private EnemiesFactoryList m_enemies;
		[SerializeField]
		private List<SplinePair> m_roads;
		[SerializeField]
		private List<ShipPair> m_ships;
		[SerializeField][Reorderable]
		private BonusesFactoryList m_bonuses;
		[SerializeField][Reorderable]
		private HealthBarsList m_healthBars;
		[SerializeField][Reorderable]
		private AmmoFactoryList m_ammo;

		private Vector3 INVISIBLE_SPAWN = new Vector3(1000, 1000, 1000);
	}

	public interface IFactory
	{
		Map GetMap(MapType type);
		Enemy GetEnemy(UnitType type);
		Ship GetShip(ShipType type);
		CurvySpline GetRoad(RoadType type);
		Bonus GetBonus(BonusType type);
		HealthBar GetPlayerHealthBar();
		HealthBar GetEnemyHealthBar();
		Body GetAmmo(AmmoType type);
	}

	[System.Serializable]
	public class MapPair : Pair<MapType, Map> { }
	[System.Serializable]
	public class EnemiesPair : Pair<UnitType, Enemy> { }
	[System.Serializable]
	public class BonusPair : Pair<BonusType, Bonus> { }
	[System.Serializable]
	public class SplinePair : Pair<RoadType, CurvySpline> { }
	[System.Serializable]
	public class ShipPair : Pair<ShipType, Ship> { }
	[System.Serializable]
	public class AmmoPair : Pair<AmmoType, Body> { }
	[System.Serializable]
	public class HealthBarPair : Pair<BarType, HealthBar> { }

	[System.Serializable]
	public class BonusesFactoryList : ReordarableList<BonusPair> { }
	[System.Serializable]
	public class EnemiesFactoryList : ReordarableList<EnemiesPair> { }
	[System.Serializable]
	public class AmmoFactoryList : ReordarableList<AmmoPair> { }
	[System.Serializable]
	public class HealthBarsList : ReordarableList<HealthBarPair> { }
}
