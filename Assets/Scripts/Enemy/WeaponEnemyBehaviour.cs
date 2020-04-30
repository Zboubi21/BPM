﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;
using System;
using PoolTypes;

public class WeaponEnemyBehaviour : WeaponBehaviour
{
    [Space]
    public LayerMask hittedLayer;
    public Attack _attack = new Attack();
    [Serializable]
    public class Attack
    {
        public int damage;
        public float rangeRadius;
        public float rangeOfAttackNoMatterWhat;
        [Space]
        public float bulletSpeed;
        [Space]
        public float timeBetweenEachBullet;
        public int nbrOfShootOnRafale;
        public float minTimeBetweenEachBurst;
        public float maxTimeBetweenEachBurst;
        [Space]
        [Range(0,1)]
        public float _debugGizmos;
        public float enemyAttackDispersement;
    }
    [Space]
    //public GameObject enemyProjectil;
    //[Space]
    [Tooltip("Pour que l'ennemies ne tir pas dans les pieds du player")]
    public float YOffset = 1f;
    EnemyController enemyController;
    EnemyAudioController audioControl;

    public override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController>();
        audioControl = GetComponent<EnemyAudioController>();
    }


    #region ShootingMethods

    RaycastHit _hit;
    //public IEnumerator CheckIfPlayerIsInSight()
    //{
    //    float distance = 0;
    //    if(Vector3.Distance(PlayerController.s_instance.transform.position, transform.position) < _attack.rangeRadius)
    //    {
    //        distance = Vector3.Distance(PlayerController.s_instance.transform.position, transform.position);
    //    }
    //    else
    //    {
    //        distance = _attack.rangeRadius;
    //    }

    //    if (Physics.Raycast(_SMG.firePoint.transform.position, _SMG.firePoint.transform.forward, out _hit, distance))
    //    {
    //        enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
    Vector3 playerPosOnShoot;
    int countAttacks;
    public IEnumerator OnEnemyShoot(int nbrOfShoot, float timeEachShoot, float minRechargeTime, float maxRechargeTime, Vector3 lastPlayerPos)
    {
        //yield return StartCoroutine(CheckIfPlayerIsInSight());
        enemyController.HasShoot = true;
        countAttacks++;
        playerPosOnShoot = lastPlayerPos;
        for (int i = 0; i < nbrOfShoot; ++i)
        {
            if (!enemyController.EnemyCantShoot)
            {
                StartCoroutine(RecoilCurve());
                audioControl.PlayAppropriateFireSound();
                InstatiateProj();
            }
            yield return new WaitForSeconds(timeEachShoot);

        }
        audioControl.PlayAppropriateLastFireSound();
        float time = UnityEngine.Random.Range(minRechargeTime, maxRechargeTime);
#if UNITY_EDITOR
        if(enemyController._debug.useDebugLogs)
            Debug.Log(this.enemyController + " I'm waiting " + time + " seconds before changing state.");
#endif
        yield return new WaitForSeconds(time);
        //Debug.LogError("hipla");
        if (!enemyController.Cara.IsDead && !enemyController.EnemyCantShoot)
        {
            //Debug.Log("Before Throwing Dices");
            if(countAttacks > enemyController.Cara.EnemyArchetype._nbrOfRafaleBeforeRepositionning)
            {
                countAttacks = 0;
                if (enemyController.ThrowBehaviorDice(enemyController.Cara.EnemyArchetype._chanceToRepositionAfterAnAttack) && enemyController.DistanceToPlayer > _attack.rangeOfAttackNoMatterWhat)
                {
                    float[] chances;
                    if (!enemyController.Cara.EnemyArchetype.useDependencyForDefensive || Mathf.InverseLerp(0, enemyController.Cara._enemyCaractéristique._health.maxHealth, enemyController.Cara.CurrentLife) <= (enemyController.Cara.EnemyArchetype.percentLifeBeforeDefensive/100f))
                    {
                        chances = new float[3] { enemyController.Cara.EnemyArchetype._chanceToGoInLookForHotSpot, enemyController.Cara.EnemyArchetype._chanceToGoInAgressive, enemyController.Cara.EnemyArchetype._chanceToGoInDefensive };
                    }
                    else
                    {
                        chances = new float[2] { enemyController.Cara.EnemyArchetype._chanceToGoInLookForHotSpot, enemyController.Cara.EnemyArchetype._chanceToGoInAgressive};

                    }

                    int chossenState = enemyController.Choose(chances);

                    switch (chossenState)
                    {
                        case 0:

                                enemyController.ChangeState((int)EnemyState.Enemy_RepositionState);
                                //enemyController.RigBuilder.layers[0].rig.weight = 0;

                            break;
                        case 1:

                                enemyController.ChangeState((int)EnemyState.Enemy_AgressiveState);
                                //enemyController.RigBuilder.layers[0].rig.weight = 0;

                            break;
                        case 2:

                                enemyController.ChangeState((int)EnemyState.Enemy_DefensiveState);
                                //enemyController.RigBuilder.layers[0].rig.weight = 0;

                            break;
                        default:
                            break;
                    }
                }
                else if((enemyController.DistanceToTarget > _attack.rangeRadius || enemyController.DistanceToPlayer > _attack.rangeOfAttackNoMatterWhat) || Physics.Linecast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), new Vector3(PlayerController.s_instance.transform.position.x, PlayerController.s_instance.transform.position.y + 1f, PlayerController.s_instance.transform.position.z), out _hit, hittedLayer))
                {
                    enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
                    //enemyController.RigBuilder.layers[0].rig.weight = 0;

                }
                else
                {
                    enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
                }
            }
            else if ((enemyController.DistanceToTarget > _attack.rangeRadius && enemyController.DistanceToPlayer > _attack.rangeOfAttackNoMatterWhat) || Physics.Linecast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), new Vector3(PlayerController.s_instance.transform.position.x, PlayerController.s_instance.transform.position.y + 1f, PlayerController.s_instance.transform.position.z), out _hit, hittedLayer))
            {
                enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
                //enemyController.RigBuilder.layers[0].rig.weight = 0;

            }
            else
            {
                enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
            }
        }
    }



    public override IEnumerator RecoilCurve()
    {
        _currentTimeToRecoverFromRecoil = 0;
        while (_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil <= 1)
        {
            _currentTimeToRecoverFromRecoil += Time.deltaTime;

            float recoil = _SMG.recoilCurve.Evaluate(_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil);

            Vector3 rotationTemp = weaponObj.transform.localRotation.eulerAngles;

            float rotationX = _originalWeaponXRotation - _SMG.recoilHeight * recoil;
            rotationTemp.x = rotationX;

            weaponObj.transform.localEulerAngles = rotationTemp;
            yield return null;
        }
        _currentTimeToRecoverFromRecoil = 0;
    }
    #endregion


    #region FeedBack Projectile Methods
    public override GameObject InstatiateProj()
    {
        _SMG.firePoint.transform.LookAt(OnSearchForLookAt());
        //if(enemyController.PlayerController.AllPreviousPos[enemyController.Cara.CurrentIndexInLateLookAt] != null)
        //{
        //}
        //else
        //{
        //    Debug.LogError("You didn't wait long enough, the player records 5 seconds of its movement, if you spawn enemies before 5 seconds they won't know at what to look at");
        //}
        /*Vector2 dispersion = UnityEngine.Random.insideUnitCircle * _attack.enemyAttackDispersement;

        Quaternion rotation = Quaternion.LookRotation(Vector3.Slerp(enemyController.transform.forward, 
            enemyController.Player.position - enemyController.transform.position, 
            (1 / enemyController.Cara._enemyCaractéristique._move.timeOfLateLookAt) * Time.deltaTime));

        _SMG.firePoint.transform.eulerAngles = new Vector3(_SMG.firePoint.transform.eulerAngles.x + enemyController.Player.position.x + dispersion.x, 
            rotation.eulerAngles.y + (enemyController.Player.position.y + YOffset) + dispersion.y, 
            _SMG.firePoint.transform.eulerAngles.z + enemyController.Player.position.z);*/

        GameObject go = ObjectPooler.Instance.SpawnProjectileFromPool(ProjectileType.EnemyProjectile, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation);
        InitiateProjVar(go.GetComponent<Projectile>());
        return go;
    }


    void InitiateProjVar(Projectile proj)
    {
        Level.AddFX(proj.muzzleFX, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, _SMG.firePoint.transform);
        proj.m_colType = Projectile.TypeOfCollision.Rigibody;
        proj.ProjectileType1 = Projectile.ProjectileType.Enemy;
        proj.ProjectileType2 = ProjectileType.EnemyProjectile;
        proj.Speed = _attack.bulletSpeed;
        proj.CurrentDamage = _attack.damage;
        proj.Shooter = transform;
    }

    public override Vector3 OnSearchForLookAt()
    {
        Vector2 dispersion = UnityEngine.Random.insideUnitCircle * _attack.enemyAttackDispersement;
        return new Vector3(playerPosOnShoot.x + dispersion.x, 
                            (playerPosOnShoot.y) + dispersion.y,
                                playerPosOnShoot.z );
    }

    #endregion
}
