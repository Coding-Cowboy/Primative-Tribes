using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Attributes that will be used for running the EnemyInSight function
    //public struct RadarJob : IJob
    //{
    //    public string ID;
    //    public float LeastDistance;
    //    public List<string> UnitIDs;
    //    public NativeArray<float> UnitDistances;
    //    public void Execute()
    //    {
    //        //get the distance at i and see if 1. that value is less than the current leastDistance
    //    }
    //    public RadarJob(string ID, int objects) : this()
    //    {
    //        this.ID = ID;
    //        this.LeastDistance = 99f;
    //        this.UnitIDs = new List<string>();
    //        this.UnitDistances = new NativeArray<float>(objects, Allocator.Temp);
    //    }
    //    public void addDistance(float Distance)
    //    {

    //    }
    //    private void Compare 
    //}

    // Start is called before the first frame update
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Gets the GameObject of the nearest enemy in their looking distance, else returns null
    public GameObject GetNearestEnemy(string Team, float distance, Vector3 UnitPosition)
    {
        float closestDistanceSqr = distance * distance;
        GameObject Enemy = null;
        GameObject[] Units = GameObject.FindGameObjectsWithTag("Unit");
        foreach (GameObject unit in Units)
        {
            Vector3 directionToTarget = unit.transform.position - UnitPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr && unit.GetComponent<UnitScript>().UnitInfo.Team != Team)
            {
                closestDistanceSqr = dSqrToTarget;
                Enemy = unit;
            }
        }

        return Enemy;
    }
}
