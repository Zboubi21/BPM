﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;
//using Sirenix.OdinInspector;
using EnemyStateEnum;
using System;
using UnityEngine.AI;

public class SpawnerController : MonoBehaviour
{


    EnemyArchetypes enemyArchetypes = new EnemyArchetypes();
    [Serializable]
    public class EnemyArchetypes
    {
        
        //[TableColumnWidth(40, Resizable = false)]
        public int waveNbr;
        //[TableColumnWidth(90)]
        public EnemyType[] enemy;
        public EnemyArray[] enemyArray;

    }
    [Serializable]
    public class EnemyArray
    {
        public float additionalTimeBeforeSpawning;
        public EnemyType enemy;
    }
    //public Dictionary<int, EnemyArchetype[]> waveManager = new Dictionary<int, EnemyArchetype[]>();
    [Space]
    //[TableList]
    public List<EnemyArchetypes> Waves = new List<EnemyArchetypes>();
    [Space]
    [Header("Spawning Variable")]
    [Header("Box Size")]
    public float sizeX;
    public float sizeZ;
    [Header("Random time between spawn")]
    public float errorPercentNegative;
    public float errorPercentPositive;

    [Header("DEBUG")]
    public float overlapRadiusOffset;
    [Space]
    LayerMask layer;

    ObjectPooler m_objectPooler;
    Vector3 spawnPosition;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    
    public void CountEnemy(int wave, WaveController controller)
    {
        for (int i = 0, l = Waves.Count; i < l; ++i)
        {
            if (Waves[i].waveNbr == wave)
            {
                for (int a = 0, f = Waves[i].enemy.Length; a < f; ++a) 
                {
                    controller.NbrOfEnemy++;
                }
            }
        }
    }

    #region CountingDebug
    public void CountAllEnemy(int wave, WaveController controller)
    {
        for (int i = 0, l = Waves.Count; i < l; ++i)
        {
            //Debug.Log(this + " all my wave nbr " + Waves[i].waveNbr + " lookingWave " + wave);
            if (Waves[i].waveNbr == wave)
            {
                for (int a = 0, f = Waves[i].enemy.Length; a < f; ++a)
                {
                    controller.NbtOfAllEnemy++;
                }
                break;
            }
        }
    }
    public void CountWantedEnemy(int wave, WaveController controller)
    {
        for (int i = 0, l = Waves.Count; i < l; ++i)
        {
            if (Waves[i].waveNbr == wave)
            {
                for (int a = 0, f = Waves[i].enemy.Length; a < f; ++a)
                {
                    controller.NbrOfWantedEnemy++;
                }
                break;
            }
        }
    }
    #endregion

    public IEnumerator WaveSpawner(int wave, WaveController controller)
    {
        for (int i = 0, l = Waves.Count; i < l; ++i)
        {
            if (Waves[i].waveNbr == wave)  // Verifie si il y a plusieur index avec le meme int, et pour chacun d'eux si ils sont égaux à la wave en cours commence à faire spawn
            {
                for (int a = 0, f = Waves[i].enemyArray.Length; a < f; ++a) // Pour chaque enemyArchetype dans la wave en cours
                {
                    yield return new WaitForSeconds(OnCreateTimeBetweenSpawn(controller) + Waves[i].enemyArray[a].additionalTimeBeforeSpawning);
                    spawnPosition = OnCreateRandomPositionInSquare();
                    while (true)
                    {
                        if (CapsuleCast(0, spawnPosition))
                        {
                            spawnPosition = OnCreateRandomPositionInSquare();
                        }
                        else
                        {
                            break;
                        }
                    }

                    GameObject go = m_objectPooler.SpawnEnemyFromPool(Waves[i].enemyArray[a].enemy, spawnPosition, transform.rotation);

                    EnemyCara cara = go.GetComponent<EnemyCara>();
                    //cara.EnemyArchetype = Waves[wave].m_enemyType[a];  // Donne à l'enemy spawned l'archetype "a" de la wave en cours
                    //cara.GiveArchetypeToTheEnemy();
                    Spawned_Tracker tracker = go.GetComponent<Spawned_Tracker>();
                    if(tracker == null)
                    {
                        tracker = go.AddComponent<Spawned_Tracker>();
                    }
                    tracker.Controller = controller;

                    EnemyController enemyController = go.GetComponent<EnemyController>();
                    SuicidalEnemyController suicidalEnemyController = go.GetComponent<SuicidalEnemyController>();


                    yield return new WaitForFixedUpdate();
                    MeshProceduralGenerator proceduralGenerator = new MeshProceduralGenerator();
                    if (enemyController != null)
                    {
                        proceduralGenerator = enemyController.gameObject.GetComponent<MeshProceduralGenerator>();
                        proceduralGenerator?.BuildCharaMesh();
                    }
                    enemyController?.On_SpawnEnemy();
                    suicidalEnemyController?.On_SpawnEnemy();
                }
            }
        }
    }

    float OnCreateTimeBetweenSpawn(WaveController controller)
    {
        return UnityEngine.Random.Range(controller.timeBetweenEachSpawn - (controller.timeBetweenEachSpawn * (errorPercentNegative / 100f)), controller.timeBetweenEachSpawn + (controller.timeBetweenEachSpawn * (errorPercentPositive / 100f)));
    }

    bool CapsuleCast(int i, Vector3 pos)
    {
        NavMeshAgent col = m_objectPooler.m_enemyPools[i].m_prefab.GetComponent<NavMeshAgent>();

        Vector3 p1 = new Vector3(pos.x, 0.5f / m_objectPooler.m_enemyPools[i].m_prefab.GetComponent<Transform>().localScale.y, pos.z);
        Vector3 p2 = p1 + Vector3.up;

        //if (activateVisualDebug)
        //{
        //    go0 = Instantiate(sphere, p1, Quaternion.identity, GameManager.s_instance.transform);
        //    go1 = Instantiate(sphere, p2, Quaternion.identity, GameManager.s_instance.transform);

        //    Destroy(go0, 2f);
        //    Destroy(go1, 2f);
        //}

        Collider[] hits = Physics.OverlapCapsule(p1, p2, col.radius + overlapRadiusOffset, 11, QueryTriggerInteraction.Collide);
        if (hits.Length > 0)
        {
            for (int a = 0, l = hits.Length; a < l; a++)
            {
                if (hits[a].CompareTag("Enemy"))
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    Vector3 OnCreateRandomPositionInSquare()
    {
        float posX = UnityEngine.Random.Range(transform.position.x - sizeX / 2f, transform.position.x + sizeX / 2f);
        float posZ = UnityEngine.Random.Range(transform.position.z - sizeZ / 2f, transform.position.z + sizeZ / 2f);
        return new Vector3(posX, transform.position.y, posZ);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), new Vector3(sizeX, 2f, sizeZ));
    }
}

/*SpawnerController[] spawners;
    [Space]
    public int[,] data;
    public float timeBetweenEachSpawn;
    [Space]
    public float timeBetweenEachWave;
    int _nbrOfEnemy;
    int _nbrOfDeadEnemy;
    int _nbrOfWave;
    bool hasStarted;

    #region Get Set
    public int NbrOfEnemy { get => _nbrOfEnemy; set => _nbrOfEnemy = value; }
    public int NbrOfDeadEnemy { get => _nbrOfDeadEnemy; set => _nbrOfDeadEnemy = value; }
    public int NbrOfWave { get => _nbrOfWave; set => _nbrOfWave = value; }
    #endregion

    private void Start()
    {
        spawners = GetComponentsInChildren<SpawnerController>();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M) && !hasStarted)
        {
            StartCoroutine(spawners[0].WaveSpawner(_nbrOfWave, this));
            hasStarted = true;
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasStarted)
        {
            StartCoroutine(spawners[0].WaveSpawner(_nbrOfWave, this));
            hasStarted = true;
        }
    }

    public void CheckLivingEnemies()
    {
        if (NbrOfDeadEnemy != 0 && NbrOfDeadEnemy == NbrOfEnemy)
        {
            //Previous wave's over
            StartCoroutine(WaitForNextWave());
        }
    }

    IEnumerator WaitForNextWave()
    {
        _nbrOfWave++;
        yield return new WaitForSeconds(timeBetweenEachWave); // time needed for all the animation/sound/voice/visual effect before next wave
        //New wave starts
        for (int i = 0, l = spawners.Length; i < l; i++)
        {
            StartCoroutine(spawners[i].WaveSpawner(_nbrOfWave, this));
        }
        //All the enemy have spawned
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(4f,1f,1f));
    }*/
