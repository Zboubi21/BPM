using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using PoolTypes;
using ScreenTypes;
using TMPro;

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
    [Header("Debug")]
    public DEBUG _debug;
    [Serializable]
    public class DEBUG
    {
        [Tooltip("Press "+"("+" pour afficher ou désafficher le canvas")]
        public Canvas canvas;
        public TMP_Text nbrOfWaveEnemyText;
        public TMP_Text totalOfEnemyText;

        public int waveToCheckOut;
    }
    int _nbrOfEnemy;
    int _nbtOfAllEnemy;
    int _nbrOfDeadEnemy;
    int _nbrOfWave = 0;
    bool hasStarted;
    int nbrOfCocoScreen;
    int _nbrOfWantedEnemy;

    #region Get Set
    public int NbrOfEnemy { get => _nbrOfEnemy; set => _nbrOfEnemy = value; }
    public int NbrOfDeadEnemy { get => _nbrOfDeadEnemy; set => _nbrOfDeadEnemy = value; }
    public int NbrOfWave { get => _nbrOfWave; set => _nbrOfWave = value; }
    public int NbrOfCocoScreen { get => nbrOfCocoScreen; set => nbrOfCocoScreen = value; }
    public int NbtOfAllEnemy { get => _nbtOfAllEnemy; set => _nbtOfAllEnemy = value; }
    public int NbrOfWantedEnemy { get => _nbrOfWantedEnemy; set => _nbrOfWantedEnemy = value; }
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
        NbrOfDeadEnemy = 0;
        NbrOfEnemy = 0;
        ChangeAllScreen(ScreenChannel.WaveCountChannel);
        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy
        _debug.canvas.gameObject.SetActive(!_debug.canvas.gameObject.activeSelf);
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.M) && !hasStarted && allSpawners.Length > 0 )
        {
            for (int i = 0; i < allSpawners.Length; ++i)
            {
                StartCoroutine(allSpawners[i].WaveSpawner(_debug.waveToCheckOut, this));
            }
            hasStarted = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _debug.canvas.gameObject.SetActive(!_debug.canvas.gameObject.activeSelf);
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
        Debug.Log("Has been called (ça devrait pas appel Paul)");
        ChangeAllScreen(ScreenChannel.WaveCountChannel);
        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasStarted)
        {
            hasStarted = true;
            for (int i = 0, l = allSpawners.Length; i < l; ++i)
            {
                StartCoroutine(allSpawners[i].WaveSpawner(_nbrOfWave, this));
                allSpawners[i].CountEnemy(_nbrOfWave, this);
            }

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
            ///Wave Starts
            ChangeAllScreen(ScreenChannel.WaveCountChannel);
            ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy

        }
    }

    public void CheckLivingEnemies()
    {
        ChangeAllScreen(ScreenChannel.EnemyCountChannel); // Increment nbr of enemy

        if (NbrOfDeadEnemy != 0 && NbrOfDeadEnemy == NbrOfEnemy)
        {
            if(maxWave == _nbrOfWave+1)
            {
                if (wavesControl.Length > 0)
                {
                    for (int i = 0, l = wavesControl.Length; i < l; ++i)
                    {
                        if (_nbrOfWave == wavesControl[i].wave.waveNbr)
                        {
                            if (wavesControl[i].wave.eventOnEndOfWave != null)
                            {
                                wavesControl[i].wave.eventOnEndOfWave.Invoke();
                            }
                            break;
                        }

                    }
                }
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
            ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.Gun, new Vector3(GameManager.Instance.RespawnPos.transform.position.x, GameManager.Instance.RespawnPos.transform.position.y+3f, GameManager.Instance.RespawnPos.transform.position.z), Quaternion.identity);
        }
    }
    int _currentCheckedWave = -1;
    private void OnDrawGizmosSelected()
    {
        if(_currentCheckedWave != _debug.waveToCheckOut)
        {
            _nbrOfWantedEnemy = 0;
            _currentCheckedWave = _debug.waveToCheckOut;
            for (int i = 0; i < allSpawners.Length; i++)
            {
                allSpawners[i].CountWantedEnemy(_debug.waveToCheckOut, this);
            }
            _debug.nbrOfWaveEnemyText.text = string.Format("Nombre d'enemy dans la vague {0} : {1}", _debug.waveToCheckOut, _nbrOfWantedEnemy);
            _nbtOfAllEnemy = 0;
            for (int i = 0; i < allSpawners.Length; i++)
            {
                for (int a = 0; a < maxWave; a++)
                {
                    allSpawners[i].CountAllEnemy(a, this);
                }
            }
            _debug.totalOfEnemyText.text = string.Format("Total enemy dans toutes les vagues : {0}", _nbtOfAllEnemy);
        }
    }
}
