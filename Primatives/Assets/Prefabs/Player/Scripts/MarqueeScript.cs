using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeScript : MonoBehaviour
{
    private MeshCollider MarqueeMeshCollider;
    private CameraController PlayerCamera;
    // Start is called before the first frame update
    void Start()
    {
        MarqueeMeshCollider = GetComponent<MeshCollider>();
        Debug.Log("Message: Change to Get the Script for that Player and not from the scene");
        PlayerCamera = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MarqueeClear()
    {
        MarqueeMeshCollider = new MeshCollider();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCamera.EnteredTrigger(other);
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: Please Remove from the Selected Units List");
    }

}
