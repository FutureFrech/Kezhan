using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

// This class form the path and detect which point where the user is.
public class Route : Singleton<Route>
{
    int pointsNum = 0;
    int pointsReached = 0;
    int[] pointsIndex;
    Vector3[] pointsPosition;
    bool startRoute = false;

    int Start, End;

    float currentDistance;
    const float threshold = 1f;

    public Vector3 CurrentDirection { get; private set; }
    public bool Terminal { get; private set; }
    public Vector3 EndPostion { get; private set; }

    public int[] PointsIndex
    {
        get
        {
            return pointsIndex;
        }

        set
        {
            pointsIndex = value;
        }
    }

    void Update()
    {
        CheckReachedPointsStatus();
    }

    //Load data from files
    /*
    public void LoadData()
    {
        WriteLog fileCommand = GameObject.Find("Manager").GetComponent<WriteLog>();
        List<string> fileInfo = fileCommand.ReadFileList(Application.persistentDataPath, "pointsData.txt");
        if (fileInfo == null)
            return;
        if (fileInfo.Count != 0)
        {
            int j = 0;
            for (int i = 0; i < fileInfo.Count; i++)
            {
                if (Str2Vector3.IfLegal(fileInfo[i]))
                {
                    positionSaved[j] = Str2Vector3.ConvertStr(fileInfo[i]);
                    j++;
                }
            }
            pointsNum = j;
        }
    }
    */

    public void SetSource()
    {
        Start = PointsManager.Instance.GetSourceNear();
    }

    public void SetDestination(int end)
    {
        End = end;
        DebugInfoManager.Instance.AddAnchorStatus("\n" + End);
        Terminal = false;
    }

    public void GetIndexFromGraph()
    {
        Graph.Instance.Floyd();
        pointsIndex = Graph.Instance.PathCreate(Start, End);
        DebugInfoManager.Instance.AddAnchorStatus(pointsIndex.ToString() + "\n");
        //pointsIndex = new int[] {2, 0, 1};
    }

    // Load points data from the Class PointsManager.cs
    public void LoadDataFromAnchor()
    {
        string temp = "";
        int index;

        pointsNum = PointsIndex.Length;
        pointsPosition = new Vector3[pointsNum];
        for (int i = 0; i < pointsNum; i++)
        {
            index = pointsIndex[i];
            if (PointsManager.Instance.PointsCollection[index] != null)
            {
                pointsPosition[i] = PointsManager.Instance.PointsCollection[index].transform.position;
                temp = temp + pointsPosition[i].ToString() + "\n";
            }
        }
        EndPostion = pointsPosition[pointsNum - 1];
    }

    // Start navigation and detect the status of points.
    public void StartNavigation()
    {
        pointsReached = -1;
        if (pointsNum > 0)
        {
            startRoute = true;

            CurrentDirection = pointsPosition[0] -
                Camera.main.transform.position;
        }
        else
        {
            startRoute = false;
        }
    }

    // Check the status of points.
    void CheckReachedPointsStatus()
    {
        if (startRoute)
        {
            if (pointsReached >= (pointsNum - 1))
            {
                Terminal = true;
                startRoute = false;
            }
            else
            {
                currentDistance = Vector3.Distance(Camera.main.transform.position,
                    pointsPosition[pointsReached + 1]);
                if (currentDistance <= threshold)
                {
                    pointsReached++;
                    if (pointsReached < (pointsNum - 1))
                    {
                        CurrentDirection = pointsPosition[pointsReached + 1] - Camera.main.transform.position;
                    }
                }
            }
        }
    }
}
