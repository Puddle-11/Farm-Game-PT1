using NUnit.Framework.Constraints;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 axisVal;

    void Update()
    {
        axisVal.x = Input.GetAxis("Horizontal");
        axisVal.y = Input.GetAxis("Vertical");

        transform.position += new Vector3(axisVal.x, 0, axisVal.y) * speed * Time.deltaTime;

    }
}
