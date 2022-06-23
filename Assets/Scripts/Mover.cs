using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update() 
    {
        GetComponent<NavMeshAgent>().destination = target.position;
    }
}
