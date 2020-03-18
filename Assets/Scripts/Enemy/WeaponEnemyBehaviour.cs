using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;
using System;

public class WeaponEnemyBehaviour : WeaponBehaviour
{
    [Space]
    public Attack _attack = new Attack();
    [Serializable]
    public class Attack
    {
        public int damage;
        public float rangeRadius;
        [Space]
        public float bulletSpeed;
        [Space]
        public float timeBetweenEachBullet;
        public int nbrOfShootOnRafale;
        public float timeBetweenEachBurst;
        [Space]
        [Range(0,1)]
        public float _debugGizmos;
        public float enemyAttackDispersement;
    }
    [Space]
    public GameObject enemyProjectil;
    [Space]
    [Tooltip("Pour que l'ennemies ne tir pas dans les pieds du player")]
    public float YOffset = 1f;
    EnemyController enemyController;

    public override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyController>();
    }


    #region ShootingMethods

    int countAttacks;
    public IEnumerator OnEnemyShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        countAttacks++;
        for (int i = 0; i < nbrOfShoot; ++i)
        {
            if (!enemyController.EnemyCantShoot)
            {
                StartCoroutine(RecoilCurve());

                InstatiateProj();
            }
            yield return new WaitForSeconds(timeEachShoot);

        }
        yield return new WaitForSeconds(recoilTimeEachBurst);
        if (!enemyController.Cara.IsDead && !enemyController.EnemyCantShoot)
        {
            //Debug.Log("Before Throwing Dices");
            if(countAttacks > enemyController.Cara.EnemyArchetype._nbrOfShootBeforeRepositionning)
            {
                countAttacks = 0;
                if (enemyController.ThrowBehaviorDice(enemyController.Cara.EnemyArchetype._chanceToRepositionAfterAnAttack))
                {
                    float[] chances = new float[3] { enemyController.Cara.EnemyArchetype._chanceToGoInLookForCover, enemyController.Cara.EnemyArchetype._chanceToGoInAgressive, enemyController.Cara.EnemyArchetype._chanceToGoInDefensive };

                    int chossenState = enemyController.Choose(chances);
                    switch (chossenState)
                    {
                        case 0:

                                enemyController.ChangeState((int)EnemyState.Enemy_RepositionState);

                            break;
                        case 1:

                                enemyController.ChangeState((int)EnemyState.Enemy_AgressiveState);

                            break;
                        case 2:

                                enemyController.ChangeState((int)EnemyState.Enemy_DefensiveState);

                            break;
                        default:
                            break;
                    }
                }
                //Debug.Log("Launch Reposition");
            }
            else
            {
                //Debug.Log("Launch Chaase");
                enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
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
        GameObject go = Instantiate(enemyProjectil, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);
        InitiateProjVar(go.GetComponent<Projectile>());
        return go;
    }


    void InitiateProjVar(Projectile proj)
    {
        proj.m_colType = Projectile.TypeOfCollision.Rigibody;
        proj.ProjectileType1 = Projectile.ProjectileType.Enemy;
        proj.Speed = _attack.bulletSpeed;
        proj.CurrentDamage = _attack.damage;
    }

    public override Vector3 OnSearchForLookAt()
    {
        Vector2 dispersion = UnityEngine.Random.insideUnitCircle * _attack.enemyAttackDispersement;
        return new Vector3(enemyController.Player.position.x + dispersion.x, (enemyController.Player.position.y + YOffset) + dispersion.y, enemyController.Player.position.z ) ;
    }

    #endregion
}
