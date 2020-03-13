namespace EnemyStateEnum {
	[System.Serializable] public enum EnemyState {

        Enemy_ChaseState,               // Numéro 1
		Enemy_IdleState,                // Numéro 0 (Lowest Priority)
        Enemy_AttackState,              // Numéro 2

        //Enemy_AgressiveState,           // Numéro 3 (Lowest Priority)
        //Enemy_DefensiveState,           // Numéro 4 (Lowest Priority)
        //Enemy_CrouchState,              // Numéro 5 (Lowest Priority)

        Enemy_StunState,                // Numéro 6 (Low Priority)

        Enemy_DieState,                 // Numéro 7

        //Enemy_VictoryState,		        // Numéro 8 (Lowest Priority)

    }
}
