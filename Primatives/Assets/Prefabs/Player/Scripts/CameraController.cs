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

    // Update is called once per frame
    void Update()
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
}
