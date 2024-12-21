using System.Collections.Generic;
using UnityEngine;
using static Household;

public class CitizensManager : MonoBehaviour
{
    public static CitizensManager instance;
    public List<Worker> allWorkers = new List<Worker>();
    public GameObject citizenPrefab;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogWarning("Two citizens managers found in scene, Deleted object at " + gameObject.name);
        }
    }
    public void AddResident()
    {
        if (citizenPrefab.GetComponent<Worker>() == null) return;
        GameObject worker = Instantiate(citizenPrefab, transform.position, Quaternion.identity);
        Worker workerComp = worker.GetComponent<Worker>();
        allWorkers.Add(workerComp);
        workerComp.SetHome(this);
        workerComp.SetName(GlobalVariables.instance.GetDefaultName());

        ForceSetState(workerComp, WorkerState.atHome);
        workerComp.StartDay();
    }


}
