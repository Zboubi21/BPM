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
