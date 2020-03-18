﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EnemyStateEnum;
using System;
using UnityEngine.UI;
using PoolTypes;

public class EnemyController : MonoBehaviour
{
    public DEBUG _debug = new DEBUG();
    [Serializable] public class DEBUG
    {
        public bool useGizmos;

        public Text m_stateText;
        public Text m_lifeText;

        public GameObject m_destinationImage;
    }

    #region State Machine

    public StateMachine m_sM = new StateMachine();
    public virtual void OnEnable()
    {
        EnemyCantShoot = false;
        cara.enabled = true;
        agent.enabled = true;
        EnableDisableCollider(true);
        ChangeState((int)EnemyState.Enemy_IdleState);
    }

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
    Transform target;
    Vector3 currentTarget;

    float distanceToTarget;
    bool _enemyCanShoot;

    #region Get Set
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Transform  Player { get => target; set => target = value; }

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public EnemyCara Cara { get => cara; set => cara = value; }
    public bool EnemyCantShoot { get => _enemyCanShoot; set => _enemyCanShoot = value; }
    public Vector3 CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public WeaponEnemyBehaviour WeaponBehavior { get => weaponBehavior; set => weaponBehavior = value; }
    #endregion


    public void Awake()
    {
        SetupStateMachine();
        agent = GetComponent<NavMeshAgent>();
        cara = GetComponent<EnemyCara>();
        weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
        manager = GameManager.Instance;
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
			new DieState(this),				    // 7 = Die
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

        //currentTarget = FindBestSpotsInRangeOfTarget(Player);

        DistanceToTarget = GetTargetDistance(currentTarget);

    }

    private void Update()
    {
        m_sM.Update();

        #region DEBUG
        #if UNITY_EDITOR
        if (_debug.useGizmos)
        {
            _debug.m_stateText.text = string.Format("{0}", m_sM.m_currentStateString);
            _debug.m_lifeText.text = string.Format("{0}", cara.CurrentLife);
        }
        _debug.m_stateText.gameObject.SetActive(_debug.useGizmos);
        _debug.m_lifeText.gameObject.SetActive(_debug.useGizmos);
        #endif
        #endregion

        DistanceToTarget = GetTargetDistance(currentTarget);
    }

    void FixedUpdate()
    {
        m_sM.FixedUpdate();
    }

    void LateUpdate()
    {
        m_sM.LateUpdate();
    }

    public float GetTargetDistance(Vector3 target)
    {
        return Vector3.Distance(target, transform.position);
    }


    #region NPC motion state methods
    public Vector3 FindBestSpotsInRangeOfTarget(Transform target)
    {
        Vector3 newTarget;
        bool hasFoundACover = false;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 lastPoint = Vector3.Lerp(target.position, transform.position, Mathf.InverseLerp(0, distance, weaponBehavior._attack.rangeRadius));
        if(choosenCover != 0 && manager.AllUsedCover.Count > 0) 
        {
            manager.AllUsedCover.RemoveAt(choosenCover - 1); // -1 pour retomber sur l'index exacte (on fait +1 plus loin pour avoir choosenCover =0 : " je n'ai pas trouvé de cover"
        }

        #region Find all cover around the player
        Collider[] allColInSphere = Physics.OverlapSphere(lastPoint, weaponBehavior._attack.rangeRadius);
        List<GameObject> allCoverInSphere = new List<GameObject>();

        for (int i = 0, l= allColInSphere.Length; i < l; ++i)
        {
            if (allColInSphere[i].CompareTag("Cover"))
            {
                bool denied = false;
                if(manager.AllUsedCover.Count > 0)
                {
                    for (int a = 0, m = manager.AllUsedCover.Count; a < m; ++a)
                    {
                        if(allColInSphere[i].gameObject == manager.AllUsedCover[a])
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
            while (count > 0)
            {
                //Choisi un point aléatoire dans un cercle de la taille de la range du NPC
                Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * weaponBehavior._attack.rangeRadius;
                newTarget = new Vector3(randomPointInCircle.x + lastPoint.x, 0.1f + lastPoint.y, randomPointInCircle.y + lastPoint.z);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(newTarget, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return newTarget;
                }
                count--;
            }
            return transform.position; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
        }
        else // The NPC has found a cover
        {
            int randomIndex = UnityEngine.Random.Range(0, allCoverInSphere.Count);
            newTarget = allCoverInSphere[randomIndex].transform.position;
            manager.AllUsedCover.Add(allCoverInSphere[randomIndex]);
            choosenCover = randomIndex+1; // +1 pour avoir 0 = je n'ai pas trouvé de cover
            return newTarget;
        }
    }

    public Vector3 MoveForward(Transform target)
    {
        int count = 200;
        Vector3 newTarget;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 newPoint = Vector3.Lerp(transform.position, target.position, Mathf.InverseLerp(0, distance, weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfAgressivity));
        while (count > 0)
        {
            //Choisi un point aléatoire dans un cercle de la taille de la range du NPC mutliplié par un coefficiant d'agressivité
            Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * (weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfAgressivity);
            newTarget = new Vector3(randomPointInCircle.x + newPoint.x, 0.1f + newPoint.y, randomPointInCircle.y + newPoint.z);
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return newTarget;
            }
            count--;
        }
        return transform.position; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
    }

    public Vector3 MoveBackward(Transform target)
    {
        int count = 200;
        Vector3 newTarget;
        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 newPoint = Vector3.LerpUnclamped(target.position, transform.position, 1 + weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfDefensivity);
        while (count > 0)
        {
            //Choisi un point aléatoire dans un cercle de la taille de la range du NPC mutliplié par un coefficiant d'agressivité
            Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * (weaponBehavior._attack.rangeRadius * Cara.EnemyArchetype._rateOfDefensivity);
            newTarget = new Vector3(randomPointInCircle.x + newPoint.x, 0.1f + newPoint.y, randomPointInCircle.y + newPoint.z);
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newTarget, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return newTarget;
            }
            count--;
        }
        return transform.position; //Si le NPC ne trouve aucun point sur navMesh il reste sur place
    }

    #endregion

    #region Random methods
    public bool ThrowBehaviorDice(float value)
    {
        float random = UnityEngine.Random.Range(0f, 100f);
        if(random < value)
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
    public IEnumerator IsStun()
    {
        EnemyCantShoot = true;
        yield return new WaitForSeconds(Cara.CurrentTimeForElectricalStun);
        EnemyCantShoot = false;
        ChangeState((int)EnemyState.Enemy_ChaseState);
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
            Destroy(spawnTracker);
        }
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if (poolTracker != null)
        {
            Destroy(poolTracker);
        }
        ObjectPooler.Instance.ReturnEnemyToPool(EnemyType.EnemyBase, gameObject);
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
        return Instantiate(obj, trans, Quaternion.identity ,parent);
    }
    public void DestroyObj(GameObject obj)
    {
        Destroy(obj);
    }
    #endregion

    //public float range;
    private void OnDrawGizmosSelected()
    {
        if (_debug.useGizmos)
        { 
            if(agent != null && weaponBehavior != null && cara != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, weaponBehavior._attack.rangeRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(new Vector3(weaponBehavior._SMG.firePoint.transform.position.x, weaponBehavior._SMG.firePoint.transform.position.y, weaponBehavior._SMG.firePoint.transform.position.z + (agent.stoppingDistance - weaponBehavior._attack.enemyAttackDispersement*2)* weaponBehavior._attack._debugGizmos), weaponBehavior._attack.enemyAttackDispersement);
                //Gizmos.color = Color.black;
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
}

/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EnemyStateEnum;
using Sirenix.OdinInspector;

public class EnemyCara : SerializedMonoBehaviour
{
    public DebugOuvreSurtoutPas _debug = new DebugOuvreSurtoutPas();
    [Serializable] public class DebugOuvreSurtoutPas
    {
        public bool reallyNeedToUseDebug;
        [ShowIf("reallyNeedToUseDebug")]
        public GameObject[] weakSpots;
        [ShowIf("reallyNeedToUseDebug")]
        public GameObject[] noSpot;
        [ShowIf("reallyNeedToUseDebug")]
        public Collider[] allCollider;

    }
    [Space]
    private EnemyArchetype enemyArchetype;
    [Space]
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
        public Move _move = new Move();
        [Serializable]
        public class Move
        {
            public float moveSpeed;
        }
        public Attack _attack = new Attack();
        [Serializable]
        public class Attack
        {
            public float rangeRadius;
        }
        public Health _health = new Health();
        [Serializable]
        public class Health
        {
            public float maxHealth;
            public int damageMultiplicatorOnWeakSpot = 1;
            public int damageMultiplicatorOnNoSpot = 1;
        }
        public StunResistance _stunResistance = new StunResistance();
        [Serializable]
        public class StunResistance
        {
            public float timeForStunResistance;
        }
    }
    EnemyController controller;
    float _currentLife;
    int _currentDamage;
    bool _isDead;
    float _currentTimeForElectricalStun;
    float _currentTimeForStunResistance;

    #region Get Set
    public float CurrentLife { get => _currentLife; set => _currentLife = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public float CurrentTimeForElectricalStun { get => _currentTimeForElectricalStun; set => _currentTimeForElectricalStun = value; }
    public EnemyArchetype EnemyArchetype { get => enemyArchetype; set => enemyArchetype = value; }
    #endregion

    public void OnEnable()
    {
        _isDead = false;

        InitializeEnemyStats();
    }


    public void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    private void Start()
    {
        if (enemyArchetype != null)
        {
            enemyArchetype.PopulateArray();
            if (EnemyArchetype.Spots.Count > 0)
            {
                for (int i = 0, l = EnemyArchetype.Spots.Count; i < l; ++i)
                {
                    _debug.weakSpots[i].SetActive(EnemyArchetype.Spots[i]);
                    _debug.noSpot[i].SetActive(!EnemyArchetype.Spots[i]);
                }
            }
        }
    }

    private void Update()
    {
        if(_currentTimeForStunResistance != 0)
        {
            _currentTimeForStunResistance -= Time.deltaTime;
            if(_currentTimeForStunResistance <= 0)
            {
                _currentTimeForStunResistance = 0;
            }
        }
    }

    public void TakeDamage(float damage, int i, bool hasToBeStun, float timeForElectricalStun)
    {
        switch (i)
        {
            case 0:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnNoSpot;

                break;
            case 1:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnWeakSpot;

                break;
            default:
                break;
        }


        if (hasToBeStun && !controller.m_sM.CompareState((int)EnemyState.Enemy_StunState) && _currentTimeForStunResistance == 0f && !controller.m_sM.CompareState((int)EnemyState.Enemy_DieState))
        {
            _currentTimeForElectricalStun = timeForElectricalStun;
            _currentTimeForStunResistance = _enemyCaractéristique._stunResistance.timeForStunResistance;
            controller.m_sM.ChangeState((int)EnemyState.Enemy_StunState);
        }

        CheckIfDead();

    }

    void CheckIfDead()
    {
        if (_currentLife <= 0)
        {
            _isDead = true;
            controller.m_sM.ChangeState((int)EnemyState.Enemy_DieState);
        }
    }

    void InitializeEnemyStats()
    {
        _currentLife = _enemyCaractéristique._health.maxHealth;
        //_currentDamage = _enemyCaractéristique._attack.damage;
    }
}
     */     //EnemyCara (au cas ou il saute a cause d'Odin)
