using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "New EnemyArchetype", menuName = "EnemyArchetype")]
public class EnemyArchetype : SerializedScriptableObject
{
    [EnumToggleButtons, HideLabel]
    public TypeOfSpot typeOfSpot;
    
    [TabGroup("Enemy Weak Spots")]
    [PropertySpace(SpaceBefore = 30, SpaceAfter = 30)]
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
    List<bool> spots;

    public List<bool> Spots { get => spots; set => spots = value; }

    public void PopulateArray()
    {
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

    [TabGroup("Enemy Behavior Chance")]
    [Title("Chance To Reposition", titleAlignment: TitleAlignments.Centered, horizontalLine: true, bold: false)]
    [HideLabel]
    [ProgressBar(0,100, r: 0.204f, g: 0.204f, b: 0.204f, Height = 20, R = 0.102f, G = 1f, B = 1f)]
    public int _chanceToRepositionAfterAnAttack = 10;
    [Space]
    [TabGroup("Enemy Behavior Chance")]
    public int _nbrOfShootBeforeRepositionning;

    [Title("Behavior Change Chance", titleAlignment: TitleAlignments.Centered, horizontalLine: true, bold: false)]

    [TabGroup("Enemy Behavior Chance")]
    [Range(0,100)]
    //[ProgressBar(0, 100, r: 0.51f, g: 0.153f, b: 1f, Height = 20, R = 0.051f, G = 0.153f, B = 1f)]
    public float _chanceToGoInAgressive = 10f;

    [TabGroup("Enemy Behavior Chance")]
    [Range(0, 100)]
    //[ProgressBar(0, 100, r: 0.51f, g: 0.153f, b: 1f, Height = 20, R = 0.051f, G = 0.153f, B = 1f)]
    public float _chanceToGoInDefensive = 10f;

    [Serializable]
    public class MyTabObject
    {
        public int A;
        public int B;
        public int C;
    }

    


    //[TitleGroup("Ints")]
    //public int SomeInt1;

    //[TitleGroup("$SomeString1", "Optional subtitle")]
    //public string SomeString1;

    //[TitleGroup("Ints", "Optional subtitle", alignment: TitleAlignments.Split)]
    //public int SomeInt2;

    //[TitleGroup("$SomeString1", "Optional subtitle")]
    //public string SomeString2;

    //[TitleGroup("Vectors")]
    //public Vector2 SomeVector2 { get; set; }

    //[TitleGroup("Ints/Buttons", indent: false)]
    //private void IntButton() { }

    //[TitleGroup("$SomeString1/Buttons", indent: false)]
    //private void StringButton() { }

    //[TitleGroup("Vectors")]
    //private void VectorButton() { }
}
