using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeScript : MonoBehaviour
{
    private MeshCollider MarqueeMeshCollider;
    // Start is called before the first frame update
    void Start()
    {
        MarqueeMeshCollider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Hit");
    }
}
