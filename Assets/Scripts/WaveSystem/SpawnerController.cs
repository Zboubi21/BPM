using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;
//using Sirenix.OdinInspector;
using EnemyStateEnum;
using System;

public class SpawnerController : MonoBehaviour
{

    EnemyType enemy = EnemyType.EnemyBase;

    EnemyArchetypes enemyArchetypes = new EnemyArchetypes();
    [Serializable]
    public class EnemyArchetypes
    {
        
        //[TableColumnWidth(40, Resizable = false)]
        public int waveNbr;
        //[TableColumnWidth(90)]
        public EnemyArchetype[] m_enemyArchetype;
    }
    //public Dictionary<int, EnemyArchetype[]> waveManager = new Dictionary<int, EnemyArchetype[]>();
    [Space]
    //[TableList]
    public List<EnemyArchetypes> Waves = new List<EnemyArchetypes>();

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    
    public IEnumerator WaveSpawner(int wave, WaveController controller)
    {
        for (int i = 0, l = Waves.Count; i < l; ++i)
        {
            if (Waves[i].waveNbr == wave)  // Verifie si il y a plusieur index avec le meme int, et pour chacun d'eux si ils sont égaux à la wave en cours commence à faire spawn
            {
                for (int a = 0, f = Waves[wave].m_enemyArchetype.Length; a < f; ++a) // Pour chaque enemyArchetype dans la wave en cours
                {

                    yield return new WaitForSeconds(controller.timeBetweenEachSpawn);

                    GameObject go = m_objectPooler.SpawnEnemyFromPool(enemy, transform.position, transform.rotation);

                    EnemyCara cara = go.GetComponent<EnemyCara>();
                    cara.EnemyArchetype = Waves[wave].m_enemyArchetype[a];  // Donne à l'enemy spawned l'archetype "a" de la wave en cours

                    Spawned_Tracker tracker = go.AddComponent<Spawned_Tracker>();
                    tracker.Controller = controller;
                    controller.NbrOfEnemy++;

                    EnemyController enemyController = go.GetComponent<EnemyController>();
                    yield return new WaitForFixedUpdate();
                    enemyController.ChangeState((int)EnemyState.Enemy_RepositionState);

                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
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
