using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EnemyStateEnum;
using Sirenix.OdinInspector;
using UnityEngine.AI;

public class EnemyCara : EnemyCaraBase
{
    [Space]
    public EnemyArchetype enemyArchetype;
    public EnemyArchetype EnemyArchetype { get => enemyArchetype; set => enemyArchetype = value; }
    public List<bool> CheckWeakSpotHit { get => checkWeakSpotHit; set => checkWeakSpotHit = value; }
    public Vector3 HitPosition { get => hitPosition; set => hitPosition = value; }

    [Space]
    public DebugOuvreSurtoutPas _debug = new DebugOuvreSurtoutPas();
    [Serializable] public class DebugOuvreSurtoutPas
    {
        public bool reallyNeedToUseDebug;
        [ShowIf("reallyNeedToUseDebug")]
        public GameObject[] weakSpots;
        [ShowIf("reallyNeedToUseDebug")]
        public Collider[] allCollider;
    }

    EnemyController enemyController;
    MeshProceduralGenerator proceduralGenerator;

    Vector3 hitPosition;

    protected override void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        proceduralGenerator = GetComponent<MeshProceduralGenerator>();
        proceduralGenerator?.BuildCharaMesh();
        base.Awake();
        if (enemyController != null)
        {
            enemyController.GetComponent<NavMeshAgent>().speed = _enemyCaractéristique._move.moveSpeed;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        GiveArchetypeToTheEnemy();
    }
    protected override void Start()
    {
        base.Start();
        GiveArchetypeToTheEnemy();
    }

    public void GiveArchetypeToTheEnemy()
    {
        if (enemyArchetype != null)
        {
            enemyArchetype.PopulateArray();
            if (EnemyArchetype.Spots.Count > 0)
            {
                for (int i = 0, l = EnemyArchetype.Spots.Count; i < l; ++i)
                {
                    if(_debug.weakSpots.Length > 0)
                    {
                        _debug.weakSpots[i]?.SetActive(EnemyArchetype.Spots[i]);
                        //_debug.noSpot[i]?.SetActive(!EnemyArchetype.Spots[i]);
                    }
                }
            }
        }
    }
    List<bool> checkWeakSpotHit = new List<bool>();
    public override void TakeDamage(float damage, int i, bool hasToBeElectricalStun, float timeForElectricalStun, bool isElectricalDamage = false)
    {
        switch (i)
        {
            case 0:
                GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.hitSomething.noSpotHits);
                _currentLife -= Mathf.CeilToInt(damage * _enemyCaractéristique._health.damageMultiplicatorOnNoSpot);
                checkWeakSpotHit.Add(false);
                break;
            case 1:
                GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.hitSomething.weakSpotHits);
                _currentLife -= Mathf.CeilToInt(damage * _enemyCaractéristique._health.damageMultiplicatorOnWeakSpot);
                checkWeakSpotHit.Add(true);
                break;
            default:
                break;
        }

        if(enemyController != null)  // pour les dummy
        {
            if((!enemyController.m_sM.CompareState((int)EnemyState.Enemy_StunState) || !enemyController.m_sM.CompareState((int)EnemyState.Enemy_ElectricalStunState)) && !enemyController.m_sM.CompareState((int)EnemyState.Enemy_DieState))
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
                                enemyController.m_sM.ChangeState((int)EnemyState.Enemy_StunState);
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
                        enemyController.m_sM.ChangeState((int)EnemyState.Enemy_ElectricalStunState);
                    }
                }
            }
            
            if (_currentLife <= _enemyCaractéristique._stunResistance.m_lifeWhenLowHealth && !m_isLowHealth)
            {
                m_isLowHealth = true;
                CheckIfLowHealth();
            }

            CheckIfDead(isElectricalDamage);
        }
    }
    protected override void CheckIfLowHealth()
    {
        // enemyController.On_EnemyIsLowHealth();
    }
    protected override void CheckIfDead(bool deadWithElectricalDamage = false)
    {
        if(enemyController != null) // pour les dummy
        {
            if (_currentLife <= 0)
            {
                _isDead = true;

                enemyController.m_sM.ChangeState((int)EnemyState.Enemy_DieState);

            }
            else if(!enemyController.m_sM.CompareState((int)EnemyState.Enemy_StunState) && !enemyController.m_sM.CompareState((int)EnemyState.Enemy_ElectricalStunState))
            {
                enemyController.m_sM.ChangeState((int)EnemyState.Enemy_AttackState);
            }
        }
    }

    public override EnemyController GetEnemyController()
    {
        return enemyController;
    }

}
