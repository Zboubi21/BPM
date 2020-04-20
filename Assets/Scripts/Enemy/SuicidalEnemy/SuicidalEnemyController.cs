using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SuicidalEnemyStateEnum;

[RequireComponent(typeof(NavMeshAgent))]
public class SuicidalEnemyController : MonoBehaviour
{

    [SerializeField] DrawGizmosType m_drawGizmosType = DrawGizmosType.NeedSelected;
    enum DrawGizmosType
    {
        Always,
        NeedSelected,
    }

    [Header("Range")]
    [Tooltip("Range when the enemy stop to move and enter ")]
    public Range m_startWaitToExplodeRange;

    [Tooltip("Range when the enemy explode automatically")]
    public Range m_automaticExplodeRange;

    [Tooltip("Range of damaged objects by the explosion")]
    public Range m_explosionRange;
    [Serializable] public class Range
    {
        public float m_range = 1;
        public Color m_color = Color.red;
    }

    NavMeshAgent m_agent;
    
#region State Machine
    [SerializeField] StateMachine m_sM = new StateMachine();
    void SetupStateMachine()
    {
        m_sM.AddStates(new List<IState> { 
			new SuicidalEnemyIdleState(this),
			new SuicidalEnemyChaseState(this),
            new SuicidalEnemyChaseState(this),
            new SuicidalEnemySelfDestructionState(this),
            new SuicidalEnemyDieState(this),
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(EnemyState));
        if (m_sM.States.Count != playerStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in SuicidalEnemyController and SuicidalEnemyState");
        }

        ChangeState((int)EnemyState.IdleState);
    }
    public void ChangeState(EnemyState newState)
    {
		m_sM.ChangeState((int)newState);
	}
    public bool CurrentState(EnemyState state)
    {
        return m_sM.CurrentStateIndex == (int)state;
    }
	public bool LastState(EnemyState state)
    {
        return m_sM.LastStateIndex == (int)state;
    }
#endregion

#region Unity Events
    void Awake()
    {
        SetupStateMachine();
        m_agent = GetComponent<NavMeshAgent>();
    }
    void FixedUpdate()
	{
		m_sM.FixedUpdate();
	}
	void Update()
	{
		m_sM.Update();
        if (Input.GetKeyDown(KeyCode.C))
            ChangeState(EnemyState.ChaseState);
	}
	void LateUpdate()
	{
		m_sM.LateUpdate();
	}
    
    void OnDrawGizmos()
    {
        if (m_drawGizmosType == DrawGizmosType.Always)
            On_DrawGizmos();
    }
    void OnDrawGizmosSelected()
    {
        if (m_drawGizmosType == DrawGizmosType.NeedSelected)
            On_DrawGizmos();
    }
#endregion

    void On_DrawGizmos()
    {
        Gizmos.color = m_startWaitToExplodeRange.m_color;
        Gizmos.DrawWireSphere(transform.position, m_startWaitToExplodeRange.m_range);
        Gizmos.color = m_automaticExplodeRange.m_color;
        Gizmos.DrawWireSphere(transform.position, m_automaticExplodeRange.m_range);
        Gizmos.color = m_explosionRange.m_color;
        Gizmos.DrawWireSphere(transform.position, m_explosionRange.m_range);
    }

    public void ChasePlayer()
    {
        m_agent.SetDestination(PlayerController.s_instance.transform.position);
    }

}
