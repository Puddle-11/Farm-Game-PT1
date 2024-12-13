using UnityEngine;

public class Cam : MonoBehaviour
{
    public static Cam instance;
    public Camera mainCam;
    public Camera rtCamera;
    public RenderTexture rt;
    public GameObject quad;
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
    private void Update()
    {
        

    }

}
