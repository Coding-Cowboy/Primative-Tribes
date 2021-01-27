using System;
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
        public string UnitID;
        public string Team;//Change to private when creation from the mouse is implemented
        public string ID;
        public string Name;
        private bool isBuilder;
        private bool isHealer;
        private List<FloatPool> floats = new List<FloatPool>();
        private List<IntPool> ints = new List<IntPool>();
        private GameObject Object;
        private Sprite Icon;
        public void SetUnitInfoInformation(string Name, bool isBuilder, bool isHealer, GameObject Object, Sprite Icon)
        {
            this.Name = Name;
            this.isBuilder = isBuilder;
            this.isHealer = isHealer;
            this.Object = Object;
            this.Icon = Icon;
        }
        public void AddFloat(string Name, float Value)
        {
            FloatPool NewEntry = new FloatPool();
            NewEntry.Name = Name;
            NewEntry.Value = Value;
            this.floats.Add(NewEntry);
        }
        //value will return -1.0f if the attribute is not within the List
        public float GetFloat(string Name)
        {
            foreach (FloatPool pool in floats)
                if (pool.Name == Name)
                    return pool.Value;
            return -1.0f;
        }
        public void AddInt(string Name, int Value)
        {
            IntPool NewEntry = new IntPool();
            NewEntry.Name = Name;
            NewEntry.Value = Value;
            this.ints.Add(NewEntry);
        }
        public int GetInt(string Name)
        {
            foreach (IntPool pool in ints)
                if (pool.Name == Name)
                    return pool.Value;
            return -1;
        }
        public Sprite GetIcon()
        {
            return this.Icon;
        }
        internal void SetTeam(string Team)
        {
            this.Team = Team;
        }
        public string toString()
        {
            string Output = "";
            Output += "Unit Information\n";
            //Output += $"Unit ID: {UnitID}\n";
            Output += $"Unit Team: {Team}\n";
            Output += $"Unit Name: {Name}\n";
            Output += $"Unit isBuilder: {isBuilder}\n";
            Output += $"Unit isHealer: {isHealer}\n";
            foreach (FloatPool pool in floats)
            {
                Output += $"{pool.Name}: {pool.Value}\n";
            }
            foreach (IntPool pool in ints)
            {
                Output += $"{pool.Name}: {pool.Value}\n";
            }
            //Debug.Log(Output);
            return Output;
        }
    }
    private ObjectScript InfoSystem;//Variable of the parent
    public Info UnitInfo;//Information about Unit from the ObjectInfoSystem
    public GameManager GM;//GameManager for the Game
    public State CurrentState;//Current state of the Unit
    public enum State { WAITING, FLEEING, MOVING, PATROLING, GATHERING, FIGHTING, BUILDING, DYING };//States that a unit can be in in the game.
    private Vector3 CurrentPosition;//Current Location of the object
    private Vector3 GoalPosition;//Location that the unit is moving to
    private LinkedList<Vector3> PatrolPositions;//LinkedList for if the unit is patroling between a set of locations
    private NavMeshAgent Agent;//NavMeshAgent for the Unit to Move
    private GameObject NearestEnemy;//The closest enemy that the GM was able to find at that frame
    public GameObject SelectionRing;//Ring to be enabled when this unit is selected
    private float Timer = 1f;//Timer for checking for enemies
    public float SpeedMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        //Collection Initialization
        //May Refactor to have a repo that the unit can access to get what information that they need.
        PatrolPositions = new LinkedList<Vector3>();
        InfoSystem = GameObject.FindGameObjectWithTag("InfoSystem").GetComponent<ObjectScript>();
        Agent = GetComponent<NavMeshAgent>();

        RetrieveInfo();
        //Setting the speed that a unit walks
        float Speed = UnitInfo.GetFloat("MovementSpeed");
        Agent.speed = Speed > -1.0f ? Speed : 2.5f;//Checks to see if the speed is not 0.0f and sets the agent speed to that value
        SelectionRing.SetActive(false);

        //Set the state machine to WAITING at start as any function that sets movement will set the state machine.
        CurrentState = State.WAITING;
    }
    // Update is called once per frame
    void Update()
    {
        //Get Current Position
        CurrentPosition = transform.position;
        //Switch for the state machine
        switch(CurrentState)
        {
            case State.WAITING:
                WaitingState();
                break;
            case State.FLEEING:
                Debug.Log($"State: {CurrentState}. Run For the Hills");
                break;
            case State.MOVING:
                MovingState();
                break;
            case State.PATROLING:
                PatrolingState();
                break;
            case State.GATHERING:
                Debug.Log($"State: {CurrentState}. Run For the Hills");
                break;
            case State.FIGHTING:
                Debug.Log($"State: {CurrentState}. Run For the Hills");
                break;
            case State.BUILDING:
                Debug.Log($"State: {CurrentState}. Run For the Hills");
                break;
            case State.DYING:
                Debug.Log($"State: {CurrentState}. Run For the Hills");
                break;
        }
        if (NearestEnemy != null)
            Debug.Log("Persuing Enemy");
        PatrolingState();
        //Setting the destination of the navmeshagent to the GoalPosition
        //Also setting the values for movement and looking distance
        Agent.destination = GoalPosition;
        Agent.speed = Agent.speed * SpeedMultiplier;

    }

    

    //Function to call every time a unit is selected and has a patrol to add to the array.
    public void SetPatrol(Vector3 NewPosition)
    {
        this.PatrolPositions.AddLast(NewPosition);//Always add to end so that we can iterate through when doing a patrol
        //Set the state of Unit to State.PATROLING
        CurrentState = State.PATROLING;
    }
    public void ClearPatrol()
    {
        this.PatrolPositions.Clear();
        this.GoalPosition = gameObject.transform.position;

        //Set the state to Waiting if the patrol is removed
        CurrentState = State.WAITING;
    }
    public void SetGoalPoint(Vector3 NewPosition, bool isNotPatrol)
    {
        this.GoalPosition = NewPosition;
        //Sets the state to Moving if not in PATROLING state
        if (isNotPatrol)
            CurrentState = State.MOVING;
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
            foreach (Vector3 position in PatrolPositions)
            {
                if (PositionsEqual(GoalPosition, Previous))
                {
                    SetGoalPoint(position,false);
                    return;
                }
                Previous = position;
            }
            //Debug.Log("First Position");
            //if did not return from foreach loop, then set the GoalPosition to the first element in array.
            GoalPosition = PatrolPositions.First();
        }
    }
    //Function should compare the parameter Vector3 of Position1 to the Vector3 of Position2
    public bool PositionsEqual(Vector3 Position1, Vector3 Position2)
    {
        return ((Position1.x <= Position2.x + 0.5f && Position1.x >= Position2.x - 0.5f) && (Position1.z <= Position2.z + 0.5f && Position1.z >= Position2.z - 0.5f));
    }
    private void RetrieveInfo()
    {
        InfoSystem.GetUnit(UnitInfo.UnitID, this);//Sending Id to get the Information about this Unit.
    }
    public void SetTeam(string Team)
    {
        UnitInfo.SetTeam(Team);
    }
    public void SetNearestEnemy(GameObject NearestEnemy)
    {
        this.NearestEnemy = NearestEnemy;
        //Sets state to the Moving state to move to the Nearest Enemies position every time.
        CurrentState = State.MOVING;
    }
    public void SetRing(bool state)
    {
        SelectionRing.SetActive(state);

    }
    private void WaitingState()
    {
        if (CurrentState == State.PATROLING)//add if the state is waiting or patrolling
        {
            //Decreasing timer if they are in these two states
            Timer -= Time.deltaTime;

            if (Timer < 0.0f)
            {
                //Find nearest enemy
                NearestEnemy = GM.GetNearestEnemy(this.UnitInfo.Team, UnitInfo.GetFloat("LookDistance"), CurrentPosition);
                //Reset timer 
                if (NearestEnemy != null)
                    //Go to the Moving State(and then the fighting state)
                    SetNearestEnemy(NearestEnemy);//Redundant but creates a single point failure for any issues with setting the enemy and state.
                Timer = 1f;
            }
        }
    }
    private void PatrolingState()
    {
        //Check to see if the unit has hit the goal position for the patrol
        if (PositionsEqual(CurrentPosition, GoalPosition))
        {
            FollowPatrol();
            //Checks to see if 1 second has passed to call the EnemyInSights function of the GameManager

        }
        if (CurrentState == State.PATROLING)//add if the state is waiting or patrolling
        {
            //Decreasing timer if they are in these two states
            Timer -= Time.deltaTime;

            if (Timer < 0.0f)
            {
                //Find nearest enemy
                NearestEnemy = GM.GetNearestEnemy(this.UnitInfo.Team, UnitInfo.GetFloat("LookDistance"), CurrentPosition);
                if (NearestEnemy != null)
                    //Go to the Moving State(and then the fighting state)
                    SetNearestEnemy(NearestEnemy);//Redundant but creates a single point failure for any issues with setting the enemy and state.
                //Reset timer 
                Timer = 1f;
            }
        }
    }
    private void MovingState()
    {
        //if the GoalPosition has been reached and the NearestEnemy has been set.
        if (PositionsEqual(CurrentPosition, GoalPosition) && NearestEnemy == null)
            CurrentState = State.WAITING;
    }
}
