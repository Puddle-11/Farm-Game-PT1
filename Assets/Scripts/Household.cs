using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Household : MonoBehaviour
{
    [SerializeField] private List<Worker> residents = new List<Worker>();
    [SerializeField] private int residentsInHouse;
    [SerializeField] private GameObject residentPrefab;
    [SerializeField] private House[] houseHoldHouses;
    [SerializeField] private int basePopulation;
    public float recallTime;
    public float startTime;
    private bool startedDay;
    public enum WorkerState
    {
        atWork,
        inTransitHome,
        inTransitWork,
        atHome


    }

    public void OnEnable()
    {
        BasePopulate();
        startedDay = true;
    }
    private void Update()
    {
        if(1 - TimeManager.instance.filteredTimeOfDay <= recallTime && startedDay)
        {
            for (int i = 0; i < residents.Count; i++)
            {
                residents[i].Recall();
            }
            startedDay = false;
        }

        if(TimeManager.instance.filteredTimeOfDay <= startTime && !startedDay)
        {

            for (int i = 0; i < residents.Count; i++)
            {
                residents[i].StartDay();
            }
            startedDay = true;
        }
    }
    public void BasePopulate()
    {
        if (residents.Count > 0) return; //we dont need to populate, we already have
        for (int i = 0; i < basePopulation; i++)
        {
            AddResident();
        }
    }
    public void AddResident()
    {
        if (residentPrefab.GetComponent<Worker>() == null) return;
        GameObject worker = Instantiate(residentPrefab, transform.position, Quaternion.identity);
        Worker workerComp = worker.GetComponent<Worker>();
        residents.Add(workerComp);
        workerComp.SetHome(this);
        workerComp.SetName(GlobalVariables.instance.GetDefaultName());
        ForceSetState(workerComp, WorkerState.atHome);
        workerComp.StartDay();
    }

    public void SignalState(Worker _worker, WorkerState _state)
    {
        Debug.Log("Signaled");
        switch (_worker.currState)
        {
            case WorkerState.atWork:
                if(_state != WorkerState.atWork) _worker.currState = WorkerState.inTransitHome;

                break;
            case WorkerState.inTransitHome:
                if (_state == WorkerState.atWork) _worker.currState = WorkerState.inTransitWork;
                else _worker.currState = _state;

                break;
            case WorkerState.inTransitWork:
                if (_state == WorkerState.atHome) _worker.currState = WorkerState.inTransitHome;
                else _worker.currState = _state;
                break;
            case WorkerState.atHome:
                if (_state != WorkerState.atHome) _worker.currState = WorkerState.inTransitWork;
                break;
        }
    }
    public void ForceSetState(Worker _worker, WorkerState _state)
    {
        _worker.currState = _state;
    }
    public void WakeResident(Worker _res)
    {
        if (residents.Contains(_res) && _res.currState != WorkerState.atWork)
        {
            SignalState(_res, WorkerState.inTransitWork);
        }
    }
    public void SleepResident(Worker _res)
    {
        if (residents.Contains(_res) && _res.currState != WorkerState.atHome)
        {
            SignalState(_res, WorkerState.inTransitHome);
        }
    }
    public void UpdateResidentsAtHome(int _val)
    {
        SetResidentsAtHome(residentsInHouse + _val);
    }
    public void SetResidentsAtHome(int _val)
    {
        residentsInHouse = _val;
    }

}
