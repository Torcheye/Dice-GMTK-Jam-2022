using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceFaceControl : MonoBehaviour
{
    public PrefabsScriptableObject prefabs;
    public int[] pointList;

    private Transform[] _faceList = new Transform[6];

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            _faceList[i] = transform.GetChild(i);
        }
    }

    private void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Destroy(_faceList[i].GetChild(0).gameObject);
            Instantiate(prefabs.gems[pointList[i]], _faceList[i]);
        }
    }

    public bool ChangeGroundFace(int increment)
    {
        int faceIndex = -1;
        float minAngle = float.MaxValue;
        for (int i = 0; i < 6; i++)
        {
            float angle = Vector3.Angle(_faceList[i].forward, Vector3.down);
            if (angle < minAngle)
            {
                faceIndex = i;
                minAngle = angle;
            }
        }

        if (faceIndex == -1)
            throw new Exception("No face is downwards");

        switch (pointList[faceIndex] + increment)
        {
            case >= 6:
                pointList[faceIndex] = 6;
                Destroy(_faceList[faceIndex].GetChild(0).gameObject);
                Instantiate(prefabs.gems[pointList[faceIndex]], _faceList[faceIndex]);
                return true;
            case < 0:
                return false;
            default:
                pointList[faceIndex] += increment;
                Destroy(_faceList[faceIndex].GetChild(0).gameObject);
                Instantiate(prefabs.gems[pointList[faceIndex]], _faceList[faceIndex]);
                return true;
        }
    }
}