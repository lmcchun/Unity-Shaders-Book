using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private float speed = 10.0f;
#pragma warning restore 0649

    void Update()
    {
        var angle = Time.deltaTime * speed;
        transform.Rotate(new Vector3(angle, angle, angle));
    }
}
