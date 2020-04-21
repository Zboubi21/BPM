using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SuicidalEnemyStateEnum;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class EnemyCaraBase : SerializedMonoBehaviour
{
    public EnemyCaractéristique _enemyCaractéristique = new EnemyCaractéristique();
    [Serializable] public class EnemyCaractéristique
    {
        public Move _move = new Move();
        [Serializable]
        public class Move
        {
            public float moveSpeed;
            //[Tooltip("The time needed for the NPC to look directly at the target ( when the target don't move ), it HAS to be less than 5 seconds")]
            //public float timeOfLateLookAt;
            public float rotationSpeed;

        }

        public Health _health = new Health();
        [Serializable]
        public class Health
        {
            public float maxHealth;
            public float damageMultiplicatorOnWeakSpot = 1;
            public float damageMultiplicatorOnNoSpot = 1;
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
    SuicidalEnemyController controller;
    protected float _currentLife;
    int _currentDamage;
    protected bool _isDead;
    protected float _currentTimeForElectricalStun;
    protected float _currentTimeForElectricalStunResistance;

    protected float _currentTimeForStun;
    float _currentTimeForStunResistance;
    int _currentIndexInLateLookAt;

    protected bool[] hasBeenStuned;

    #region Get Set
    public float CurrentLife { get => _currentLife; set => _currentLife = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }

    public float CurrentTimeForStun { get => _currentTimeForStun; set => _currentTimeForStun = value; }
    public float CurrentTimeForStunResistance { get => _currentTimeForStunResistance; set => _currentTimeForStunResistance = value; }

    public float CurrentTimeForElectricalStun { get => _currentTimeForElectricalStun; set => _currentTimeForElectricalStun = value; }
    public float CurrentTimeForElectricalStunResistance { get => _currentTimeForElectricalStunResistance; set => _currentTimeForElectricalStunResistance = value; }
    public int CurrentIndexInLateLookAt { get => _currentIndexInLateLookAt; set => _currentIndexInLateLookAt = value; }
    #endregion

    protected virtual void OnEnable()
    {
        _isDead = false;
        playerController = PlayerController.s_instance;
        InitializeEnemyStats();
    }

    PlayerController playerController;
    protected virtual void Awake()
    {
        controller = GetComponent<SuicidalEnemyController>();
        playerController = PlayerController.s_instance;
        if (controller != null)
        {
            controller.GetComponent<NavMeshAgent>().speed = _enemyCaractéristique._move.moveSpeed;
            if(playerController != null)
            {
                //if(playerController.maxRecordPositionTime > 0)
                //{
                //    if(_enemyCaractéristique._move.timeOfLateLookAt != 0)
                //    {
                //        _currentIndexInLateLookAt = Mathf.FloorToInt(playerController.maxRecordPositionTime * 50f - _enemyCaractéristique._move.timeOfLateLookAt * 50f);
                //    }
                //}
                //else
                //{
                //    Debug.LogError("You didn't wait long enough, the player records 5 seconds of its movement, if you spawn enemies before 5 seconds they won't know at what to look at");
                //}
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

                _currentLife -= Mathf.CeilToInt(damage * _enemyCaractéristique._health.damageMultiplicatorOnNoSpot);

                break;
            case 1:

                _currentLife -= Mathf.CeilToInt(damage * _enemyCaractéristique._health.damageMultiplicatorOnWeakSpot);

                break;
            default:
                break;
        }

        if(controller != null)  // pour les dummy
        {
            if((!controller.SM.CompareState((int)EnemyState.StunState) || !controller.SM.CompareState((int)EnemyState.ElectricalStunState)) && !controller.SM.CompareState((int)EnemyState.DieState))
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
                                controller.SM.ChangeState((int)EnemyState.StunState);
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
                        controller.SM.ChangeState((int)EnemyState.ElectricalStunState);
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
                controller.SM.ChangeState((int)EnemyState.DieState);
            }
            else if(!controller.SM.CompareState((int)EnemyState.StunState) && !controller.SM.CompareState((int)EnemyState.ElectricalStunState))
            {
                // controller.SM.ChangeState((int)EnemyState.Enemy_AttackState);
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
    //private void OnDrawGizmos()
    //{
    //    if (controller != null)
    //    {
    //        if (playerController != null)
    //        {
    //            if (playerController.maxRecordPositionTime > 0 && _currentIndexInLateLookAt != 0)
    //            {
    //                Gizmos.color = Color.red;
    //                Gizmos.DrawWireSphere(playerController.AllPreviousPos[_currentIndexInLateLookAt], 0.1f);
    //            }
    //        }
    //    }
    //}
    
}
