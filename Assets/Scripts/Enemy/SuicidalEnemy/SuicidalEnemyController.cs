using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SuicidalEnemyStateEnum;
using UnityEngine.UI;
using PoolTypes;

[RequireComponent(typeof(NavMeshAgent))]
public class SuicidalEnemyController : MonoBehaviour
{

    [Header("Debug")]
      
#region State Machine
    [SerializeField] StateMachine m_sM = new StateMachine();
    void SetupStateMachine()
    {
        m_sM.AddStates(new List<IState> { 
			// new SuicidalEnemyIdleState(this),
			new SuicidalEnemySpawnState(this),
			new SuicidalEnemyChaseState(this),
            new SuicidalEnemySelfDestructionState(this),
            new SuicidalEnemyDieState(this),
            new SuicidalEnemyStunState(this),
            new SuicidalEnemyElectricalStunState(this),
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(EnemyState));
        if (m_sM.States.Count != playerStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in SuicidalEnemyController and SuicidalEnemyState");
        }

        // ChangeState((int)EnemyState.IdleState);
        ChangeState((int)EnemyState.SpawnState);
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

    [SerializeField] Debugs m_debug;
    [Serializable] class Debugs
    {
        public Text m_stateTxt;
        public Text m_lifeTxt;
    }

    [SerializeField] Transform m_explosionRoot;
    [SerializeField] DrawGizmosType m_drawGizmosType = DrawGizmosType.NeedSelected;
    enum DrawGizmosType
    {
        Always,
        NeedSelected,
    }
    
    [Space]

    public SelfDestruction m_selfDestruction;
    [Serializable] public class SelfDestruction
    {
        [Tooltip("Time to wait before enter in SelfDestructionState")]
        public float m_waitTimeToStartSelfDestruction = 0.2f;

        [Tooltip("Range when the enemy stop to move and enter in SelfDestructionState")]
        public Range m_startWaitToExplodeRange;
        public float m_waitTimeToExplode = 1;
    }

    [Tooltip("Range when the enemy explode automatically")]
    public Range m_automaticExplodeRange;

    public Explosion m_explosion;
    [Serializable] public class Explosion
    {
        [Tooltip("Range of damaged objects by the explosion")]
        public Range m_explosionRange;
        public LayerMask m_damagedLayer;
        public int m_playerDamage = 25;
        public int m_enemyDamage = 25;
        public int m_environmentDamage = 1;
    }
    [Serializable] public class Range
    {
        public float m_range = 1;
        public Color m_color = Color.red;
    }

    [Header("Delay")]
    public float m_waitTimeToSpawn = 1;
    public float m_waitTimeToDie = 0.25f;

    NavMeshAgent m_agent;
    EnemyCaraBase m_enemyChara;
    Animator m_animator;

#region Get / Set
    public StateMachine SM { get => m_sM; }
    public EnemyCaraBase EnemyChara { get => m_enemyChara; }
#endregion

#region Unity Events
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        SetupStateMachine();
        m_agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        m_enemyChara = GetComponent<EnemyCaraBase>();
    }
    void FixedUpdate()
	{
		m_sM.FixedUpdate();
        ShowDebug();
	}
	void Update()
	{
		m_sM.Update();
        // if (Input.GetKeyDown(KeyCode.C))
        //     ChangeState(EnemyState.ChaseState);
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
        Gizmos.color = m_selfDestruction.m_startWaitToExplodeRange.m_color;
        Gizmos.DrawWireSphere(m_explosionRoot.position, m_selfDestruction.m_startWaitToExplodeRange.m_range);
        Gizmos.color = m_automaticExplodeRange.m_color;
        Gizmos.DrawWireSphere(m_explosionRoot.position, m_automaticExplodeRange.m_range);
        Gizmos.color = m_explosion.m_explosionRange.m_color;
        Gizmos.DrawWireSphere(m_explosionRoot.position, m_explosion.m_explosionRange.m_range);
    }

    void ShowDebug()
    {
        m_debug.m_stateTxt.text = string.Format("{0}", m_sM.m_currentStateString);
        m_debug.m_lifeTxt.text = string.Format("{0}", m_enemyChara.CurrentLife);
    }

#region Public Functions
    public void On_SpawnEnemy()
    {
        ChangeState((int)EnemyState.SpawnState);
    }
    public void ChasePlayer()
    {
        m_agent.SetDestination(PlayerController.s_instance.transform.position);
    }
    public void StopEnemyMovement(bool stop)
    {
        m_agent.isStopped = stop;
    }
    public float GetPlayerDistance()
    {
        return Vector3.Distance(transform.position, PlayerController.s_instance.transform.position);
    }
    bool m_playerInRange = false;
    public bool EnemyInRangeOfPlayer()
    {
        if (GetPlayerDistance() >= m_selfDestruction.m_startWaitToExplodeRange.m_range && m_playerInRange)
        {
            m_playerInRange = false;
        }
        else if (GetPlayerDistance() < m_selfDestruction.m_startWaitToExplodeRange.m_range && !m_playerInRange)
        {
            m_playerInRange = true;
        }
        return m_playerInRange;
    }
    public void On_EnemyExplode()
    {
        List<EnemyCaraBase> enemies = new List<EnemyCaraBase>();
        List<DestroyableObjectController> destroyableObjs = new List<DestroyableObjectController>();

        Collider[] colliders = Physics.OverlapSphere(m_explosionRoot.position, m_explosion.m_explosionRange.m_range, m_explosion.m_damagedLayer);
        if (colliders != null)
        {
            for (int i = 0, l = colliders.Length; i < l; ++i)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    BPMSystem _BPMSystem = colliders[i].GetComponent<BPMSystem>();
                    if(_BPMSystem != null)
                    {
                        _BPMSystem.LoseBPM(m_explosion.m_playerDamage, transform);
                    }
                }
                else if (colliders[i].CompareTag("DestroyableObject"))
                {
                    DestroyableObject destroyableObject = colliders[i].GetComponent<DestroyableObject>();
                    if (destroyableObject != null && !destroyableObjs.Contains(destroyableObject.Controller))
                    {
                        destroyableObjs.Add(destroyableObject.Controller);
                        destroyableObject.TakeDamage(m_explosion.m_environmentDamage);
                    }

                    DestroyableObjectController destroyableObjectController = colliders[i].GetComponent<DestroyableObjectController>();
                    if (destroyableObjectController != null && !destroyableObjs.Contains(destroyableObjectController))
                    {
                        destroyableObjs.Add(destroyableObjectController);
                        destroyableObjectController.TakeDamage(m_explosion.m_environmentDamage);
                    }
                }
                else if (colliders[i].CompareTag("NoSpot") || colliders[i].CompareTag("WeakSpot"))
                {
                    ReferenceScipt refScript = colliders[i].GetComponent<ReferenceScipt>();
                    if(refScript != null)
                    {
                        if(refScript.cara != null && !enemies.Contains(refScript.cara))
                        {
                            enemies.Add(refScript.cara);
                            refScript.cara.TakeDamage(m_explosion.m_enemyDamage, 0, false, 0);
                        }
                    }
                }
            }
        }
        ReturnToPool();
    }
    public void On_EnemyDie()
    {
        ReturnToPool();
    }
    void ReturnToPool()
    {
        // gameObject.SetActive(false);
        ObjectPooler.Instance.ReturnEnemyToPool(EnemyType.Rusher, gameObject);
    }

    public void SetAnimation(string name)
    {
        m_animator.SetTrigger(name);
    }

#endregion

}
