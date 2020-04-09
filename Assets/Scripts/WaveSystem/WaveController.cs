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
    [Space]
    public WaveScreenController[] screenController;
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
            [Space]
            public UnityEvent eventOnStartWave;
            [Space]
            public UnityEvent eventOnEndOfWave;

        }
    }
    [Tooltip("Temps d'attente par défaut si aucun autre n'est référencé")]
    public float timeBetweenEachWave;
    public float timeBetweenEachSpawn;
    [Space]
    public int maxWave;
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
        //screenController = GetComponentsInChildren<WaveScreenController>();
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
    public void ResetWaveSystem()
    {
        if (screenController.Length > 0)
        {
            for (int i = 0, l = screenController.Length; i < l; ++i)
            {
                screenController[i].SetWaveController(this);
            }
        }
        _nbrOfWave = -1;
        NbrOfDeadEnemy = 0;
        NbrOfEnemy = 0;
        hasStarted = false;
        ChangeAllScreen(ScreenChannel.WaveCountChannel);
        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasStarted)
        {
            for (int i = 0, l = allSpawners.Length; i < l; ++i)
            {
                StartCoroutine(allSpawners[i].WaveSpawner(_nbrOfWave, this));
                allSpawners[i].CountEnemy(_nbrOfWave, this);
            }
            ///Wave Starts
            ChangeAllScreen(ScreenChannel.WaveCountChannel);
            ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy

            hasStarted = true;
        }
    }

    public void CheckLivingEnemies()
    {
        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy

        if (NbrOfDeadEnemy != 0 && NbrOfDeadEnemy == NbrOfEnemy)
        {
            if(maxWave == _nbrOfWave+1)
            {
                ///Final wave's over
            }
            else
            {
                ///Current wave's over
                StartCoroutine(WaitForNextWave());
            }
        }
    }

    IEnumerator WaitForNextWave()
    {

        #region Time For Next Wave
        float time = 0f;

        if(wavesControl.Length > 0)
        {
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
        }
        else
        {
            time = timeBetweenEachWave;
        }
        #endregion

        _nbrOfWave++;
        yield return new WaitForSeconds(time); // time needed for all the animation/sound/voice/visual effect before next wave

        if (wavesControl.Length > 0)
        {
            for (int i = 0, l = wavesControl.Length; i < l; ++i)
            {
                if (_nbrOfWave == wavesControl[i].wave.waveNbr)
                {
                    if (wavesControl[i].wave.eventOnStartWave != null)
                    {
                        wavesControl[i].wave.eventOnStartWave.Invoke();
                    }
                    break;
                }
            }
        }

        NbrOfDeadEnemy = 0;
        NbrOfEnemy = 0;
        for (int i = 0, l = allSpawners.Length; i < l; i++)
        {
            allSpawners[i].CountEnemy(_nbrOfWave, this);
        }
        ///All the enemy have been counted
        ChangeAllScreen(ScreenChannel.EnemyCountChannel);

        ///New wave starts
        ChangeAllScreen(ScreenChannel.WaveCountChannel); // Increment nbr of wave


        for (int i = 0, l = allSpawners.Length; i < l; i++)
        {
            StartCoroutine(allSpawners[i].WaveSpawner(_nbrOfWave, this));
        }

        ///All the enemy have spawned
        yield return new WaitForSeconds(1f); // temps d'animation de fin 
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
            ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.Gun, new Vector3(PlayerController.s_instance.transform.position.x, PlayerController.s_instance.transform.position.y*3f, PlayerController.s_instance.transform.position.z), Quaternion.identity);
        }
    }
}
