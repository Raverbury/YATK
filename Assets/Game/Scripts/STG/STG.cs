using UnityEngine;

namespace STG
{
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
    }
}