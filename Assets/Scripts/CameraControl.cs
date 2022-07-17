using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float cameraRotateSpeed = 1;

    private Vector3 _mousePosLast;

    private void Update()
    {
        if (Input.GetMouseButton(1))
            RotateCamera();
        _mousePosLast = Input.mousePosition;
    }

    private void RotateCamera()
    {
        float mouseDeltaX = (Input.mousePosition - _mousePosLast).x;
        transform.Rotate(Vector3.up, mouseDeltaX * Time.deltaTime * cameraRotateSpeed);
    }

    public Vector3 GetDirection()
    {
        Vector3 forward = transform.forward;
        Vector3[] directionList = { Vector3.forward, Vector3.left, Vector3.right, Vector3.back };

        Vector3 result = Vector3.zero;
        float maxDot = float.NegativeInfinity;
        foreach (var dir in directionList)
        {
            float dot = Vector3.Dot(dir, forward);
            if (!(dot > maxDot))
                continue;

            result = dir;
            maxDot = dot;
        }

        return result;
    }
}