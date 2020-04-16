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

        [Header("Feedback")]
        public ChangeImageValues m_criticalBpmFeedBackScreen;

        [Space]
        public float BPMGain_OnNoSpot;
        public float BPMGain_OnWeak;
        //public int BPMGain_OnArmor;
        //public int BPMGain_OnDestructableEnvironment;
        [Space]
        public Image BPM_Gauge;
        public PlayerBpmGui m_playerBpmGui;
        //public Image Electra_Gauge;

        public Gauge m_bpmGauge;
        [Serializable] public class Gauge
        {
            public MeshRenderer m_gaugeShader;

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

    [Space]
    public Overdrenaline _overdrenaline = new Overdrenaline();
    [Serializable]
    public class Overdrenaline
    {
        [Tooltip("In seconds")]
        public float overdrenalineCooldown = 60f;
        public float timeOfOverAdrenaline = 15f;
        [Space]
        public Image _overadrenalineCoolDownGauge;
        public Image _overdrenalineFeedBack;
        public Image _overdrenalineButton;

        [Header("Feedback")]
        public ChangeImageValues m_overadrenalineFeedBackScreen;
        public ParticleSystem m_overadrenalineFeedBackParticles;
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

        // _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
        FeedBackBPM();

        GainBPM(0f);
    }

    private void Update()
    {
        FuryHandeler();

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            GainBPM(25);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LoseBPM(25);
        }
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

                CheckCriticalLevelOfBPM();
            }
            else
            {
                _currentBPM = 0;
                
                if (m_playerCanDie)
                    On_PlayerHasNoBpm();
            }
        }
        ChangeWeaponLevel(_currentBPM);
        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(false, Mathf.CeilToInt(BPMLoss));
    }

    public void GainBPM(float BPMGain)
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
        _BPM.m_playerBpmGui.On_PlayerGetBpm(true, Mathf.CeilToInt(BPMGain));
    }
    void CheckCriticalLevelOfBPM()
    {
        if (_currentBPM < _BPM.criticalLvlOfBPM && !m_isInCriticalLevelOfBPM)
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
            _overdrenaline._overdrenalineButton.gameObject.SetActive(true);
            _canUseFury = true;
        }
    }

    void FeedBackBPM()
    {
        _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
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
        _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Silder_BPM", m_currentBpmGaugeValue);
        _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Slide_BPM_Arriere", m_currentBpmGaugeValue);

        // float cardioLengthTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioLength, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioLength, deltaBPM);
        // m_currentCardioLength = Mathf.Lerp(m_currentCardioLength, cardioLengthTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeLengthSpeed);
        // _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Pique", m_currentCardioLength);

        float cardioHeightTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioHeight, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioHeight, deltaBPM);
        m_currentCardioHeight = Mathf.Lerp(m_currentCardioHeight, cardioHeightTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeHeightSpeed);
        _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_Anim_pic", m_currentCardioHeight);
        

        float cardioSpeedTarget = Mathf.Lerp(_BPM.m_bpmGauge.m_gaugePolish.m_minCardioSpeed, _BPM.m_bpmGauge.m_gaugePolish.m_maxCardioSpeed, deltaBPM);
        m_currentCardioSpeed = Mathf.Lerp(m_currentCardioSpeed, cardioSpeedTarget, Time.deltaTime * _BPM.m_bpmGauge.m_gaugePolish.m_changeCardioSpeed);

        m_currentBpmShaderGauge = m_currentBpmShaderGauge + 1 * m_currentCardioSpeed * Time.deltaTime;
        _BPM.m_bpmGauge.m_gaugeShader.material.SetFloat("_CurrentPos", m_currentBpmShaderGauge);
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
        SceneReloader.s_instance.On_ResetLvl();
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
                    _currentWeaponState = WeaponState.Level2;
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(2);
                    PlayerController.s_instance.On_BpmLevelChanged(2);
                    ChangeBpmShaderGaugeLength();
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
                    _currentWeaponState = WeaponState.Level1;
                    ChangeBpmShaderGaugeLength();
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(1);
                    PlayerController.s_instance.On_BpmLevelChanged(1);
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
                _currentWeaponState = WeaponState.Level0;
                ChangeBpmShaderGaugeLength();
                _BPM.m_playerBpmGui.On_WeaponLvlChanged(0);
                PlayerController.s_instance.On_BpmLevelChanged(0);
            }
        }
        ChangeWeaponStats();
    }

    void DeactivateWeaponLevel(float currentBPM)
    {
        if (currentBPM < _weaponsLevel.secondWeaponLevel)
        {
            if (currentBPM < _weaponsLevel.firstWeaponLevel)
            {
                _currentWeaponState = WeaponState.Level0;
            }
            else
            {
                _currentWeaponState = WeaponState.Level1;
            }
            ChangeWeaponStats();
        }
        else
        {
            _currentWeaponState = WeaponState.Level2;
        }
    }

    void ChangeWeaponStats()
    {
        weapon.ChangeWeaponStats();
    }
    #endregion

    #region Overadrenaline

    void FuryHandeler()
    {
        if (HasUsedFury())
        {
            //Set le BPM au max quand on active l'overadrénaline
            GainBPM(_BPM.maxBPM - _currentBPM); // Le problème quand on fait ça c'est que on peut relancer l'overadrénaline

            _canUseFury = false;

            _overdrenaline._overdrenalineButton.gameObject.SetActive(false);
            StartCoroutine(OnOverADActivate());
        }
        FuryCoolDownHandeler();
    }

    IEnumerator OnOverADActivate()
    {
        ActivateBool(true);
        // play anim fuey
        // play sound fury
        // play fx fury
        _currentWeaponState = WeaponState.Fury;
        ChangeBpmShaderGaugeLength();
        ChangeWeaponStats();
        audioControl?.PlayWeaponUpgradeSound(2);

        _overdrenaline.m_overadrenalineFeedBackScreen.SwitchValue();
        var mainOveradrenalineFeedBackParticles = _overdrenaline.m_overadrenalineFeedBackParticles.main;
        mainOveradrenalineFeedBackParticles.loop = true;
        _overdrenaline.m_overadrenalineFeedBackParticles.Play();
        audioControl?.On_Overadrenaline(true);
        PlayerController.s_instance.On_ActivateOveradrenaline(true);

        yield return new WaitForSeconds(_overdrenaline.timeOfOverAdrenaline);

        _currentOverdrenalineCooldown = 0;
        ChangeBpmShaderGaugeLength();
        audioControl?.PlayWeaponDegradeSound(0);

        _currentWeaponState = WeaponState.Level2;
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
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(b);
        _isCurrentlyOnFury = b;
        _BPM.m_playerBpmGui.On_OverAdrenalineActivated(b);
        m_playerController.On_OveradrenalineIsActivated(b);
    }

    void FuryCoolDownHandeler()
    {
        if (!_canUseFury && _currentOverdrenalineCooldown != _overdrenaline.overdrenalineCooldown)
        {
            _currentOverdrenalineCooldown += Time.deltaTime;

            _overdrenaline._overadrenalineCoolDownGauge.fillAmount = Mathf.InverseLerp(0, _overdrenaline.overdrenalineCooldown, _currentOverdrenalineCooldown);

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

    bool HasUsedFury()
    {
        return (_canUseFury && Input.GetButtonDown("OverAdrenaline") && _furyCoolDownOver);
    }

    void AddDamageIndicator(Transform shooter)
    {
        DamageIndicator di = ObjectPooler.Instance.SpawnObjectFromPool(ObjectType.DamageIndicator, m_damageIndicator.m_indicatorRoot.position, Quaternion.identity, m_damageIndicator.m_indicatorRoot).GetComponent<DamageIndicator>();
        di.SetupIndicator(m_playerController.m_references.m_cameraPivot, shooter);
    }
    #endregion
}
