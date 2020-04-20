using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicidalEnemyChaseState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyChaseState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    public void Enter()
    {
    }

    public void FixedUpdate()
    {
        m_enemyController.ChasePlayer();
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
