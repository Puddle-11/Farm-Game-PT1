using Unity.VisualScripting;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public static Cam instance;
    public Camera mainCam;
    public Camera rtCamera;
    public RenderTexture rt;
    public GameObject quad;
    public GameObject debugSphere;
    public LayerMask lm;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogWarning("Destroyed instance of camera singleton at " + gameObject.name);
            Destroy(gameObject);
        }

    }

    public RaycastHit MouseRaycast(LayerMask _lm)
    {
        return MouseRaycast(mainCam.farClipPlane, _lm);
    }
    public RaycastHit MouseRaycast(float _distance, LayerMask _lm)
    {
        Physics.Raycast(RTtoMainCamSpace(Input.mousePosition), mainCam.transform.forward, out RaycastHit res, _distance, _lm);

        return res;
    }
    public Vector3 RTtoMainCamSpace(Vector3 _pos /*mouse position*/)
    {
        //rtCamera, camera pointed at the quad with the render texture on it (this camera is what is rendered to screen)
        //quad, the quad the render texture is currently on
        //mainCam, the camera pointed at the scene, rendering to a RT
        
        _pos = rtCamera.ScreenToWorldPoint(_pos); //world position of the mouse acording to the RT camera
        Vector3 localpos = transform.InverseTransformPoint(_pos); //local position of the mouse acording to the RT camera

        Vector2 clipSpace = new Vector2 //formally known as NDC, a normalized value between (-1, -1) and (1,1)
            (
            localpos.x / (quad.transform.localScale.x / 2),
            localpos.y / (quad.transform.localScale.y / 2)
            );
        Vector3 viewSpace = new Vector3 //a normalized value between (0,0) (1,1)
            (
            (clipSpace.x + 1) / 2,
            (clipSpace.y + 1) / 2,
            _pos.z
            );
        return mainCam.ViewportToWorldPoint(viewSpace);

    }

}
