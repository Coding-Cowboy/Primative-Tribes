using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    //Variable for the Minimum y that the camera can scroll into
    public float minY = 20f;
    //Variable for the Maximum y that the camera can scroll out to
    public float maxY = 120f;
    //Camera Component on the Player
    private Camera PlayerCamera;
    //Starting Position that the mouse is dragging from.
    private Vector3 StartDragPosition = Vector3.zero;//P1 from example
    private Vector3 StartWorldPosition;
    private Vector3 EndDragPosition { get; set; } = Vector3.zero;
    private Vector3 EndWorldPosition;
    //Flag for if the mouse is dragging for objects to select
    private bool isDragging = false;
    //private GameObject HitObject;
    private Rect DrawBox;
    private List<GameObject> SelectedObjects = new List<GameObject>();

    private GameObject Marquee;
    private MeshCollider selectionBox;
    private Mesh selectionMesh;

    //the corners of our 2d selection box
    Vector3[] corners;

    //the vertices of our meshcollider
    Vector3[] verts;
    Vector3[] vecs;

    //UISystem for communicating to the UI for what to display
    public UISystem PlayerUI;
    ////Graphics Raycaster from the Player UI
    //public GraphicRaycaster PlayerGR;
    ////Event System from the Scene
    //public EventSystem SceneEventSystem;
    private bool isPatrolling;//The flag for right clicks to register as a patrol or a single move order
    private LinkedList<Vector3> PatrolPositions;//LinkedList for if the unit(s) is patroling between a set of locations
    private bool UIFlag = false;//Flag for when a UI element is pressed
    void Start()
    {
        Debug.Log("Message: Create Squad GameObject with all the SelectedObjects in SquadCreation Function");
        Debug.Log("Message: Create Functionality for a Double Click that will select all units within the camera screen");
        Debug.Log("Message: Create Functionality for pressing a key to select all of a players units in the level");
        PlayerCamera = GetComponentInChildren<Camera>();
        selectionMesh = new Mesh();
        MarqueeCreation();
        //SceneEventSystem = FindObjectOfType<EventSystem>();
    }

    private void MarqueeCreation()
    {
        Marquee = new GameObject("Marquee");
        Marquee.AddComponent<MeshCollider>();
        Marquee.AddComponent<MarqueeScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        MouseClick();
        MouseDragging();
        MouseUnclick();
        SquadCreation();
        UnitMove();
    }


    private void OnGUI()
    {
        if (isDragging)
        {
            //Creating Vector3 off the current Drag positions
            Vector3 Rect1 = StartDragPosition;
            Vector3 Rect2 = Input.mousePosition;/*EndDragPosition;*/
            Rect1.y = Screen.height - Rect1.y;
            Rect2.y = Screen.height - Rect2.y;

            //Calculation of the corners
            Vector3 TopLeft = Vector3.Min(Rect1, Rect2);
            Vector3 BottomRight = Vector3.Max(Rect1, Rect2);

            //Creating the Rectangle
            DrawBox = Rect.MinMaxRect(TopLeft.x, TopLeft.y, BottomRight.x, BottomRight.y);

            //New Texture to set
            Texture2D _whiteTexture;
            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();
            DrawScreenRect(new Color(0.8f, 0.8f, 0.95f, 0.25f), _whiteTexture);

            //Draw for the RectBorder
            // Top
            DrawScreenRect(new Rect(DrawBox.xMin, DrawBox.yMin, DrawBox.width, 2.0f), new Color(0.8f, 0.8f, 0.95f), _whiteTexture);
            // Left
            DrawScreenRect(new Rect(DrawBox.xMin, DrawBox.yMin, 2.0f, DrawBox.height), new Color(0.8f, 0.8f, 0.95f), _whiteTexture);
            // Right
            DrawScreenRect(new Rect(DrawBox.xMax - 2.0f, DrawBox.yMin, 2.0f, DrawBox.height), new Color(0.8f, 0.8f, 0.95f), _whiteTexture);
            // Bottom
            DrawScreenRect(new Rect(DrawBox.xMin, DrawBox.yMax - 2.0f, DrawBox.width, 2.0f), new Color(0.8f, 0.8f, 0.95f), _whiteTexture);
        }
    }
    private void DrawScreenRect(Color color, Texture2D _whiteTexture)
    {
        //Setting the GUI for the Box inside
        GUI.color = color;
        GUI.DrawTexture(DrawBox, _whiteTexture);
        GUI.color = Color.white;
    }
    private void DrawScreenRect(Rect DrawBox, Color color, Texture2D _whiteTexture)
    {
        //Setting the GUI for the Box inside
        GUI.color = color;
        GUI.DrawTexture(DrawBox, _whiteTexture);
        GUI.color = Color.white;
    }
    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDragPosition = Input.mousePosition;
            StartWorldPosition = RayCasting().point;
            //Debug.Log($"Start of Dragging:{StartWorldPosition}");
        }
    }
    private void MouseDragging()
    {
        if (Input.GetMouseButton(0))
        {
            //Checks to see if the magnitude is within a range for dragging
            if ((StartDragPosition - Input.mousePosition).magnitude > 20)
            {
                isDragging = true;
            }
        }
    }
    private void MouseUnclick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                RaycastHit hit = RayCasting();
                //List<RaycastResult> results = new List<RaycastResult>();
                //GraphicRaycasting(results);
                //foreach(RaycastResult rr in results)
                //{
                //    Debug.Log(rr.gameObject.name);
                //}
                //Logic for creating a squad for Units
                if (hit.transform.gameObject.tag == "Unit")
                {
                    if (Input.GetKey(KeyCode.LeftShift)) //Inclusive Selection
                    {
                        //Add to the Collection if this is the first selected object in the collection
                        if (SelectedObjects.Count == 0)
                            SelectedObjects.Add(hit.transform.gameObject);
                        //Else if the first element of the list does not contain the tag "Unit"
                        else
                        {
                            if (SelectedObjects[0].tag == "Unit")
                                //Logic for if this is not the first unit
                                SelectedObjects.Add(hit.transform.gameObject);
                            else
                            {
                                foreach (GameObject gm in SelectedObjects)
                                    gm.GetComponent<UnitScript>().SetRing(false);
                                SelectedObjects.Clear();
                                SelectedObjects.Add(hit.transform.gameObject);
                            }
                        }
                        hit.transform.gameObject.GetComponent<UnitScript>().SetRing(true);
                    }
                    else //Exclusive Selection
                    {
                        foreach (GameObject gm in SelectedObjects)
                            gm.GetComponent<UnitScript>().SetRing(false);
                        SelectedObjects.Clear();
                        SelectedObjects.Add(hit.transform.gameObject);
                        hit.transform.gameObject.GetComponent<UnitScript>().SetRing(true);
                    }
                }

                //Logic for selecting multiple buldings of the same type(Work on later)

                //Logic for selection all "Other" Game objects in the drawbox(Due Later)
                //Logic for if the Player clicked their UI
                else if (UIFlag)
                {
                    Debug.Log("UI Clicked");//Do Nothing
                    SetUIFlag(false);
                }
                //The Player clicked on the ground
                else if (hit.transform.gameObject.tag == "Ground" && !UIFlag)/* (hit.transform.gameObject.tag == "Ground")*/
                {
                    foreach (GameObject gm in SelectedObjects)
                        gm.GetComponent<UnitScript>().SetRing(false);
                    SelectedObjects.Clear();
                }
            }
            //Else if the player is still dragging
            else
            {
                verts = new Vector3[5];
                int i = 0;
                EndDragPosition = Input.mousePosition;
                EndWorldPosition = RayCasting().point;

                //Getting the corners for the Marquee Drag
                corners = GetBoundingBox(StartWorldPosition, EndWorldPosition);

                //will give the base of the triangular prism the vectors of the corners
                foreach (Vector3 corner in corners)
                {
                    Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                    //Setting the tip of the triangle to the ray.origin
                    verts[4] = ray.origin;
                    //setting the verts to the corners
                    verts[i] = corner;
                    i++;
                }


                //generate the mesh
                selectionMesh = GenerateSelectionMesh(verts, vecs);
                //foreach (Vector3 corner in corners)
                //{
                //    Debug.Log($"Corner:{corner}");
                //}
                //i = 0;
                //foreach (Vector3 vertex in verts)
                //{
                //    Debug.Log($"vertex {i}:{vertex}");
                //    i++;
                //}
                //foreach (Vector3 vector in vecs)
                //{
                //    Debug.Log($"vector:{vector}");
                //}

                ///Section is for creating a new selection box and marquee, then setting those to be colliders that the objects will be able to be selected with. finally they will get destroyed to remove the collider from the scene
                selectionBox = new MeshCollider();
                MarqueeCreation();
                selectionBox = Marquee.GetComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    foreach(GameObject gm in SelectedObjects)
                        gm.GetComponent<UnitScript>().SetRing(false);

                    SelectedObjects.Clear();
                }
                //Clear the selection box
                Destroy(selectionBox, 0.02f);
                Destroy(Marquee, 0.02f);
            } //End of Marquee Draw
            isDragging = false;
        }
    }
    //Function is used to create a squad from all the selected objects and copies over the List to the UISystem
    private void SquadCreation()
    {
        if (SelectedObjects.Count > 0)
            CopySelectedObjects();
    }
    private void UnitMove()
    {
        //If the SelectedObjects is not null and the first element is a Unit
        if (SelectedObjects.Count > 0 && SelectedObjects[0].tag == "Unit" && Input.GetMouseButtonDown(1))
        {
            RaycastHit hit = RayCasting();
            if(isPatrolling)
            {
                PatrolPositions.AddLast(hit.point);
            }
            foreach(GameObject unit in SelectedObjects)
                unit.GetComponent<UnitScript>().SetGoalPoint(hit.point, true);
        }
    }
    //Function that deals with teh camera position and the camera height from keyboard inputs.
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
    //Function to get the RaycastHit for the camera
    private RaycastHit RayCasting()
    {
        //Perform Raycast to see if the mouse up is at a different point from the mousedown
        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        return hit;
    }
    ////Function for the Graphics Raycast of the camera
    //private void GraphicRaycasting(List<RaycastResult> results)
    //{
    //    //Create the PointerEventData with null for the EventSystem
    //    PointerEventData ped = new PointerEventData(null);
    //    //Set required parameters, in this case, mouse position
    //    ped.position = Input.mousePosition;
    //    //Create list to receive all results
    //    results = new List<RaycastResult>();
    //    //Raycast it
    //    PlayerGR.Raycast(ped, results);
    //}
    //create a bounding box (4 corners in order) from the start and end mouse position
    private Vector3[] GetBoundingBox(Vector3 p1, Vector3 p2)
    {
        Vector3 newP1;
        Vector3 newP2;
        Vector3 newP3;
        Vector3 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.z > p2.z) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector3(p2.x, p1.y, p1.z);
                newP3 = new Vector3(p1.x, p2.y, p2.z);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector3(p1.x, p2.y, p2.z);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector3(p2.x, p1.y, p2.z);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.z > p2.z) // if p1 is above p2
            {
                newP1 = new Vector3(p2.x, p1.y, p1.z);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector3(p1.x, p2.y, p2.z);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector3(p1.x, p2.y, p2.z);
                newP3 = new Vector3(p2.x, p1.y, p2.z);
                newP4 = p1;
            }

        }

        Vector3[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }
    //generate a mesh from the 4 bottom points
    Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        //Vector3[] verts = new Vector3[5];
        int[] tris = { 0, 1, 2, //Bottom
                       1, 3, 2,
                       0, 1, 4,
                       1, 3, 4,
                       3, 2, 4,
                       2, 0, 4};

        //Giving Dumby Data for testing
        //verts = new Vector3[5];
        //verts[0] = new Vector3(0, 0, 0);
        //verts[1] = new Vector3(1, 0, 0);
        //verts[2] = new Vector3(0, 0, -1);
        //verts[3] = new Vector3(1, 0, -1);
        //verts[4] = new Vector3(0.5f, 1, -0.5f);


        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }


        selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;
        selectionMesh.RecalculateNormals();

        //int[] temp = selectionMesh.triangles;
        //foreach (int i in temp)
        //{
        //    Debug.Log(i);
        //}
        return selectionMesh;
    }

    public void EnteredTrigger(Collider other)
    {
        SelectedObjects.Add(other.gameObject);
        if (other.gameObject.tag == "Unit")
            other.GetComponent<UnitScript>().SetRing(true);
    }
    public void CopySelectedObjects()
    {
        PlayerUI.CopySelectedObjects(SelectedObjects);
    }
    public void SetUIFlag(bool flag)
    {
        UIFlag = flag;
    }
}
