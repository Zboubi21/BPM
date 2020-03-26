namespace PoolTypes {
    [System.Serializable] public enum PoolType {
		EnemyType,
        ProjectileType,
        ObjectType,
	}
	[System.Serializable] public enum EnemyType {
		EnemyBase,
	}
    [System.Serializable] public enum ProjectileType {

        ProjectileLevel1,
        ProjectileLevel2,
        ProjectileLevel3

	}
    [System.Serializable] public enum ObjectType {
        BpmGuiValues,
        Gun
	}
}
