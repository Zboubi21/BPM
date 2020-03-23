using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PoolTypes;

public class PlayerBpmGui : MonoBehaviour
{
    
    [Header("BPM Value")]
    [SerializeField] TextMeshProUGUI m_bpmValue;

    [Header("BPM Get/Lost")]
    public GameObject m_spawnTest;
    [SerializeField] Transform m_spawnPos;
    [SerializeField] Color m_getBpmColor = Color.green;
    [SerializeField] Color m_lostBpmColor = Color.red;
    [SerializeField] float m_timeToDo = 0.25f;
    [SerializeField] float m_targetedYPos = 0.1f;

    ObjectPooler m_objectPooler;
    ObjectType m_bpmToSpawn = ObjectType.BpmGuiValues;

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
        
        // GameObject spawnedBpm = m_objectPooler.SpawnObjectFromPool(m_bpmToSpawn, m_spawnPos.position, m_spawnPos.rotation);
        GameObject spawnedBpm = GameObject.Instantiate(m_spawnTest, m_spawnPos.position, m_spawnPos.rotation, m_spawnPos);

        Color guiColor = Color.white;

        TextMeshProUGUI spawnedText = spawnedBpm.GetComponent<TextMeshProUGUI>();
        if (spawnedText != null)
        {
            guiColor = getBpm ? m_getBpmColor : m_lostBpmColor;
            spawnedText.color = guiColor;
            string newText = getBpm ? "+" + bpmValue : "-" + bpmValue;
            spawnedText.text = newText;
        }

        PlayerBpmGuiValues guiValue = spawnedBpm.GetComponent<PlayerBpmGuiValues>();
        if (guiValue != null)
        {
            float targetedYPos = getBpm ? m_targetedYPos : - m_targetedYPos;
            guiValue.StartToMove(targetedYPos, m_timeToDo);
            guiValue.StartToChangeColor(guiColor, m_timeToDo);
        }
    }
    
}
