using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    public ObjectScript parent;//Variable of the parent
    private Vector3 CurrentPosition;//Current Location of the object
    private Vector3 GoalPosition;//Location that the unit is moving to
    private LinkedList<Vector3> PatrolPositions;//LinkedList for if the unit is patroling between a set of locations
    private NavMeshAgent Agent;//NavMeshAgent for the Unit to Move

    private enum State { WAITING, FLEEING, MOVING, FIGHTING, BUILDING, DYING};//States that a unit can be in in the game.
    // Start is called before the first frame update
    void Start()
    {
        //Collection Initialization
        //May Refactor to have a repo that the unit can access to get what information that they need.
        parent = GetComponent<ObjectScript>();
        PatrolPositions = new LinkedList<Vector3>();
        Agent = GetComponent<NavMeshAgent>();
        //float temp = parent.GetFloatVariable("WalkSpeed");
        //Debug.Log(temp);
    }

    // Update is called once per frame
    void Update()
    {
        //Get Current Position
        CurrentPosition = transform.position;
        //Check to see if the unit has hit the goal position for the patrol
        if (PositionsEqual(CurrentPosition,GoalPosition))
        {
            FollowPatrol();
            Debug.Log("True");
        }
        //Setting the destination of the navmeshagent to the GoalPosition
        Agent.destination = GoalPosition;

    }
    //Function to call every time a unit is selected and has a patrol to add to the array.
    public void SetPatrol(Vector3 NewPosition)
    {
        this.PatrolPositions.AddLast(NewPosition);//Always add to end so that we can iterate through when doing a patrol
    }
    public void ClearPatrol()
    {
        this.PatrolPositions.Clear();
    }
    public void SetGoalPoint(Vector3 NewPosition)
    {
        this.GoalPosition = NewPosition;
        Debug.Log($"New Position: {GoalPosition}");
    }
    public Vector3 GetCurrentPosition()
    {
        return this.CurrentPosition;
    }
    private void FollowPatrol()
    {
        //Check to see if the List is empty
        if (PatrolPositions.Count == 0)
            return;
        else
        {
            Vector3 Previous = new Vector3(9999f, 9999f, 9999f);//Setting position to a point in the shadow realm
            //Iterate through each point and get the next point after the current point.
            //*Note* should not clear if attacking enemy
            foreach( Vector3 position in PatrolPositions)
            {
                if (PositionsEqual(GoalPosition,Previous))
                {
                    SetGoalPoint(position);
                    Debug.Log("Next Position");
                    return;
                }
                Previous = position;
            }
            Debug.Log("First Position");
            //if did not return from foreach loop, then set the GoalPosition to the first element in array.
            GoalPosition = PatrolPositions.First();
        }
    }
    //Function should compare the parameter Vector3 of Position1 to the Vector3 of Position2
    private bool PositionsEqual(Vector3 Position1,Vector3 Position2)
    {
        return ((Position1.x <= Position2.x + 0.5f && Position1.x >= Position2.x - 0.5f) && (Position1.z <= Position2.z + 0.5f && Position1.z >= Position2.z - 0.5f));
    }
}
