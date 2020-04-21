using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemyIdleState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyIdleState(SuicidalEnemyController enemyController)
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
