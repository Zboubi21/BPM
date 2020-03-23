using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBpmGuiValues : MonoBehaviour
{

    public void StartToMove(float yPosToReach, float timeToDo)
    {
        StartCoroutine(ChangePos(yPosToReach, timeToDo));
    }
    IEnumerator ChangePos(float yPosToReach, float timeToDo)
    {
        Vector3 fromPos = transform.localPosition;
        Vector3 toPos = new Vector3(fromPos.x, yPosToReach, fromPos.z);
        Vector3 actualPos = fromPos;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromPos, toPos);
        float speed = distance / timeToDo;

        while (actualPos != toPos)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            transform.localPosition = Vector3.Lerp(fromPos, toPos, fracJourney);
            yield return null;
        }
        Destroy(gameObject); // À remplacer avec l'ObjectPooler !
    }

    public void StartToChangeColor(Color color, float timeToDo)
    {
        StartCoroutine(ChangeColor(color, timeToDo));
    }
    IEnumerator ChangeColor(Color color, float timeToDo)
    {
        Color fromColor = color;
        Color toColor = color;
        toColor.a = 0;
        Color actualColor = fromColor;

        float fracJourney = 0;
        float distance = Mathf.Abs(fromColor.r - toColor.r) + Mathf.Abs(fromColor.g - toColor.g) + Mathf.Abs(fromColor.b - toColor.b) + Mathf.Abs(fromColor.a - toColor.a);
        float speed = distance / timeToDo;

        TextMeshProUGUI guiText = GetComponent<TextMeshProUGUI>();

        while (actualColor != toColor)
        {
            fracJourney += (Time.deltaTime) * speed / distance;
            
            if (guiText != null)
                guiText.color = Color.Lerp(fromColor, toColor, fracJourney);
            yield return null;
        }
        Destroy(gameObject); // À remplacer avec l'ObjectPooler !
    }

}
