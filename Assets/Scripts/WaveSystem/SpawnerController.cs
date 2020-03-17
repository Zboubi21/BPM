using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;
using Sirenix.OdinInspector;
using EnemyStateEnum;

public class SpawnerController : SerializedMonoBehaviour
{

    EnemyType enemy = EnemyType.EnemyBase;

    public Dictionary<int, EnemyArchetype[]> waveManager = new Dictionary<int, EnemyArchetype[]>();

    ObjectPooler m_objectPooler;

    private void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }
    

    public IEnumerator WaveSpawner(int wave, WaveController controller)
    {
        if (waveManager.ContainsKey(wave))
        {
            for (int a = 0, f = waveManager[wave].Length; a < f; ++a)
            {

                yield return new WaitForSeconds(controller.timeBetweenEachSpawn);

                GameObject go = m_objectPooler.SpawnEnemyFromPool(enemy, transform.position, transform.rotation);

                EnemyCara cara = go.GetComponent<EnemyCara>();
                cara.EnemyArchetype = waveManager[wave].GetValue(a) as EnemyArchetype;

                Spawned_Tracker tracker = go.AddComponent<Spawned_Tracker>();
                tracker.Controller = controller;
                controller.NbrOfEnemy++;


                EnemyController enemyController = go.GetComponent<EnemyController>();
                yield return new WaitForFixedUpdate();
                enemyController.ChangeState((int)EnemyState.Enemy_RepositionState);

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
