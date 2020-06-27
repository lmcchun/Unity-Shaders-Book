using UnityEngine;

public class Translating : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    private Vector3 startPoint = Vector3.zero;

    [SerializeField]
    private Vector3 endPoint = Vector3.zero;

    [SerializeField]
    private Vector3 lookAt = Vector3.zero;

    [SerializeField]
    private bool pingpong = true;
#pragma warning restore 0649

    private Vector3 curEndPoint = Vector3.zero;

    void Start()
    {
        transform.position = startPoint;
        curEndPoint = endPoint;
    }

    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, curEndPoint, Time.deltaTime * speed);
        transform.LookAt(lookAt);
        if (pingpong)
        {
            if (Vector3.Distance(transform.position, curEndPoint) < 0.001f)
            {
                curEndPoint = Vector3.Distance(curEndPoint, endPoint) < Vector3.Distance(curEndPoint, startPoint) ? startPoint : endPoint;
            }
        }
    }
}
