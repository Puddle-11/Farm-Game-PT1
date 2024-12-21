using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables instance;
    public string[] defaultNames;
    private List<string> availableNames = new List<string>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            InitVars();
        }
        else if(instance != this)
        {
            Debug.LogWarning("Two global variable scripts detected, detroyed gameobject at: " + gameObject.name);
            Destroy(gameObject);
        }


    }
    private void InitVars()
    {
        InitNameList();
    }
    private void InitNameList()
    {
        availableNames = defaultNames.ToList();

    }
    public string GetDefaultName()
    {
        if(availableNames.Count <= 0) InitNameList();
        
        int index = Random.Range(0, availableNames.Count);
        string res = availableNames[index];
        availableNames.RemoveAt(index);
        return res;
    }
}
