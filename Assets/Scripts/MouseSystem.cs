using UnityEngine;

public class MouseSystem : MonoBehaviour
{
    public Vector3 pos;
    public RenderTexture rt;

    private void Update()
    {
        Vector3 temp = Input.mousePosition;
        temp.x *= (rt.width / (float)Screen.width);
        temp.y *= (rt.height / (float)Screen.height);

        temp.z = 10;
    
        pos = Cam.instance.mainCam.ScreenToWorldPoint(temp);
        transform.position = pos;
    }
}
