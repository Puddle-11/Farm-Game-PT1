using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ConfineToPlane : MonoBehaviour
{

    public Transform target;
    public LayerMask mask;
    private void Update()
    {

        bool state = false;
        Vector3 rayPoint = target.position + Vector3.up * 150;
        RaycastHit hit;
        if (Physics.Raycast(rayPoint, Vector3.down, out hit, 500, mask))
        {
            if (hit.normal == Vector3.up)
            {
                state = true;
            Debug.DrawRay(rayPoint, Vector3.down * 500, Color.green);
            }

        }
        if (state == false)
        {
            Debug.DrawRay(rayPoint, Vector3.down * 500, Color.red);
        }
    }



}
