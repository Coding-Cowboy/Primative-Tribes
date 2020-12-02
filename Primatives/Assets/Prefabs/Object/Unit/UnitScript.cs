using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public ObjectScript parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponent<ObjectScript>();
        float temp = parent.GetFloatVariable("WalkSpeed");
        Debug.Log(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
