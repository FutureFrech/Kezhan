using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Academy.HoloToolkit.Unity;
using Academy.HoloToolkit.Sharing;

public class MenuTrigger : MonoBehaviour {

    private GameObject FITModel;
    private TextMesh MenuText;
    private GameObject theMenu;
    private GameObject floormenu;
    static string roomnumber;

    void Awake()
    {
        theMenu = GameObject.Find("Menu");
        floormenu = GameObject.Find("FloorMenu");
    }

    // Use this for initialization
    void Start () {
        FITModel = SingleFloorManager.Instance.gameObject;
        FITModel.SetActive(false);

        //roomnumber = "";
    }

    // Update is called once per frame
    void Update () {

    }

    public void OnSelect()//在gesture里定义了sendmessage
    {
        MenuText = gameObject.GetComponentInChildren<TextMesh>();
        if (MenuText.text == "Show the Model")
            ShowModel();
        else if (MenuText.text == "Hide the Model")
            FITModel.SetActive(false);
        else if (MenuText.text == "Unfold Vertically")
            SingleFloorManager.Instance.Split();
        else if (MenuText.text == "Unfold Horizontally")
            SingleFloorManager.Instance.Split_x();
        else if (MenuText.text == "Fold the Model")
            SingleFloorManager.Instance.Merge();
        else if (MenuText.text == "Select the Destination")
        {
            theMenu.SetActive(false);
            //floormenu.SetActive(true);
            ShowKeyBoard();
        }
        else if (MenuText.text == "OK")
        {
            Route.Instance.SetDestination(int.Parse(roomnumber));
            floormenu.SetActive(false);
            theMenu.SetActive(false);
            ShowMenu();
            roomnumber = "";
            DebugInfoManager.Instance.AddAnchorStatus(" OK\n");
        }
        else if (MenuText.text == "P1")
        {
            PointsManager.Instance.SetPoint1(int.Parse(roomnumber));
            roomnumber = "";
            DebugInfoManager.Instance.AddAnchorStatus(" P1 OK\n");
        }
        else if (MenuText.text == "P2")
        {
            PointsManager.Instance.SetPoint2(int.Parse(roomnumber));
            roomnumber = "";
            DebugInfoManager.Instance.AddAnchorStatus(" P2 OK\n");
        }
        else if (MenuText.text == "Add")
        {
            PointsManager.Instance.AddLineClicked();
            roomnumber = "";
            DebugInfoManager.Instance.SetAnchorStatus("Add Line\n");
        }
        else if (MenuText.text == "Del")
        {
            PointsManager.Instance.DelLineClicked();
            roomnumber = "";
            DebugInfoManager.Instance.AddAnchorStatus("Del Line\n");
        }
        else
        {
            roomnumber += MenuText.text;
            DebugInfoManager.Instance.AddAnchorStatus(MenuText.text);
        }
    }

    private void ShowModel()
    {
        if (FITModel.activeSelf == false)
        {
            FITModel.SetActive(true);
            FITModel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2 + Camera.main.transform.right * 1;//前向2，右向1
            Vector3 tempEulerAngles = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.forward).eulerAngles;
            tempEulerAngles.x = 0;
            tempEulerAngles.z = 0;
            FITModel.transform.eulerAngles = tempEulerAngles;
        }
        else
        {
            SingleFloorManager.Instance.ShowAllFloor();
        }
    }

    private void ShowMenu()
    {
        if (theMenu.activeSelf == false)
        {
            theMenu.SetActive(true);
            theMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            Vector3 tempEulerAngles = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.forward).eulerAngles;
            tempEulerAngles.x = 0;
            tempEulerAngles.z = 0;
            theMenu.transform.eulerAngles = tempEulerAngles;
        }
    }

    private void ShowKeyBoard()
    {
        if (floormenu.activeSelf == false)
        {
            floormenu.SetActive(true);
            floormenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f; // + Camera.main.transform.right * 1;//前向2，右向1
            Vector3 tempEulerAngles = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.forward).eulerAngles;
            tempEulerAngles.x = 0;
            tempEulerAngles.z = 0;
            floormenu.transform.eulerAngles = tempEulerAngles;
        }
        else
        {
            ;
        }
    }
}
