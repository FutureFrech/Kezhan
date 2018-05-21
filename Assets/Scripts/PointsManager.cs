using UnityEngine;
using Academy.HoloToolkit.Unity;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;


public class PointsManager : Singleton<PointsManager>
{

    public GameObject pointPrefab;

    #region Variables to Modify
    const int maxPoints = 100;
    GameObject[] pointsCollection = new GameObject[maxPoints];
    int pointsNum = 0;
    public int PointsNum { get { return pointsNum; } }
    public GameObject[] PointsCollection { get { return pointsCollection; } }
    int Point1, Point2;
    #endregion

    #region Variables for Anchor Saving
    private WorldAnchorStore store; // For anchor storing and loding
    // Assuming the Same regions, floors and points
    int maxRegion = 3, nowRegion;
    int maxFloor = 5, nowFloor;
    int maxPoint = 10, nowPoint;
    #endregion

    private void Start()
    {
        WorldAnchorStore.GetAsync(AnchorStoreLoaded);
    }

    #region Add and Del Points & Lines
    // Add new points to collection
    // ToDo: Modify, this is for competition
    // TODO: Graph func, Model func remain to be set
    public void AddPoint()
    {
        // Initiate a new object
        pointsCollection[pointsNum] = Instantiate(pointPrefab, Camera.main.transform);

        // Add a name for anchor
        pointsCollection[pointsNum].name = pointsNum.ToString();
        SaveAnchor(pointsCollection[pointsNum]);

        Vector3 p = pointsCollection[pointsNum].transform.position;

        Model2D.Instance.AddModelPoint(p);

        DebugInfoManager.Instance.SetAnchorStatus("Point " + pointsNum.ToString() + " Added\n");

        // Params for graph
        if (pointsNum > 0)
        {
            AddNewPoint(pointsNum - 1, pointsNum);
        }

        if (pointsNum < maxPoints - 1)
        {
            pointsNum++;
        }
    }

    public void DelPoint()
    {
        if (pointsNum > 0)
        {
            pointsNum--;
            DeleteAnchor(pointsCollection[pointsNum].name);
            DestroyImmediate(pointsCollection[pointsNum]);

            // Delete in Graph
            Graph.Instance.DeletePoint(pointsNum);

            if (pointsNum > 0)
            {
                DelLine(pointsNum - 1, pointsNum);
            }
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < maxPoints; i++)
        {
            if (pointsCollection[i] == null)
            {
                break;
            }
            else
            {
                DeleteAnchor(pointsCollection[i].name);
                DestroyImmediate(pointsCollection[i]);
            }
        }

        pointsNum = 0;
    }

    public void LoadPoint(string name)
    {
        GameObject obj = Instantiate(pointPrefab);
        obj.name = name;
        LoadAnchor(obj);
    }

    public void EndAddingPoint()
    {
        AddLine(0, pointsNum - 1);
        GestureManager.Instance.Transition(GestureManager.Instance.ClickRecognizer);
        DebugInfoManager.Instance.SetAnchorStatus("Adding complete\n");
    }

    public void AddNewPoint(int id1, int id2)
    {
        float cost;
        Vector3 p1 = pointsCollection[id1].transform.position;
        Vector3 p2 = pointsCollection[id2].transform.position;
        cost = Vector3.Distance(p1, p2);

        // Call Function in Graph
        // Params: id1, id2, cost
        Graph.Instance.AddPoint(cost);

        // Call Function in Model
        Model2D.Instance.AddModelLine(id1, id2);
    }

    public void AddLine(int id1, int id2)
    {
        float cost;
        Vector3 p1 = pointsCollection[id1].transform.position;
        Vector3 p2 = pointsCollection[id2].transform.position;
        cost = Vector3.Distance(p1, p2);

        // Call Function in Graph
        // Params: id1, id2, cost
        Graph.Instance.AddEdge(id1, id2, cost);

        // Call Function in Model
        Model2D.Instance.AddModelLine(id1, id2);
    }

    public void DelLine(int id1, int id2)
    {
        Model2D.Instance.DelModelLine(id1, id2);
        DebugInfoManager.Instance.AddAnchorStatus("Del model\n");
        Graph.Instance.DeleteEdge(id1, id2);
    }

    public void SetPoint1(int id)
    {
        Point1 = id;
    }

    public void SetPoint2(int id)
    {
        Point2 = id;
    }

    public void AddLineClicked()
    {
        AddLine(Point1, Point2);
    }

    public void DelLineClicked()
    {
        DelLine(Point1, Point2);
    }
    #endregion

    #region Load and Save Anchors
    private void AnchorStoreLoaded(WorldAnchorStore store)
    {
        this.store = store;
    }

    private void LoadAnchor(GameObject obj)
    {
        bool retTrue = store.Load(obj.name, obj);
        if (!retTrue)
        {
            // Until the objIWantAnchored has an anchor saved at least once it will not be in the AnchorStore
        }
    }

    private void SaveAnchor(GameObject obj)
    {
        bool retTrue;
        WorldAnchor anchor = obj.AddComponent<WorldAnchor>();
        // Remove any previous worldanchor saved with the same name so we can save new one
        store.Delete(obj.name);
        retTrue = store.Save(obj.name, anchor);
        if (!retTrue)
        {
        }
    }

    public void DeleteAnchor(string name)
    {
        store.Delete(name);
    }

    public void DeleteAllAnchor()
    {
        string[] ids = this.store.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            store.Delete(ids[index]);
        }
    }
    #endregion

    #region Anchoring the World
    private string GetAnchorName(int region, int floor, int point)
    {
        string name;

        name = region.ToString() + floor.ToString() + point.ToString();

        return name;
    }

    private string LastAnchorName()
    {
        if (nowRegion == 0 && nowFloor == 0 && nowPoint == 0)
        {
            return "000";
        }

        int r, f, p;
        int f_, r_;

        p = nowPoint - 1 + maxPoint;
        f_ = p / maxPoint - 1;
        p %= maxPoint;

        f = nowFloor + f_ + maxFloor;
        r_ = f / maxFloor - 1;
        f %= maxFloor;

        r = nowRegion + r_;

        string name = r.ToString() + f.ToString() + p.ToString();
        return name;
    }

    private void SetNextAnchorNum()
    {
        if (nowRegion == maxRegion && nowFloor == maxFloor && nowPoint == maxPoint)
        {
            return;
        }

        int f_, r_;

        nowPoint++;
        f_ = nowPoint / maxPoint;
        nowPoint %= maxPoint;

        nowFloor += f_;
        r_ = nowFloor / maxFloor;
        nowFloor %= maxFloor;

        nowRegion += r_;
        // nowRegion %= maxRegion;
    }

    private void SetLastAnchorNum()
    {
        if (nowRegion == 0 && nowFloor == 0 && nowPoint == 0)
        {
            return;
        }

        int f_, r_;

        nowPoint = nowPoint - 1 + maxPoint;
        f_ = nowPoint / maxPoint - 1;
        nowPoint %= maxPoint;

        nowFloor = nowFloor + f_ + maxFloor;
        r_ = nowFloor / maxFloor - 1;
        nowFloor %= maxFloor;

        nowRegion = nowRegion + r_;
    }
    #endregion

    #region Set Source and Destination
    public int GetSourceNear()
    {
        int start = -1;
        float dist = 0, temp;
        Vector3 pos = Camera.main.transform.position;

        //DebugInfoManager.Instance.AddAnchorStatus(PointsNum.ToString() + "\n");
        for (int i = 0; i < PointsNum; i++)
        {
            if (PointsCollection[i] != null)
            {
                temp = Vector3.Distance(pos, PointsCollection[i].transform.position);
                if (i == 0)
                {
                    dist = temp;
                    start = 0;
                }
                else if (dist > temp)
                {
                    dist = temp;
                    start = i;
                }
            }
        }

        //DebugInfoManager.Instance.AddAnchorStatus(start.ToString() + " " + dist.ToString() + "\n");
        return start;
    }

    public void SetDestAnchor(string p0, string p1, float r)
    {
        Vector3 pos = GetDestPos(p0, p1, r);

        GameObject obj = Instantiate(pointPrefab);
        obj.transform.position = pos;
        obj.name = "Destination";
        SaveAnchor(obj);
    }

    public Vector3 GetDestPos(string p0, string p1, float r)
    {
        GameObject obj0, obj1;
        Vector3 dest;

        obj0 = Instantiate(pointPrefab);
        obj0.name = p0;
        obj1 = Instantiate(pointPrefab);
        obj1.name = p1;

        LoadAnchor(obj0);
        LoadAnchor(obj1);

        dest = obj0.transform.position * (1 - r) + obj1.transform.position * r;

        DestroyImmediate(obj0.GetComponent<WorldAnchor>());
        DestroyImmediate(obj0);
        DestroyImmediate(obj1.GetComponent<WorldAnchor>());
        DestroyImmediate(obj1);

        return dest;
    }
    #endregion

    #region Remained Functions
    // Whether or not to show points
    public void ShowAllPoints(bool ifShow)
    {
        for (int i = 0; i < maxPoints; i++)
        {
            if (pointsCollection[i] != null)
                pointsCollection[i].SetActive(ifShow);
        }
    }
    #endregion
}
