using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorDoorOpener : MonoBehaviour
{
    Animator animator;

    public bool usePlayerStart = false;
    static public bool hasBeenInMenu = false;


    public Transform PlayPosition;
    public Transform CreditPosition;
    public Transform QuitPosition;
    [Space]
    GameObject MenuCamera;

    [Header("Transition Control")]
    public FadeHandeler transistionAnimation;

    [Header("Canvas Fade Control")]
    public FadeOverCurveHandeler menuWithLogoCanvasGroup;
    public FadeOverCurveHandeler backButtonCanvasGroup;

    [Header("Fade On Leaving")]
    public FadeOverCurveHandeler panelCanvasGroup;

    [Space]
    [Header("Audio")]
    public AudioSource movingElevator;
    public AudioSource ambianceElevator;
    public FadeHandeler audioFade;

    [Space]
    [Header("Event On Starting Game")]
    public float timeBeforeFiringVoiceLine;
    public AudioSource voiceLine;
    public float timeBeforeOpeningDoors;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (usePlayerStart && !hasBeenInMenu)
        {
            if (PlayerController.s_instance != null)
            {
                //canvas.gameObject.SetActive(true);
                //PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(false);
                //PlayerController.s_instance.On_PlayerEnterInCinematicState(true);
                MenuCamera = PlayerController.s_instance.m_references.m_worldCamera.gameObject;
                StartCoroutine(EventOnStartingGame());
                //PlayerController.s_instance.m_references.m_worldCamera.enabled = false;
                //Cursor.visible = true;
            }
        }
        else
        {
            PlayerController.s_instance.transform.position = GameManager.Instance.RespawnPos.position;
        }
    }

    IEnumerator Transistion(AnimationCurve curve, float timeOfAnimation, Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot, bool reverseAnimation = false)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation <= timeOfAnimation)
        {
            _currentTimeOfAnimation += Time.deltaTime;

            float value = curve.Evaluate(_currentTimeOfAnimation / timeOfAnimation);

            if (!reverseAnimation)
            {
                MenuCamera.transform.position = Vector3.Lerp(startPos, endPos, /*Mathf.SmoothStep(0, 1, */value/*)*/);
                MenuCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, /*Mathf.SmoothStep(0, 1, */value/*)*/);
            }
            else
            {
                MenuCamera.transform.position = Vector3.Lerp(endPos, startPos, /*Mathf.SmoothStep(0, 1, */value/*)*/);
                MenuCamera.transform.rotation = Quaternion.Slerp(endRot , startRot, /*Mathf.SmoothStep(0, 1, */value/*)*/);
            }
            yield return null;
        }
    }

    IEnumerator CanvasGroupeFade(AnimationCurve curve, float timeOfAnimation, CanvasGroup canvasGroup, bool reverseFade = false)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation <= timeOfAnimation)
        {
            _currentTimeOfAnimation += Time.deltaTime;

            float value = curve.Evaluate(_currentTimeOfAnimation / timeOfAnimation);

            if (!reverseFade)
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, value);
            }
            else
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, value);
            }
            yield return null;
        }
        canvasGroup.interactable = !reverseFade;
        canvasGroup.blocksRaycasts = !reverseFade;
    }

    IEnumerator AudioFade(AnimationCurve curve, float timeOfAnimation,AudioSource source, bool reverseAnimation = false)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation <= timeOfAnimation)
        {
            _currentTimeOfAnimation += Time.deltaTime;

            float value = curve.Evaluate(_currentTimeOfAnimation / timeOfAnimation);

            if (!reverseAnimation)
            {
                source.volume = Mathf.Lerp(0, 1, /*Mathf.SmoothStep(0, 1, */value/*)*/);
            }
            else
            {
                source.volume = Mathf.Lerp(1, 0, /*Mathf.SmoothStep(0, 1, */value/*)*/);
            }
            yield return null;
        }
        if(source.volume == 0)
        {
            source.Stop();
        }
    }

    public void ButtonCredit()
    {
        StartCoroutine(Transistion(transistionAnimation.fadeEffect, transistionAnimation.timeOfFade, PlayPosition.position, PlayPosition.rotation, CreditPosition.position, CreditPosition.rotation));
        StartCoroutine(CanvasGroupeFade(menuWithLogoCanvasGroup.fadeEffect, menuWithLogoCanvasGroup.timeOfFade, menuWithLogoCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>(), true));
        StartCoroutine(CanvasGroupeFade(backButtonCanvasGroup.fadeEffect, backButtonCanvasGroup.timeOfFade, backButtonCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>()));
    }
    public void BackFromCredit()
    {
        StartCoroutine(Transistion(transistionAnimation.fadeEffect, transistionAnimation.timeOfFade, CreditPosition.position, CreditPosition.rotation, PlayPosition.position, PlayPosition.rotation));
        StartCoroutine(CanvasGroupeFade(menuWithLogoCanvasGroup.fadeEffect, menuWithLogoCanvasGroup.timeOfFade, menuWithLogoCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>()));
        StartCoroutine(CanvasGroupeFade(backButtonCanvasGroup.fadeEffect, backButtonCanvasGroup.timeOfFade, backButtonCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>(), true));
    }

    public void ButtonStart()
    {
        hasBeenInMenu = true;
        StartCoroutine(CanvasGroupeFade(menuWithLogoCanvasGroup.fadeEffect, menuWithLogoCanvasGroup.timeOfFade, menuWithLogoCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>(), true));
        StartCoroutine(EventOnStartingGame());
    }


    IEnumerator EventOnStartingGame()
    {
        yield return new WaitForSeconds(timeBeforeFiringVoiceLine);
        voiceLine?.Play();
        yield return new WaitForSeconds(timeBeforeOpeningDoors);
        animator.SetTrigger("Open");
        movingElevator?.Play();
        //PlayerController.s_instance.m_scriptOrder.m_cameraControls.ChangeCursorState(true);
        //PlayerController.s_instance.On_PlayerEnterInCinematicState(false);
    }


    public void OnLeavingElevator()
    {
        StartCoroutine(AudioFade(audioFade.fadeEffect, audioFade.timeOfFade, ambianceElevator, true));
        animator.SetTrigger("Close");
    }

    public void ButtonQuit()
    {
        StartCoroutine(ButtonQuitAnimationControl());
    }

    IEnumerator ButtonQuitAnimationControl()
    {
        StartCoroutine(CanvasGroupeFade(menuWithLogoCanvasGroup.fadeEffect, menuWithLogoCanvasGroup.timeOfFade, menuWithLogoCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>(), true));
        yield return StartCoroutine(Transistion(transistionAnimation.fadeEffect, transistionAnimation.timeOfFade, PlayPosition.position, PlayPosition.rotation, QuitPosition.position, QuitPosition.rotation));
        yield return StartCoroutine(CanvasGroupeFade(panelCanvasGroup.fadeEffect, panelCanvasGroup.timeOfFade, panelCanvasGroup.objectToApplyFade.GetComponent<CanvasGroup>()));
        SceneReloader.s_instance?.LeaveGame();
    }

    public void OnQuittingToMenu()
    {
        hasBeenInMenu = false;
        SceneReloader.s_instance?.On_ResetLvl();
    }
}
