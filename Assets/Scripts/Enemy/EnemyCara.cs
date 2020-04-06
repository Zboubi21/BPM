using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EnemyStateEnum;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class EnemyCara : SerializedMonoBehaviour
{
    public DebugOuvreSurtoutPas _debug = new DebugOuvreSurtoutPas();
    [Serializable] public class DebugOuvreSurtoutPas
    {
        public bool reallyNeedToUseDebug;
        [ShowIf("reallyNeedToUseDebug")]
        public GameObject[] weakSpots;
        [ShowIf("reallyNeedToUseDebug")]
        public GameObject[] noSpot;
        [ShowIf("reallyNeedToUseDebug")]
        public Collider[] allCollider;

    }
    [Space]
    private EnemyArchetype enemyArchetype;
    [Space]
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
        public Move _move = new Move();
        [Serializable]
        public class Move
        {
            public float moveSpeed;
            [Tooltip("The time needed for the NPC to look directly at the target ( when the target don't move ), it HAS to be less than 5 seconds")]
            public float timeOfLateLookAt;
        }

        public Health _health = new Health();
        [Serializable]
        public class Health
        {
            public float maxHealth;
            public int damageMultiplicatorOnWeakSpot = 1;
            public int damageMultiplicatorOnNoSpot = 1;
        }
        public StunResistance _stunResistance = new StunResistance();
        [Serializable]
        public class StunResistance
        {
            public float timeOfStun;
            [Range(0,100)]
            public float[] allPercentLifeBeforeGettingStuned;
            public float timeOfElectricalStunResistance;
        }
    }
    EnemyController controller;
    protected float _currentLife;
    int _currentDamage;
    bool _isDead;
    float _currentTimeForElectricalStun;
    float _currentTimeForElectricalStunResistance;

    float _currentTimeForStun;
    float _currentTimeForStunResistance;
    int _currentIndexInLateLookAt;

    bool[] hasBeenStuned;

    #region Get Set
    public float CurrentLife { get => _currentLife; set => _currentLife = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public EnemyArchetype EnemyArchetype { get => enemyArchetype; set => enemyArchetype = value; }

    public float CurrentTimeForStun { get => _currentTimeForStun; set => _currentTimeForStun = value; }
    public float CurrentTimeForStunResistance { get => _currentTimeForStunResistance; set => _currentTimeForStunResistance = value; }

    public float CurrentTimeForElectricalStun { get => _currentTimeForElectricalStun; set => _currentTimeForElectricalStun = value; }
    public float CurrentTimeForElectricalStunResistance { get => _currentTimeForElectricalStunResistance; set => _currentTimeForElectricalStunResistance = value; }
    public int CurrentIndexInLateLookAt { get => _currentIndexInLateLookAt; set => _currentIndexInLateLookAt = value; }
    #endregion

    public void OnEnable()
    {
        _isDead = false;
        playerController = PlayerController.s_instance;
        InitializeEnemyStats();
    }

    PlayerController playerController;
    public void Awake()
    {
        controller = GetComponent<EnemyController>();
        playerController = PlayerController.s_instance;
        if (controller != null)
        {
            controller.GetComponent<NavMeshAgent>().speed = _enemyCaractéristique._move.moveSpeed;
            if(playerController != null)
            {
                if(playerController.maxRecordPositionTime > 0)
                {
                    _currentIndexInLateLookAt = Mathf.FloorToInt(playerController.maxRecordPositionTime * 50f - _enemyCaractéristique._move.timeOfLateLookAt * 50f);
                }
                else
                {
                    Debug.LogError("You didn't wait long enough, the player records 5 seconds of its movement, if you spawn enemies before 5 seconds they won't know at what to look at");
                }
            }
        }
        if(_enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned.Length > 0)
        {
            hasBeenStuned = new bool[_enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned.Length];

            for (int i = 0, l = _enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned.Length; i < l; ++i)
            {
                hasBeenStuned[i] = false;
            }
        }
    }

    protected virtual void Start()
    {
        if (enemyArchetype != null)
        {
            enemyArchetype.PopulateArray();
            if (EnemyArchetype.Spots.Count > 0)
            {
                for (int i = 0, l = EnemyArchetype.Spots.Count; i < l; ++i)
                {
                    _debug.weakSpots[i].SetActive(EnemyArchetype.Spots[i]);
                    _debug.noSpot[i].SetActive(!EnemyArchetype.Spots[i]);
                }
            }
        }
    }

    private void Update()
    {
        if(_currentTimeForStunResistance != 0)
        {
            _currentTimeForStunResistance -= Time.deltaTime;
            if(_currentTimeForStunResistance <= 0)
            {
                _currentTimeForStunResistance = 0;
            }
        }

        if (_currentTimeForElectricalStunResistance != 0)
        {
            _currentTimeForElectricalStunResistance -= Time.deltaTime;
            if (_currentTimeForElectricalStunResistance <= 0)
            {
                _currentTimeForElectricalStunResistance = 0;
            }
        }

        if (_currentTimeForElectricalStun != 0)
        {
            _currentTimeForElectricalStun -= Time.deltaTime;
            if (_currentTimeForElectricalStun <= 0)
            {
                _currentTimeForElectricalStun = 0;
            }
        }

        if (_currentTimeForStun != 0)
        {
            _currentTimeForStun -= Time.deltaTime;
            if (_currentTimeForStun <= 0)
            {
                _currentTimeForStun = 0;
            }
        }

    }

    public virtual void TakeDamage(float damage, int i, bool hasToBeElectricalStun, float timeForElectricalStun)
    {
        switch (i)
        {
            case 0:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnNoSpot;

                break;
            case 1:

                _currentLife -= damage * _enemyCaractéristique._health.damageMultiplicatorOnWeakSpot;

                break;
            default:
                break;
        }

        if(controller != null)  // pour les dummy
        {
            if((!controller.m_sM.CompareState((int)EnemyState.Enemy_StunState) || !controller.m_sM.CompareState((int)EnemyState.Enemy_ElectricalStunState)) && !controller.m_sM.CompareState((int)EnemyState.Enemy_DieState))
            {
                if (!hasToBeElectricalStun)
                {
                    if(_enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned.Length > 0)
                    {
                        for (int a = 0, l = _enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned.Length; a < l; ++a)
                        {
                            if (_enemyCaractéristique._stunResistance.allPercentLifeBeforeGettingStuned[a] > Mathf.InverseLerp(0, _enemyCaractéristique._health.maxHealth, CurrentLife)*100f && !hasBeenStuned[a])
                            {
                                _currentTimeForStun = _enemyCaractéristique._stunResistance.timeOfStun;
                                hasBeenStuned[a] = true;
                                controller.m_sM.ChangeState((int)EnemyState.Enemy_StunState);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (_currentTimeForElectricalStunResistance == 0f)
                    {
                        _currentTimeForElectricalStun = timeForElectricalStun;
                        _currentTimeForElectricalStunResistance = _enemyCaractéristique._stunResistance.timeOfElectricalStunResistance;
                        controller.m_sM.ChangeState((int)EnemyState.Enemy_ElectricalStunState);
                    }
                }
            }
            CheckIfDead();
        }
    }

    protected virtual void CheckIfDead()
    {
        if(controller != null) // pour les dummy
        {
            if (_currentLife <= 0)
            {
                _isDead = true;
                controller.m_sM.ChangeState((int)EnemyState.Enemy_DieState);
            }
            else if(!controller.m_sM.CompareState((int)EnemyState.Enemy_StunState) && !controller.m_sM.CompareState((int)EnemyState.Enemy_ElectricalStunState))
            {
                controller.m_sM.ChangeState((int)EnemyState.Enemy_AttackState);
            }
        }
    }

    void InitializeEnemyStats()
    {

        _currentLife = _enemyCaractéristique._health.maxHealth;
        if(controller != null)
        {
            controller.GetComponent<NavMeshAgent>().speed = _enemyCaractéristique._move.moveSpeed;
        }
        //_currentDamage = _enemyCaractéristique._attack.damage;
    }
}
