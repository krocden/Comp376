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
        offset = new Vector3(0, rectTransform.rect.height * 1.50f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition + offset;
    }
}
