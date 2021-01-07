using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
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
    [System.Serializable]
    public class StringPool
    {
        public string Name;
        public string Value;
    }
    [System.Serializable]
    public class GameObjectPool
    {
        public string Name;
        public GameObject Value;
    }


    [System.Serializable]
    public class UnitPool
    {
        public string UnitID;
        public string Name;
        public bool isBuilder;
        public bool isHealer;
        public List<FloatPool> floats;
        public List<IntPool> ints;
        public GameObject Object;
        public Sprite Icon;
    }
    //public List<GameObjectPool> gameobjects;
    public List<UnitPool> Units;


    //public Dictionary<string, Queue<GameObject>> GameObjectValues;
    void Start()
    {
        //Initialization of all the Dictionaries that an Object could need for variables.
        //GameObjectValues = new Dictionary<string, Queue<GameObject>>();

        //Foreach loops for inserting the variables into the Dictionaries
        //foreach (FloatPool pool in floats)
        //{
        //    Queue<float> fl = new Queue<float>();

        //    //for( int i = 0; i < floats.Count; i++)
        //    //{

        //    //}
        //}
    }

    //function will search for the variable will print if the Unit is not found
    public void GetUnit(string ID, UnitScript Unit)
    {
        foreach (UnitPool pool in Units)
        {
            if (pool.UnitID == ID)
            {
                //Perform Transfering of Data
                Unit.UnitInfo.SetUnitInfoInformation(pool.Name, pool.isBuilder,pool.isHealer,pool.Object, pool.Icon);
                foreach (FloatPool fpool in pool.floats)
                {
                    Unit.UnitInfo.AddFloat(fpool.Name, fpool.Value);
                }
                foreach (IntPool ipool in pool.ints)
                {
                    Unit.UnitInfo.AddInt(ipool.Name, ipool.Value);
                }
                Unit.UnitInfo.toString();
                return;
            }
        }
    }
}
