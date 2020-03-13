using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "New EnemyArchetype", menuName = "EnemyArchetype")]
public class EnemyArchetype : SerializedScriptableObject
{
    //public Head _head = new Head();
    //[Serializable]
    //public class Head
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public RightShoulder _rightShoulder = new RightShoulder();
    //[Serializable]
    //public class RightShoulder
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public LeftShoulder _leftShoulder = new LeftShoulder();
    //[Serializable]
    //public class LeftShoulder
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public Torso _torso = new Torso();
    //[Serializable]
    //public class Torso
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public Backo _backo = new Backo();
    //[Serializable]
    //public class Backo
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public RightKnee _rightKnee = new RightKnee();
    //[Serializable]
    //public class RightKnee
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    //[Space]
    //public LeftKnee _leftKnee = new LeftKnee();
    //[Serializable]
    //public class LeftKnee
    //{
    //    public TypeOfSpot _typeOfSpot;
    //}
    [Space]
    [EnumToggleButtons, HideLabel]
    public TypeOfSpot typeOfSpot;

    [System.Flags]
    public enum TypeOfSpot
    {
        Head = 1 << 1,
        RightShoulder = 1 << 2,
        LeftShoulder = 1 << 3,
        Torso = 1 << 4,
        Backo = 1 << 5,
        RightKnee = 1 << 6,
        LeftKnee = 1 << 7,
        All = Head | RightShoulder | LeftShoulder | Torso | Backo | RightKnee | LeftKnee,
    }

    //public enum TypeOfSpot
    //{
    //    Head,
    //    RightShoulder,
    //    LeftShoulder,
    //    Torso,
    //    Backo,
    //    RightKnee,
    //    LeftKnee,
    //}

    List<bool> spots;

    public List<bool> Spots { get => spots; set => spots = value; }

    //public TypeOfSpot[] e_TypeOfSpot { get => _typeOfSpot; set => _typeOfSpot = value; }

    public void PopulateArray()
    {
        //var value = TypeOfSpot.Head | TypeOfSpot.LeftKnee;
        Spots.Clear();

        foreach (TypeOfSpot flagToCheck in Enum.GetValues(typeof(TypeOfSpot)))
        {
            if (flagToCheck != TypeOfSpot.All)
            {
                if (typeOfSpot.HasFlag(flagToCheck))
                {
                    Spots.Add(true);
                }
                else
                {
                    Spots.Add(false);
                }
            }
        }
    }

    //public enum TypeOfSpot
    //{
    //    NoSpot,
    //    WeakSpot,
    //}


    //[Title("Default")]
    //public SomeBitmaskEnum DefaultEnumBitmask;

    //[Title("Standard Enum")]
    //[EnumToggleButtons]
    //public SomeEnum SomeEnumField;

    //[EnumToggleButtons, HideLabel]
    //public SomeEnum WideEnumField;

    //[Title("Bitmask Enum")]
    //[EnumToggleButtons]
    //public SomeBitmaskEnum BitmaskEnumField;


}
