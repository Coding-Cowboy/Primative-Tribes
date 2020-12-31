using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class ResourcesPool
    {
        public string Name;
        public int Value;
    }
    //UI for the Resources that a player has
    public List<ResourcesPool> ResourceList= new List<ResourcesPool>();
    public TextMeshProUGUI ResourcesPane;

    //UI for Objects. Player Actions, Info, and SelectedObjects
    public Sprite ObjectIcon; //Will be for the Current First Object Selected
    public enum ObjectType {Unit,Building,Object};
    public ObjectType SelectedObjectType;

    //Left Bottom Tabs

    //Middle Bottom Tabs
    public Image PanelImage;
    public TextMeshProUGUI PanelText;

    //SelectedObjects List
    private List<GameObject> SelectedObjects;
    //The CameraController
    public CameraController PlayerCamScript;
    void Start()
    {
        SelectedObjects = new List<GameObject>();
        Debug.Log("Make an Image display the Camera Render to allow the full camera to be seen in the Body");
    }

    // Update is called once per frame
    void Update()
    {
        //Setting the Resources Panel
        ResourcesPane.SetText("Resources\n");
        foreach (ResourcesPool pool in ResourceList)
        {
            ResourcesPane.text += pool.Value > 0 ? $"{pool.Name}:{pool.Value}\n" : "";
        }
        if(SelectedObjects.Count > 0 )
        {
            //Creating the UI for the first selected element for the Middle Bottom
            if (SelectedObjects[0].tag == "Unit")
                SelectedObjectType = ObjectType.Unit;
        }
    }
    private void GetSelectedObjects()
    {
        SelectedObjects = PlayerCamScript.GetSelectedObjects();
    }
    public void TabClicked(Button Tab)
    {
        if (Tab.name == "ImageTab")
        {
            PanelImage.sprite = ObjectIcon;
            PanelImage.preserveAspect = true;
            PanelText.text = "";
        }
        else if (Tab.name == "InfoTab")
        {
            PanelImage.sprite = null;
            PanelText.text = "Object Info\n";
        }

    }
}
