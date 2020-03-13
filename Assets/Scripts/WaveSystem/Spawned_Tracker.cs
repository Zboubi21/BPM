using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawned_Tracker : MonoBehaviour
{
    WaveController _controller;

    #region Get Set
    public WaveController Controller { get => _controller; set => _controller = value; }
    #endregion

    public void CallDead()
    {
        Controller.NbrOfDeadEnemy++;
        Controller.CheckLivingEnemies();
    }
}
