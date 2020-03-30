namespace PoolTypes {
    [System.Serializable] public enum PoolType {
		EnemyType,
        ProjectileType,
        ObjectType,
        FxType,
	}
	[System.Serializable] public enum EnemyType {
		EnemyBase,
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
        ImpactEnemy
    }
    [System.Serializable] public enum ObjectType {
        BpmGuiValues,
        Gun
	}
}
