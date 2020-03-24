using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using PoolTypes;

public class WaveController : MonoBehaviour
{
    SpawnerController[] spawners;
    WaveScreenController[] screenController;
    [Space]
    public WaveControl[] waveControl;
    [Serializable]
    public class WaveControl
    {
        public Waves wave;
        [Serializable]
        public class Waves
        {
            public int waveNbr;
            public float timeForNextWave;
            public UnityEvent eventOnEndOfWave;
        }
    }
    [Tooltip("Temps d'attente par défaut si aucun autre n'est référencé")]
    public float timeBetweenEachWave;
    public float timeBetweenEachSpawn;
    [Space]
    int _nbrOfEnemy;
    int _nbrOfDeadEnemy;
    int _nbrOfWave;
    bool hasStarted;
    int nbrOfCocoScreen;

    #region Get Set
    public int NbrOfEnemy { get => _nbrOfEnemy; set => _nbrOfEnemy = value; }
    public int NbrOfDeadEnemy { get => _nbrOfDeadEnemy; set => _nbrOfDeadEnemy = value; }
    public int NbrOfWave { get => _nbrOfWave; set => _nbrOfWave = value; }
    public int NbrOfCocoScreen { get => nbrOfCocoScreen; set => nbrOfCocoScreen = value; }
    #endregion

    private void Start()
    {
        spawners = GetComponentsInChildren<SpawnerController>();
        screenController = GetComponentsInChildren<WaveScreenController>();
        _nbrOfWave = 0;
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
            ///Current wave's over

            StartCoroutine(WaitForNextWave());
        }
    }

    IEnumerator WaitForNextWave()
    {
        float time = 0f;
        for (int i = 0, l = waveControl.Length; i < l; ++i)
        {
            if(_nbrOfWave == waveControl[i].wave.waveNbr)
            {
                time = waveControl[i].wave.timeForNextWave;
                if(waveControl[i].wave.eventOnEndOfWave != null)
                {
                    waveControl[i].wave.eventOnEndOfWave.Invoke();
                }
                break;
            }
            else
            {
                time = timeBetweenEachWave;
            }
        }
        _nbrOfWave++;
        ChangeAllScreen();
        yield return new WaitForSeconds(time); // time needed for all the animation/sound/voice/visual effect before next wave

        ///New wave starts

        for (int i = 0, l = spawners.Length; i < l; i++)
        {
            StartCoroutine(spawners[i].WaveSpawner(_nbrOfWave, this));
        }
        //All the enemy have spawned
    }

    void ChangeAllScreen()
    {
        for (int i = 0, l = screenController.Length; i < l; ++i)
        {
            screenController[i].OnChangeDisplayInfo(this);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(4f,1f,1f));
    }

    public void AddCocoScreen()
    {
        NbrOfCocoScreen++;
        if(NbrOfCocoScreen == screenController.Length)
        {
            ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.Gun, new Vector3(0, 20, 0), Quaternion.identity);
        }
    }
}
