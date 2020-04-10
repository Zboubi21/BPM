namespace EnemyStateEnum {
	[System.Serializable] public enum EnemyState {

        Enemy_ChaseState,               // Numéro 0
		Enemy_IdleState,                // Numéro 1 (Lowest Priority)
        Enemy_AttackState,              // Numéro 2

        Enemy_RepositionState,          // Numéro 3
        Enemy_AgressiveState,           // Numéro 4 (Lowest Priority)
        Enemy_DefensiveState,           // Numéro 5 (Lowest Priority)

        Enemy_StunState,                // Numéro 6 (Low Priority)
        Enemy_ElectricalStunState,                // Numéro 7 (Low Priority)
        Enemy_DieState,                 // Numéro 8

        Enemy_SpawnState,               // Numéro 9

        //Enemy_VictoryState,		        // Numéro 8 (Lowest Priority)

    }
}
