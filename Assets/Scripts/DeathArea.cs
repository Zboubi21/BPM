﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoxCollider))]
public class DeathArea : MonoBehaviour
{
    
    [SerializeField] bool m_showGizmos = true;
    [SerializeField] CanvasGroupeHandeler[] fadeHandeler;

    BoxCollider boxCol;
    public BoxCollider BoxCol { get => boxCol = GetComponent<BoxCollider>(); }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && fadeHandeler[0].canvasGroup != null && fadeHandeler[1].canvasGroup != null)
        {
            //col.GetComponent<BPMSystem>().On_PlayerFallIntoTheVoid(transform);
            StartCoroutine(FadeControl(col));
        }
    }

    IEnumerator FadeControl(Collider col)
    {
        yield return StartCoroutine(StartFadeBlack(0, 1, 0));
        col.GetComponent<BPMSystem>().On_PlayerFallIntoTheVoid(transform);
        yield return StartCoroutine(StartFadeBlack(1, 0, 1));
    }

    IEnumerator StartFadeBlack(int start, int end, int index)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / fadeHandeler[index].timeOfFade <= 1)
        {
            yield return new WaitForSeconds(0.01f);
            _currentTimeOfAnimation += Time.deltaTime;

            float value = fadeHandeler[index].fadeEffect.Evaluate(_currentTimeOfAnimation / fadeHandeler[index].timeOfFade);

            Color color = fadeHandeler[index].canvasGroup.GetComponent<Image>().color;
            color.a = Mathf.Lerp(start, end, value);
            fadeHandeler[index].canvasGroup.GetComponent<Image>().color = color;
            yield return null;

        }
        _currentTimeOfAnimation = 0;

    }

    void OnDrawGizmos()
    {
        if (m_showGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + BoxCol.center, BoxCol.size);
        }
    }

}
