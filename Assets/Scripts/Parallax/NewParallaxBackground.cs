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
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
        if (Mathf.Abs(transform.position.x) / 2 >= textureWidthSingle)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }
}
