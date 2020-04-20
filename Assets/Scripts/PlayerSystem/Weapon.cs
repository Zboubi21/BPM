using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName ="New Weapon", menuName = "Weapon")]
public class Weapon : SerializedScriptableObject
{
    public WeaponLevel0 _weaponLevel0 = new WeaponLevel0();
    [Serializable] public class WeaponLevel0
    {
        [Header("Level 0 stats")]
        public int damage;
        public float attackCooldown;
        [Space]
        [Header("Level 0 BPM stats")]
        public float BPMGainOnHit;
        public float BPMGainOnHitWeakSpot;
        public int BPMCost;
        [Space]
        [Header("Level 0 bullet stats")]
        public float bulletSpeed;
        public bool useElectricalBullet;
        [ShowIf("useElectricalBullet")]
        public float timeOfElectricalStun;
        public GameObject bullet;

    }
    public WeaponLevel1 _weaponLevel1 = new WeaponLevel1();
    [Serializable]
    public class WeaponLevel1
    {
        [Header("Level 1 stats")]
        public int damage;
        public float attackCooldown;
        [Space]
        [Header("Level 1 BPM stats")]
        public float BPMGainOnHit;
        public float BPMGainOnHitWeakSpot;
        public int BPMCost;
        [Space]
        [Header("Level 1 bullet stats")]
        public float bulletSpeed;
        public bool useElectricalBullet;
        [ShowIf("useElectricalBullet")]
        public float timeOfElectricalStun;
        public GameObject bullet;

    }
    public WeaponLevel2 _weaponLevel2 = new WeaponLevel2();
    [Serializable]
    public class WeaponLevel2
    {
        [Header("Level 2 stats")]
        public int damage;
        public float attackCooldown;
        [Space]
        [Header("Level 2 BPM stats")]
        public float BPMGainOnHit;
        public float BPMGainOnHitWeakSpot;
        public int BPMCost;
        [Space]
        [Header("Level 2 bullet stats")]
        public float bulletSpeed;
        public bool useElectricalBullet;
        [ShowIf("useElectricalBullet")]
        public float timeOfElectricalStun;
        public GameObject bullet;

    }
    public WeaponFury _weaponFury = new WeaponFury();
    [Serializable]
    public class WeaponFury
    {
        [Header("Fury stats")]
        public int damage;
        public float attackCooldown;
        [Space]
        [Header("Fury BPM stats")]
        public float BPMGainOnHit;
        public float BPMGainOnHitWeakSpot;
        public int BPMCost;
        [Space]
        [Header("Fury bullet stats")]
        public float bulletSpeed;
        public bool useElectricalBullet;
        [ShowIf("useElectricalBullet")]
        public float timeOfElectricalStun;
        public GameObject bullet;

    }
}
