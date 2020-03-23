using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBpmGui : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI m_bpmValue;

    public void SetPlayerBpm(float bpmValue)
    {
        m_bpmValue.text = bpmValue.ToString();
    }
    public void On_PlayerGetBpm(bool getBpm, float bpmValue)
    {

    }
    
}
