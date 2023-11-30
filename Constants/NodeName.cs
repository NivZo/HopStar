public static class NodeName {
    
    public enum Game
    {
        TransitionerLayer,
        ScreenTransitioner,
        TransitionPlayer,
    }
    
    public enum Stage
    {
        Stage
    }

    public enum Planet
    {
        PtBody,
        PtCollShape,
        PtSprite,
    }

    public enum Portal
    {
        PtlArea,
        PtlCollShape,
        PtlSprite
    }

    public enum Player
    {
        ShipBody,
        SpSprite,
        SpSpriteShader,
        SpCollPolygon,
        ControlledCamera,
        PlayerController,
        SpBottomMarker,
        SpEngineParticleEmitter,
        AnimationPlayer,
        PlayerBars,
        HealthBar,
        ShieldBar,
        ScrapCount,
    }

    public enum Orbit
    {
        OrbArea,
        OrbCollShape
    }

    public enum Weapon
    {
        WpnVisionArea,
        WpnVisionCollShape
    }

    public enum Projectile
    {
        Projectile,
        PrjSprite,
        PrjCollShape,
    }

    public enum Indicator
    {
        Indicator,
        DistanceLabel
    }

    public enum Combatant
    {
        Combatant,
        CombatantBody,
        CombatantSprite,
        CombatantCollPolygon,
        CombatantEngineParticleEmitter,
        CombatantAnimationPlayer,
    }

    public enum RayPathfinder
    {
        RayLeft,
        RayMiddle,
        RayRight
    }

    public enum Scrap
    {
        Scrap,
        PickupArea,
        PickupRange,
    }
}