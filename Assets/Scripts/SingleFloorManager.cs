﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

public class SingleFloorManager : Singleton<SingleFloorManager>
{
    private GameObject[] singleModel = new GameObject[6];
    private enum Status
    {
        normal = 0,
        split = 1,
        merge = 2,
        split_x = 3,
    };
    private Status modelStatus;
    private Vector3[] startPoint;  //Vector3 三维向量，存储坐标
    private Vector3[] zeroPoint;
    private Vector3[] splitPoint;
    private Vector3[] split_zPoint;
    private Vector3[] targetPoint;
    private float splitOffset = 0.05f;  //f是？splitoffset的含义？下同
    private float splitOffset_x = 1.1f;
    private bool ifMove;
    private int tick;  //推测是计时器/延时
    private const int movingFrames = 50;  //含义？

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 6; i++)
            singleModel[i] = gameObject.transform.Find("fit" + (i + 1).ToString()).gameObject;
        Init();
        modelStatus = Status.merge;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ifMove)
        {
            tick++;
            if (tick == movingFrames)
            {
                for (int i = 0; i < 6; ++i)
                {
                    singleModel[i].transform.position = targetPoint[i];  //计时结束时把终点位置信息赋给模型
                }
                ifMove = false;
                tick = 0;
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                {
                    singleModel[i].transform.position += (targetPoint[i] - startPoint[i]) / movingFrames;  //移动坐标，（目标-起始）/步长
                }
            }
			
        }

        // Test fold and unfold in unity emulator
        if (Input.GetKey(KeyCode.A))
            Split();
        if (Input.GetKey(KeyCode.S))
            Split_x();
        if (Input.GetKey(KeyCode.D))
            Merge();
    }

    // Initialize all the elements
    private void Init()
    {
        startPoint = new Vector3[6];
        targetPoint = new Vector3[6];
        ifMove = false;
        tick = 0;
        modelStatus = Status.normal;
    }


    // Show all the floor of the model
    public void ShowAllFloor()
    {
        for (int i = 0; i < 6; i++)
            singleModel[i].SetActive(true);
    }

    private void UpdateStandPoint()  //纵向拉开
    {
        zeroPoint = new Vector3[6];
        splitPoint = new Vector3[6];
        split_zPoint = new Vector3[6];
        int i = 0;
        for (; i < 6; i++)
        {
            zeroPoint[i] = singleModel[i].transform.position;
            splitPoint[i] = zeroPoint[i];
            splitPoint[i].y += splitOffset * i;
            split_zPoint[i] = zeroPoint[i];
            split_zPoint[i] += splitOffset_x * i * this.transform.right;
        }
    }

    public void Split()
    {
        if (modelStatus == Status.split || modelStatus == Status.split_x)
            return;
        UpdateStandPoint();
        targetPoint = splitPoint;
        for (int i = 0; i < 6; ++i)
            startPoint[i] = singleModel[i].transform.position;
        ifMove = true;
        modelStatus = Status.split;
    }

    public void Split_x()  //和上面那个有啥区别？
    {
        if (modelStatus == Status.split_x || modelStatus == Status.split)
            return;
        UpdateStandPoint();
        targetPoint = split_zPoint;
        for (int i = 0; i < 6; ++i)
            startPoint[i] = singleModel[i].transform.position;
        ifMove = true;
        modelStatus = Status.split_x;
    }

    public void Merge()  //还没看
    {
        if (modelStatus == Status.merge)
            return;
        zeroPoint = new Vector3[6];
        if (modelStatus == Status.split)
        {
            int i = 0;
            for (; i < 6; i++)
            {
                zeroPoint[i] = singleModel[i].transform.position;
                zeroPoint[i].y -= splitOffset * i;
            }
        }
        if (modelStatus == Status.split_x)
        {
            int i = 0;
            for (; i < 6; i++)
            {
                zeroPoint[i] = singleModel[i].transform.position;
                zeroPoint[i] -= splitOffset_x * i * this.transform.right;
            }
        }
        targetPoint = zeroPoint;
        for (int i = 0; i < 6; ++i)
            startPoint[i] = singleModel[i].transform.position;
        ifMove = true;
        modelStatus = Status.merge;
    }

}
