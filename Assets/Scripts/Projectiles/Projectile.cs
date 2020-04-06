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
    [Header("SFX")]
    public AudioSource hitSource;

    public AllDifferentHitSound[] allDifferentHitSound;
    [Serializable]
    public class AllDifferentHitSound
    {
        public AudioClip[] allDifferentClip;
        public Pitch Pitch;
        public Volume Volume;
    }

    #region Serializable class
    [Serializable]
    public class Pitch
    {
        [Range(0.1f, 1f)]
        public float pitch;
        float _currentPitch;
        [Space]
        [Range(0, 1)]
        public float pitchRandomizer;
        public float CurrentPitch { get => _currentPitch; set => _currentPitch = value; }
    }
    [Serializable]
    public class Volume
    {
        [Range(0, 1)]
        public float volume;
        float _currentVolume;
        [Space]
        [Range(0, 1)]
        public float volumeRandomizer;
        public float CurrentVolume { get => _currentVolume; set => _currentVolume = value; }
    }
    #endregion

    [Space]
    public float m_maxLifeTime = 2;
    [Header("DEBUG")]
    [Space]
    public TypeOfCollision m_colType = TypeOfCollision.DoubleRaycasts;
    public float forceBuffer = 50f;
    Rigidbody rb;
    SphereCollider col;


    PoolTypes.ProjectileType projectileType;
    RaycastHit _hit;
    bool m_dieWhenHit = true;

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
    float m_speed = 25;
    int _currentDamage;
    int damage;

    bool _hasToStun;
    float _timeForElectricalStun;

    Transform m_shooter;


    #region Get Set
    public WeaponBehaviour WeaponBehaviour { get => _WeaponBehaviour; set => _WeaponBehaviour = value; }
    public WeaponPlayerBehaviour WeaponPlayerBehaviour { get => _WeaponPlayerBehaviour; set => _WeaponPlayerBehaviour = value; }
    public LayerMask RayCastCollision { get => m_rayCastCollision; set => m_rayCastCollision = value; }
    public BPMSystem BPMSystem { get => m_BPMSystem; set => m_BPMSystem = value; }

    public int CurrentDamage { get => _currentDamage; set => _currentDamage = value; }
    public float CurrentBPMGain { get => _currentBPMGain; set => _currentBPMGain = value; }
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
    #endregion

    public void Awake()
    {
        switch (m_colType)
        {
            case TypeOfCollision.Rigibody:

                rb = gameObject.AddComponent<Rigidbody>();
                col = gameObject.AddComponent<SphereCollider>();

                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
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

        StartCoroutine(DestroyAnyway());

    }
    private void OnEnable()
    {
        switch (m_colType)
        {
            case TypeOfCollision.Rigibody:

                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
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
                    else
                    {
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

                    BPMGain = BPMSystem._BPM.BPMGain_OnNoSpot * CurrentBPMGain;

                    FeedbackPlayerHitMarker(tag);

                    if(collider != null)
                    {
                        if(refScript != null)
                        {
                            if(refScript.cara != null)
                            {
                                refScript.cara.TakeDamage(CurrentDamage, 0, HasToStun, TimeForElectricalStun);
                            }
                        }
                    }

                    break;

                // Le tir du player touche un WeakSpot
                case "WeakSpot":

                    BPMGain = BPMSystem._BPM.BPMGain_OnWeak * CurrentBPMGain;

                    FeedbackPlayerHitMarker(tag);

                    if (collider != null)
                    {
                        if (refScript != null)
                        {
                            if (refScript.cara != null)
                            {
                                refScript.cara.TakeDamage(CurrentDamage, 1, HasToStun, TimeForElectricalStun);
                            }
                        }
                    }

                    break;

                default:

                    BPMGain = 0;

                    break;
            }
            #endregion
            BPMSystem.GainBPM(BPMGain);
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

        // Voir avec Paul pour cette partie car si la cover d'un ennemi est détruite, l'ennemi doit être au courant et réagir en fonction !
        if (collider.CompareTag("Cover"))
        {
            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();
            if (destroyableObject != null)
                destroyableObject.BreakObject();
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

        if (collider.GetComponent<Rigidbody>() != null)
        {
            Rigidbody _rb = collider.GetComponent<Rigidbody>();
            _rb.AddForceAtPosition(-(_hit.normal * forceBuffer), _hit.point);
        }

        DestroyProj();
    }

    IEnumerator DestroyAnyway()
    {
        yield return new WaitForSeconds(m_maxLifeTime);
        DestroyProj();
    }

    void DestroyProj()
    {
        if((int)ProjectileType2 < (int)PoolTypes.ProjectileType.EnemyProjectile && allDifferentHitSound.Length > 0 && allDifferentHitSound[(int)ProjectileType2].allDifferentClip.Length > 0)
        {
            StartSoundFromArray(hitSource, allDifferentHitSound[(int)ProjectileType2].allDifferentClip, allDifferentHitSound[(int)ProjectileType2].Volume.volume, allDifferentHitSound[(int)ProjectileType2].Volume.volumeRandomizer, allDifferentHitSound[(int)ProjectileType2].Pitch.pitch, allDifferentHitSound[(int)ProjectileType2].Pitch.pitchRandomizer);
        }
        ObjectPooler.Instance.ReturnProjectileToPool(ProjectileType2, gameObject);
    }

    void FeedbackPlayerHitMarker(string tag)
    {
        if (_WeaponPlayerBehaviour != null)
            _WeaponPlayerBehaviour.SetPlayerHitmarker(tag);
    }

    void StartSoundFromArray(AudioSource audioSource, AudioClip[] audioClip, float volume, float volumeRandomizer, float pitch, float pitchRandomizer)
    {
        if (audioClip.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return;
        }

        AudioClip sound = GetAudioFromArray(audioClip);
        float volumeValue = GetRandomValue(volume, volumeRandomizer);
        float pitchValue = GetRandomValue(pitch, pitchRandomizer);

        audioSource.volume = volumeValue;
        audioSource.pitch = pitchValue;

        audioSource.PlayOneShot(sound);
    }

    AudioClip GetAudioFromArray(AudioClip[] audios)
    {
        if (audios.Length == 0)
        {
            Debug.LogWarning("No audioClip in the array!");
            return null;
        }

        return audios[UnityEngine.Random.Range(0, audios.Length)];
    }

    float GetRandomValue(float baseValue, float randomizerRange)
    {
        return baseValue - UnityEngine.Random.Range(-randomizerRange, randomizerRange);
    }
}
