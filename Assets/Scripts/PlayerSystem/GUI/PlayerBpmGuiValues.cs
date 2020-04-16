using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PoolTypes;

public class PlayerBpmGuiValues : MonoBehaviour
{

    bool m_isStoped = false;

    void OnEnable()
    {
        m_isStoped = false;
    }

    public void StartToMove(float xPosToReach, float timeToDo)
    {
        StartCoroutine(ChangePos(xPosToReach, timeToDo));
    }
    IEnumerator ChangePos(float xPosToReach, float timeToDo)
    {
        Vector3 fromPos = transform.localPosition;
        Vector3 toPos = new Vector3(xPosToReach, fromPos.y, fromPos.z);
        Vector3 actualPos = fromPos;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);
        float speed = distance / timeToDo;

        while (actualPos != toPos)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            actualPos = Vector3.Lerp(fromPos, toPos, fracJourney);
            transform.localPosition = actualPos;
            yield return null;
        }
        On_StopUseValue();
    }

    public void StartToChangeColor(Color color, float timeToDo, float delay)
    {
        StartCoroutine(ChangeColor(color, timeToDo, delay));
    }
    IEnumerator ChangeColor(Color color, float timeToDo, float delay)
    {
        yield return new WaitForSeconds(delay);
        Color fromColor = color;
        Color toColor = color;
        toColor.a = 0;
        Color actualColor = fromColor;

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float speed = distance / (timeToDo - delay);

        TextMeshProUGUI guiText = GetComponent<TextMeshProUGUI>();

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            
            if (guiText != null)
            {
                actualColor = Color.Lerp(fromColor, toColor, fracJourney);
                guiText.color = actualColor;
            }
            yield return null;
        }
        On_StopUseValue();
    }

    void On_StopUseValue()
    {
        if (m_isStoped)
            return;
        m_isStoped = true;
        ObjectPooler.Instance.ReturnObjectToPool(ObjectType.BpmGuiValues, gameObject);
    }

}
