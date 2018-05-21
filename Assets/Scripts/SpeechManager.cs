using Academy.HoloToolkit.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using Academy.HoloToolkit.Sharing;

public class SpeechManager : Singleton<SpeechManager>
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();
    private GameObject FITModel;
    private GameObject theMenu;
    private GameObject floormenu;
    GameObject guidedArrowObj;

    // Use this for initialization
    void Start()
    {
        FITModel = SingleFloorManager.Instance.gameObject;
        FITModel.SetActive(false);//note when performing in Unity
        guidedArrowObj = GuidedArrow.Instance.gameObject;
        guidedArrowObj.SetActive(false);

        theMenu = GameObject.Find("Menu");
        theMenu.SetActive(false);
        floormenu = GameObject.Find("FloorMenu");
        floormenu.SetActive(false);

        #region MenuCommand
        keywords.Add("Show Menu", () =>
        {
            if (theMenu.activeSelf == false)
            {
                theMenu.SetActive(true);
                theMenu.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
                Vector3 tempEulerAngles = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.forward).eulerAngles;
                tempEulerAngles.x = 0;
                tempEulerAngles.z = 0;
                theMenu.transform.eulerAngles = tempEulerAngles;
            }
            else
            {
                ;
            }
        });
        keywords.Add("Hide Menu", () =>
        {
            theMenu.SetActive(false);
        });
        #endregion

        #region ModelCommand
        keywords.Add("Show Model", () =>
        {
            if (FITModel.activeSelf == false)
            {
                FITModel.SetActive(true);
                FITModel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
                Vector3 tempEulerAngles = Quaternion.FromToRotation(Vector3.forward, Camera.main.transform.forward).eulerAngles;
                tempEulerAngles.x = 0;
                tempEulerAngles.z = 0;
                FITModel.transform.eulerAngles = tempEulerAngles;
            }
            else
            {
                SingleFloorManager.Instance.ShowAllFloor();
            }
        });
        keywords.Add("Hide Model", () =>
        {
            FITModel.SetActive(false);
        });
        keywords.Add("Unfold", () =>
        {
            SingleFloorManager.Instance.Split();
        });
        keywords.Add("Unfold X", () =>
        {
            SingleFloorManager.Instance.Split_x();
        });
        keywords.Add("Fold", () =>
        {
            SingleFloorManager.Instance.Merge();
        });
        #endregion

        #region GestureCommand
        keywords.Add("Tap", () =>
        {
            GestureManager.Instance.Transition(GestureManager.Instance.ClickRecognizer);
        });

        keywords.Add("Move", () =>
        {
            GestureManager.Instance.Transition(GestureManager.Instance.MoveRecognizer);
        });

        keywords.Add("Rotate", () =>
        {
            GestureManager.Instance.Transition(GestureManager.Instance.RotateRecognizer);
        });

        keywords.Add("Scale", () =>
        {
            GestureManager.Instance.Transition(GestureManager.Instance.ZoomRecognizer);
        });

        keywords.Add("Save", () =>
        {
            GestureManager.Instance.Transition(GestureManager.Instance.SaveRecognizer);
        });
        #endregion

        #region NavigationCommand
        keywords.Add("Load", () =>
        {
            Route.Instance.SetSource();
            Route.Instance.GetIndexFromGraph();
            Route.Instance.LoadDataFromAnchor();
            /*
            */
            DebugInfoManager.Instance.UpdateLoadStatus(true);
        });

        keywords.Add("Start Navigation", () =>
        {
            DebugInfoManager.Instance.UpdateNavigationStatus(false, true);
            Route.Instance.StartNavigation();
            guidedArrowObj.SetActive(true);
            GuidedArrow.Instance.GASwitch(true);
        });

        keywords.Add("Stop Navigation", () =>
        {
            GuidedArrow.Instance.GASwitch(false);
        });

        keywords.Add("Clear Points", () =>
        {
            PointsManager.Instance.Dispose();
            DebugInfoManager.Instance.UpdateLoadStatus(false);
        });

        keywords.Add("Hide Points", () =>
        {
            PointsManager.Instance.ShowAllPoints(false);
        });

        keywords.Add("Show Points", () =>
        {
            PointsManager.Instance.ShowAllPoints(true);
        });

        keywords.Add("Show Arrow", () =>
        {
            GuidedArrow.Instance.ShowArrow(true);
        });

        keywords.Add("Hide Arrow", () =>
        {
            GuidedArrow.Instance.ShowArrow(false);
        });
        #endregion

        #region environmentCommand
        // Turn to Table model
        keywords.Add("Show Mesh", () =>
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = true;
        });

        keywords.Add("Hide Mesh", () =>
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = false;
        });
        #endregion

        #region Anchor Command
        // keywords.Add("Delete Point", PointsManager.Instance.DelPoint);
        keywords.Add("End Adding Points", PointsManager.Instance.EndAddingPoint);
        keywords.Add("Zoom In", Model2D.Instance.ZoomIn);
        keywords.Add("Zoom Out", Model2D.Instance.ZoomOut);
        #endregion

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }

}