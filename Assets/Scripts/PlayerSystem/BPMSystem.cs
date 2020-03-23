﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BPMSystem : MonoBehaviour
{
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
    }
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
        Level2
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

    private void Start()
    {
        m_playerController = GetComponent<PlayerController>();

        _currentBPM = _BPM.startingBPM;
        _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;

        // _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
        FeedBackBPM();

        GainBPM(0);
    }

    private void Update()
    {
        FuryHandeler();

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            GainBPM(100);
        }
        #endif
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss, bool playerShot = false)
    {
        if (_isCurrentlyOnFury)
            return;

        // ---------- Dire au WeaponPlayerBehaviour que playerShot = true pour empêcher de pouvoir se suicider en tirant dans le vide ----------
        if (playerShot && _currentBPM <= _BPM.criticalLvlOfBPM)
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
    }

    #endregion

    #region Activate and Deactivate Weapon
    void ChangeWeaponLevel(float currentBPM)
    {
        if (currentBPM >= _weaponsLevel.firstWeaponLevel)
        {
            if(currentBPM >= _weaponsLevel.secondWeaponLevel)
            {
                if (_currentWeaponState != WeaponState.Level2)
                {
                    _currentWeaponState = WeaponState.Level2;
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(2);
                }
            }
            else
            {
                if (_currentWeaponState != WeaponState.Level1)
                {
                    _currentWeaponState = WeaponState.Level1;
                    _BPM.m_playerBpmGui.On_WeaponLvlChanged(1);
                }
            }
            ChangeWeaponLevel();
        }
        else
        {
            if (_currentWeaponState != WeaponState.Level0)
            {
                _currentWeaponState = WeaponState.Level0;
                _BPM.m_playerBpmGui.On_WeaponLvlChanged(0);
            }
        }
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
            ChangeWeaponLevel();
        }
        else
        {
            _currentWeaponState = WeaponState.Level2;
        }
    }

    void ChangeWeaponLevel()
    {
        GetComponent<WeaponBehaviour>().ChangeWeaponStats();
    }

    // script contien les armes ?
    // les armes regarde la state ?
    // enum en privé comment l'atteindre ?

    #endregion

    #region Overadrenaline

    void FuryHandeler()
    {
        if (HasUsedFury())
        {
            _canUseFury = false;

            _currentOverdrenalineCooldown = 0;

            _overdrenaline._overdrenalineButton.gameObject.SetActive(false);
            StartCoroutine(OnOverADActivate());
        }
        _furyCoolDownOver = FuryCoolDownHandeler();
    }

    IEnumerator OnOverADActivate()
    {
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(true);
        _isCurrentlyOnFury = true;
        _BPM.m_playerBpmGui.On_OverAdrenalineActivated(true);
        m_playerController.On_OveradrenalineIsActivated(true);
        yield return new WaitForSeconds(_overdrenaline.timeOfOverAdrenaline);
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(false);
        _isCurrentlyOnFury = false;
        _BPM.m_playerBpmGui.On_OverAdrenalineActivated(false);
        m_playerController.On_OveradrenalineIsActivated(false);
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
        return (_canUseFury && Input.GetKey(KeyCode.A) && _furyCoolDownOver);
    }
    #endregion
}
