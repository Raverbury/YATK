using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace STG
{
    public enum EnemyBulletType : int
    {
        ARROW_BLACK = 0,
        ARROW_DARK_RED = 1,
        ARROW_RED = 2,
        ARROW_DARK_PURPLE = 3,
        ARROW_PURPLE = 4,
        ARROW_DARK_BLUE = 5,
        ARROW_BLUE = 6,
        ARROW_DARK_SKY = 7,
        ARROW_SKY = 8,
        ARROW_DARK_GREEN = 9,
        ARROW_GREEN = 10,
        ARROW_DARK_YELLOW = 11,
        ARROW_LIGHT_YELLOW = 12,
        ARROW_YELLOW = 13,
        ARROW_ORANGE = 14,
        ARROW_WHITE = 15,

        AMULET_BLACK = 16,
        AMULET_DARK_RED = 17,
        AMULET_RED = 18,
        AMULET_DARK_PURPLE = 19,
        AMULET_PURPLE = 20,
        AMULET_DARK_BLUE = 21,
        AMULET_BLUE = 22,
        AMULET_DARK_SKY = 23,
        AMULET_SKY = 24,
        AMULET_DARK_GREEN = 25,
        AMULET_LIGHT_GREEN = 26,
        AMULET_GREEN = 27,
        AMULET_DARK_YELLOW = 28,
        AMULET_YELLOW = 29,
        AMULET_ORANGE = 30,
        AMULET_WHITE = 31,

        RICE_BLACK = 32,
        RICE_DARK_RED = 33,
        RICE_RED = 34,
        RICE_DARK_PURPLE = 35,
        RICE_PURPLE = 36,
        RICE_DARK_BLUE = 37,
        RICE_BLUE = 38,
        RICE_DARK_SKY = 39,
        RICE_SKY = 40,
        RICE_DARK_GREEN = 41,
        RICE_GREEN = 42,
        RICE_LIME = 43,
        RICE_DARK_YELLOW = 44,
        RICE_YELLOW = 45,
        RICE_ORANGE = 46,
        RICE_WHITE = 47,
    }

    public enum PlayerShotType : int
    {
        IN_REIMU_AMULET_RED = 0,
        IN_REIMU_AMULET_BLUE = 1,
        IN_MARISA_MISSILE_RED = 2,
        IN_MARISA_MISSILE_BLUE = 3,
        IN_YUKARI_NEEDLE_PURPLE = 4,
        IN_YUKARI_NEEDLE_YELLOW = 5,
    }

    public enum ItemType : ushort
    {
        POWER_ITEM = 0,
        POINT_ITEM = 1,
        BIG_POWER_ITEM = 2,
        BOMB_ITEM = 3,
        FULL_POWER_ITEM = 4,
        LIFE_ITEM = 5,
        STAR_ITEM = 6,
        TIME_ORB_ITEM = 7,
    }

    class Lifecycle
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            Application.targetFrameRate = 60;
        }
    }

    class Constant
    {
        public const int GAME_WIDTH = 384;
        public const int GAME_HEIGHT = 448;
        public const int GAME_CENTER_X = 192;
        public const int GAME_CENTER_Y = -224;

        public const int LAYER_SCREEN_EDGE = 16;
        public const int LAYER_PLAYABLE_AREA = 17;
        public const int LAYER_ENEMY_BULLET = 18;
        public const int LAYER_PLAYER_BULLET = 19;
        public const int LAYER_ENEMY = 20;
        public const int LAYER_PLAYER = 21;
        public const int LAYER_ITEM = 22;
        public const int LAYER_PLAYER_BOMB = 23;

        public static int GAME_BORDER_LEFT
        {
            get
            {
                return 10;
            }
        }
        public static int GAME_BORDER_RIGHT
        {
            get
            {
                return GAME_WIDTH - 10;
            }
        }
        public static int GAME_BORDER_TOP
        {
            get
            {
                return -10;
            }
        }
        public static int GAME_BORDER_BOTTOM
        {
            get
            {
                return -GAME_HEIGHT + 10;
            }
        }

        public static ushort ENEMY_BULLET_GRAZED_BIT = 0;
        public static ushort ENEMY_BULLET_DELETABLE_BIT = 1;
    }

    public static class MyExtensions
    {
        public static int Modulus(this int number, int divisor)
        {
            int rem = number % divisor;
            return rem < 0 ? rem + divisor : rem;
        }
    }

    [Serializable]
    public class Stat
    {
        public float baseStat;
        [HideInInspector]
        public float flatBonusStat;
        [HideInInspector]
        public Dictionary<string, float> percentageBonusStat = new();

        public float GetFinalStat()
        {
            return baseStat * GetTotalMultiplier() + flatBonusStat;
        }

        public float GetTotalMultiplier()
        {
            float multiplier = 1f;
            foreach (var kvp in percentageBonusStat)
            {
                multiplier *= 1f + kvp.Value;
            }
            return multiplier;
        }

        /// <summary>
        /// Add a bonus in the form of % (i.e. 20% bonus attack, or 0.2f)
        /// Multipliers stack multiplicatively
        /// </summary>
        /// <param name="effectKey"></param>
        /// <param name="bonusMultiplier"></param>
        public void AddMultiplier(string effectKey, float bonusMultiplier) {
            percentageBonusStat.Add(effectKey, bonusMultiplier);
        }

        public void RemoveMultiplier(string effectKey) {
            percentageBonusStat.Remove(effectKey);
        }
    }
}