namespace Game.Combat
{
    public enum DamageType
    {
        DT_INVALID = 0,
        
        DT_ENEMY,
        DT_SHARED, // == DT_PLAYER
        DT_ENVIRONMENTAL,
        DT_INSTANTKILL,
        
        DT_MAX,
    }
}