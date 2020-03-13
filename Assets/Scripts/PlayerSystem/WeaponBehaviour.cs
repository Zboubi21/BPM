using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponBehaviour : MonoBehaviour
{
    public GameObject weaponObj;

    public SMG _SMG = new SMG();

    protected Transform projectilRoot;

    protected float _originalWeaponXRotation;
    protected int _currentDamage;
    protected int _currentnbrOfShoot;
    protected float _currentTimeToRecoverFromRecoil;
    protected float _currentAttackSpeed;
    protected float _currentTimeBetweenEachBurst;
    protected float _currentProjectilSpeed;

    bool canShoot = true;

    public bool CanShoot { get => canShoot; set => canShoot = value; }

    public virtual void Awake()
    {
        _originalWeaponXRotation = weaponObj.transform.localRotation.eulerAngles.x;
    }

    public virtual void Start()
    {
        projectilRoot = GameManager.Instance.transform;
    }

    public virtual void Update()
    {

    }

    public virtual void ChangeWeaponStats()
    {
        
    }

    public virtual IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        yield return null;
    }

    public virtual IEnumerator RecoilCurve()
    {
        yield return null;
    }


    #region Proj Init Methods

    public virtual Vector3 OnSearchForLookAt()
    {
        return Vector3.zero;
    }

    public virtual GameObject InstatiateProj()
    {
        return null;
    }

    public virtual void InitiateProjectileVar(GameObject gameObject, Vector3 posToLookAt)
    {
       
    }

    public virtual Vector3 GetProjectilLookAt()
    {
        return Vector3.zero;
    }
    #endregion


}