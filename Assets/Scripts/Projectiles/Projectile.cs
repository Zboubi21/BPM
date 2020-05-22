using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;
using System;

public class Projectile : MonoBehaviour
{
    #region Projectile Type
    ProjectileType m_projectileType = ProjectileType.Player;
    public enum ProjectileType
    {
        Player,
        Enemy
    }
    #endregion Projectile Type
    public enum TypeOfCollision
    {
        Rigibody,
        DoubleRaycasts,
        UpdateRaycasts
    }
    [Space]
    [Header("VFX")]
    public FxType muzzleFX;
    public FxType impactFX;
    [Space]
    public int layer;

    [Space]
    public float m_maxLifeTime = 2;
    [Tooltip("This only affect destroyable object")]
    public float m_explosionRange = 1;
    [Header("DEBUG")]
    [Space]
    public TypeOfCollision m_colType = TypeOfCollision.DoubleRaycasts;
    public float forceBuffer = 50f;

    ParticleSystem particle;
    TrailRenderer trail;
    Rigidbody rb;
    SphereCollider col;


    PoolTypes.ProjectileType projectileType;
    RaycastHit _hit;
    bool m_dieWhenHit = true;
    bool hasReachedDestination;
    float deltaLength;
    float newLength;
    Vector3 m_awakeDistance;
    Vector3 m_currentDistance;

    WeaponBehaviour _WeaponBehaviour;
    WeaponPlayerBehaviour _WeaponPlayerBehaviour;
    BPMSystem m_BPMSystem;
    LayerMask m_rayCastCollision;

    Collider m_col;

    Vector3 m_distanceToReach;
    Vector3 m_transfoPos;
    Vector3 m_transfoDir;

    float m_BPMGain;
    float _currentBPMGain;
    float _currentBPMGainWeakSpot;
    float m_speed = 25;
    int _currentDamage;
    int damage;

    bool _hasToStun;
    float _timeForElectricalStun;
    bool _isElectricalProjectile = false;

    Transform m_shooter;


    #region Get Set
    public WeaponBehaviour WeaponBehaviour { get => _WeaponBehaviour; set => _WeaponBehaviour = value; }
    public WeaponPlayerBehaviour WeaponPlayerBehaviour { get => _WeaponPlayerBehaviour; set => _WeaponPlayerBehaviour = value; }
    public LayerMask RayCastCollision { get => m_rayCastCollision; set => m_rayCastCollision = value; }
    public BPMSystem BPMSystem { get => m_BPMSystem; set => m_BPMSystem = value; }

    public int CurrentDamage { get => _currentDamage; set => _currentDamage = value; }
    public float CurrentBPMGain { get => _currentBPMGain; set => _currentBPMGain = value; }
    public float CurrentBPMGainWeakSpot { get => _currentBPMGainWeakSpot; set => _currentBPMGainWeakSpot = value; }
    public int Damage { get => damage; set => damage = value; }
    public float BPMGain { get => m_BPMGain; set => m_BPMGain = value; }
    public float Speed { get => m_speed; set => m_speed = value; }

    public Vector3 TransfoPos { get => m_transfoPos; set => m_transfoPos = value; }
    public Vector3 TransfoDir { get => m_transfoDir; set => m_transfoDir = value; }
    public Collider Col { get => m_col; set => m_col = value; }
    public Vector3 DistanceToReach { get => m_distanceToReach; set => m_distanceToReach = value; }

    public ProjectileType ProjectileType1 { get => m_projectileType; set => m_projectileType = value; }
    public bool HasToStun { get => _hasToStun; set => _hasToStun = value; }
    public float TimeForElectricalStun { get => _timeForElectricalStun; set => _timeForElectricalStun = value; }
    public PoolTypes.ProjectileType ProjectileType2 { get => projectileType; set => projectileType = value; }
    public Transform Shooter { get => m_shooter; set => m_shooter = value; }
    public bool IsElectricalProjectile { get => _isElectricalProjectile; set => _isElectricalProjectile = value; }
    #endregion

    public void Awake()
    {
        switch (m_colType)
        {
            case TypeOfCollision.Rigibody:

                rb = gameObject.AddComponent<Rigidbody>();
                col = gameObject.AddComponent<SphereCollider>();

                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.interpolation = RigidbodyInterpolation.None;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.useGravity = false;

                col.isTrigger = true;
                col.radius = 0.05f;




                break;
            case TypeOfCollision.DoubleRaycasts:

                #region Set Starting Length

                m_awakeDistance = transform.localPosition;
                deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                newLength = deltaLength;

                #endregion

                StartCoroutine(CalculateDistance());

                break;
            case TypeOfCollision.UpdateRaycasts:

                m_awakeDistance = transform.localPosition;
                deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                newLength = deltaLength;

                break;
        }
        gameObject.layer = layer;
        particle = GetComponent<ParticleSystem>();
        trail = GetComponent<TrailRenderer>();


        StartCoroutine(DestroyAnyway());

    }
    private void OnEnable()
    {
        switch (m_colType)
        {
            case TypeOfCollision.Rigibody:

                rb.interpolation = RigidbodyInterpolation.None;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.useGravity = false;

                col.isTrigger = true;
                col.radius = 0.05f;


                break;
            case TypeOfCollision.DoubleRaycasts:

                #region Set Starting Length

                m_awakeDistance = transform.localPosition;
                deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                newLength = deltaLength;

                #endregion

                StartCoroutine(CalculateDistance());

                break;
            case TypeOfCollision.UpdateRaycasts:

                m_awakeDistance = transform.localPosition;
                deltaLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                newLength = deltaLength;

                break;
        }
        gameObject.layer = layer;
        
        StartCoroutine(DestroyAnyway());
    }

    #region When using RayCast

    IEnumerator CalculateDistance()
    {
        while (newLength > 0)
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            m_currentDistance = transform.localPosition;
            newLength = deltaLength - Vector3.Distance(m_currentDistance, m_awakeDistance);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        OnProjectilReachingMaxDistance();
    }

    void OnProjectilReachingMaxDistance()
    {
        bool ray = OnCastRay(TransfoPos);
        if (ray)
        {
            //string tag = _hit.collider.tag;
            if (Col == _hit.collider)
            {
                StopCoroutine(CalculateDistance());

                SwitchForWeakSpots(_hit.collider);
            }
            else //Si la cible a bougé avant que le projectile ait atteind sa cible
            {
                #region Set New Length
                m_awakeDistance = m_currentDistance = transform.localPosition;
                m_distanceToReach = _hit.point;

                deltaLength = newLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                TransfoPos = m_awakeDistance;
                TransfoDir = transform.forward;
                Col = _hit.collider;
                #endregion

                StartCoroutine(CalculateDistance());
            }
        }
    }

    bool OnCastRay(Vector3 start)
    {
        return Physics.Raycast(start, TransfoDir, out _hit, Mathf.Infinity, RayCastCollision, QueryTriggerInteraction.Collide);
    }

    #endregion

    #region When Using Rigibody

    private void OnTriggerEnter(Collider other)
    {
        if (col != null)
        {
            SwitchForWeakSpots(other);
        }
    }
    #endregion

    #region When Using UpdateRayCast

    private void FixedUpdate()
    {
        if (m_colType == TypeOfCollision.Rigibody)
        {
            rb.velocity = transform.forward * Speed;
        }

        if (m_colType == TypeOfCollision.UpdateRaycasts)
        {
            if (OnCastRay(transform.position))
            {
                //m_awakeDistance = transform.localPosition;
                if (Col == _hit.collider)
                {
                        newLength = _hit.distance - (Speed * Time.deltaTime);

                    if (newLength > 0)
                    {
                        //m_distanceToReach = _hit.point;
                        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
                        m_currentDistance = transform.localPosition;

                        //Debug.DrawLine(m_currentDistance, m_distanceToReach, Color.red);

                    }
                    else if(!hasReachedDestination) //Pour que la fonction ne soit appelée qu'une fois
                    {
                        hasReachedDestination = true;

                        transform.position = _hit.point;
                        SwitchForWeakSpots(_hit.collider);
                    }
                }
                else
                {
                    #region Set New Length
                    m_awakeDistance = m_currentDistance = transform.localPosition;
                    m_distanceToReach = _hit.point;

                    deltaLength = newLength = Vector3.Distance(m_distanceToReach, m_awakeDistance);
                    TransfoPos = m_awakeDistance;
                    TransfoDir = transform.forward;
                    Col = _hit.collider;
                    #endregion
                }
            }
        }

        //var em = particle.GetComponent<ParticleSystemRenderer>();
        //em.enabled = !hasReachedDestination;
    }


    #endregion


    void SwitchForWeakSpots(Collider collider)
    {
        if(m_projectileType == ProjectileType.Player)       
        {
            string tag = collider.tag;
            #region Switch For WeakSpots
            ReferenceScipt refScript = collider.GetComponent<ReferenceScipt>();
            switch (tag)
            {
                // Le tir du player touche un NoSpot
                case "NoSpot":

                    // BPMGain = BPMSystem._BPM.BPMGain_OnNoSpot * CurrentBPMGain;
                    BPMGain = CurrentBPMGain;

                    FeedbackPlayerHitMarker(tag);

                    if(collider != null)
                    {
                        if(refScript != null)
                        {
                            if(refScript.cara != null)
                            {
                                refScript.cara.TakeDamage(CurrentDamage, 0, HasToStun, TimeForElectricalStun, IsElectricalProjectile);
                                // refScript.cara.GetComponent<EnemyCara>().HitPosition = _hit.point;
                                EnemyCara enemyCara = refScript.cara.GetComponent<EnemyCara>();
                                if (enemyCara != null)
                                {
                                    enemyCara.HitPosition = _hit.point;
                                    enemyCara.ImpactPosition = refScript.positionImpact;
                                }
                            }
                        }
                    }
                    BPMSystem.GainBPM(BPMGain);
                    
                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    // BPMGain = BPMSystem._BPM.BPMGain_OnWeak * CurrentBPMGainWeakSpot;
                    BPMGain = CurrentBPMGainWeakSpot;

                    FeedbackPlayerHitMarker(tag);

                    if (collider != null)
                    {
                        if (refScript != null)
                        {
                            if (refScript.cara != null)
                            {
                                refScript.cara.TakeDamage(CurrentDamage, 1, HasToStun, TimeForElectricalStun, IsElectricalProjectile);
                                // refScript.cara.GetComponent<EnemyCara>().HitPosition = _hit.point;
                                EnemyCara enemyCara = refScript.cara.GetComponent<EnemyCara>();
                                if (enemyCara != null)
                                {
                                    enemyCara.HitPosition = _hit.point;
                                    enemyCara.ImpactPosition = refScript.positionImpact;
                                }
                            }
                        }
                    }
                    BPMSystem.GainBPM(BPMGain);

                    break;

                default:

                    BPMGain = 0;
                    BPMSystem.GainBPM(BPMGain);

                    break;
            }
            #endregion
        }
        else if(m_projectileType == ProjectileType.Enemy)
        {
            if (collider.CompareTag("Player"))
            {
                BPMSystem _BPMSystem = collider.GetComponent<BPMSystem>();
                if(_BPMSystem != null)
                {
                    _BPMSystem.LoseBPM(CurrentDamage, Shooter);
                }
            }
        }

        if (collider.CompareTag("DestroyableObject"))
        {
            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();
            if (destroyableObject != null)
                destroyableObject.TakeDamage(CurrentDamage);

            DestroyableObjectController destroyableObjectController = collider.GetComponent<DestroyableObjectController>();
            if (destroyableObjectController != null)
                destroyableObjectController.TakeDamage(CurrentDamage);
        }

        Vector3 pos = Vector3.zero;
        switch (m_colType)
        {
            case TypeOfCollision.Rigibody:
                pos = transform.position;
                break;
            case TypeOfCollision.DoubleRaycasts:
                pos = _hit.point;
                break;
            case TypeOfCollision.UpdateRaycasts:
                pos = _hit.point;
                break;
        }
        Level.AddFX(impactFX, pos, transform.rotation);    //Impact FX
        if(m_projectileType == ProjectileType.Player)
        {
            Collider[] hitsCollider = Physics.OverlapSphere(transform.position, m_explosionRange);
            for (int i = 0, l = hitsCollider.Length; i < l; ++i)
            {
                if (hitsCollider[i].CompareTag("DestroyableObject"))
                {
                    if (hitsCollider[i].gameObject.TryGetComponent(out DestroyableObject destroyable))
                    {
                        destroyable.TakeDamage(CurrentDamage);

                    }else if (hitsCollider[i].gameObject.TryGetComponent(out DestroyableObjectController destroyableObject))
                    {
                        destroyableObject.TakeDamage(CurrentDamage);
                    }
                }
            }
        }
        if (collider.GetComponent<Rigidbody>() != null)
        {
            Rigidbody _rb = collider.GetComponent<Rigidbody>();
            _rb.AddForceAtPosition(-(_hit.normal * forceBuffer), _hit.point);
        }
        
        StartCoroutine(WaitForTrailToDestroy());
    }

    IEnumerator DestroyAnyway()
    {
        yield return new WaitForSeconds(m_maxLifeTime);
        StartCoroutine(WaitForTrailToDestroy());
    }

    IEnumerator WaitForTrailToDestroy()
    {
        yield return new WaitForSeconds(2f);
        //var em = particle.GetComponent<ParticleSystemRenderer>();
        //em.enabled = false;
        DestroyProj();
    }

    void DestroyProj()
    {
        ObjectPooler.Instance.ReturnProjectileToPool(ProjectileType2, gameObject);
        
        hasReachedDestination = false;
        //var em = particle.GetComponent<ParticleSystemRenderer>();
        //em.enabled = true;
    }

    void FeedbackPlayerHitMarker(string tag)
    {
        if (_WeaponPlayerBehaviour != null)
            _WeaponPlayerBehaviour.SetPlayerHitmarker(tag);
    }


}
