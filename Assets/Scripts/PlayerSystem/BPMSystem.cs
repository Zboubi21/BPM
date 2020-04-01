using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BPMSystem : MonoBehaviour
{
    [SerializeField] bool m_canChangeWeaponLvl = true;
    public BPM _BPM = new BPM();
    [Serializable]
    public class BPM
    {
        public int maxBPM = 300;
        public int startingBPM = 100;
        public int criticalLvlOfBPM = 75;
        [Space]
        public int BPMGain_OnNoSpot;
        public int BPMGain_OnWeak;
        //public int BPMGain_OnArmor;
        //public int BPMGain_OnDestructableEnvironment;
        [Space]
        public Image BPM_Gauge;
        public PlayerBpmGui m_playerBpmGui;
        //public Image Electra_Gauge;

        [Header("BPM Gauge")]
        public MeshRenderer m_bpmGaugeShader;
        public float m_gaugeSpeed = 1;
    }
    float m_targetedGaugeValue;
    float m_currentBpmGaugeValue;

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

    }
    float _currentOverdrenalineCooldown;
    bool _canUseFury;
    bool _furyCoolDownOver;
    bool _isCurrentlyOnFury;
    bool m_isInCriticalLevelOfBPM = false;
    PlayerController m_playerController;
    WeaponBehaviour weapon;
    PlayerAudioController audioControl;
    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        weapon = GetComponent<WeaponBehaviour>();
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
        #endif
    }
    void FixedUpdate()
    {
        SetBpmGaugeShader();
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss)
    {
        if (_isCurrentlyOnFury)
            return;

        float _newCurrentBPM = _currentBPM - BPMLoss;

        if (!_isCurrentlyOnFury)
        {
            if (_newCurrentBPM > 0)
            {
                _currentBPM -= BPMLoss;
                // DeactivateWeaponLevel(_currentBPM);

                CheckCriticalLevelOfBPM();
            }
            else
            {
                _currentBPM = 0;
                ///Tuer le personnage / Brancher respawn
            }
        }
        ChangeWeaponLevel(_currentBPM);
        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(false, BPMLoss);
    }

    public void GainBPM(float BPMGain)
    {
        if (_isCurrentlyOnFury)
            return;

        float _newCurrentBPM = _currentBPM + BPMGain;

        if (_newCurrentBPM < _BPM.maxBPM)
        {
            _currentBPM += BPMGain;
        }
        else
        {
            _currentBPM = _BPM.maxBPM;
            if (_furyCoolDownOver) ///Fury dispo
            {
                _overdrenaline._overdrenalineButton.gameObject.SetActive(true);
                _canUseFury = true;
            }

        }
        CheckCriticalLevelOfBPM();  // Pas sûr de le mettre ici
        ChangeWeaponLevel(_currentBPM);
        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(true, BPMGain);
    }
    void CheckCriticalLevelOfBPM()
    {
        if (_currentBPM < _BPM.criticalLvlOfBPM && !m_isInCriticalLevelOfBPM)
        {
            m_isInCriticalLevelOfBPM = true;
            _BPM.m_playerBpmGui.On_CriticalLevelOfBPM(m_isInCriticalLevelOfBPM);
        }
        else if(_currentBPM > _BPM.criticalLvlOfBPM && m_isInCriticalLevelOfBPM)
        {
            m_isInCriticalLevelOfBPM = false;
            _BPM.m_playerBpmGui.On_CriticalLevelOfBPM(m_isInCriticalLevelOfBPM);
        }
    }

    void FeedBackBPM()
    {
        _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
        _BPM.m_playerBpmGui.SetPlayerBpm(_currentBPM);
        m_targetedGaugeValue = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
    }

    void SetBpmGaugeShader()
    {
        m_currentBpmGaugeValue = Mathf.Lerp(m_currentBpmGaugeValue, m_targetedGaugeValue, Time.deltaTime * _BPM.m_gaugeSpeed);
        _BPM.m_bpmGaugeShader.material.SetFloat("_Silder_BPM", m_currentBpmGaugeValue);
        _BPM.m_bpmGaugeShader.material.SetFloat("_Slide_BPM_Arriere", m_currentBpmGaugeValue);
    }

    #endregion

    #region Activate and Deactivate Weapon
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

                    }
                    else if (_currentWeaponState == WeaponState.Level0)
                    {
                        if (audioControl != null)
                        {
                            audioControl.PlayWeaponUpgradeSound(0);
                        }
                    }
                    _currentWeaponState = WeaponState.Level1;
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(1);

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
                _currentWeaponState = WeaponState.Level0;
                _BPM.m_playerBpmGui.On_WeaponLvlChanged(0);

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
            _canUseFury = false;

            _currentOverdrenalineCooldown = 0;

            //Set le BPM au max quand on active l'overadrénaline
            // GainBPM(_BPM.maxBPM - _currentBPM); // Le problème quand on fait ça c'est que on peut relancer l'overadrénaline

            _overdrenaline._overdrenalineButton.gameObject.SetActive(false);
            StartCoroutine(OnOverADActivate());
        }
        _furyCoolDownOver = FuryCoolDownHandeler();
    }

    IEnumerator OnOverADActivate()
    {
        ActivateBool(true);
        // play anim fuey
        // play sound fury
        // play fx fury
        _currentWeaponState = WeaponState.Fury;
        ChangeWeaponStats();
        if (audioControl != null)
        {
            audioControl.PlayWeaponUpgradeSound(2);
        }
        yield return new WaitForSeconds(_overdrenaline.timeOfOverAdrenaline);
        if (audioControl != null)
        {
            audioControl.PlayWeaponDegradeSound(0);
        }

        _currentWeaponState = WeaponState.Level2;
        ChangeWeaponStats();
        ActivateBool(false);
    }
    void ActivateBool(bool b)
    {
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(b);
        _isCurrentlyOnFury = b;
        _BPM.m_playerBpmGui.On_OverAdrenalineActivated(b);
        m_playerController.On_OveradrenalineIsActivated(b);
    }

    bool FuryCoolDownHandeler()
    {
        if (!_canUseFury && _currentOverdrenalineCooldown != _overdrenaline.overdrenalineCooldown)
        {
            _currentOverdrenalineCooldown += Time.deltaTime;

            _overdrenaline._overadrenalineCoolDownGauge.fillAmount = Mathf.InverseLerp(0, _overdrenaline.overdrenalineCooldown, _currentOverdrenalineCooldown);

            if (_currentOverdrenalineCooldown >= _overdrenaline.overdrenalineCooldown)
            {
                _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    bool HasUsedFury()
    {
        return (_canUseFury && Input.GetButtonDown("OverAdrenaline") && _furyCoolDownOver);
    }
    #endregion
}
