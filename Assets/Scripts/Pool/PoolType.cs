namespace PoolTypes {
    [System.Serializable] public enum PoolType {
		EnemyType,
        ProjectileType,
        ObjectType,
        FxType,
	}
	[System.Serializable] public enum EnemyType {
		Rusher,
        Sniper,
        Suicidal,
	}
    [System.Serializable] public enum ProjectileType {

        ProjectileLevel1,
        ProjectileLevel2,
        ProjectileLevel3,
        ProjectileLevel4,
        EnemyProjectile

	}
    [System.Serializable] public enum FxType
    {
        MuzzleFlashLevel1,
        MuzzleFlashLevel2,
        MuzzleFlashLevel3,
        MuzzleFlashLevel4,
        MuzzleFlashEnemy,
        ImpactLevel1,
        ImpactLevel2,
        ImpactLevel3,
        ImpactLevel4,
        ImpactEnemy,
        EnemyStun,
        EnemyElectricalStun,
        Enemy_Spawn,
        Enemy_Dissolve,
        Enemy_Disintegration,
        SuicidalEnemy_Spawn,
        SuicidalEnemy_Dissolve,
        SuicidalEnemy_Disintegration,
        DestroyableObjectSmall,
        DestroyableObjectMedium,
        DestroyableObjectLarge,
    }
    [System.Serializable] public enum ObjectType {
        BpmGuiValues,
        Gun,
        DamageIndicator
	}
}
