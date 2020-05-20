using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TypeOfFireEnum;
using PoolTypes;
using ScreenTypes;
using UnityEngine.UI;

public class WeaponPlayerBehaviour : WeaponBehaviour
{
    [Space]
    public Weapon weaponStats;
    float _currentBPMGain;
    float _currentBPMGainWeakSpot;
    float _currentBPMCost;
    float _currentTimeOfElctricalStun;
    bool _currentHasToStun;

    GameObject _currentProjectil;

    BPMSystem _BPMSystem;
    ObjectPooler objectPooler;
    PlayerAudioController audioControl;

    public GameObject playerCamera;
    public LayerMask rayCastCollision;
    public LayerMask rayCastEnemyCollision;

    [Header("Anims")]
    [SerializeField] Animations m_animations;
    [Serializable] class Animations
    {
        public float m_fireAnimValue = 0.75f;

        [Header("Anims")]
        public int m_fireLayer = 2;
        public float m_fireLayerToValue = 0.7f;
        public float m_timeToChange = 0.1f;
        public AnimationCurve m_changeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    }

    [Header("Feedback")]
    [SerializeField] PlayerBpmCrosshairController m_crosshair;
    [SerializeField] Hitmarker m_hitmarkers;
    IEnumerator m_showPlayerHitMarker;
    [Serializable] class Hitmarker
    {
        public Image[] m_img;

        [Header("Colors")]
        public Color m_onEnemyNoSpot = Color.red;
        public Color m_onEnemyWeakspot = Color.yellow;

        [Header("Timers")]
        public float m_timeToShow = 0.125f;
        public float m_timeToHideMarker = 0.125f;
        public float m_waitTimeToShowMarker = 0.01f;

        [Header("Images")]
        public Sprite[] m_hitmarkersImgs;
    }
    int m_currentShootCount = 0;

    [Header("VFX")]
    public GameObject[] _insideLaser;
    

    int defaultDistance = 500;

    RaycastHit _hit;
    public WeaponRecoil[] weaponRecoil;
    [Serializable]
    public class WeaponRecoil
    {
        //[Header("Weapon Recoil")]
        //public bool m_useRecoil = false;
        //[Header("Recoil_Transform")]
        //public Transform RecoilPositionTranform;
        //public Transform RecoilRotationTranform;
        //[Space(10)]
        //[Header("Recoil_Settings")]
        //public float PositionDampTime;
        //public float RotationDampTime;
        //[Space(10)]
        //public float PositionRecoil;
        //public float RotationRecoil;
        //[Space(10)]
        //public Vector3 RecoilRotation;
        //public Vector3 RecoilKickBack;
        //[Space(15)]
        public string name;
        [Header("Camera Recoil")]
        public bool m_useCameraRecoil = false;
        [Header("Recoil Settings")]
        public float rotationSpeed = 6;
        public float returnSpeed = 25;
        [Space]
        [Header("Recoil Strength")]
        public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    }
    Vector3 currentRotation;
    Vector3 rot;
    Vector3 CurrentPositionRecoil;
    Vector3 CurrentRotationRecoil;

    Vector3 RotationOutput;


    public override void Awake()
    {
        base.Awake();
        _BPMSystem = GetComponent<BPMSystem>();
    }

    public override void Start()
    {
        base.Start();
        audioControl = PlayerController.s_instance.m_references.m_playerAudio;
        objectPooler = ObjectPooler.Instance;
        ChangeWeaponStats();

        m_fireAnimationDistance = m_animations.m_fireLayerToValue;
        m_fireAnimationSpeed = m_fireAnimationDistance / m_animations.m_timeToChange;
    }

    bool m_isShooting = false;
    public bool IsShooting { get => m_isShooting; }
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

                if (Input.GetKey(KeyCode.Mouse0) && CanShoot && !m_playerIsDashing)
                {
                    StartCoroutine(OnShoot(1, _currentAttackSpeed, 0));
                    PlayerController.s_instance.SetPlayerWeaponAnim("Fire", true);
                    PlayerController.s_instance.SetPlayerWeaponAnim("FireValue", m_animations.m_fireAnimValue);
                    StartChangeFireLayerValueCorout(m_animations.m_fireLayerToValue);
                    if (!m_isShooting)
                        m_isShooting = true;
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    // audioControl.PlayAppropriateLastFireSound((int)_BPMSystem.CurrentWeaponState);
                    PlayerController.s_instance.SetPlayerWeaponAnim("Fire", false);
                    PlayerController.s_instance.SetPlayerWeaponAnim("FireValue", 0);
                    StartChangeFireLayerValueCorout(0);
                    m_crosshair.On_StopShoot();
                    m_isShooting = false;
                }
                break;
        }
        #region Comments
        /*
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (WeaponForwardRaycast())
            {
                if (_hit.collider.CompareTag("Screen"))
                {
                    WaveScreenController ctrler = _hit.collider.GetComponent<WaveScreenController>();

                    if(ctrler != null)
                    {
                        int i = (int)ctrler._screenChannel + 1;

                        if (i == (int)ScreenChannel.CocoChannel)
                        {
                            i++;
                        }

                        if (i <= (int)ScreenChannel.ScoreCountChannel)
                        {
                            ctrler.SwitchChannel(i);
                        }
                        else
                        {
                            ctrler.SwitchChannel(0);
                        }
                    }
                }
            }
        }
        */
        #endregion
        #region Easter Egg
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(WeaponForwardRaycast())
            {
                if (_hit.collider.CompareTag("Screen"))
                {
                    _hit.collider.GetComponent<WaveScreenController>().SwitchChannel();
                }
            }
        }
        #endregion
    }

    IEnumerator m_changeFireLayerValueCorout;
    float m_fireAnimationSpeed;
    float m_fireAnimationDistance;
    void StartChangeFireLayerValueCorout(float toValue)
    {
        if (m_changeFireLayerValueCorout != null)
            StopCoroutine(m_changeFireLayerValueCorout);
        m_changeFireLayerValueCorout = ChangeFireLayerValueCorout(toValue);
        StartCoroutine(m_changeFireLayerValueCorout);
    }
    IEnumerator ChangeFireLayerValueCorout(float toValue)
    {
        float fromValue = PlayerController.s_instance.GetPlayerWeaponLayerLength(m_animations.m_fireLayer);
        float actualValue = fromValue;
        float fracJourney = 0;
        while (actualValue != toValue)
        {
            fracJourney += (Time.deltaTime) * m_fireAnimationSpeed / m_fireAnimationDistance;
            actualValue = Mathf.Lerp(fromValue, toValue, m_animations.m_changeCurve.Evaluate(fracJourney));
            PlayerController.s_instance.SetPlayerWeaponLayerLength(m_animations.m_fireLayer, actualValue);
            yield return null;
        }
    }

    bool m_playerIsDashing;
    public void On_PlayerDash(bool hasDash)
    {
        m_playerIsDashing = hasDash;
        if (hasDash)
            StartChangeFireLayerValueCorout(0);
    }

    ProjectileType proj;

    public BPMSystem BPMSystem { get => _BPMSystem; set => _BPMSystem = value; }

    public override void ChangeWeaponStats()
    {
        int weaponLevel;
        switch (_BPMSystem.CurrentWeaponState)
        {
            case BPMSystem.WeaponState.Level0:

                InitiateWeaponVar(weaponStats._weaponLevel0.damage, weaponStats._weaponLevel0.attackCooldown, weaponStats._weaponLevel0.BPMGainOnHit, weaponStats._weaponLevel0.BPMGainOnHitWeakSpot, weaponStats._weaponLevel0.BPMCost, weaponStats._weaponLevel0.bullet, weaponStats._weaponLevel0.bulletSpeed, weaponStats._weaponLevel0.useElectricalBullet, weaponStats._weaponLevel0.timeOfElectricalStun);
                weaponLevel = 0;
                proj = ProjectileType.ProjectileLevel1;
                break;
            case BPMSystem.WeaponState.Level1:

                InitiateWeaponVar(weaponStats._weaponLevel1.damage, weaponStats._weaponLevel1.attackCooldown, weaponStats._weaponLevel1.BPMGainOnHit, weaponStats._weaponLevel1.BPMGainOnHitWeakSpot, weaponStats._weaponLevel1.BPMCost, weaponStats._weaponLevel1.bullet, weaponStats._weaponLevel1.bulletSpeed, weaponStats._weaponLevel1.useElectricalBullet, weaponStats._weaponLevel1.timeOfElectricalStun);
                weaponLevel = 1;
                proj = ProjectileType.ProjectileLevel2;

                break;
            case BPMSystem.WeaponState.Level2:

                InitiateWeaponVar(weaponStats._weaponLevel2.damage, weaponStats._weaponLevel2.attackCooldown, weaponStats._weaponLevel2.BPMGainOnHit, weaponStats._weaponLevel2.BPMGainOnHitWeakSpot, weaponStats._weaponLevel2.BPMCost, weaponStats._weaponLevel2.bullet, weaponStats._weaponLevel2.bulletSpeed, weaponStats._weaponLevel2.useElectricalBullet, weaponStats._weaponLevel2.timeOfElectricalStun);
                weaponLevel = 2;
                proj = ProjectileType.ProjectileLevel3;

                break;
            case BPMSystem.WeaponState.Fury:

                InitiateWeaponVar(weaponStats._weaponFury.damage, weaponStats._weaponFury.attackCooldown, weaponStats._weaponFury.BPMGainOnHit, weaponStats._weaponFury.BPMGainOnHitWeakSpot, weaponStats._weaponFury.BPMCost, weaponStats._weaponFury.bullet, weaponStats._weaponFury.bulletSpeed, weaponStats._weaponFury.useElectricalBullet, weaponStats._weaponFury.timeOfElectricalStun);
                weaponLevel = 3;
                proj = ProjectileType.ProjectileLevel4;

                break;
            default:
                weaponLevel = 0;
                proj = ProjectileType.ProjectileLevel1;
                break;
        }

        for (int i = 0, l = _insideLaser.Length; i < l; ++i)
        {
            if (i != weaponLevel-1)
            {
                _insideLaser[i].SetActive(false);
            }
            else
            {
                _insideLaser[i].SetActive(true);
            }
        }
    }

    void InitiateWeaponVar(int damage, float attackSpeed, float BPMGain, float BPMGainWeakSpot, float BPMCost, GameObject projectileObject, float projectileSpeed, bool hasToStun, float timeOfElctricalStun)
    {
        _currentDamage = damage;

        _currentAttackSpeed = attackSpeed;
        _currentBPMGain = BPMGain;
        _currentBPMGainWeakSpot = BPMGainWeakSpot;
        _currentBPMCost = BPMCost;

        _currentProjectil = projectileObject;
        _currentProjectilSpeed = projectileSpeed;

        _currentHasToStun = hasToStun;
        _currentTimeOfElctricalStun = timeOfElctricalStun;
    }

    #region ShootingMethods
    public override IEnumerator OnShoot(int nbrOfShoot, float timeEachShoot, float recoilTimeEachBurst)
    {
        CanShoot = false;

        m_crosshair.On_Shoot();

        for (int i = 0; i < nbrOfShoot; ++i)
        {
            //StartCoroutine(RecoilCurve());
           // _BPMSystem.LoseBPM(_currentBPMCost);
            if (_currentProjectil != null)
            {
                InitiateRayCast(InstatiateProj());
                audioControl.PlayAppropriateFireSound((int)_BPMSystem.CurrentWeaponState);
            }
            else
            {
                InitiateRayCast();
            }

            Fire();
            _BPMSystem.LoseBPM(_currentBPMCost);
            yield return new WaitForSeconds(timeEachShoot);
        }
        yield return new WaitForSeconds(recoilTimeEachBurst);

        CanShoot = true;
    }

    

    void FixedUpdate()
    {
        //if (weaponRecoil.m_useRecoil)
        //{
        //    CurrentPositionRecoil = Vector3.Lerp(CurrentPositionRecoil, Vector3.zero, weaponRecoil.PositionRecoil * Time.deltaTime);
        //    CurrentRotationRecoil = Vector3.Lerp(CurrentRotationRecoil, Vector3.zero, weaponRecoil.RotationRecoil * Time.deltaTime);

        //    weaponRecoil.RecoilPositionTranform.localPosition = Vector3.Slerp(weaponRecoil.RecoilPositionTranform.localPosition, CurrentRotationRecoil, weaponRecoil.PositionDampTime * Time.fixedDeltaTime);
        //    weaponRecoil.RecoilRotationTranform.localRotation = Quaternion.Euler(RotationOutput);
        //    RotationOutput = Vector3.Slerp(RotationOutput, CurrentPositionRecoil, weaponRecoil.RotationDampTime * Time.fixedDeltaTime);
        //}

        if (weaponRecoil[(int)_BPMSystem.CurrentWeaponState].m_useCameraRecoil)
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, weaponRecoil[(int)_BPMSystem.CurrentWeaponState].returnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, currentRotation, weaponRecoil[(int)_BPMSystem.CurrentWeaponState].rotationSpeed * Time.deltaTime);
            if(PlayerController.s_instance.m_references.m_cameraControl != null)
            {
                PlayerController.s_instance.m_references.m_cameraControl.transform.localRotation = Quaternion.Euler(rot);
            }
        }
        else if(weaponRecoil.Length > 1)
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, weaponRecoil[(int)_BPMSystem.CurrentWeaponState-1].returnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, currentRotation, weaponRecoil[(int)_BPMSystem.CurrentWeaponState - 1].rotationSpeed * Time.deltaTime);
            if (PlayerController.s_instance.m_references.m_cameraControl != null)
            {
                PlayerController.s_instance.m_references.m_cameraControl.transform.localRotation = Quaternion.Euler(rot);
            }
        }
        else
        {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, weaponRecoil[0].returnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, currentRotation, weaponRecoil[0].rotationSpeed * Time.deltaTime);
            if (PlayerController.s_instance.m_references.m_cameraControl != null)
            {
                PlayerController.s_instance.m_references.m_cameraControl.transform.localRotation = Quaternion.Euler(rot);
            }
        }
        
        SetCurrentEnemyTargeted();
        SetPlayerCrosshairColor();
    }
    public void Fire()
    {
        //CurrentPositionRecoil += new Vector3(weaponRecoil.RecoilRotation.x, UnityEngine.Random.Range(-weaponRecoil.RecoilRotation.y, weaponRecoil.RecoilRotation.y), UnityEngine.Random.Range(-weaponRecoil.RecoilRotation.z, weaponRecoil.RecoilRotation.z));
        //CurrentRotationRecoil += new Vector3(UnityEngine.Random.Range(-weaponRecoil.RecoilKickBack.x, weaponRecoil.RecoilKickBack.x), UnityEngine.Random.Range(-weaponRecoil.RecoilKickBack.y, weaponRecoil.RecoilKickBack.y), weaponRecoil.RecoilKickBack.z);

        if (weaponRecoil[(int)_BPMSystem.CurrentWeaponState].m_useCameraRecoil)
        {
            currentRotation += new Vector3(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState].recoilRotation.x, UnityEngine.Random.Range(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState].recoilRotation.y, weaponRecoil[(int)_BPMSystem.CurrentWeaponState].recoilRotation.y), UnityEngine.Random.Range(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState].recoilRotation.z, weaponRecoil[(int)_BPMSystem.CurrentWeaponState].recoilRotation.z));
        }
        else if (weaponRecoil.Length > 1)
        {
            currentRotation += new Vector3(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState-1].recoilRotation.x, UnityEngine.Random.Range(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState - 1].recoilRotation.y, weaponRecoil[(int)_BPMSystem.CurrentWeaponState - 1].recoilRotation.y), UnityEngine.Random.Range(-weaponRecoil[(int)_BPMSystem.CurrentWeaponState - 1].recoilRotation.z, weaponRecoil[(int)_BPMSystem.CurrentWeaponState - 1].recoilRotation.z));
        }
        else
        {
            currentRotation += new Vector3(-weaponRecoil[0].recoilRotation.x, UnityEngine.Random.Range(-weaponRecoil[0].recoilRotation.y, weaponRecoil[0].recoilRotation.y), UnityEngine.Random.Range(-weaponRecoil[0].recoilRotation.z, weaponRecoil[0].recoilRotation.z));
        }
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
        if (WeaponForwardRaycast())
        {
            return _hit.point;
        }
        return playerCamera.transform.position + playerCamera.transform.forward * defaultDistance;
    }
    #endregion

    #region RayCast Methods

    void InitiateRayCast(GameObject projectileFeedback)
    {
        if (WeaponForwardRaycast())
        {
            string tag = _hit.collider.tag;

            #region Initiate Proj Var
            Projectile projVar = projectileFeedback.GetComponent<Projectile>();
            Level.AddFX(projVar.muzzleFX, _SMG.firePoint.transform.position, _SMG.firePoint.transform.rotation, _SMG.firePoint.transform);
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
                projVar.CurrentBPMGainWeakSpot = _currentBPMGainWeakSpot;
                projVar.CurrentDamage = _currentDamage;
                projVar.HasToStun = _currentHasToStun;
                projVar.TimeForElectricalStun = _currentTimeOfElctricalStun;
                projVar.Speed = _currentProjectilSpeed;
                projVar.ProjectileType2 = proj;

                if (_BPMSystem.CurrentWeaponState == BPMSystem.WeaponState.Fury)
                    projVar.IsElectricalProjectile = true;

                projVar.WeaponPlayerBehaviour = this;
            }
            #endregion

        }
    }
    void InitiateRayCast()
    {
        if (WeaponForwardRaycast())
        {
            string tag = _hit.collider.tag;
        }
    }

    bool WeaponForwardRaycast()
    {
        return Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastCollision, QueryTriggerInteraction.Collide);
    }

    bool WeaponForwardRaycastForEnemy()
    {
        // Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 100f, Color.blue);
        return Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, Mathf.Infinity, rayCastEnemyCollision, QueryTriggerInteraction.Collide);
    }

    EnemyCaraBase m_currentEnemyCharaBase;
    EnemyCaraBase m_lastEnemyCharaBase;
    bool m_isTouchEnemy = false;
    void SetCurrentEnemyTargeted()
    {
        if (!WeaponForwardRaycastForEnemy())
        {
            if(m_currentEnemyCharaBase == null) // Si le Player ne vise aucun NPC
            {
                return;
            }
            else // Si le Player a visé un NPC mais ne le vise plus
            {
                /// Deactivate shader on current
                On_ActivateShader(false, m_currentEnemyCharaBase);
                m_currentEnemyCharaBase = null;
            }
        }
        else // Si le Player vise un NPC
        {
            if(m_currentEnemyCharaBase == null) // Si le Player vise un NPC après n'en avoir visé aucun
            {
                ReferenceScipt enemyRef = _hit.collider.GetComponent<ReferenceScipt>();
                if(enemyRef != null)
                {
                    m_currentEnemyCharaBase = enemyRef.cara;
                    m_lastEnemyCharaBase = m_currentEnemyCharaBase;
                    /// Activate shader on current
                    On_ActivateShader(true, m_currentEnemyCharaBase);
                }
            }
            else // Si le Player vise un autre NPC juste après en avoir visé un ( Son raycast n'a jamais touché autre chose qu'un NPC, entre les deux NPC )
            {
                ReferenceScipt enemyRef = _hit.collider.GetComponent<ReferenceScipt>();
                if (enemyRef != null)
                {
                    m_currentEnemyCharaBase = enemyRef.cara;
                    if (m_currentEnemyCharaBase != m_lastEnemyCharaBase) // Si le NPC visé est bien différent du NPC visé précédement
                    {
                        /// Deactive shader on last
                        On_ActivateShader(false, m_lastEnemyCharaBase);

                        /// Activate shader on current
                        On_ActivateShader(true, m_currentEnemyCharaBase);
                        m_lastEnemyCharaBase = m_currentEnemyCharaBase;
                    }
                }
            }
        }
    }
    void On_ActivateShader(bool activate, EnemyCaraBase enemy)
    {
        if (enemy == null)
            return;
            
        SuicidalEnemyController suicidalEnemy = enemy.GetComponent<SuicidalEnemyController>();
        suicidalEnemy?.On_ShowEnemyWeakSpot(activate);

        EnemyController rucherEnemy = enemy.GetComponent<EnemyController>();
        rucherEnemy?.On_ShowEnemyWeakSpot(activate);
    }

    void SetPlayerCrosshairColor()
    {
        if (!WeaponForwardRaycast())
            return;
        
        m_crosshair.On_ChangeCrosshairColor(_hit.collider.tag);
    }
    public void SetPlayerHitmarker(string colliderTag)
    {
        if (colliderTag == "NoSpot")
        {
            SetImagesColor(m_hitmarkers.m_img, m_hitmarkers.m_onEnemyNoSpot);
            // SetSpriteImages(m_hitmarkers.m_img, m_hitmarkers.m_noSpotMarker);
            audioControl?.On_HitMarkerNoSpot();
        }
        else if (colliderTag == "WeakSpot")
        {
            SetImagesColor(m_hitmarkers.m_img, m_hitmarkers.m_onEnemyWeakspot);
            // SetSpriteImages(m_hitmarkers.m_img, m_hitmarkers.m_weakSpotMarker);
            audioControl?.On_HitMarkerWeakSpot();
        }
        
        if (m_showPlayerHitMarker != null)
            StopCoroutine(m_showPlayerHitMarker);
        m_showPlayerHitMarker = ShowPlayerHitMarker();
        StartCoroutine(m_showPlayerHitMarker);
    }
    IEnumerator ShowPlayerHitMarker()
    {
        EnableImages(m_hitmarkers.m_img, false);
        yield return new WaitForSeconds(m_hitmarkers.m_waitTimeToShowMarker);

        if (m_hitmarkers.m_hitmarkersImgs.Length != m_currentShootCount)
            SetSpriteImages(m_hitmarkers.m_img, m_hitmarkers.m_hitmarkersImgs[m_currentShootCount]);

        EnableImages(m_hitmarkers.m_img, true);
        
        Color imageColor = new Color(m_hitmarkers.m_img[0].color.r, m_hitmarkers.m_img[0].color.g, m_hitmarkers.m_img[0].color.b, 1);
        SetImagesColor(m_hitmarkers.m_img, imageColor);

        if (m_currentShootCount < m_hitmarkers.m_hitmarkersImgs.Length)
            m_currentShootCount ++;

        yield return new WaitForSeconds(m_hitmarkers.m_timeToShow);

        Color fromColor = m_hitmarkers.m_img[0].color;
        Color toColor = new Color(m_hitmarkers.m_img[0].color.r, m_hitmarkers.m_img[0].color.g, m_hitmarkers.m_img[0].color.b, 0);

        Color actualColor = fromColor;
        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.a - toColor.a);
        float speed = distance / m_hitmarkers.m_timeToHideMarker;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualColor = Color.Lerp(fromColor, toColor, fracJourney);
            SetImagesColor(m_hitmarkers.m_img, actualColor);
            yield return null;
        }

        // Reset Shoot
        m_currentShootCount = 0;
        // m_hitmarkers.m_img.rectTransform.localScale = Vector3.one;
    }
    void SetImageColor(Image image, Color color)
    {
        if (image.color != color)
            image.color = color;
    }
    void SetImagesColor(Image[] image, Color color)
    {
        for (int i = 0, l = image.Length; i < l; ++i)
        {
            if (image[i] != null)
                if (image[i].color != color)
                    image[i].color = color;
        }
    }
    void SetSpriteImages(Image[] image, Sprite newSprite)
    {
        for (int i = 0, l = image.Length; i < l; ++i)
        {
            if (image[i] != null)
                if (image[i].sprite != newSprite)
                    image[i].sprite = newSprite;
        }
    }
    void EnableImages(Image[] image, bool enable)
    {
        for (int i = 0, l = image.Length; i < l; ++i)
        {
            if (image[i] != null)
                if (image[i].enabled != enable)
                    image[i].enabled = enable;
        }
    }

    #endregion

}
