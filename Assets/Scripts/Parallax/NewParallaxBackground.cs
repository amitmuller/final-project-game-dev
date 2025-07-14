using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class NewParallaxBackground : MonoBehaviour
{
    [SerializeField] private float speed;

    private float textureWidthSingle;

    private void Start()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        textureWidthSingle = sprite.bounds.size.x;
    }

    private void FixedUpdate()
    {
        transform.localPosition += Vector3.left * speed * Time.fixedDeltaTime;
        if (Mathf.Abs(transform.localPosition.x) / 2 >= textureWidthSingle)
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
