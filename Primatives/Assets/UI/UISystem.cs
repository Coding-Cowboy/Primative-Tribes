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
    

    //Left Bottom Tabs
    //Buttons for the patrols
    public List<Button> UnitButtons;
    private bool isPatrolling;

    //Middle Bottom Tabs
    public Image PanelImage;
    public TextMeshProUGUI PanelText;
    public enum PanelType { Image, Info, Description };
    public PanelType SelectedPanelType;

    //SelectedObjects List
    private List<GameObject> SelectedObjects;
    //The CameraController
    public CameraController PlayerCamScript;
    void Start()
    {
        UnitButtonEnabler(false);
        SelectedObjects = new List<GameObject>();
        Debug.Log("Rework UI design to look more modern like Halo Wars without discarding the UI mechanics of AOE");
        SelectedPanelType = PanelType.Image;
    }

    // Update is called once per frame
    void Update()
    {
        ResourcePanel();

        //Update UI when the Player clicks on an Object
        if(SelectedObjects.Count > 0)
        {
            TabClicked((int)SelectedPanelType);
        }
        //Gets the List of Selected Objects for the UI
        //GetSelectedObjects();
    }

    private void ResourcePanel()
    {
        //Setting the Resources Panel
        ResourcesPane.SetText("Resources\n");
        foreach (ResourcesPool pool in ResourceList)
        {
            ResourcesPane.text += pool.Value > 0 ? $"{pool.Name}:{pool.Value}\n" : "";
        }
    }

    //Will Be called from the Player to set the SelectedObjects for the UI.
    public void CopySelectedObjects(List<GameObject> SelectedObjects)
    {
        foreach (GameObject Object in SelectedObjects)
            this.SelectedObjects.Add(Object);
    }
    //Function for displaying the Object information for switching tabs of the Middle-Bottom Tab for the Image, Info, and Description
    public void TabClicked(int type)
    {
        PanelType Type = (PanelType)type;
        //Creating the UI for the first selected element for the Middle Bottom
        if (Type == PanelType.Image)
        {
            //If it is a Unit
            if (SelectedObjects.Count > 0 && SelectedObjects[0].tag == "Unit")
            {
                PanelImage.sprite = SelectedObjects[0].GetComponent<UnitScript>().UnitInfo.GetIcon();
                UnitButtonEnabler(true);
            }
            //No Object is Selected
            else
            {
                PanelImage.sprite = ObjectIcon;
                UnitButtonEnabler(false);
            }
            PanelImage.preserveAspect = true;
            PanelText.text = "";
            SelectedPanelType = Type;
        }
        else if (Type == PanelType.Info)
        {
            PanelText.text = "Object Info\n";
            //If it is a Unit
            if (SelectedObjects.Count > 0 && SelectedObjects[0].tag == "Unit")
            {
                PanelText.text += SelectedObjects[0].GetComponent<UnitScript>().UnitInfo.toString();
                UnitButtonEnabler(true);
            }
            //No objects are selected
            else
            {
                PanelText.text += "No Object is selected.\n";
                UnitButtonEnabler(false);

            }
            PanelImage.sprite = null;
            SelectedPanelType = Type;
        }
    }
    private void UnitButtonEnabler(bool flag)
    {
        foreach(Button button in UnitButtons)
        {
            button.interactable = flag;
        }
    }
    public void UnitPatrol()
    {
        if (!isPatrolling)
        {
            isPatrolling = true;
            //Enable the flag for the CameraController
            PlayerCamScript.SetisPatrolling(isPatrolling);
        }
        else
        {
            isPatrolling = false;
            //Disable the flag for the CameraController and set the List of Units to patrol
            PlayerCamScript.SetisPatrolling(isPatrolling);
            PlayerCamScript.EndPatrol();
        }
    }
}
