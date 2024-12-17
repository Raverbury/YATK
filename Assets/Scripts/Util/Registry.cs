using UnityEngine;

[CreateAssetMenu(fileName = "Registry", menuName = "ScriptableObjects/Registry")]
public class Registry : ScriptableObject {
    public LoopableBGM BGM_08_EXTRA;
    public LoopableBGM BGM_08_MOKOU;
    public LoopableBGM BGM_10_MENU;
    public LoopableBGM BGM_NEW_GAMEOVER;

    public AudioClip SFX_PLAYER_MISS;
    public AudioClip SFX_PLAYER_POWERUP;
    public AudioClip SFX_PLAYER_SHOOT;
    public AudioClip SFX_SPELL_START;
    public AudioClip SFX_EXPLODE;
    public AudioClip SFX_ITEM_0;
    public AudioClip SFX_ITEM_1;
    public AudioClip SFX_PLAYER_GRAZE;
    public AudioClip SFX_TIMEOUT_0;
    public AudioClip SFX_TIMEOUT_1;
    public AudioClip SFX_PLAYER_EXTEND;
    public AudioClip SFX_MASTER_SPARK;
    public AudioClip SFX_TAN00;
    public AudioClip SFX_TAN01;
    public AudioClip SFX_GUN00;

    public AudioClip SFX_PAUSE;
    public AudioClip SFX_CONFIRM;
    public AudioClip SFX_SELECT;
    public AudioClip SFX_CANCEL;
    public AudioClip SFX_INVALID;
}