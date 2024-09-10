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
        ARROW_DARK_YELLOW = 1,
        ARROW_LIGHT_YELLOW = 12,
        ARROW_YELLOW = 13,
        ARROW_ORANGE = 14,
        ARROW_WHITE = 15,
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

        public static int ENEMY_BULLET_GRAZED_BIT = 0;
        public static int ENEMY_BULLET_DELETABLE_BIT = 1;
    }
}