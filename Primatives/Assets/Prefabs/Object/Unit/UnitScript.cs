using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour
{
    [System.Serializable]
    public class FloatPool
    {
        public string Name;
        public float Value;
    }
    [System.Serializable]
    public class IntPool
    {
        public string Name;
        public int Value;
    }
    //Information for the Unit to obtain on start
    [System.Serializable]
    public class Info
    {
        public string ID;
        private string Team;
        private string Name;
        private bool isBuilder;
        private bool isHealer;
        private List<FloatPool> floats;
        private List<IntPool> ints;
        private GameObject Object;
        public void SetUnitInfoInformation(string Name, bool isBuilder, bool isHealer, GameObject Object)
        {
            this.Name = Name;
            this.isBuilder = isBuilder;
            this.isHealer = isHealer;
            this.Object = Object;
        }
        public void AddFloat(string Name, float Value)
        {
            FloatPool NewEntry = new FloatPool();
            NewEntry.Name = Name;
            NewEntry.Value = Value;
            this.floats.Add(NewEntry);
        }
        public void AddInt(string Name, int Value)
        {
            IntPool NewEntry = new IntPool();
            NewEntry.Name = Name;
            NewEntry.Value = Value;
            this.ints.Add(NewEntry);
        }
        internal void SetTeam(string Team)
        {
            this.Team = Team;
        }
        public void toString()
        {
            Debug.Log("Unit Information"); 
            Debug.Log($"Unit ID: {ID}");
            Debug.Log($"Unit Team: {Team}");
            Debug.Log($"Unit Name: {Name}");
            Debug.Log($"Unit isBuilder: {isBuilder}");
            Debug.Log($"Unit isHealer: {isHealer}");
            foreach(FloatPool pool in floats)
            {
                Debug.Log($"Variable Name: {pool.Name} and Value: {pool.Value}");
            }
            foreach (IntPool pool in ints)
            {
                Debug.Log($"Variable Name: {pool.Name} and Value: {pool.Value}");
            }
        }
    }
    public ObjectScript InfoSystem;//Variable of the parent
    public Info UnitInfo;//Information about Unit from the ObjectInfoSystem
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
        InfoSystem = GetComponent<ObjectScript>();
        PatrolPositions = new LinkedList<Vector3>();
        Agent = GetComponent<NavMeshAgent>();

        RetrieveInfo();
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
    public bool PositionsEqual(Vector3 Position1,Vector3 Position2)
    {
        return ((Position1.x <= Position2.x + 0.5f && Position1.x >= Position2.x - 0.5f) && (Position1.z <= Position2.z + 0.5f && Position1.z >= Position2.z - 0.5f));
    }
    private void RetrieveInfo()
    {
        InfoSystem.GetUnit(UnitInfo.ID, this);//Sending Id to get the Information about this Unit.
    }
    public void SetTeam(string Team)
    {
        UnitInfo.SetTeam(Team);
    }
    
}
