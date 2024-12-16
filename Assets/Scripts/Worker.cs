using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public Household familyHouse;
    public House house;
    [SerializeField] private NavMeshAgent agent;



    public enum Worktype
    {
        Farmer,
        Parent,
        Lumberjack,
    }
    public void Recall()
    {
        agent.SetDestination(house.transform.position);
    }
    public void StartDay()
    {

    }
}
