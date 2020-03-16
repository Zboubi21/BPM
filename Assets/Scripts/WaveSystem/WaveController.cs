using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public SpawnerController[] spawners;
    [Space]
    public float timeBetweenEachSpawn;
    int _nbrOfEnemy;
    int _nbrOfDeadEnemy;
    int _nbrOfWave;
    bool hasStarted;

    #region Get Set
    public int NbrOfEnemy { get => _nbrOfEnemy; set => _nbrOfEnemy = value; }
    public int NbrOfDeadEnemy { get => _nbrOfDeadEnemy; set => _nbrOfDeadEnemy = value; }
    #endregion

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
            _nbrOfWave++;
            for (int i = 0, l = spawners.Length; i < l; i++)
            {
                if (spawners[i].waveManager.ContainsKey(i))
                {
                    StartCoroutine(spawners[i].WaveSpawner(_nbrOfWave, this));
                    //ActivateAllSpawner(_nbrOfWave);
                }
            }
            //End of Wave
        }
    }

    void ActivateAllSpawner(int waveNbr)
    {
        for (int i = 0, l = spawners.Length; i < l; ++i)
        {
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(4f,1f,1f));
    }
}
