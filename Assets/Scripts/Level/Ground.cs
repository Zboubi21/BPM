using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GroundType;

public class Ground : MonoBehaviour
{
    
    [SerializeField] GroundTypeEnum m_groundType = GroundTypeEnum.Stone;
    public GroundTypeEnum GroundType { get => m_groundType; }
    
}
