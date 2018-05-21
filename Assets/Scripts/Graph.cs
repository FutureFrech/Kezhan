using Academy.HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Graph : Singleton<Graph>
{
    int capacity;
    int vexnum;   //大楼关键点个数
    float[][] arc;   //记录相邻关键点之间的距离
    float[][] dis;   //记录关键点间最短路径的信息
    int[][] path;  //记录最短路径的时下一步的点序号

    // Use this for initialization
    void Start () {
        vexnum = 1;
        capacity = 2 * vexnum;
        arc = new float[capacity][];
        dis = new float[capacity][];
        path = new int[capacity][];
        for (int i = 0; i < capacity; i++)
        {
            arc[i] = new float[capacity];
            dis[i] = new float[capacity];
            path[i] = new int[capacity];
            for (int k = 0; k < vexnum; k++)
            {
                //邻接矩阵初始化为无穷大
                arc[i][k] = 1000000;
            }
        }
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    //扩容
    void Stretch()
    {
        this.capacity = 2 * capacity;
        float[][] newarc;
        float[][] newdis;
        int[][] newpath;
        newarc = new float[capacity][];
        newdis = new float[capacity][];
        newpath = new int[capacity][];
        for (int i = 0; i < capacity; i++)
        {
            newarc[i] = new float[capacity];
            newdis[i] = new float[capacity];
            newpath[i] = new int[capacity];
        }
        for (int i = 0; i < vexnum; i++)
        {
            for (int j = 0; j < vexnum; j++)
            {
                newarc[i][j] = arc[i][j];
                newdis[i][j] = arc[i][j];
                newpath[i][j] = path[i][j];
            }
        }
        arc = newarc;
        dis = newdis;
        path = newpath;
    }

    //添加一个点
    public void AddPoint(float distance)
    {
        this.vexnum = vexnum + 1;
        DebugInfoManager.Instance.AddAnchorStatus(vexnum.ToString() + " " + distance.ToString() + "\n");
        if (vexnum == capacity)
        {
            Stretch();
        }
        for (int i = 0; i < vexnum; i++)
        {
            //邻接矩阵初始化为无穷大
            arc[vexnum - 1][i] = 1000000;
            arc[i][vexnum - 1] = 1000000;
        }
        arc[vexnum - 1][vexnum - 2] = distance;
        arc[vexnum - 2][vexnum - 1] = distance;
    }

    //缩容
    void Shrink()
    {
        this.capacity = capacity / 2;
        float[][] newarc;
        float[][] newdis;
        int[][] newpath;
        char[][] newdir;
        newarc = new float[capacity][];
        newdis = new float[capacity][];
        newpath = new int[capacity][];
        newdir = new char[capacity][];
        for (int i = 0; i < capacity; i++)
        {
            newarc[i] = new float[capacity];
            newdis[i] = new float[capacity];
            newpath[i] = new int[capacity];
            newdir[i] = new char[capacity];
        }
        for (int i = 0; i < vexnum; i++)
        {
            for (int j = 0; j < vexnum; j++)
            {
                newarc[i][j] = arc[i][j];
                newdis[i][j] = arc[i][j];
                newpath[i][j] = path[i][j];
            }
        }
        arc = newarc;
        dis = newdis;
        path = newpath;
    }

    //删除一个点
    public void DeletePoint(int point)
    {
        this.vexnum = vexnum - 1;

        for (int i = 0; i < vexnum + 1; i++)
        {
            for (int j = 0; j < vexnum + 1; j++)
            {
                if (i == point || j == point)
                {
                    continue;
                }
                else if (i < point && j < point)
                {
                    continue;
                }
                else if (i < point && j >= point)
                {
                    arc[i][j - 1] = arc[i][j];
                }
                else if (i >= point && j < point)
                {
                    arc[i - 1][j] = arc[i][j];
                }
                else
                {
                    arc[i - 1][j - 1] = arc[i][j];
                }
            }
        }
        if (vexnum == capacity / 2)
        {
            Shrink();
        }
    }

    //添加一条边
    public void AddEdge(int start, int end, float distance)
    {
        arc[start][end] = distance;
        arc[end][start] = distance;
        CreateGraph();
        Floyd();
    }

    //删除一条边
    public void DeleteEdge(int start, int end)
    {
        arc[start][end] = 1000000;
        arc[end][start] = 1000000;
        CreateGraph();
        Floyd();
    }

    //添补末点和起点，形成环路
    public void Circle(float distance)
    {
        this.AddEdge(vexnum - 1, 0, distance);
    }

    //dis和path矩阵初始化
    public void CreateGraph()
    {
        // inputarc(vexnum);
        for (int i = 0; i < vexnum; i++)
        {
            for (int j = 0; j < vexnum; j++)
            {
                dis[i][j] = arc[i][j];
                path[i][j] = j;
            }
        }
    }

    //从文件中读取邻接表，并输出在屏幕
    //public void InputArc(int n)
    //{
    //    FileStream fs = new FileStream("data.txt", FileMode.Open);
    //    StreamReader sr = new StreamReader(fs);
    //    string[] arcline;
    //    string[] dirline;
    //    arcline = new string[n];
    //    dirline = new string[n];
    //    for (int i = 0; i < n; i++)
    //    {
    //        arcline[i] = sr.ReadLine();
    //        for (int j = 0; j < n; j++)
    //        {
    //            arc[i][j] = float.Parse(arcline[i].Split(' ')[j]);
    //            if (arc[i][j] == -1) arc[i][j] = 1000000;
    //        }
    //    }
    //    sr.Close();
    //    fs.Close();

    //}

    //求最短路径
    public void Floyd()
    {
        CreateGraph();
        //三重循环，用于计算每个点对的最短路径
        int row = 0;
        int col = 0;
        int temp = 0;
        float select = 0;
        for (temp = 0; temp < vexnum; temp++)
        {
            for (row = 0; row < vexnum; row++)
            {
                for (col = 0; col < vexnum; col++)
                {
                    //为了防止溢出，所以需要引入一个select值
                    select = (dis[row][temp] - 1000000) >= 0 || (dis[temp][col] - 1000000) >= 0
                                 ? 1000000 : (dis[row][temp] + dis[temp][col]);
                    if (dis[row][col] > select)
                    {
                        //更新我们的D矩阵
                        dis[row][col] = select;
                        //更新我们的P矩阵
                        path[row][col] = path[row][temp];
                    }
                }
            }
        }
    }
    //输出最短路径列表
    public int[] PathCreate(int start, int end)
    {
        int tempstart = 0;
        int step = 0;
        tempstart = start;
        while (tempstart != end)
        {
            step++;
            tempstart = path[tempstart][end];
        }
        tempstart = start;
        //循环输出途径的每条路径。
        int[] pathlist;
        pathlist = new int[step + 1];
        int i = 0;
        while (tempstart != end)
        {
            pathlist[i] = tempstart;
            tempstart = path[tempstart][end];
            i++;
        }
        pathlist[i] = end;
        return pathlist;
    }
}
