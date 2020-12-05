using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //public Vector3 TestGoalPosition;
    public List<Vector3> PatrolPositions;
    public UnitScript Unit;
    private bool flag = false;
    // Start is called before the first frame update
    void Start()
    {
        //Unit.SetGoalPoint(TestGoalPosition);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Unit.GetCurrentPosition().Equals(TestGoalPosition))
            flag = true;
        if(flag)
        {
            foreach(Vector3 position in PatrolPositions)
            {
                Unit.SetPatrol(position);
            }
            flag = false;
            //Set the Goal Point to the first element in the array to start the patrol
            Unit.SetGoalPoint(PatrolPositions[0]);
        }
    }
    //public void TaskOnClick()
    //{
    //    Debug.Log("Production Item Clicked");
    //}

}
