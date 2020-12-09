using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    //variable for the speed at which the camera will span
    public float panSpeed = 20f;
    //Variable for the thickness for the screen movement
    public float panBorderThickness = 10f;
    //Variable for clamping the camera position for map movements
    //panLimit.x = x pnaLimit.y = z
    public Vector2 panLimit;
    //Variable for the speed at which the camera will scroll
    public float scrollSpeed = 200f;

    public float minY = 20f;

    public float maxY = 120f;
    private Camera PlayerCamera;
    private Vector3 StartDragPosition;
    private GameObject HitObject;
    void Start()
    {
        PlayerCamera = GetComponentInChildren<Camera>();
        if (PlayerCamera != null)
            Debug.Log("Good");
    }
    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        MouseClick();
        MouseUnclick();
        UnitMove();
    }


    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Clear the previous HitObject
            HitObject = null;

            //Perform the Raycast to see what GameObject they Hit or where to start dragging
            Debug.Log("Left Click");
            RaycastHit hit = RayCasting();
            Debug.Log("Hit " + hit.collider.gameObject.name);

            StartDragPosition = hit.point; 
        }
    }
    private void MouseUnclick()
    {    
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit = RayCasting();
            //If the player has dragged the mouse then get all GmaeObjects in the drawn Box, also returns
            if (StartDragPosition != hit.point)
            {
                MouseDragging();
                return;
            }
            if (hit.collider.gameObject.tag == "Unit")
            {
                //If the player only selected 
                HitObject = hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.tag == "Building")
            {
                HitObject = hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.tag == "Other")
            {
                HitObject = hit.collider.gameObject;
            }
        }
    }


    private void MouseDragging()
    {

    }
    private void UnitMove()
    {
        //If the HitObject is not null then perform a simple move
        if (HitObject != null && HitObject.tag == "Unit" && Input.GetMouseButtonDown(1))
        {
            RaycastHit hit = RayCasting();
            HitObject.GetComponent<UnitScript>().SetGoalPoint(hit.point, true);
        }
    }
    private void CameraMovement()
    {
        //Storing the position in a temp variable
        Vector3 pos = transform.position;

        //If the user hits the W key or is near the edge of the screen
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        //If the user hits the S key or is near the edge of the screen
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        //If the user hits the D key or is near the edge of the screen
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        //If the user hits the A key or is near the edge of the screen
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        //Clamping the pos so that the player can only go within the map limit.
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        //Applying the position after taking user inputs
        transform.position = pos;
    }
    private RaycastHit RayCasting()
    {
        //Perform Raycast to see if the mouse up is at a different point from the mousedown
        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out hit);
        return hit;
    }
}
