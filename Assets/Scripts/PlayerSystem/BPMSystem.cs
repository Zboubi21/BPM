using System.Collections;
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

    private void Start()
    {
        _currentBPM = _BPM.startingBPM;
        _currentOverdrenalineCooldown = _overdrenaline.overdrenalineCooldown;

        // _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
        FeedBackBPM();

        GainBPM(0f);
    }

    private void Update()
    {
        FuryHandeler();
    }

    #region BPM Gain and Loss
    public void LoseBPM(float BPMLoss)
    {
        float _newCurrentBPM = _currentBPM - BPMLoss;

        if (!_isCurrentlyOnFury)
        {
            if (_newCurrentBPM > 0)
            {
                _currentBPM -= BPMLoss;
                DeactivateWeaponLevel(_currentBPM);

                if (_currentBPM < _BPM.criticalLvlOfBPM)
                {
                    ///Brancher le level critique de BPM
                }
            }
            else
            {
                _currentBPM = 0;
                ///Tuer le personnage / Brancher respawn
            }
        }
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
        ActivateWeaponLevel(_currentBPM);

        FeedBackBPM();
        _BPM.m_playerBpmGui.On_PlayerGetBpm(true, BPMGain);
    }

    void FeedBackBPM()
    {
        _BPM.BPM_Gauge.fillAmount = Mathf.InverseLerp(0, _BPM.maxBPM, _currentBPM);
        _BPM.m_playerBpmGui.SetPlayerBpm(_currentBPM);
    }

    #endregion

    #region Activate and Deactivate Weapon
    void ActivateWeaponLevel(float currentBPM)
    {
        if (currentBPM >= _weaponsLevel.firstWeaponLevel)
        {
            if(currentBPM >= _weaponsLevel.secondWeaponLevel)
            {
                _currentWeaponState = WeaponState.Level2;
            }
            else
            {
                _currentWeaponState = WeaponState.Level1;
            }

            ChangeWeaponLevel();

        }
        else
        {
            _currentWeaponState = WeaponState.Level0;
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
        yield return new WaitForSeconds(_overdrenaline.timeOfOverAdrenaline);
        _overdrenaline._overdrenalineFeedBack.gameObject.SetActive(false);
        _isCurrentlyOnFury = false;
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
