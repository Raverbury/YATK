using UnityEngine;

public class ShotSheet : MonoBehaviour
{
    private static ShotSheet instance = null;
    #region arrows
    [Header("Arrows")]
    [SerializeField]
    private ShotData arrow_black;
    public static ShotData ARROW_BLACK {
        get {
            return instance.arrow_black;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_red;
    public static ShotData ARROW_DARK_RED {
        get {
            return instance.arrow_dark_red;
        }
    }
    [SerializeField]
    private ShotData arrow_red;
    public static ShotData ARROW_RED {
        get {
            return instance.arrow_red;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_purple;
    public static ShotData ARROW_DARK_PURPLE {
        get {
            return instance.arrow_dark_purple;
        }
    }
    [SerializeField]
    private ShotData arrow_purple;
    public static ShotData ARROW_PURPLE {
        get {
            return instance.arrow_purple;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_blue;
    public static ShotData ARROW_DARK_BLUE {
        get {
            return instance.arrow_dark_blue;
        }
    }
    [SerializeField]
    private ShotData arrow_blue;
    public static ShotData ARROW_BLUE {
        get {
            return instance.arrow_blue;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_sky;
    public static ShotData ARROW_DARK_SKY {
        get {
            return instance.arrow_dark_sky;
        }
    }
    [SerializeField]
    private ShotData arrow_sky;
    public static ShotData ARROW_SKY {
        get {
            return instance.arrow_sky;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_green;
    public static ShotData ARROW_DARK_GREEN {
        get {
            return instance.arrow_dark_green;
        }
    }
    [SerializeField]
    private ShotData arrow_green;
    public static ShotData ARROW_GREEN {
        get {
            return instance.arrow_green;
        }
    }
    [SerializeField]
    private ShotData arrow_dark_yellow;
    public static ShotData ARROW_DARK_YELLOW {
        get {
            return instance.arrow_dark_yellow;
        }
    }
    [SerializeField]
    private ShotData arrow_light_yellow;
    public static ShotData ARROW_LIGHT_YELLOW {
        get {
            return instance.arrow_light_yellow;
        }
    }
    [SerializeField]
    private ShotData arrow_yellow;
    public static ShotData ARROW_YELLOW {
        get {
            return instance.arrow_yellow;
        }
    }
    [SerializeField]
    private ShotData arrow_orange;
    public static ShotData ARROW_ORANGE {
        get {
            return instance.arrow_orange;
        }
    }
    [SerializeField]
    private ShotData arrow_white;
    public static ShotData ARROW_WHITE {
        get {
            return instance.arrow_white;
        }
    }
    #endregion

    #region weapons
    [Header("Weapons")]
    [SerializeField]
    private ShotData in_reimu_amulet_blue;
    public static ShotData IN_REIMU_AMULET_BLUE {
        get {
            return instance.in_reimu_amulet_blue;
        }
    }
    [SerializeField]
    private ShotData in_reimu_amulet_red;
    public static ShotData IN_REIMU_AMULET_RED {
        get {
            return instance.in_reimu_amulet_red;
        }
    }
    [SerializeField]
    private ShotData in_yukari_needle_purple;
    public static ShotData IN_YUKARI_NEEDLE_PURPLE {
        get {
            return instance.in_yukari_needle_purple;
        }
    }
    [SerializeField]
    private ShotData in_yukari_needle_yellow;
    public static ShotData IN_YUKARI_NEEDLE_YELLOW {
        get {
            return instance.in_yukari_needle_yellow;
        }
    }
    [SerializeField]
    private ShotData in_marisa_missile_blue;
    public static ShotData IN_MARISA_MISSILE_BLUE {
        get {
            return instance.in_marisa_missile_blue;
        }
    }
    [SerializeField]
    private ShotData in_marisa_missile_red;
    public static ShotData IN_MARISA_MISSILE_RED {
        get {
            return instance.in_marisa_missile_red;
        }
    }
    #endregion


    private void Awake() {
        if (instance != null) {
            Destroy(this);
        }
        instance = this;
    }
}