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

    public List<FloatPool> floats;
    public List<IntPool> ints;
    public List<StringPool> strings;
    public List<GameObjectPool> gameobjects;


    
    public Dictionary<string, Queue<GameObject>> GameObjectValues;
    void Start()
    {
        //Initialization of all the Dictionaries that an Object could need for variables.
        GameObjectValues = new Dictionary<string, Queue<GameObject>>();

        //Foreach loops for inserting the variables into the Dictionaries
        //foreach (FloatPool pool in floats)
        //{
        //    Queue<float> fl = new Queue<float>();

        //    //for( int i = 0; i < floats.Count; i++)
        //    //{

        //    //}
        //}


        //testing for value checking.
        //foreach(FloatPool pool in floats)
        //{
        //    if (pool.Name == "WalkSpeed")
        //        Debug.Log($"Variable Name: {pool.Name} Value: {pool.Value}");
        //}

    }
    /**
     * Function will search for the variable that is specified and return it, else it will return 0f
     */
    public float GetFloatVariable(string Name)
	{
		foreach(FloatPool pool in floats)
		{
			if(pool.Name == Name)
			{
				return pool.Value;
			}
		}
		return 0f; //Will return 0f if the variable is not in the list
	}
    //Function will search for the variable that is specified and return it, else it will return 0
    public int GetIntVariable(string Name)
    {
        foreach (IntPool pool in ints)
        {
            if (pool.Name == Name)
            {
                return pool.Value;
            }
        }
        return 0; //Will return 0f if the variable is not in the list
    }
    //Function will search for the variable that is specified and return it, else it will return null
    public string GetStringVariable(string Name)
    {
        foreach (StringPool pool in strings)
        {
            if (pool.Name == Name)
            {
                return pool.Value;
            }
        }
        return null; //Will return null if the variable is not in the list
    }
    //Function will search for the variable that is specified and return it, else it will return null
    public GameObject GetGameObjectVariable(string Name)
    {
        foreach (GameObjectPool pool in gameobjects)
        {
            if (pool.Name == Name)
            {
                return pool.Value;
            }
        }
        return null; //Will return null if the variable is not in the list
    }
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
