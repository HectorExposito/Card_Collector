using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollectionAspectRatio : MonoBehaviour
{
    [SerializeField] private GameObject backButton;
    private void LateUpdate()
    {
        if (Camera.main.aspect == 16 / 9 && backButton.GetComponent<RectTransform>().anchoredPosition.y!=-1235)
        {
            backButton.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(backButton.GetComponent<RectTransform>().anchoredPosition.x, -1235);
        }
    }
}
