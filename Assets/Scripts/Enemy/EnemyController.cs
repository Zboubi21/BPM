using System.Collections;
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

    List<GameObject> allUsedCover = new List<GameObject>();
    int choosenCover;

    EnemyCara cara;
    WeaponEnemyBehaviour weaponBehavior;
    NavMeshAgent agent;
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
    public List<GameObject> AllUsedCover { get => allUsedCover; set => allUsedCover = value; }
    #endregion


    public void Awake()
    {
        SetupStateMachine();
        agent = GetComponent<NavMeshAgent>();
        cara = GetComponent<EnemyCara>();
        weaponBehavior = GetComponent<WeaponEnemyBehaviour>();
    }

    void SetupStateMachine()
    {
        m_sM.AddStates(new List<IState> { 
            new ChaseState(this),				// 0 = Chase
            new IdleState(this),				// 1 = Idle
			new AttackState(this),				// 2 = Attack
            new RepositionState(this),          // 3 = Reposition
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

        currentTarget = FindBestSpotsInRangeOfTarget(Player);

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

    public Vector3 FindBestSpotsInRangeOfTarget(Transform target)
    {
        Vector3 newTarget;

        float distance = Vector3.Distance(transform.position, Player.transform.position);
        Vector3 lastPoint = Vector3.Lerp(Player.transform.position, transform.position, Mathf.InverseLerp(0, distance, Cara._enemyCaractéristique._attack.rangeRadius));

        #region Find all cover around the player
        Collider[] allColInSphere = Physics.OverlapSphere(lastPoint, Cara._enemyCaractéristique._attack.rangeRadius);
        List<GameObject> allCoverInSphere = new List<GameObject>(); 
        for (int i = 0, l= allColInSphere.Length; i < l; ++i)
        {
            if (allColInSphere[i].CompareTag("Cover"))
            {
                bool denied = false;
                if(allUsedCover.Count > 0)
                {
                    for (int a = 0, m = allUsedCover.Count; a < m; ++a)
                    {
                        if(allColInSphere[i].gameObject == allUsedCover[m])
                        {
                            denied = true;
                            break;
                        }
                    }
                }
                if (!denied)
                {
                    allCoverInSphere.Add(allColInSphere[i].gameObject);
                }
            }
        }
        #endregion

        if (allCoverInSphere.Count == 0)  // The NPC hasn't found a cover
        {
            while (true)
            {
                //Choisi un point aléatoire dans un cercle de la taille de la range du NPC
                Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * Cara._enemyCaractéristique._attack.rangeRadius;
                newTarget = new Vector3(randomPointInCircle.x + lastPoint.x, 0.1f + lastPoint.y, randomPointInCircle.y + lastPoint.z);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(newTarget, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return newTarget;
                }
            }
        }
        else // The NPC has found a cover
        {
            int randomIndex = UnityEngine.Random.Range(0, allCoverInSphere.Count);
            newTarget = allCoverInSphere[randomIndex].transform.position;
            allUsedCover.Add(allCoverInSphere[randomIndex]);
            choosenCover = randomIndex;
            return newTarget;
        }
    }


    public bool ThrowBehaviorDice(float value)
    {
        float random = UnityEngine.Random.Range(0, 100);
        if(random < value)
        {
            return true;
        }
        return false;
    }


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
        StartCoroutine(OnWaitForAnimToEnd(time));
    }
    IEnumerator OnWaitForAnimToEnd(float time)
    {
        cara.enabled = false;
        yield return new WaitForSeconds(time);    //Animation time
        agent.enabled = false;
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


    private void OnDrawGizmosSelected()
    {
        if (_debug.useGizmos)
        { 
            if(agent != null && weaponBehavior != null && cara != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, cara._enemyCaractéristique._attack.rangeRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(new Vector3(weaponBehavior._SMG.firePoint.transform.position.x, weaponBehavior._SMG.firePoint.transform.position.y, weaponBehavior._SMG.firePoint.transform.position.z + (agent.stoppingDistance - weaponBehavior._attack.enemyAttackDispersement*2)* weaponBehavior._attack._debugGizmos), weaponBehavior._attack.enemyAttackDispersement);
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
