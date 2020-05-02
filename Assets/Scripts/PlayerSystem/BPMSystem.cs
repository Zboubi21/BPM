using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using PoolTypes;

public class BPMSystem : MonoBehaviour
{
    [SerializeField] bool m_playerCanDie = true;
    [SerializeField] bool m_playerCanTakeDamage = true;
    [SerializeField] bool m_canChangeWeaponLvl = true;
    public BPM _BPM = new BPM();
    [Serializable]
    public class BPM
    {
        public int maxBPM = 300;
        public int startingBPM = 100;
        public int criticalLvlOfBPM = 75;
        public int m_activateFuryBPM = 500;
        public int m_loseBpmWhenFallIntoTheVoid = 25;

        public GainBpmLvlUp m_bpmGainedWhendLvlUp;
        [Serializable] public class GainBpmLvlUp
        {
            public int m_getLvl2Bpm = 25;
            public int m_getLvl3Bpm = 50;
        }

        [Header("Feedback")]
        public ChangeImageValues m_criticalBpmFeedBackScreen;

        // public float BPMGain_OnNoSpot;
        // public float BPMGain_OnWeak;
        //public int BPMGain_OnArmor;
        //public int BPMGain_OnDestructableEnvironment;

        public PlayerBpmGui m_playerBpmGui;

        public Gauge m_bpmGauge;
        [Serializable] public class Gauge
        {
            public SkinnedMeshRenderer m_gaugeShader;
            public int m_matNbr = 0;

            [Header("General Speed")]
            public float m_gaugeSpeed = 1;

            public GaugePolish m_gaugePolish;
            [Serializable] public class GaugePolish
            {
                [Header("Moving Delta")]
                public float m_minMoveDeltaValue = 0.025f;
                public float m_maxMoveDeltaValue = 0.1f;

                [Header("Moving Speed")]
                public float m_minMoveDeltaSpeed = 0.1f;
                public float m_maxMoveDeltaSpeed = 0.25f;

                [Header("Cardio Speed")]
                public float m_minCardioSpeed = 40;
                public float m_maxCardioSpeed = 50;
                public float m_changeCardioSpeed = 10;

                [Header("Pique Length")]
                // public float m_minCardioLength = 0.5f;
                // public float m_maxCardioLength = 2.5f;
                // public float m_changeLengthSpeed = 10;
                public float m_lvl1Length = 0.5f;
                public float m_lvl2Length = 1f;
                public float m_lvl3Length = 1.5f;
                public float m_furyLength = 2.5f;

                [Header("Pique Height")]
                [Range(0, 1)] public float m_minCardioHeight = 0;
                [Range(0, 1)] public float m_maxCardioHeight = 1;
                public float m_changeHeightSpeed = 10;
            }
        }
    
        public Eyelet m_eyelet;
        [Serializable] public class Eyelet
        {
            public EyeletSettings m_firstEyeletMesh;
            [Space]
            public EyeletSettings m_secondEyeletMesh;
            [Space]
            public EyeletSettings m_thirdEyeletMesh;

            [Header("Strips")]
            public SkinnedMeshRenderer m_stripMesh;
            public int m_stripMaterialNbr = 4;
            public EyeletSettings m_weaponStrip;
        }
        [Serializable] public class EyeletSettings
        {
            public MeshRenderer[] m_mesh;
            public Color m_fromColor = Color.black;
            public Color m_toColor = Color.white;
            public float m_timeToChangeColor = 0.5f;
            [HideInInspector] public float m_speed;
            public AnimationCurve m_changeColorCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        }
    }

    float m_targetedGaugeValue;
    float m_currentBpmGaugeValue;
    float m_currentCardioSpeed;
    float m_currentCardioLength;
    float m_currentCardioHeight;

    float _currentBPM;

    [Space]
    public WeaponsLevel _weaponsLevel = new WeaponsLevel();
    [Serializable]
    public class WeaponsLevel
    {
        public int firstWeaponLevel = 150;
        public int secondWeaponLevel = 225;
    }

    public enum WeaponState
    {
        Level0,
        Level1,
        Level2,
        Fury
    }
    WeaponState _currentWeaponState = WeaponState.Level0;
    public WeaponState CurrentWeaponState { get => _currentWeaponState; set => _currentWeaponState = value; }
    WeaponState _lastWeaponState = WeaponState.Level0;

    [Space]
    public Overdrenaline _overdrenaline = new Overdrenaline();
    [Serializable]
    public class Overdrenaline
    {
        [Tooltip("In seconds")]
        public float overdrenalineCooldown = 60f;
        public float timeOfOverAdrenaline = 15f;

        [Header("Feedback")]
        public ChangeImageValues m_overadrenalineFeedBackScreen;
        public ParticleSystem m_overadrenalineFeedBackParticles;

        [Header("Shader")]
        public SkinnedMeshRenderer m_mesh;
        public int m_matNbr = 0;
        public float m_timeToShowCantActivateFury = 0.5f;
    }

    [Space]
    [SerializeField] DamageIndicatorParameters m_damageIndicator;
    [Serializable] public class DamageIndicatorParameters
    {
        public Transform m_indicatorRoot;
    }

    [Header("Heart sound")]
    // public AudioSource m_heartAudioSource;
    // public float m_waitTimeBetweenSound = 0.25f, m_waitTimeAfterSound = 0.33f;
    // public AudioClip m_firstClip, m_secondClip;
    
    float _currentOverdrenalineCooldown;
    bool _canUseFury;
    bool _furyCoolDownOver = true;
    bool _isCurrentlyOnFury;
    public bool IsCurrentlyOnFury { get => _isCurrentlyOnFury; }
    public float CurrentBPM { get => _currentBPM; }

    bool m_isInCriticalLevelOfBPM = false;
    PlayerController m_playerController;
    WeaponPlayerBehaviour weapon;
    PlayerAudioController audioControl;

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        weapon = GetComponent<WeaponPlayerBehaviour>();
        audioControl = m_playerController.m_references.m_playerAudio;
        _currentBPM = _BPM.startingBPM;
        _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;

        FeedBackBPM();

        GainBPM(0f);
        SetupEyeletEmissive();
        // Debug.LogError("Coucou Hugo et Quentin, je fais ça juste pour faire chier J :) ");
    }

    private void Update()
    {
        FuryHandeler();

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
            GainBPM(25);
        if (Input.GetKeyDown(KeyCode.Y))
            LoseBPM(25);
        // if (Input.GetKey(KeyCode.B))
        #endif
    }
    void FixedUpdate()
    {
        SetBpmGaugeShader();
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss, Transform shooter = null)
    {
        if (!m_playerCanTakeDamage)
            return;
        
        if (shooter != null)
            AddDamageIndicator(shooter);

        if (_isCurrentlyOnFury)
            return;

        float _newCurrentBPM = _currentBPM - BPMLoss;

        if (!_isCurrentlyOnFury)
        {
            if (_newCurrentBPM > 0)
            {
                _currentBPM -= Mathf.CeilToInt(BPMLoss);
                // DeactivateWeaponLevel(_currentBPM);

                // CheckCriticalLevelOfBPM();
            }
            else
            {
                _currentBPM = 0;
                
                if (m_playerCanDie)
                    On_PlayerHasNoBpm();
            }
            CheckCriticalLevelOfBPM();
        }
        ChangeWeaponLevel(_currentBPM);
        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(false, Mathf.CeilToInt(BPMLoss));
        UpdateEyeletFuryFeedback();
    }

    public void GainBPM(float BPMGain, bool specialGain = false)
    {
        if (_isCurrentlyOnFury)
            return;

        if (hasLoseAWeaponLevel)
        {
            switch (_currentWeaponState)
            {
                case WeaponState.Level0:
                    if(BPMGain == weapon.weaponStats._weaponLevel0.BPMGainOnHit)
                    {
                        BPMGain = weapon.weaponStats._weaponLevel1.BPMGainOnHit;
                    }
                    break;
                case WeaponState.Level1:
                    if (BPMGain == weapon.weaponStats._weaponLevel1.BPMGainOnHit)
                    {
                        BPMGain = weapon.weaponStats._weaponLevel2.BPMGainOnHit;
                    }
                    break;
                default:
                    break;
            }
            hasLoseAWeaponLevel = false;
        }
        float _newCurrentBPM = _currentBPM + BPMGain;

        if (_newCurrentBPM < _BPM.maxBPM)
        {
            _currentBPM += Mathf.CeilToInt(BPMGain);
        }
        else
        {
            _currentBPM = _BPM.maxBPM;
        }
        CheckCanActivateFury();
        CheckCriticalLevelOfBPM();  // Pas sûr de le mettre ici
        ChangeWeaponLevel(_currentBPM);
        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(true, Mathf.CeilToInt(BPMGain), specialGain);
        UpdateEyeletFuryFeedback();
    }
    void CheckCriticalLevelOfBPM()
    {
        if (_currentBPM <= _BPM.criticalLvlOfBPM && !m_isInCriticalLevelOfBPM)
        {
            m_isInCriticalLevelOfBPM = true;
            _BPM.m_playerBpmGui.On_CriticalLevelOfBPM(m_isInCriticalLevelOfBPM);
            _BPM.m_criticalBpmFeedBackScreen.SwitchValue();
            audioControl?.On_CriticalBpm(true);
        }
        else if(_currentBPM > _BPM.criticalLvlOfBPM && m_isInCriticalLevelOfBPM)
        {
            m_isInCriticalLevelOfBPM = false;
            _BPM.m_playerBpmGui.On_CriticalLevelOfBPM(m_isInCriticalLevelOfBPM);
            _BPM.m_criticalBpmFeedBackScreen.SwitchValue();
            audioControl?.On_CriticalBpm(false);
        }
    }
    void CheckCanActivateFury()
    {
        if (_currentBPM >= _BPM.m_activateFuryBPM && _furyCoolDownOver && !_canUseFury && !_isCurrentlyOnFury)
        {
            _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_BPMReady", 1);
            _canUseFury = true;
        }
    }

    void FeedBackBPM()
    {
        _BPM.m_playerBpmGui.SetPlayerBpm(_currentBPM);
        m_targetedGaugeValue = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
    }

    float m_currentBpmShaderGauge = 0;
    void SetBpmGaugeShader()
    {
        float deltaBPM = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);

        float deltaValue = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minMoveDeltaValue, _BPM.m_bpmGauge.m_gaugePolish.m_maxMoveDeltaValue, deltaBPM);
        float deltaSpeed = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minMoveDeltaSpeed, _BPM.m_bpmGauge.m_gaugePolish.m_maxMoveDeltaSpeed, deltaBPM);
        float delta = Mathf.PingPong(Time.time * deltaSpeed, deltaValue);
        m_currentBpmGaugeValue = Mathf.Lerp(m_currentBpmGaugeValue, m_targetedGaugeValue + delta, Time.deltaTime * _BPM.m_bpmGauge.m_gaugeSpeed);
        _BPM.m_bpmGauge.m_gaugeShader.materials[_BPM.m_bpmGauge.m_matNbr].SetFloat("_Silder_BPM", m_currentBpmGaugeValue);
        _BPM.m_bpmGauge.m_gaugeShader.materials[_BPM.m_bpmGauge.m_matNbr].SetFloat("_Slide_BPM_Arriere", m_currentBpmGaugeValue);

        // float cardioLengthTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioLength, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioLength, deltaBPM);
        // m_currentCardioLength = Mathf.Lerp(m_currentCardioLength, cardioLengthTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeLengthSpeed);
        // _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Pique", m_currentCardioLength);

        float cardioHeightTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioHeight, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioHeight, deltaBPM);
        m_currentCardioHeight = Mathf.Lerp(m_currentCardioHeight, cardioHeightTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeHeightSpeed);
        _BPM.m_bpmGauge.m_gaugeShader.materials[_BPM.m_bpmGauge.m_matNbr].SetFloat("_Anim_pic", m_currentCardioHeight);
        

        float cardioSpeedTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioSpeed, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioSpeed, deltaBPM);
        m_currentCardioSpeed = Mathf.Lerp(m_currentCardioSpeed, cardioSpeedTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeCardioSpeed);

        m_currentBpmShaderGauge = m_currentBpmShaderGauge + 1 * m_currentCardioSpeed * Time.deltaTime;
        _BPM.m_bpmGauge.m_gaugeShader.materials[_BPM.m_bpmGauge.m_matNbr].SetFloat("_CurrentPos", m_currentBpmShaderGauge);
    }

    void ChangeBpmShaderGaugeLength()
    {
        float value = 0;
        switch (_currentWeaponState)
        {
            case WeaponState.Level0:
                value = _BPM.m_bpmGauge.m_gaugePolish.m_lvl1Length;
            break;
            case WeaponState.Level1:
                value = _BPM.m_bpmGauge.m_gaugePolish.m_lvl2Length;
            break;
            case WeaponState.Level2:
                value = _BPM.m_bpmGauge.m_gaugePolish.m_lvl3Length;
            break;
            case WeaponState.Fury:
                value = _BPM.m_bpmGauge.m_gaugePolish.m_furyLength;
            break;
        }
        _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Pique", value);
    }

    void On_PlayerHasNoBpm()
    {
        SceneReloader.s_instance?.On_ResetLvl();
    }
    public void On_PlayerFallIntoTheVoid()
    {
        transform.position = GameManager.Instance.RespawnPos.position;
        LoseBPM(_BPM.m_loseBpmWhenFallIntoTheVoid);
    }

    #endregion

    #region Activate and Deactivate Weapon
    bool hasLoseAWeaponLevel;
    void ChangeWeaponLevel(float currentBPM)
    {
        if (!m_canChangeWeaponLvl)
            return;
        
        if (currentBPM >= _weaponsLevel.firstWeaponLevel)
        {
            if(currentBPM >= _weaponsLevel.secondWeaponLevel)
            {
                if (_currentWeaponState != WeaponState.Level2)
                {
                    if(audioControl != null)
                    {
                        audioControl.PlayWeaponUpgradeSound(1);
                    }
                    ChangeWeaponState(WeaponState.Level2);
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(2);
                    PlayerController.s_instance.On_BpmLevelChanged(2);
                    ChangeBpmShaderGaugeLength();
                    UpdateEyeletLvlFeedback();

                    if (_lastWeaponState == WeaponState.Level1)
                        GainBPM(_BPM.m_bpmGainedWhendLvlUp.m_getLvl3Bpm, true);
                }
            }
            else
            {
                if (_currentWeaponState != WeaponState.Level1)
                {
                    if (_currentWeaponState == WeaponState.Level2)
                    {
                        if (audioControl != null)
                        {
                            audioControl.PlayWeaponDegradeSound(1);
                        }
                        hasLoseAWeaponLevel = true;
                    }
                    else if (_currentWeaponState == WeaponState.Level0)
                    {
                        if (audioControl != null)
                        {
                            audioControl.PlayWeaponUpgradeSound(0);
                        }
                    }
                    ChangeWeaponState(WeaponState.Level1);
                    ChangeBpmShaderGaugeLength();
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(1);
                    PlayerController.s_instance.On_BpmLevelChanged(1);
                    UpdateEyeletLvlFeedback();

                    if (_lastWeaponState == WeaponState.Level0)
                        GainBPM(_BPM.m_bpmGainedWhendLvlUp.m_getLvl2Bpm, true);
                }
            }
        }
        else
        {
            if (_currentWeaponState != WeaponState.Level0)
            {
                if (audioControl != null)
                {
                    audioControl.PlayWeaponDegradeSound(2);
                }
                hasLoseAWeaponLevel = true;
                ChangeWeaponState(WeaponState.Level0);
                ChangeBpmShaderGaugeLength();
                _BPM.m_playerBpmGui.On_WeaponLvlChanged(0);
                PlayerController.s_instance.On_BpmLevelChanged(0);
                UpdateEyeletLvlFeedback();
            }
        }
        ChangeWeaponStats();
    }

    void ChangeWeaponState(WeaponState newState)
    {
        _lastWeaponState = _currentWeaponState;
        _currentWeaponState = newState;
    }

    void DeactivateWeaponLevel(float currentBPM)
    {
        if (currentBPM < _weaponsLevel.secondWeaponLevel)
        {
            if (currentBPM < _weaponsLevel.firstWeaponLevel)
            {
                ChangeWeaponState(WeaponState.Level0);
            }
            else
            {
                ChangeWeaponState(WeaponState.Level1);
            }
            ChangeWeaponStats();
        }
        else
        {
            ChangeWeaponState(WeaponState.Level2);
        }
    }

    void ChangeWeaponStats()
    {
        weapon?.ChangeWeaponStats();
    }
    #endregion

    #region Overadrenaline

    void FuryHandeler()
    {
        if (Input.GetButtonDown("OverAdrenaline") && CanUsedFury())
        {
            //Set le BPM au max quand on active l'overadrénaline
            GainBPM(_BPM.maxBPM - _currentBPM); // Le problème quand on fait ça c'est que on peut relancer l'overadrénaline

            _canUseFury = false;

            _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_BPMReady", 0);
            StartCoroutine(OnOverADActivate());
        }
        else if (Input.GetButtonDown("OverAdrenaline") && !CanUsedFury() && !m_showCanActivateOverFeedback)
        {
            StartCoroutine(ShowCanActivateOverFeedback());
        }
        FuryCoolDownHandeler();
    }
    
    bool m_showCanActivateOverFeedback = false;
    IEnumerator ShowCanActivateOverFeedback()
    {
        audioControl?.On_CantActivateOverFeedback();
        m_showCanActivateOverFeedback = true;
        _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_NotReady", 1);
        yield return new WaitForSeconds(_overdrenaline.m_timeToShowCantActivateFury);
        _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_NotReady", 0);
        m_showCanActivateOverFeedback = false;
    }

    IEnumerator OnOverADActivate()
    {
        ActivateBool(true);
        // play anim fuey
        // play sound fury
        // play fx fury
        ChangeWeaponState(WeaponState.Fury);
        ChangeBpmShaderGaugeLength();
        ChangeWeaponStats();
        audioControl?.PlayWeaponUpgradeSound(2);

        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.overAdrenaline.onUsingFury);

        _overdrenaline.m_overadrenalineFeedBackScreen.SwitchValue();
        var mainOveradrenalineFeedBackParticles = _overdrenaline.m_overadrenalineFeedBackParticles.main;
        mainOveradrenalineFeedBackParticles.loop = true;
        _overdrenaline.m_overadrenalineFeedBackParticles.Play();
        audioControl?.On_Overadrenaline(true);
        PlayerController.s_instance.On_ActivateOveradrenaline(true);

        _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_FuryActivated", 1);

        // ------ Wait the OverAdrenaline Timer ------
        float timer = 0;
        while (timer < _overdrenaline.timeOfOverAdrenaline)
        {
            timer += Time.deltaTime;
            float cooldownValue = Mathf.InverseLerp(_overdrenaline.timeOfOverAdrenaline, 0, timer);
            _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetFloat("_CooldownValue", cooldownValue);
            yield return null;
        }
        // -------------------------------------------

        _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetInt("_FuryActivated", 0);
        _currentOverdrenalineCooldown = 0;
        ChangeBpmShaderGaugeLength();
        audioControl?.PlayWeaponDegradeSound(0);

        ChangeWeaponState(WeaponState.Level2);
        ChangeWeaponStats();
        ActivateBool(false);
        
        _overdrenaline.m_overadrenalineFeedBackScreen.SwitchValue();
        mainOveradrenalineFeedBackParticles.loop = false;
        audioControl?.On_Overadrenaline(false);
        PlayerController.s_instance.On_ActivateOveradrenaline(false);
    }
    ParticleSystem ps;
    void ActivateBool(bool b)
    {
        _isCurrentlyOnFury = b;
        _BPM.m_playerBpmGui.On_OverAdrenalineActivated(b);
    }

    void FuryCoolDownHandeler()
    {
        if (!_canUseFury && _currentOverdrenalineCooldown != _overdrenaline.overdrenalineCooldown)
        {
            _currentOverdrenalineCooldown += Time.deltaTime;

            // _overdrenaline._overadrenalineCoolDownGauge.fillAmount = Mathf.InverseLerp(0, _overdrenaline.overdrenalineCooldown, _currentOverdrenalineCooldown);
            float cooldownValue = Mathf.InverseLerp(0, _overdrenaline.overdrenalineCooldown, _currentOverdrenalineCooldown);
            _overdrenaline.m_mesh.materials[_overdrenaline.m_matNbr].SetFloat("_CooldownValue", cooldownValue);

            if (_currentOverdrenalineCooldown >= _overdrenaline.overdrenalineCooldown)
            {
                _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;
                _furyCoolDownOver = true;
                CheckCanActivateFury();
            }
            if (_furyCoolDownOver)
                _furyCoolDownOver = false;
        }
        else
        {
            if (!_furyCoolDownOver)
                _furyCoolDownOver = true;
        }
    }

    bool CanUsedFury()
    {
        return (_canUseFury && _furyCoolDownOver);
    }

    void AddDamageIndicator(Transform shooter)
    {
        DamageIndicator di = ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.DamageIndicator, m_damageIndicator.m_indicatorRoot.position, Quaternion.identity, m_damageIndicator.m_indicatorRoot).GetComponent<DamageIndicator>();
        di.SetupIndicator(m_playerController.m_references.m_cameraPivot, shooter);
    }
#endregion

#region Eyelet
    IEnumerator[] m_changeEyeletEmissiveCorout = new IEnumerator[4];
    void SetupEyeletEmissive()
    {
        // Setup Eyelet
        if (_BPM.m_eyelet.m_firstEyeletMesh != null)
            for (int i = 0, l = _BPM.m_eyelet.m_firstEyeletMesh.m_mesh.Length; i < l; ++i)
            {
                Material mat = _BPM.m_eyelet.m_firstEyeletMesh.m_mesh[i].material;
                if (mat != null)
                {
                    mat.EnableKeyword("_EMISSION");
                    if (CurrentWeaponState == WeaponState.Level0)
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_firstEyeletMesh.m_fromColor);
                    else
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_firstEyeletMesh.m_toColor);
                }
            }

        if (_BPM.m_eyelet.m_secondEyeletMesh != null)
            for (int i = 0, l = _BPM.m_eyelet.m_secondEyeletMesh.m_mesh.Length; i < l; ++i)
            {
                Material mat = _BPM.m_eyelet.m_secondEyeletMesh.m_mesh[i].material;
                if (mat != null)
                {
                    mat.EnableKeyword("_EMISSION");
                    if (CurrentWeaponState != WeaponState.Level2)
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_secondEyeletMesh.m_fromColor);
                    else
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_secondEyeletMesh.m_toColor);
                }
            }

        if (_BPM.m_eyelet.m_thirdEyeletMesh != null)
            for (int i = 0, l = _BPM.m_eyelet.m_thirdEyeletMesh.m_mesh.Length; i < l; ++i)
            {
                Material mat = _BPM.m_eyelet.m_thirdEyeletMesh.m_mesh[i].material;
                if (mat != null)
                {
                    mat.EnableKeyword("_EMISSION");
                    if (CurrentWeaponState != WeaponState.Level2)
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_thirdEyeletMesh.m_fromColor);
                    else
                        SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_thirdEyeletMesh.m_toColor);
                }
            }

        _BPM.m_eyelet.m_firstEyeletMesh.m_speed = GetDistanceFromColors(_BPM.m_eyelet.m_firstEyeletMesh.m_fromColor, _BPM.m_eyelet.m_firstEyeletMesh.m_toColor) / _BPM.m_eyelet.m_firstEyeletMesh.m_timeToChangeColor;
        _BPM.m_eyelet.m_secondEyeletMesh.m_speed = GetDistanceFromColors(_BPM.m_eyelet.m_secondEyeletMesh.m_fromColor, _BPM.m_eyelet.m_secondEyeletMesh.m_toColor) / _BPM.m_eyelet.m_secondEyeletMesh.m_timeToChangeColor;
        _BPM.m_eyelet.m_thirdEyeletMesh.m_speed = GetDistanceFromColors(_BPM.m_eyelet.m_thirdEyeletMesh.m_fromColor, _BPM.m_eyelet.m_thirdEyeletMesh.m_toColor) / _BPM.m_eyelet.m_thirdEyeletMesh.m_timeToChangeColor;
    
        // Setup Strip
        if (_BPM.m_eyelet.m_stripMesh != null)
        {
            Material mat = _BPM.m_eyelet.m_stripMesh.materials[_BPM.m_eyelet.m_stripMaterialNbr];
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                if (CurrentWeaponState == WeaponState.Level0)
                    SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_weaponStrip.m_fromColor);
                else
                    SetEmissiveMaterialColor(mat, _BPM.m_eyelet.m_weaponStrip.m_toColor);
            }
        }
        _BPM.m_eyelet.m_weaponStrip.m_speed = GetDistanceFromColors(_BPM.m_eyelet.m_weaponStrip.m_fromColor, _BPM.m_eyelet.m_weaponStrip.m_toColor) / _BPM.m_eyelet.m_weaponStrip.m_timeToChangeColor;
    }
    void SetEmissiveMaterialColor(Material mat, Color color)
    {
        mat.SetColor("_EmissiveColor", color);
    }
    Color GetEmissiveMaterialColor(Material mat)
    {
        return mat.GetColor("_EmissiveColor");
    }
    void UpdateEyeletLvlFeedback()
    {
        // Level UP to lvl 1
        if (CurrentWeaponState == WeaponState.Level1 && _lastWeaponState == WeaponState.Level0)
        {
            if (_BPM.m_eyelet.m_firstEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 0, ChangeEyeletEmissive(_BPM.m_eyelet.m_firstEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_firstEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_firstEyeletMesh.m_toColor, _BPM.m_eyelet.m_firstEyeletMesh.m_speed, _BPM.m_eyelet.m_firstEyeletMesh.m_changeColorCurve));
            
            if (_BPM.m_eyelet.m_stripMesh != null)
                StartChangeEyeletEmissiveCorout(true, 3, ChangeEyeletEmissive(_BPM.m_eyelet.m_stripMesh.materials[_BPM.m_eyelet.m_stripMaterialNbr], _BPM.m_eyelet.m_stripMesh.materials[_BPM.m_eyelet.m_stripMaterialNbr], _BPM.m_eyelet.m_weaponStrip.m_toColor, _BPM.m_eyelet.m_weaponStrip.m_speed, _BPM.m_eyelet.m_weaponStrip.m_changeColorCurve));
        }
        // Level DOWN to lvl 0
        else if (CurrentWeaponState == WeaponState.Level0 && _lastWeaponState == WeaponState.Level1)
        {
            if (_BPM.m_eyelet.m_firstEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 0, ChangeEyeletEmissive(_BPM.m_eyelet.m_firstEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_firstEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_firstEyeletMesh.m_fromColor, _BPM.m_eyelet.m_firstEyeletMesh.m_speed, _BPM.m_eyelet.m_firstEyeletMesh.m_changeColorCurve));
        
            if (_BPM.m_eyelet.m_stripMesh != null)
                StartChangeEyeletEmissiveCorout(true, 3, ChangeEyeletEmissive(_BPM.m_eyelet.m_stripMesh.materials[_BPM.m_eyelet.m_stripMaterialNbr], _BPM.m_eyelet.m_stripMesh.materials[_BPM.m_eyelet.m_stripMaterialNbr], _BPM.m_eyelet.m_weaponStrip.m_fromColor, _BPM.m_eyelet.m_weaponStrip.m_speed, _BPM.m_eyelet.m_weaponStrip.m_changeColorCurve));
        }
        // Level UP to lvl 2
        else if (CurrentWeaponState == WeaponState.Level2 && _lastWeaponState == WeaponState.Level1)
        {
            if (_BPM.m_eyelet.m_secondEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 1, ChangeEyeletEmissive(_BPM.m_eyelet.m_secondEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_secondEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_secondEyeletMesh.m_toColor, _BPM.m_eyelet.m_secondEyeletMesh.m_speed, _BPM.m_eyelet.m_secondEyeletMesh.m_changeColorCurve));

        }
        // Level DOWN to lvl 1
        else if (CurrentWeaponState == WeaponState.Level1 && _lastWeaponState == WeaponState.Level2)
        {
            if (_BPM.m_eyelet.m_secondEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 1, ChangeEyeletEmissive(_BPM.m_eyelet.m_secondEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_secondEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_secondEyeletMesh.m_fromColor, _BPM.m_eyelet.m_secondEyeletMesh.m_speed, _BPM.m_eyelet.m_secondEyeletMesh.m_changeColorCurve));
        }
    }
    bool m_haveBpmFury = false;
    void UpdateEyeletFuryFeedback()
    {
        // Level UP to Fury
        if (_currentBPM >= _BPM.m_activateFuryBPM && !m_haveBpmFury)
        {
            m_haveBpmFury = true;
            if (_BPM.m_eyelet.m_thirdEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 2, ChangeEyeletEmissive(_BPM.m_eyelet.m_thirdEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_thirdEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_thirdEyeletMesh.m_toColor, _BPM.m_eyelet.m_thirdEyeletMesh.m_speed, _BPM.m_eyelet.m_thirdEyeletMesh.m_changeColorCurve));
        }
        // Level DOWN to lvl 2
        else if (_currentBPM < _BPM.m_activateFuryBPM && m_haveBpmFury)
        {
            m_haveBpmFury = false;
            if (_BPM.m_eyelet.m_thirdEyeletMesh != null)
                StartChangeEyeletEmissiveCorout(true, 2, ChangeEyeletEmissive(_BPM.m_eyelet.m_thirdEyeletMesh.m_mesh[0].material, _BPM.m_eyelet.m_thirdEyeletMesh.m_mesh[1].material, _BPM.m_eyelet.m_thirdEyeletMesh.m_fromColor, _BPM.m_eyelet.m_thirdEyeletMesh.m_speed, _BPM.m_eyelet.m_thirdEyeletMesh.m_changeColorCurve));
        }
    }
    void StartChangeEyeletEmissiveCorout(bool start, int arrayIndex, IEnumerator coroutine = null)
    {
        if (m_changeEyeletEmissiveCorout[arrayIndex] != null)
            StopCoroutine(m_changeEyeletEmissiveCorout[arrayIndex]);
        if (start && coroutine != null)
        {
            m_changeEyeletEmissiveCorout[arrayIndex] = coroutine;
            StartCoroutine(m_changeEyeletEmissiveCorout[arrayIndex]);
        }
    }
    IEnumerator ChangeEyeletEmissive(Material firstMaterial, Material secondMaterial, Color toColor, float speed, AnimationCurve curve)
    {
        Color fromColor = GetEmissiveMaterialColor(firstMaterial);

        float distance = GetDistanceFromColors(fromColor, toColor);

        Color actualColor = fromColor;
        float fracJourney = 0;

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualColor = Color.Lerp(fromColor, toColor, curve.Evaluate(fracJourney));
            SetEmissiveMaterialColor(firstMaterial, actualColor);
            SetEmissiveMaterialColor(secondMaterial, actualColor);
            yield return null;
        }
    }
    float GetDistanceFromColors(Color color1, Color color2)
    {
        return Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b) + Mathf.Abs(color1.a - color2.a);
    }
#endregion

}
