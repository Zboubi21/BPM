using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using PoolTypes;
using ScreenTypes;

public class WaveController : MonoBehaviour
{
    public SpawnerController[] allSpawners;
    WaveScreenController[] screenController;
    [Space]
    public WaveControl[] wavesControl;
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
        //spawners = GetComponentsInChildren<SpawnerController>();
        screenController = GetComponentsInChildren<WaveScreenController>();
        if (screenController.Length > 0)
        {
            for (int i = 0, l = screenController.Length; i < l; ++i)
            {
                screenController[i].SetWaveController(this);
            }
        }
        _nbrOfWave = 0;
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M) && !hasStarted)
        {
            StartCoroutine(allSpawners[0].WaveSpawner(_nbrOfWave, this));
            hasStarted = true;
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasStarted)
        {
            StartCoroutine(allSpawners[0].WaveSpawner(_nbrOfWave, this));
            allSpawners[0].CountEnemy(_nbrOfWave, this);
            ///Wave Starts
            ChangeAllScreen(ScreenChannel.WaveCountChannel);
            hasStarted = true;
        }
    }

    public void CheckLivingEnemies()
    {

        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy

        if (NbrOfDeadEnemy != 0 && NbrOfDeadEnemy == NbrOfEnemy)
        {
            ///Current wave's over

            StartCoroutine(WaitForNextWave());
        }
    }

    IEnumerator WaitForNextWave()
    {
        float time = 0f;
        for (int i = 0, l = wavesControl.Length; i < l; ++i)
        {
            if(_nbrOfWave == wavesControl[i].wave.waveNbr)
            {
                time = wavesControl[i].wave.timeForNextWave;
                if(wavesControl[i].wave.eventOnEndOfWave != null)
                {
                    wavesControl[i].wave.eventOnEndOfWave.Invoke();
                }
                break;
            }
            else
            {
                time = timeBetweenEachWave;
            }
        }
        _nbrOfWave++;
        ChangeAllScreen(ScreenChannel.WaveCountChannel); // Increment nbr of wave
        yield return new WaitForSeconds(time); // time needed for all the animation/sound/voice/visual effect before next wave

        ///New wave starts
        NbrOfDeadEnemy = 0;
        NbrOfEnemy = 0;

        for (int i = 0, l = allSpawners.Length; i < l; i++)
        {
            StartCoroutine(allSpawners[i].WaveSpawner(_nbrOfWave, this));
            allSpawners[i].CountEnemy(_nbrOfWave, this);
        }
        ///All the enemy have spawned
        ChangeAllScreen(ScreenChannel.EnemyCountChannel);
    }

    void ChangeAllScreen(ScreenChannel chanel)
    {
        if (screenController.Length > 0)
        {
            for (int i = 0, l = screenController.Length; i < l; ++i)
            {
                screenController[i].OnChangeDisplayInfo(chanel);
            }
        }
    }


    public void AddCocoScreen()
    {
        NbrOfCocoScreen++;
        if(NbrOfCocoScreen == screenController.Length)
        {
            ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.Gun, new Vector3(PlayerController.s_instance.transform.position.x, 20f, PlayerController.s_instance.transform.position.z), Quaternion.identity);
        }
    }
}
