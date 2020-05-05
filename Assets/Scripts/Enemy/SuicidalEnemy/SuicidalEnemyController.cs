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
			new SuicidalEnemyIdleState(this),
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

        ChangeState(EnemyState.IdleState);
        // ChangeState(EnemyState.SpawnState);
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
    public EnemyState GetLastState()
    {
        return (EnemyState)m_sM.LastStateIndex;
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

    [Header("Move Speeds")]
    public float m_basicMoveSpeed = 6;
    public float m_explosionMoveSpeed = 10;

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

    // [Tooltip("Range when the enemy explode automatically")]
    // public Range m_automaticExplodeRange;

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

    [Header("Spawn")]
    public Spawn m_spawn;
    [Serializable] public class Spawn
    {
        public bool m_faceToPlayerWhenSpawned = true;
        public float m_waitTimeToSpawn = 0.5f;
    }

    [Space]

    public SuicidalEnemyAudioController m_audioController;

    [Header("FX & Shaders")]
    public EnemySpawnerShaderController m_shaderController;
    public ParticleSystem m_stunParticles;
    public ParticleSystem m_lowHealthParticles;
    public ParticleSystem m_detonationParticles;
    public ParticleSystem m_explosionParticles;
    [Space]
    public float m_waitTimeToDie = 0.5f;
    [Space]
    [SerializeField] GameObject[] m_weakSpots;
    [Space]
    [SerializeField] GameObject[] m_goToHideWhenExplode;
    [SerializeField] float m_waitTimeToReturnToPoolWhenExplode = 2;

    NavMeshAgent m_agent;
    EnemyCaraBase m_enemyChara;
    Animator m_animator;
    Collider[] enemyColliders;
    // bool m_canBeMouseOver = true;

#region Get / Set
    public StateMachine SM { get => m_sM; }
    public EnemyCaraBase EnemyChara { get => m_enemyChara; }
    // public bool CanBeMouseOver { get => m_canBeMouseOver; set => m_canBeMouseOver = value; }
#endregion

#region Unity Events
    void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        enemyColliders = GetComponentsInChildren<Collider>();
        SetupStateMachine();
        m_agent = GetComponent<NavMeshAgent>();
        SetEnemyAgentSpeed(m_basicMoveSpeed);
    }
    void OnDisable()
    {
        m_lowHealthParticles?.Stop(true);
    }
    void Start()
    {
        m_enemyChara = GetComponent<EnemyCaraBase>();
    }
    void FixedUpdate()
	{
		m_sM.FixedUpdate();
        ShowDebug();
        CheckMovements();
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
        // Gizmos.color = m_automaticExplodeRange.m_color;
        // Gizmos.DrawWireSphere(m_explosionRoot.position, m_automaticExplodeRange.m_range);
        Gizmos.color = m_explosion.m_explosionRange.m_color;
        Gizmos.DrawWireSphere(m_explosionRoot.position, m_explosion.m_explosionRange.m_range);
    }

    void ShowDebug()
    {
        m_debug.m_stateTxt.text = string.Format("{0}", m_sM.m_currentStateString);
        m_debug.m_lifeTxt.text = string.Format("{0}", m_enemyChara.CurrentLife);
    }

    bool m_isMoving = false;
    void CheckMovements()
    {
        Vector2 vel = new Vector2(m_agent.velocity.x, m_agent.velocity.z);
        // Debug.Log("Velocity = " + vel.normalized);
        // Debug.Log("Velocity = " + vel);
        if (vel != Vector2.zero && !m_isMoving)
        {
            m_isMoving = true;
            m_audioController?.On_StartToMove(true);
            // Debug.Log("Start to move");
        }
        else if (vel == Vector2.zero && m_isMoving)
        {
            m_isMoving = false;
            m_audioController?.On_StartToMove(false);
            // Debug.Log("Stop to move");
        }
    }

#region Public Functions
    public void On_SpawnEnemy()
    {
        if (m_spawn.m_faceToPlayerWhenSpawned)
            FaceToTarget(PlayerController.s_instance.transform.position);
        
        ActivateGOToHideWhenExplode(true);

        ChangeState(EnemyState.SpawnState);
        m_shaderController.On_StartSpawnShader();
        m_audioController?.On_Spawn();
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
    // public bool EnemyInClosedRangeOfPlayer()
    // {
    //     if (GetPlayerDistance() >= m_automaticExplodeRange.m_range)
    //     {
    //         return false;
    //     }
    //     else if (GetPlayerDistance() < m_automaticExplodeRange.m_range)
    //     {
    //         return true;
    //     }
    //     return false;
    // }
    bool m_isWaitingToExplode = false;
    IEnumerator m_waitTimeToExplodeCorout;
    public void On_EnemyEnterInSelfDestructionState()
    {
        if (!m_isWaitingToExplode)
        {
            m_isWaitingToExplode = true;
            m_audioController?.On_SelfDestructionIsActivated();
            m_audioController?.On_StartToMoveFast(true);
            m_detonationParticles?.Play(true);

            m_waitTimeToExplodeCorout = WaitTimeToExplode();
            StartCoroutine(m_waitTimeToExplodeCorout);
        }
    }
    IEnumerator WaitTimeToExplode()
    {
        m_audioController?.On_EnemyWaitToExplode();
        yield return new WaitForSeconds(m_selfDestruction.m_waitTimeToExplode);
        On_EnemyExplode();
        m_isWaitingToExplode = false;
    }
    void On_EnemyExplode()
    {
        SetAnimation("Explode");

        m_audioController?.On_EnemyExplode();
        StopEnemyMovement(true);

        m_explosionParticles?.Play(true);

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
            GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.suicidalBotFriendlyFire + (GameManager.Instance.scoreSystem.killSomething.bonusScoreOnFriendlyFire * enemies.Count-1));
        }
        StartCoroutine(WaitTimeToReturnToPoolWhenExplode());
    }
    IEnumerator WaitTimeToReturnToPoolWhenExplode()
    {
        ActivateGOToHideWhenExplode(false);
        yield return new WaitForSeconds(m_waitTimeToReturnToPoolWhenExplode);
        ReturnToPool();
    }
    void ActivateGOToHideWhenExplode(bool activate)
    {
        if (m_goToHideWhenExplode == null)
            return;
        for (int i = 0, l = m_goToHideWhenExplode.Length; i < l; ++i)
        {
            m_goToHideWhenExplode[i]?.SetActive(activate);
        }
    }

    public void On_EnemyGoingToDie(bool dieWithElectricalDamage = false)
    {
        if (m_waitTimeToExplodeCorout != null)
            StopCoroutine(m_waitTimeToExplodeCorout);
        // m_canBeMouseOver = false;
        On_ShowEnemyWeakSpot(false);

        m_stunParticles?.Stop(true);
        m_lowHealthParticles?.Stop(true);
        m_detonationParticles?.Stop(true);

        m_sM.ChangeState((int)EnemyState.DieState);

        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.beforeSelfDestructKill);

        if (dieWithElectricalDamage)
        {
            m_shaderController?.On_StartDisintegrationShader();
            m_audioController?.On_EnemyIsDisintegrate();
        }
        else
        {
            m_shaderController?.On_StartDissolveShader();
            m_audioController?.On_EnemyDie();
        }
        m_audioController?.On_StartToMoveFast(false);
    }
    public void On_EnemyDie()
    {
        ReturnToPool();
    }
    void ReturnToPool()
    {
        Spawned_Tracker spawnTracker = GetComponent<Spawned_Tracker>();
        if (spawnTracker != null)
            spawnTracker.CallDead();
        m_isWaitingToExplode = false;
        m_audioController?.On_StartToMoveFast(false);
        StopAllCoroutines();
        ObjectPooler.Instance.ReturnEnemyToPool(EnemyType.Suicidal, gameObject);
    }

    public void On_EnemyStartStun(bool startStun)
    {
        if (startStun)
        {
            var mainStunParticles = m_stunParticles.main;
            mainStunParticles.loop = true;
            m_stunParticles.Play(true);
        }
        else
        {
            var mainStunParticles = m_stunParticles.main;
            mainStunParticles.loop = false;
            m_stunParticles.Stop(true);
        }
    }

    public void On_EnemyIsLowHealth()
    {
        m_lowHealthParticles?.Play(true);
    }

    public void SetAnimation(string name)
    {
        m_animator.SetTrigger(name);
    }

    public void ActivateEnemyColliders(bool activate)
    {
        if (enemyColliders == null)
            return;
        for (int i = 0, l = enemyColliders.Length; i < l; ++i)
        {
            if (enemyColliders[i] != null)
            {   
                enemyColliders[i].enabled = activate;
            }
        }
    }
    public void SetEnemyAgentSpeed(float newSpeed)
    {
        m_agent.speed = newSpeed;
    }

    public void On_ShowEnemyWeakSpot(bool show)
    {
        if (m_weakSpots == null)
            return;
        for (int i = 0, l = m_weakSpots.Length; i < l; ++i)
        {
            m_weakSpots[i]?.SetActive(show);
        }
    }

#endregion

    void FaceToTarget(Vector3 targetPos)
    {
        transform.rotation = Quaternion.LookRotation(targetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
    }

}
