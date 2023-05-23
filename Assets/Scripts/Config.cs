public static class Config
{
    #region Scene Names

    public const string MAIN_MENU_SCENE_NAME = "Main Menu";
    public const string MAIN_SCENE_NAME = "Main";

    #endregion

    #region Tags

    public const string PROYECTILE_CONTAINER_TAG = "ProyectileContainer";
    public const string EFFECT_CONTAINER_TAG = "EffectContainer";
    public const string CROSSHAIR_TAG = "CrossHair";
    public const string PLAYER_TAG = "Player";

    #endregion

    #region Class Names

    public const string GUN_CLASS_NAME = "Gun";
    public const string MELEE_WEAPON_CLASS_NAME = "MeleeWeapon";

    #endregion

    #region Weapon System

    public const string GUN_CONTAINER_NAME = "Gun Container";
    public const string MELEE_WEAPON_CONTAINER_NAME = "MeleeWeapon Container";

    public const float EXPLOSIVE_DISABLE_TIME = 2f;
    public const float ROCKET_CONSTANT_SPEED = 25f;

    #endregion

    #region Camera Shake

    public const string SHAKE_FILE = "Cinemachine/6D Shake";

    public const float CAMERASHAKE_HIT_AMPLITUDE = 3f;
    public const float CAMERASHAKE_HIT_DURATION = .1f;

    public const float CAMERASHAKE_EXPLOSION_AMPLITUDE = 10f;
    public const float CAMERASHAKE_EXPLOSION_DURATION = .5f;

    #endregion

    #region Inventory

    public const float FAST_SWAP_GAMEPLAY_PANEL_SHOW_DURATION = 2f;

    #endregion

    #region Time Delays

    public const float SMALL_DELAY = .1f;
    public const float MEDIUM_DELAY = .2f;
    public const float LARGE_DELAY = .3f;
    public const float BIG_DELAY = .5f;

    #endregion

    #region Scene Transitions

    public const string CROSSFADE_TRANSITION = "Crossfade";
    public const string CROSSFADE_START_TRIGGER = "Start";
    public const string CROSSFADE_END_TRIGGER = "End";
    public const float START_TRANSITION_DURATION = .5f;
    public const float END_TRANSITION_DURATION = 1f;

    #endregion

    #region UI

    public const string ANIMATOR_SHOW_COUNTERS = "Appear";
    public const string ANIMATOR_HIDE_COUNTERS = "Disappear";

    #endregion

    #region SFX

    public const string CLICK_SFX = "Click";
    public const string PAUSE_SFX = "Pause";
    public const string WRONG_SFX = "Wrong";

    #endregion
}
