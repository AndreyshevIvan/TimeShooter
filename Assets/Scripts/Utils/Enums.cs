using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame
{
	[System.Serializable]
	public enum UnitType
	{
		BASE_ENEMY,
		BASE_ENEMY_HARD,
		ROCKET_COPTER,
		TARGET_TURRET,
		ANGLE_TURRET,
		BOSS,
	}

	[System.Serializable]
	public enum ShipType
	{
		STANDART,
	}

	[System.Serializable]
	public enum RoadType
	{
		BIG,
		LEFT,
		RIGHT,
		DIFF_1,
		DIFF_2,
		COME_TO_ME_RIGHT,
		COME_TO_ME_LEFT,
		COME_TO_ME_MIDDLE,
		COME_TO_ME_AND_BACK_RIGHT,
		COME_TO_ME_AND_BACK_LEFT,
		PLAYER_START,
	}

	[System.Serializable]
	public enum BonusType
	{
		STAR,
		HEALTH,
		MODIFICATION,
		TRIPLE_GUN,
		SQUARE_GUN,
		RANDOM_GUN,
	}

	[System.Serializable]
	public enum BarType
	{
		PLAYER_HEALTH,
		ENEMY_HEALTH,
	}

	[System.Serializable]
	public enum MapType
	{
		FIRST,
		SECOND,
		THIRD,
	}

	[System.Serializable]
	public enum AmmoType
	{
		// Enemy
		TARGET_TURRET,
		ANGLE_TURRET,
		COPTER_ROCKET,

		// Player
		PLAYER_BOMB,
		PLAYER_ROCKET,
		PLAYER_BULLET,
	}

	[System.Serializable]
	public enum ResType
	{
		LOCALE,
		LEVEL_PRICE,
	}

	[System.Serializable]
	public enum UpdType
	{
		FIXED,
		UI,
	}

	[System.Serializable]
	public enum Layer
	{
		UNTOUCH = 0,
		PLAYER_BULLET = 8,
		PLAYER = 9,
		FLY_ENEMY = 10,
		GROUND_ENEMY = 15,
		WORLD_BOX = 31,
	}

	[System.Serializable]
	public enum GameplayState
	{
		INITIALIZATION,
		BEFORE_PLAYING,
		PLAYING,
		PAUSE,
		END,
	}
}
