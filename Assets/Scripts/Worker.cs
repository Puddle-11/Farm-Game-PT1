using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Worker : MonoBehaviour
{
    public Household familyHouse;
    public House house;
    [SerializeField] private NavMeshAgent agent;
    public Household.WorkerState currState;
    public LayerMask ground;
    public string name;
    public Mood currentMood;
    public Activity currentActivity;
    public void Update()
    {
        if(currState == Household.WorkerState.inTransitWork)
        {
            SetTarget(Cam.instance.MouseRaycast(ground).point);
        }
    }

    public enum Mood
    {
        Scared,
        Sad,
        Happy,
        Content,
        Playful,
        Emberassed,
        Angry,
    }
    public enum Activity
    {
        Working,
        Playing,
        Exploring,
        StayIn,
        VisitFriend,
    }
    public enum Worktype
    {
        Farmer,
        Parent,
        Lumberjack,
    }
    public void Recall()
    {
        SetTarget(familyHouse.transform.position);
        familyHouse.SignalState(this, Household.WorkerState.inTransitHome);
    }
    public void StartDay()
    {
        familyHouse.SignalState(this, Household.WorkerState.inTransitWork);
    }
    public void SetTarget(Vector3 _pos)
    {
        agent.destination = _pos;
    }
    public void SetHome(Household _household)
    {
        familyHouse = _household;
    }
    public void SetName(string _name)
    {
        name = _name;
    }
    public Activity GetValidActivity()
    {
        int length = Enum.GetNames(typeof(Activity)).Length;
        List<int> ValidActivities = new List<int>();

        //inits valid activities list
        for (int i = 0; i < length; i++)
        {
            ValidActivities.Add(i);
        }
        while (true)
        {

        }
    }
    private bool IsActivityValid(Activity _activity)
    {
        switch (_activity)
        {
            case Activity.Working:
                break;
            case Activity.Playing:
                break;
            case Activity.Exploring:
            case Activity.StayIn:
                return true;
            case Activity.VisitFriend:
                break;
            default:
                break;
        }
        return false;


    }
}
