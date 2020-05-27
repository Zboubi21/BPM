using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(BoxCollider))]
public class DeathArea : MonoBehaviour
{
    
    [SerializeField] bool m_showGizmos = true;
    [SerializeField] bool m_isWire = true;
    [SerializeField] FadeOverCurveHandeler[] fadeHandeler;

    BoxCollider boxCol;
    public BoxCollider BoxCol { get => boxCol = GetComponent<BoxCollider>(); }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && fadeHandeler[0].objectToApplyFade != null && fadeHandeler[1].objectToApplyFade != null)
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

            Color color = fadeHandeler[index].objectToApplyFade.GetComponent<Image>().color;
            color.a = Mathf.Lerp(start, end, value);
            fadeHandeler[index].objectToApplyFade.GetComponent<Image>().color = color;
            yield return null;

        }
        _currentTimeOfAnimation = 0;

    }

    void OnDrawGizmos()
    {
        if (m_showGizmos)
        {
            Gizmos.color = Color.red;
            if (m_isWire)
                Gizmos.DrawWireCube(transform.position + BoxCol.center, BoxCol.size);
            else
                Gizmos.DrawCube(transform.position + BoxCol.center, BoxCol.size);
        }
    }

}
