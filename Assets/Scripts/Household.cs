using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Household : MonoBehaviour
{
    public Dictionary<string, int> test = new Dictionary<string, int>();
    [SerializeField] private Worker[] residents;
    [SerializeField] private int residentsInHouse;
    [SerializeField] private GameObject residentPrefab;
    [SerializeField] private House[] houseHoldHouses;


}
