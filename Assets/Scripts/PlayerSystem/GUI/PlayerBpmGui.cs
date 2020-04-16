﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PoolTypes;

public class PlayerBpmGui : MonoBehaviour
{
    
    [Header("BPM Text Value")]
    [SerializeField] TextMeshProUGUI m_bpmValue;
    [SerializeField] TMP_FontAsset[] m_bpmLvl = new TMP_FontAsset[3];
    [SerializeField] ChangeScaleValues m_textAnim;

    [Header("BPM Get/Lost")]
    // public GameObject m_spawnTest;
    [SerializeField] Transform m_spawnPos;
    [SerializeField] Color m_getBpmColor = Color.green;
    [SerializeField] Color m_lostBpmColor = Color.red;
    [SerializeField] float m_timeToDo = 0.25f;
    [SerializeField] float m_targetedXPos = 0.02f;
    [SerializeField] float m_delayToHideValue = 0.25f;

    [Header("Feedbacks")]
    [SerializeField] ChangeImageValues m_criticalLevelOfBPMFeedback;
    [SerializeField] ChangeImageValues m_overadrenalineFeedback;
    [SerializeField] Color m_normalBpmValueColor = Color.yellow;
    [SerializeField] Color m_criticalBpmValueColor = Color.red;
    [SerializeField] Color m_overadrenalineBpmValueColor = Color.blue;

    ObjectPooler m_objectPooler;
    int m_lastWeaponLvl;

    void Start()
    {
        m_objectPooler = ObjectPooler.Instance;
    }

    public void SetPlayerBpm(float bpmValue)
    {
        m_bpmValue.text = bpmValue.ToString();
    }
    public void On_PlayerGetBpm(bool getBpm, float bpmValue)
    {
        if (bpmValue == 0)
            return;
        
        // GameObject spawnedBpm = GameObject.Instantiate(m_spawnTest, m_spawnPos.position, m_spawnPos.rotation, m_spawnPos);
        GameObject spawnedBpm = m_objectPooler.SpawnObjectFromPool(ObjectType.BpmGuiValues, m_spawnPos.position, m_spawnPos.rotation, m_spawnPos);

        Color guiColor = Color.white;

        TextMeshProUGUI spawnedText = spawnedBpm.GetComponent<TextMeshProUGUI>();
        if (spawnedText != null)
        {
            guiColor = getBpm ? m_getBpmColor : m_lostBpmColor;
            spawnedText.color = guiColor;
            // string newText = getBpm ? "+" + bpmValue : "-" + bpmValue;
            // spawnedText.text = newText;
            spawnedText.text = bpmValue.ToString();
        }

        PlayerBpmGuiValues guiValue = spawnedBpm.GetComponent<PlayerBpmGuiValues>();
        if (guiValue != null)
        {
            float targetedYPos = getBpm ? -m_targetedXPos : m_targetedXPos;
            guiValue.StartToMove(targetedYPos, m_timeToDo);
            guiValue.StartToChangeColor(guiColor, m_timeToDo, m_delayToHideValue);
        }
    }
    public void On_WeaponLvlChanged(int weaponLvl)
    {
        // Debug.Log("Level_" + weaponLvl);
        switch (weaponLvl)
        {
            case 0:
                m_bpmValue.font = m_bpmLvl[0];
            break;
            case 1:
                m_bpmValue.font = m_bpmLvl[1];
            break;
            case 2:
                m_bpmValue.font = m_bpmLvl[2];
            break;
        }
        if (m_lastWeaponLvl > weaponLvl)    // Le niveau de l'arme a baissé
            m_textAnim.SwitchValue(false);
        else                                // Le niveau de l'arme a augmenté
            m_textAnim.SwitchValue(true);

        m_lastWeaponLvl = weaponLvl;
    }
    public void On_CriticalLevelOfBPM(bool inCriticalLevel)
    {
        m_criticalLevelOfBPMFeedback.SwitchValue();
        m_bpmValue.color = inCriticalLevel ? m_criticalBpmValueColor : m_normalBpmValueColor;
    }
    public void On_OverAdrenalineActivated(bool isActivated)
    {
        // m_criticalLevelOfBPMFeedback.StopChangingValues();
        m_overadrenalineFeedback.SwitchValue();
        if (isActivated)
        {
            m_bpmValue.color = m_overadrenalineBpmValueColor;
            m_bpmValue.font = m_bpmLvl[3];
        }
        else
        {
            m_bpmValue.color = m_normalBpmValueColor;
            m_bpmValue.font = m_bpmLvl[2];
        }
    }
    
}
