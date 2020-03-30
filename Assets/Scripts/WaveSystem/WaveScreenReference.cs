using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveScreenReference : MonoBehaviour
{
    public Image backGround;
    [Space]
    public TMP_Text[] changingTexts;
    public TMP_Text[] staticTexts;
    [Space]
    public Image[] decorativeImages;
    [Space]
    public Animator animator;
    public AnimationClip[] anims;
}
