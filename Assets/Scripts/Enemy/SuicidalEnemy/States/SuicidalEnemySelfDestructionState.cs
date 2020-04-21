using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicidalEnemySelfDestructionState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemySelfDestructionState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    public void Enter()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    public void Exit()
    {
    }

}
