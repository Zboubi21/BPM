using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangePoutch : MonoBehaviour
{
    public int scoreValue;
    PoutchChara poutchChara;

    ShootingRangeController shootingRangeController;

    //public void FakeConstructor(ShootingRangeController ctrl)
    //{
    //    shootingRangeController = ctrl;
    //}

    private void Start()
    {
        shootingRangeController = ShootingRangeController.s_instance;
        poutchChara = GetComponent<PoutchChara>();
        shootingRangeController.ShootingRangePoutches.Add(gameObject);
        go = false;
    }
    bool go;
    private void Update()
    {
        if (poutchChara.IsDead && !go)
        {
            go = true;
            shootingRangeController.AddScore(scoreValue, gameObject);
        }
    }
}
