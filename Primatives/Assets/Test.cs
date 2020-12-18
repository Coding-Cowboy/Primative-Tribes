using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 TestGoalPosition;
    public List<Vector3> PatrolPositions;
    public UnitScript Unit;
    //private bool flag = true;
    // Start is called before the first frame update
    void Start()
    {
        //Unit.SetGoalPoint(TestGoalPosition,true);
        Unit.SetTeam("1");
    }

    // Update is called once per frame
    void Update()
    {
        //if (Unit.PositionsEqual(Unit.GetCurrentPosition(),TestGoalPosition))
        //    flag = true;
        //else
        //    flag = false;


        //if(flag)
        //{
        //    foreach(Vector3 position in PatrolPositions)
        //    {
        //        Unit.SetPatrol(position);
        //    }
        //    flag = false;
        //}
    }
    //public void TaskOnClick()
    //{
    //    Debug.Log("Production Item Clicked");
    //}

}
