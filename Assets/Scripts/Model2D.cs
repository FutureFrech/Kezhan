using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Academy.HoloToolkit.Unity;

public class ModelLine
{
    public Vector3 P1, P2;
    public GameObject Line;

    public ModelLine(Vector3 p1, Vector3 p2, GameObject obj)
    {
        Line = obj;
        P1 = p1;
        P2 = p2;
    }
}

public class Model2D : Singleton<Model2D>
{
    // 假定实际采样点都在一个水平面上
    // 建模使模型在 X0Y 平面上，
    // 所以需要在用户转身时，进行 Y 轴上的旋转

    const float PI = 3.14159f; // 圆周率，计算旋转角度时使用
    const float WIDTH = 0.02f; // 圆柱的直径缩放比例
    const float RULE = 5e-2f; // 现实中的点缩放成模型中的点的比例
    const float SCALE = 1f / 2; // 圆柱默认为 2 单位长度

    List<Vector3> ModelPoints = new List<Vector3>(); // 存储模型中的点
    Vector3[] ActualPoints; // 暂时作为存储实际点的变量
    GameObject model; // 模型、空物体，需要初始化
    List<ModelLine> lines = new List<ModelLine>(); // 存储模型中的边
    Vector3 origin = new Vector3(); // 以第一个点为原点建立模型

    // Use this for initialization
    void Start()
    {
        DebugInfoManager.Instance.AddAnchorStatus("Init\n");
        InitModel();
    }

    // Update is called once per frame
    void Update()
    {
        // 修改模型在用户视角的位置
        UpdateModelPosition();
    }

    void SetActualPoints()
    {
        ActualPoints = new Vector3[4]
        {
            new Vector3(1,1,0),
            new Vector3(5,1,2),
            new Vector3(3,1,7),
            new Vector3(0,1,2)
        };
    }

    void InitModel()
    {
        model = new GameObject("Model");
        model.transform.position = new Vector3(0, 0, 0);
    }

    public void AddModelPoint(Vector3 p)
    {
        if (ModelPoints.Count == 0)
        {
            origin.x = p.x;
            origin.y = p.z;
            origin.z = 0;
        }

        Vector3 mp = new Vector3()
        {
            x = RULE * (p.x - origin.x),
            y = RULE * (p.z - origin.y),
            z = 0
        };

        ModelPoints.Add(mp);
    }

    public void AddModelLine(Vector3 p1, Vector3 p2)
    {
        Vector3 center = (p1 + p2) / 2 + model.transform.position;
        Vector3 dir = p1 - p2;
        if (dir.x < 0)
        {
            dir = -dir;
        }
        float theta = -(float)Math.Acos(dir.y / dir.magnitude) / PI * 180;

        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        line.transform.parent = model.transform;
        line.transform.localScale = new Vector3(WIDTH, SCALE * dir.magnitude, WIDTH);
        line.transform.Rotate(0, 0, theta);
        line.transform.position = center;

        ModelLine mLine = new ModelLine(p1, p2, line);
        lines.Add(mLine);
    }

    public void AddModelLine(int id1, int id2)
    {
        Vector3 p1 = ModelPoints[id1], p2 = ModelPoints[id2];

        Vector3 center = (p1 + p2) / 2 + model.transform.position;
        center = (p1 + p2) / 2;
        Vector3 dir = p1 - p2;
        if (dir.x < 0)
        {
            dir = -dir;
        }
        float theta = -(float)Math.Acos(dir.y / dir.magnitude) / PI * 180;

        GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        line.transform.parent = model.transform;
        line.transform.localScale = new Vector3(WIDTH, SCALE * dir.magnitude, WIDTH);
        line.transform.rotation = model.transform.rotation;
        line.transform.Rotate(0, 0, theta, Space.Self);
        line.transform.localPosition = center;

        ModelLine mLine = new ModelLine(p1, p2, line);
        lines.Add(mLine);
    }

    public void DelModelLine(int id1, int id2)
    {
        foreach (ModelLine l in lines)
        {
            if ((l.P1 == ModelPoints[id1] && l.P2 == ModelPoints[id2]) || (l.P1 == ModelPoints[id2] && l.P2 == ModelPoints[id1]))
            {
                lines.Remove(l);
                DestroyImmediate(l.Line);
                break;
            }
        }
    }

    public void DelModelLine(Vector3 p1, Vector3 p2)
    {
        foreach (ModelLine l in lines)
        {
            if ((l.P1 == p1 && l.P2 == p2) || (l.P1 == p2 && l.P2 == p1))
            {
                lines.Remove(l);
                DestroyImmediate(l.Line);
                break;
            }
        }
    }

    void Debug()
    {
        for (int i = 0; i < ActualPoints.Length; i++)
        {
            DebugInfoManager.Instance.AddAnchorStatus("Add Point\n");
            AddModelPoint(ActualPoints[i]);
            if (i > 0)
            {
                AddModelLine(ModelPoints[i - 1], ModelPoints[i]);
            }
        }
    }

    public void ZoomIn()
    {
        model.transform.localScale *= 0.9f;
    }

    public void ZoomOut()
    {
        model.transform.localScale /= 0.9f;
    }

    void UpdateModelPosition()
    {
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;
        Vector3 rightDirection = Camera.main.transform.right;
        Vector3 upDirection = Camera.main.transform.up;
        model.transform.position = headPosition + gazeDirection * 5 + rightDirection * 1f + upDirection * 0.5f;
        model.transform.rotation = Camera.main.transform.rotation;
    }

}
