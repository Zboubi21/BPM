using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TypeOfFireEnum;
using PoolTypes;

public class WeaponPlayerBehaviour : WeaponBehaviour
{
    [Space]
    public Weapon weaponStats;
    float _currentBPMGain;
    float _currentBPMCost;
    float _currentTimeOfElctricalStun;
    bool _currentHasToStun;

    GameObject _currentProjectil;

    BPMSystem _BPMSystem;
    ObjectPooler objectPooler;

    public Camera playerCamera;
    public LayerMask rayCastCollision;
    [Header("VFX")]
    public GameObject[] _insideLaser;

    int defaultDistance = 500;

    RaycastHit _hit;
    public WeaponRecoil weaponRecoil = new WeaponRecoil();
    [Serializable]
    public class WeaponRecoil
    {
        [Header("Recoil_Transform")]
        public Transform RecoilPositionTranform;
        public Transform RecoilRotationTranform;
        [Space(10)]
        [Header("Recoil_Settings")]
        public float PositionDampTime;
        public float RotationDampTime;
        [Space(10)]
        public float PositionRecoil;
        public float RotationRecoil;
        [Space(10)]
        public Vector3 RecoilRotation;
        public Vector3 RecoilKickBack;

    }
    Vector3 CurrentPositionRecoil;
    Vector3 CurrentRotationRecoil;

    Vector3 RotationOutput;


    public override void Awake()
    {
        base.Awake();
        _BPMSystem = GetComponent<BPMSystem>();
        objectPooler = ObjectPooler.Instance;
        ChangeWeaponStats();
    }

    public override void Update()
    {

        switch (_SMG.typeOfFire)
        {
            case TypeOfFire.OnClick:

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    StartCoroutine(OnShoot(1, 0, 0));
                }

                break;
            case TypeOfFire.Rafale:

                if (Input.GetKeyDown(KeyCode.Mouse0) && CanShoot)
                {
                    StartCoroutine(OnShoot(_currentnbrOfShoot, _currentAttackSpeed, _currentTimeBetweenEachBurst));
                }

                break;
            case TypeOfFire.Auto:

                if (Input.GetKey(KeyCode.Mouse0) && CanShoot)
                {
                    StartCoroutine(OnShoot(1, _currentAttackSpeed, 0));
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
            {
                if (_hit.collider.CompareTag("Screen"))
                {
                    _hit.collider.GetComponent<WaveScreenController>().SwitchChannel();
                }
            }
        }

    }
   
    ProjectileType proj;
    public override void ChangeWeaponStats()
    {
        int weaponLevel;
        switch (_BPMSystem.CurrentWeaponState)
        {
            case BPMSystem.WeaponState.Level0:

                InitiateWeaponVar(weaponStats._weaponLevel0.damage, weaponStats._weaponLevel0.attackCooldown, weaponStats._weaponLevel0.BPMGainOnHit, weaponStats._weaponLevel0.BPMCost, weaponStats._weaponLevel0.bullet, weaponStats._weaponLevel0.bulletSpeed, weaponStats._weaponLevel0.useElectricalBullet, weaponStats._weaponLevel0.timeOfElectricalStun);
                weaponLevel = 0;
                proj = ProjectileType.ProjectileLevel1;
                break;
            case BPMSystem.WeaponState.Level1:

                InitiateWeaponVar(weaponStats._weaponLevel1.damage, weaponStats._weaponLevel1.attackCooldown, weaponStats._weaponLevel1.BPMGainOnHit, weaponStats._weaponLevel1.BPMCost, weaponStats._weaponLevel1.bullet, weaponStats._weaponLevel1.bulletSpeed, weaponStats._weaponLevel1.useElectricalBullet, weaponStats._weaponLevel1.timeOfElectricalStun);
                weaponLevel = 1;
                proj = ProjectileType.ProjectileLevel2;

                break;
            case BPMSystem.WeaponState.Level2:

                InitiateWeaponVar(weaponStats._weaponLevel2.damage, weaponStats._weaponLevel2.attackCooldown, weaponStats._weaponLevel2.BPMGainOnHit, weaponStats._weaponLevel2.BPMCost, weaponStats._weaponLevel2.bullet, weaponStats._weaponLevel2.bulletSpeed, weaponStats._weaponLevel2.useElectricalBullet, weaponStats._weaponLevel2.timeOfElectricalStun);
                weaponLevel = 2;
                proj = ProjectileType.ProjectileLevel3;

                break;
            default:
                weaponLevel = 0;
                proj = ProjectileType.ProjectileLevel1;
                break;
        }

        for (int i = 0, l = _insideLaser.Length; i < l; ++i)
        {
            if (i != weaponLevel)
            {
                _insideLaser[i].SetActive(false);
            }
            else
            {
                _insideLaser[i].SetActive(true);
            }
        }
    }

    void InitiateWeaponVar(int damage, float attackSpeed, float BPMGain, float BPMCost, GameObject projectileObject, float projectileSpeed, bool hasToStun, float timeOfElctricalStun)
    {
        _currentDamage = damage;

        _currentAttackSpeed = attackSpeed;
        _currentBPMGain = BPMGain;
        _currentBPMCost = BPMCost;

        _currentProjectil = projectileObject;
        _currentProjectilSpeed = projectileSpeed;

        _currentHasToStun = hasToStun;
        _currentTimeOfElctricalStun = timeOfElctricalStun;
    }

    #region ShootingMethods
    public override IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        //CanShoot = false;

        for (int i = 0; i < nbrOfShoot; ++i)
        {
            //StartCoroutine(RecoilCurve());

            _BPMSystem.LoseBPM(_currentBPMCost);
            if (_currentProjectil != null)
            {
                InitiateRayCast(InstatiateProj());
            }
            else
            {
                InitiateRayCast();
            }

            Fire();
            yield return new WaitForSeconds(timeEachShoot);
        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        //CanShoot = true;
    }

    public void AddRecoil(float upRecoil, float sideRecoil)
    {


    }
    
    void FixedUpdate()
    {
        CurrentPositionRecoil = Vector3.Lerp(CurrentPositionRecoil, Vector3.zero, weaponRecoil.PositionRecoil * Time.deltaTime);
        CurrentRotationRecoil = Vector3.Lerp(CurrentRotationRecoil, Vector3.zero, weaponRecoil.RotationRecoil * Time.deltaTime);

        weaponRecoil.RecoilPositionTranform.localPosition = Vector3.Slerp(weaponRecoil.RecoilPositionTranform.localPosition, CurrentRotationRecoil, weaponRecoil.PositionDampTime * Time.fixedDeltaTime);
        weaponRecoil.RecoilRotationTranform.localRotation = Quaternion.Euler(RotationOutput);
        RotationOutput = Vector3.Slerp(RotationOutput, CurrentPositionRecoil, weaponRecoil.RotationDampTime * Time.fixedDeltaTime);
        
    }
    public void Fire()
    {
        CurrentPositionRecoil += new Vector3(weaponRecoil.RecoilRotation.x, UnityEngine.Random.Range(-weaponRecoil.RecoilRotation.y, weaponRecoil.RecoilRotation.y), UnityEngine.Random.Range(-weaponRecoil.RecoilRotation.z, weaponRecoil.RecoilRotation.z));
        CurrentRotationRecoil += new Vector3(UnityEngine.Random.Range(-weaponRecoil.RecoilKickBack.x, weaponRecoil.RecoilKickBack.x), UnityEngine.Random.Range(-weaponRecoil.RecoilKickBack.y, weaponRecoil.RecoilKickBack.y), weaponRecoil.RecoilKickBack.z);
    }

    /*public override IEnumerator RecoilCurve()
    {
        _currentTimeToRecoverFromRecoil = 0;
        while (_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeToRecoverFromRecoil += Time.deltaTime;

            float recoil = _SMG.recoilCurve.Evaluate(_currentTimeToRecoverFromRecoil / _SMG.timeToRecoverFromRecoil);

            Vector3 rotationTemp = weaponObj.transform.localRotation.eulerAngles;

            float rotationX = _originalWeaponXRotation - _SMG.recoilHeight * recoil;
            rotationTemp.x = rotationX;

            weaponObj.transform.localEulerAngles = rotationTemp;
        }
        _currentTimeToRecoverFromRecoil = 0;
    }*/
    #endregion

        
    #region FeedBack Projectile Methods
    public override GameObject InstatiateProj()
    {
        _SMG.firePoint.transform.LookAt(OnSearchForLookAt());
        
        //GameObject go = Instantiate(_currentProjectil, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, projectilRoot);
        GameObject go = objectPooler.SpawnProjectileFromPool(proj, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation);
        return go;
    }

    public override Vector3 OnSearchForLookAt()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
        {
            return _hit.point;
        }
        return playerCamera.transform.position + playerCamera.transform.forward * defaultDistance;
    }
    #endregion

    #region RayCast Methods

    void InitiateRayCast(GameObject projectileFeedback)
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
        {
            string tag = _hit.collider.tag;

            #region Initiate Proj Var
            Projectile projVar = projectileFeedback.GetComponent<Projectile>();
            Level.AddFX(projVar.m_muzzleFlash, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, _SMG.firePoint.transform);
            if (projVar != null)
            {
                projVar.ProjectileType1 = Projectile.ProjectileType.Player;
                projVar.DistanceToReach = _hit.point;
                projVar.Col = _hit.collider;
                projVar.BPMSystem = _BPMSystem;
                projVar.RayCastCollision = rayCastCollision;
                projVar.TransfoPos = playerCamera.transform.position;
                projVar.TransfoDir = playerCamera.transform.forward;
                projVar.CurrentBPMGain = _currentBPMGain;
                projVar.CurrentDamage = _currentDamage;
                projVar.HasToStun = _currentHasToStun;
                projVar.TimeForElectricalStun = _currentTimeOfElctricalStun;
                projVar.Speed = _currentProjectilSpeed;
                projVar.ProjectileType2 = proj;
            }
            #endregion

        }
    }
    void InitiateRayCast()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide))
        {
            string tag = _hit.collider.tag;
        }
    }


    #endregion

}
