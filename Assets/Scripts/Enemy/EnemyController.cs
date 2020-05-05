using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using System;
using UnityEngine.UI;
using PoolTypes;
using UnityEngine.Animations.Rigging;

public class EnemyController : MonoBehaviour
{
    public DEBUG _debug = new DEBUG();
    [Serializable]
    public class DEBUG
    {
        public bool useGizmos;
        public bool useDebugLogs;

        public Text m_stateText;
        public Text m_lifeText;

        public Image stunResistance;
        public Image stunTime;

        public GameObject m_destinationImage;

        public float obstacleAvoidance = 3f;
        public MultiAimConstraint aimConstraint;
        public GameObject boneToMove;
    }

    public Spawn m_spawn;
    [Serializable]
    public class Spawn
    {
        public float m_timeToSpawn = 3;
        public bool m_faceToPlayerWhenSpawned = true;
    }

    #region State Machine

    public StateMachine m_sM = new StateMachine();
    public float maxTimeInStates;
    public float m_waitTimeToDie = 2;
    public virtual void OnEnable()
    {
        EnemyCantShoot = false;
        cara.enabled = true;
        agent.enabled = true;
        EnableDisableCollider(true);
        ChangeState((int)EnemyState.Enemy_IdleState);
    }
    [Header("VFX")]
    public FxType shockVFX;
    public FxType electricalStunVFX;
    [SerializeField] ParticleSystem m_stunPS;
    [SerializeField] ParticleSystem m_lowHealthPS;
    [SerializeField] EnemySpawnerShaderController m_shaderController;

    public void ChangeState(int i)
    {
        m_sM.ChangeState(i);
    }

    public int GetLastStateIndex()
    {
        return m_sM.LastStateIndex;
    }

    #endregion

    int choosenCover;

    EnemyCara cara;
    WeaponEnemyBehaviour weaponBehavior;
    NavMeshAgent agent;
    GameManager manager;
    EnemyAudioController audioControl;
    Transform target;
    Vector3 currentTarget;
    ObjectPooler objectPooler;
    PlayerController playerController;
    Animator anim;
    RigBuilder rigBuilder;

    float distanceToTarget;
    float distanceToPlayer;
    bool _enemyCanShoot;
    bool isInMotion;
    bool hasShoot;
    Collider[] enemyColliders;
    public Quaternion finalStateOfTheBoneRotation;
    [Tooltip("Pour que l'ennemies ne tir pas dans les pieds du player")]
    public float YOffset = 1f;

    #region Get Set
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Transform Player { get => target; set => target = value; }

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public EnemyCara Cara { get => cara; set => cara = value; }
    public bool EnemyCantShoot { get => _enemyCanShoot; set => _enemyCanShoot = value; }
    public Vector3 CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public WeaponEnemyBehaviour WeaponBehavior { get => weaponBehavior; set => weaponBehavior = value; }
    public ObjectPooler ObjectPooler { get => objectPooler; set => objectPooler = value; }
    public bool IsInMotion { get => isInMotion; set => isInMotion = value; }
    public EnemyAudioController AudioControl { get => audioControl; set => audioControl = value; }
    public float DistanceToPlayer { get => distanceToPlayer; set => distanceToPlayer = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public Animator Anim { get => anim; set => anim = value; }
    public bool HasShoot { get => hasShoot; set => hasShoot = value; }

    public bool CanBeMouseOver { get => m_canBeMouseOver; set => m_canBeMouseOver = value; }
    public RigBuilder RigBuilder { get => rigBuilder; set => rigBuilder = value; }
    #endregion

    public void Awake()
    {
        SetupStateMachine();
        agent = GetComponent<NavMeshAgent>();
        cara = GetComponent<EnemyCara>();
        weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
        audioControl = GetComponent<EnemyAudioController>();
        manager = GameManager.Instance;
        objectPooler = ObjectPooler.Instance;
        anim = GetComponent<Animator>();
        enemyColliders = GetComponentsInChildren<Collider>();
        rigBuilder = GetComponent<RigBuilder>();
        HasShoot = false;
        //if (_debug.aimConstraint != null)
        //{
        //    WeightedTransformArray sourceObjects = _debug.aimConstraint.data.sourceObjects;
        //    sourceObjects.SetTransform(0, PlayerController.s_instance.m_references.m_cameraPivot);
        //    _debug.aimConstraint.data.sourceObjects = sourceObjects;
        //}
    }

    void SetupStateMachine()
    {
        m_sM.AddStates(new List<IState> {
            new ChaseState(this),				// 0 = Chase
            new IdleState(this),				// 1 = Idle
			new AttackState(this),				// 2 = Attack
            new RepositionState(this),          // 3 = Reposition
            new AgressiveState(this),           // 4 = Agressive
            new DefensiveState(this),           // 5 = Defensive
            new StunState(this),				// 6 = Stun
            new ElectricalStunState(this),		// 7 = Elec Stun
			new DieState(this),				    // 8 = Die
			new SpawnState(this),				// 9 = Spawn
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(EnemyState));
        if (m_sM.States.Count != playerStateNames.Length)
        {
            Debug.LogError("You need to have the same number of State in PlayerController and PlayerStateEnum");
        }

        ChangeState((int)EnemyState.Enemy_IdleState);
    }

    private void Start()
    {
        Player = PlayerController.s_instance.gameObject.transform;
        playerController = PlayerController.s_instance;
        

        //currentTarget = FindBestSpotsInRangeOfTarget(Player);

        DistanceToTarget = GetTargetDistance(currentTarget);
        DistanceToPlayer = GetTargetDistance(Player.position);
    }

    private void Update()
    {
        m_sM.Update();

        #region DEBUG
        // #if UNITY_EDITOR
        if (_debug.useGizmos)
        {
            _debug.m_stateText.text = string.Format("{0}", m_sM.m_currentStateString);
            _debug.m_lifeText.text = string.Format("{0}", cara.CurrentLife);
            if (!m_sM.CompareState((int)EnemyState.Enemy_ElectricalStunState))
            {
                //_debug.stunResistance.fillAmount = Mathf.InverseLerp(Cara._enemyCaractéristique._stunResistance.percentLifeBeforeGettingStuned, 0, Cara.CurrentTimeForStunResistance);
                _debug.stunTime.fillAmount = Mathf.InverseLerp(0, debugStunTime, cara.CurrentTimeForStun);
            }
            else if (!m_sM.CompareState((int)EnemyState.Enemy_StunState))
            {
                _debug.stunResistance.fillAmount = Mathf.InverseLerp(Cara._enemyCaractéristique._stunResistance.timeOfElectricalStunResistance, 0, Cara.CurrentTimeForElectricalStun);
                _debug.stunTime.fillAmount = Mathf.InverseLerp(0, debugStunTime, cara.CurrentTimeForElectricalStun);
            }

            //_debug.spine.LookAt(Player.position);
        }
        _debug.m_stateText.gameObject.SetActive(_debug.useGizmos);
        _debug.m_lifeText.gameObject.SetActive(_debug.useGizmos);
        // #endif
        #endregion

        DistanceToTarget = GetTargetDistance(currentTarget);
        DistanceToPlayer = GetTargetDistance(Player.position);
        //_debug.aimSpine = playerController.transform;

    }

    void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    void LateUpdate()
    {
        if(finalStateOfTheBoneRotation.eulerAngles != Vector3.zero && m_sM.CompareState((int)EnemyState.Enemy_AttackState))
        {
            _debug.boneToMove.transform.rotation = finalStateOfTheBoneRotation;
        }
        m_sM.LateUpdate();
    }

    public float GetTargetDistance(Vector3 target)
    {
        return Vector3.Distance(target, transform.position);
    }


    public IEnumerator MaxTimeInThatState(float time, EnemyState state)
    {
        yield return new WaitForSeconds(time);
        if (m_sM.CompareState((int)state))
        {
            ChangeState((int)EnemyState.Enemy_RepositionState);
        }
    }

    #region NPC motion state methods

    bool hasFoundACover = false;
    public Vector3 FindBestSpotsInRangeOfTarget(Transform target)
    {

        Vector3 newTarget;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 lastPoint = Vector3.Lerp(target.position, transform.position, Mathf.InverseLerp(0, distance, weaponBehavior._attack.rangeRadius));
        if (hasFoundACover && manager.AllUsedCover.Count > 0)
        {
            manager.AllUsedCover.RemoveAt(choosenCover); // -1 pour retomber sur l'index exacte (on fait +1 plus loin pour avoir choosenCover =0 : " je n'ai pas trouvé de cover")
            hasFoundACover = false;
        }

        #region Find all cover around the player
        Collider[] allColInSphere = Physics.OverlapSphere(lastPoint, weaponBehavior._attack.rangeRadius);
        List<GameObject> allCoverInSphere = new List<GameObject>();

        for (int i = 0, l = allColInSphere.Length; i < l; ++i)
        {
            if (allColInSphere[i].CompareTag("PointChaud"))
            {
                bool denied = false;
                if (manager.AllUsedCover.Count > 0)
                {
                    for (int a = 0, m = manager.AllUsedCover.Count; a < m; ++a)
                    {
                        if (allColInSphere[i].gameObject == manager.AllUsedCover[a])
                        {
                            denied = true;
                            break;
                        }
                    }
                }
                if (!denied)
                {
                    allCoverInSphere.Add(allColInSphere[i].gameObject);
                    hasFoundACover = true;
                }
            }
        }

        #endregion

        if (allCoverInSphere.Count == 0 || !hasFoundACover)  // The NPC hasn't found a cover
        {
            int count = 200;
            hasFoundACover = false;
            bool playerIsOnNavMesh = true;
            while (count > 0 && !m_sM.CompareState((int)EnemyState.Enemy_DieState))
            {
                //Choisi un point aléatoire dans un cercle de la taille de la range du NPC
                Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * weaponBehavior._attack.rangeRadius;
                if (playerIsOnNavMesh)
                {
                    newTarget = new Vector3(randomPointInCircle.x + lastPoint.x, lastPoint.y, randomPointInCircle.y + lastPoint.z);
                }
                else
                {
                    newTarget = new Vector3(randomPointInCircle.x + lastPoint.x, transform.position.y, randomPointInCircle.y + lastPoint.z);
                }
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(newTarget, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return newTarget;
                }
                else
                {
                    playerIsOnNavMesh = false;
                }
                count--;
            }
            return transform.position; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
        }
        else // The NPC has found a cover
        {
            int randomIndex = UnityEngine.Random.Range(0, allCoverInSphere.Count - 1);
            newTarget = allCoverInSphere[randomIndex].transform.position;
            manager.AllUsedCover.Add(allCoverInSphere[randomIndex]);
            choosenCover = randomIndex; // +1 pour avoir 0 = je n'ai pas trouvé de cover
            return newTarget;
        }
    }

    public Vector3 MoveForward(Transform target)
    {
        int count = 200;
        Vector3 newTarget;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 newPoint = Vector3.Lerp(transform.position, target.position, Cara.EnemyArchetype._rateOfAgressivity /*Mathf.InverseLerp(0, distance, weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfAgressivity)*/);
        bool playerIsOnNavMesh = true;
        while (count > 0 && !m_sM.CompareState((int)EnemyState.Enemy_DieState))
        {
            //Choisi un point aléatoire dans un cercle de la taille de la range du NPC mutliplié par un coefficiant d'agressivité
            Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle /* (weaponBehavior._attack.rangeRadius * (Cara.EnemyArchetype._rateOfAgressivity/2))*/;

            if (playerIsOnNavMesh)
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, newPoint.y, randomPointInCircle.y + newPoint.z);
            }
            else
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, transform.position.y, randomPointInCircle.y + newPoint.z);
            }
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return newTarget;
            }
            else
            {
                playerIsOnNavMesh = false;
            }
            count--;
        }
        return newPoint; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
    }

    public Vector3 MoveBackward(Transform target)
    {
        int count = 200;
        Vector3 newTarget;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 newPoint = Vector3.LerpUnclamped(target.position, transform.position, 1 /*+ weaponBehavior._attack.rangeRadius **/+ Cara.EnemyArchetype._rateOfDefensivity);
        bool playerIsOnNavMesh = true;
        while (count > 0 && !m_sM.CompareState((int)EnemyState.Enemy_DieState))
        {
            //Choisi un point aléatoire dans un cercle de radius 1;
            Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle /* (weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfDefensivity)*/;
            if (playerIsOnNavMesh)
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, newPoint.y, randomPointInCircle.y + newPoint.z);
            }
            else
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, transform.position.y, randomPointInCircle.y + newPoint.z);
            }
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return newTarget;
            }
            else
            {
                playerIsOnNavMesh = false;
            }
            count--;
        }
        return newPoint; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
    }

    public Vector3 ChaseATarget(Transform target)
    {
        int count = 200;
        Vector3 newTarget;
        Vector3 newPoint = target.position;
        bool playerIsOnNavMesh = true;
        while (count > 0 && !m_sM.CompareState((int)EnemyState.Enemy_DieState))
        {
            //Choisi un point aléatoire dans un cercle de radius 1;
            Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * weaponBehavior._attack.rangeOfAttackNoMatterWhat;/* (weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfDefensivity)*/;
            if (playerIsOnNavMesh)
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, newPoint.y, randomPointInCircle.y + newPoint.z);
            }
            else
            {
                newTarget = new Vector3(randomPointInCircle.x + newPoint.x, transform.position.y, randomPointInCircle.y + newPoint.z);
            }
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                //Debug.Log("J'ai trouver une destination ! " + this);
                return newTarget;
            }
            else
            {
                playerIsOnNavMesh = false;
            }
            //Debug.Log(this +" : "+ count);
            count--;
        }
        ChangeState((int)EnemyState.Enemy_RepositionState);
        return transform.position; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
    }


    public void AnimationBlendTree()
    {
        anim.SetFloat("VelocityX", transform.right.normalized.x);
        anim.SetFloat("VelocityY", transform.forward.normalized.y);
    }

    #endregion

    #region Random methods
    public bool ThrowBehaviorDice(float value)
    {
        float random = UnityEngine.Random.Range(0f, 100f);
        if (random < value)
        {
            return true;
        }
        return false;
    }
    public int Choose(float[] allChances)
    {
        float total = 0;

        for (int i = 0, l = allChances.Length; i < l; ++i)
        {
            total += allChances[i];
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < allChances.Length; i++)
        {
            if (randomPoint < allChances[i])
            {
                return i;
            }
            else
            {
                randomPoint -= allChances[i];
            }
        }
        return allChances.Length - 1;
    }
    #endregion

    #region Is Stun State
    float debugStunTime;
    public IEnumerator IsStun(float time, EnemyState state)
    {
        EnemyCantShoot = true;
        debugStunTime = time;
        if (state == EnemyState.Enemy_StunState)
        {
            Level.AddFX(shockVFX, transform.position, Quaternion.identity);
        }
        else if (state == EnemyState.Enemy_ElectricalStunState)
        {
            Level.AddFX(electricalStunVFX, transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(time);
        EnemyCantShoot = false;
        if (m_sM.CompareState((int)state))
        {
            ChangeState((int)m_sM.LastStateIndex);
        }
    }
    #endregion

    #region NPC is dead methods

    public void KillNPC(float time)
    {
        EnemyCantShoot = Cara.IsDead;
        EnableDisableCollider(false);
        StartCoroutine(OnWaitForAnimToEnd(time));
    }
    IEnumerator OnWaitForAnimToEnd(float time)
    {
        cara.enabled = false;
        agent.enabled = false;
        yield return new WaitForSeconds(time);    //Animation time
        Spawned_Tracker spawnTracker = GetComponent<Spawned_Tracker>();
        if (spawnTracker != null)
        {
            spawnTracker.CallDead();
        }
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if (poolTracker != null)
        {
            Destroy(poolTracker);
        }
        anim.SetLayerWeight(1, 0);
        ObjectPooler.Instance.ReturnEnemyToPool(EnemyType.Rusher, gameObject);
    }

    void EnableDisableCollider(bool b)
    {
        for (int i = 0, l = Cara._debug.allCollider.Length; i < l; ++i)
        {
            Cara._debug.allCollider[i].enabled = b;
        }
    }
    #endregion

    #region Instantiate and destroy object
    public GameObject OnInstantiate(GameObject obj, Vector3 trans)
    {
        return Instantiate(obj, trans, Quaternion.identity);
    }
    public GameObject OnInstantiate(GameObject obj, Vector3 trans, Transform parent)
    {
        return Instantiate(obj, trans, Quaternion.identity, parent);
    }
    public void DestroyObj(GameObject obj)
    {
        Destroy(obj);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (_debug.useGizmos)
        {
            if (agent != null && weaponBehavior != null && cara != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, weaponBehavior._attack.rangeRadius);
                Gizmos.color = Color.green;
                //Gizmos.DrawWireSphere(new Vector3(weaponBehavior._SMG.firePoint.transform.position.x, weaponBehavior._SMG.firePoint.transform.position.y, weaponBehavior._SMG.firePoint.transform.position.z + weaponBehavior._attack.rangeRadius /*- weaponBehavior._attack.enemyAttackDispersement*2)*/* weaponBehavior._attack._debugGizmos), weaponBehavior._attack.enemyAttackDispersement);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, weaponBehavior._attack.rangeOfAttackNoMatterWhat);
                //Gizmos.DrawWireSphere(Vector3.LerpUnclamped(transform.position, Vector3.zero, range), 1f);

            }
            else
            {
                agent = GetComponent<NavMeshAgent>();
                weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
                cara = GetComponent<EnemyCara>();
            }
        }
    }

#region Enemy
    public void On_SpawnEnemy()
    {
        if (m_spawn.m_faceToPlayerWhenSpawned)
            FaceToTarget(PlayerController.s_instance.transform.position);
        ChangeState((int)EnemyState.Enemy_SpawnState);
        m_shaderController.On_StartSpawnShader();
        audioControl?.On_Spawn();
    }
    public void On_EnemyGoingToDie(bool dieWithElectricalDamage = false)
    {
        m_lowHealthPS?.Stop(true);
        if (dieWithElectricalDamage)
        {
            m_shaderController?.On_StartDisintegrationShader();
            audioControl?.On_EnemyIsDisintegrate();
        }
        else
        {
            m_shaderController?.On_StartDissolveShader();
            audioControl?.On_EnemyDie();
        }
    }
#endregion

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

    bool m_canBeMouseOver = true;


    public void On_EnemyIsMouseOver(bool isMouseOver)
    {
        // for (int i = 0, l = m_weakSpots.Length; i < l; ++i)
        // {
        //     m_weakSpots[i].SetActive(isMouseOver);
        // }
        // for (int i = 0, l = m_noWeakSpots.Length; i < l; ++i)
        // {
        //     m_noWeakSpots[i].SetActive(!isMouseOver);
        // }
    }

    public void On_EnemyIsLowHealth()
    {
        m_lowHealthPS?.Play(true);
    }
    public void On_EnemyIsStunned(bool isStun)
    {
        if (isStun)
        {
            var mainStunParticles = m_stunPS.main;
            mainStunParticles.loop = true;
            m_stunPS.Play(true);
        }
        else
        {
            var mainStunParticles = m_stunPS.main;
            mainStunParticles.loop = false;
            m_stunPS.Stop(true);
        }
    }

    void FaceToTarget(Vector3 targetPos)
    {
        
        transform.rotation = Quaternion.LookRotation(targetPos, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
    }

}
