using JetBrains.Annotations;
using Mono.Cecil.Cil;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MouseSystem : MonoBehaviour
{
    public float depthThreshhold;
    public float testResolution;

    public Structure[] buildings;
    public uint buildingType;
    public LayerMask _ground;
    public LayerMask structureMask;
    public GameObject phantom;



    private float rot = 0;
    public float rotationSpeed = 0;
    public MouseType mt;
    private GameObject selectedObj;
    private Vector3 initPosition;
    private float holdRotationChange = 0;
    public float hoverHeight;
    public Forest forestRef;
    public enum MouseType
    {
        Building = 0,
        Breaking = 1,
        Selecting = 2,
        Moving = 3,
    }
    [System.Serializable]
    public struct Structure
    {
        public GameObject prefab;

        public Mesh selectorMesh;
    }
    private void Start()
    {
        SetPhantomMesh(buildings[buildingType].selectorMesh);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateMouseType((MouseType)((((int)mt)+1) % 4));
        }

        switch (mt)
        {
            case MouseType.Building:
                PlaceObject();
                break;
            case MouseType.Breaking:
                BreakObject();

                break;
            case MouseType.Selecting:
                SelectObject();
                break;
            case MouseType.Moving:
                MoveObject();
                phantom.SetActive(false);
                break;
            default:
                break;
        }

    }
    public void UpdateMouseType(MouseType _newType)
    {
        CleanVariables();
        mt = _newType;


    }
    public void CleanVariables()
    {
        rot = 0;
        selectedObj = null;
        initPosition = Vector3.zero;

    }
    public void PlaceObject()
    {
        RaycastHit hit = Cam.instance.MouseRaycast(_ground);
        if (TestPlace(hit, buildings[buildingType].prefab))
        {
            if (Input.GetMouseButtonDown(0))
            {

                GameObject t = Instantiate(buildings[buildingType].prefab, hit.point, Quaternion.AngleAxis(rot, new Vector3(0, 1, 0)));
                forestRef.AddStructure(t);

            }
            SetPhantomMesh(buildings[buildingType].selectorMesh);
            phantom.SetActive(true);
        }
        else phantom.SetActive(false);

        

        if (Input.mouseScrollDelta.y > 0) rot += rotationSpeed * Time.deltaTime;
        else if (Input.mouseScrollDelta.y < 0) rot -= rotationSpeed * Time.deltaTime;
        
        phantom.transform.rotation = Quaternion.AngleAxis(rot, hit.normal);
        phantom.transform.position = hit.point;
    }
    public void MoveObject()
    {
        RaycastHit hit = Cam.instance.MouseRaycast(structureMask);
        RaycastHit ghit = Cam.instance.MouseRaycast(_ground);
        if (selectedObj != null)
        {
            selectedObj.transform.position = ghit.point + Vector3.up * hoverHeight;
        }
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedObj = hit.collider.gameObject;
                forestRef.RemoveStructure(selectedObj);
                initPosition = selectedObj.transform.position;
                holdRotationChange = 0;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedObj != null)
            {
                if (!TestPlace(ghit, selectedObj))
                {
                    selectedObj.transform.position = initPosition;
                }
                else
                {
                    selectedObj.transform.position = ghit.point;

                }
                forestRef.AddStructure(selectedObj);

                selectedObj = null;
                initPosition = Vector3.zero;

            }
        }

    }
    public void BreakObject()
    {
        RaycastHit hit = Cam.instance.MouseRaycast(structureMask);
        if (hit.collider != null)
        {
            phantom.SetActive(true);
            phantom.transform.position = hit.collider.transform.position;
            phantom.transform.rotation = hit.collider.transform.rotation;
        }
        else
        {
            phantom.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                forestRef.RemoveStructure(hit.collider.gameObject);

                Destroy(hit.collider.gameObject);
            }
        }
    }
    public void SelectObject()
    {
        RaycastHit hit = Cam.instance.MouseRaycast(structureMask);
        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedObj = hit.collider.gameObject;
                phantom.SetActive(true);
                phantom.transform.position = selectedObj.transform.position;
                phantom.transform.rotation = selectedObj.transform.rotation;
            }
        }
        else if(selectedObj == null)
        {

            phantom.SetActive(false);
        }
    }
    public void SetPhantomMesh(Mesh m)
    {
        if (phantom.TryGetComponent(out MeshFilter mfRef))
        {
            mfRef.mesh = m;
        }
    }
    public bool TestPlace(RaycastHit _data, GameObject _toPlace)
    {

        bool res = false;
        if (_data.normal != Vector3.up) return res;
        res = true;
        if (_toPlace.TryGetComponent(out MeshRenderer mr))
        {
            //mr.bounds

        }
        else if (_toPlace.TryGetComponent(out Collider coll))
        {
            //coll.bounds
        }
        return res;

    }
}
