using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    RectTransform rectTransform;
    Vector3 offset;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        offset = new Vector3(rectTransform.rect.width * 0.51f, rectTransform.rect.height * 0.51f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition + offset;
    }
}
