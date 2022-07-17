using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CameraControl cameraControl;
    public Transform diceModel;
    public GameObject shatteredModel;

    private bool _canMove = true;
    private DiceFaceControl _diceFaceControl;
    private Vector3 _lastPos;
    private Quaternion _lastRotation;
    private bool _isSpawning;
    private bool _beginRecordPosition;

    private void Awake()
    {
        _diceFaceControl = diceModel.GetComponent<DiceFaceControl>();
        _isSpawning = true;
        _lastPos = transform.position;
        _lastRotation = diceModel.rotation;
    }

    private void Update()
    {
        HandleMove(cameraControl.GetDirection());
    }

    private void HandleMove(Vector3 forward)
    {
        if (!_canMove || _isSpawning)
            return;

        const float inputThreshold = 0.01f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float x = 0, z = 0;

        if (Mathf.Abs(horizontal) < inputThreshold && Mathf.Abs(vertical) < inputThreshold)
            return;

        _canMove = false;

        if (Mathf.Abs(horizontal) > inputThreshold)
            x = horizontal > 0 ? 1 : -1;
        else if (Mathf.Abs(vertical) > inputThreshold)
            z = vertical > 0 ? 1 : -1;

        Vector3 destLocal = forward * z + Vector3.Cross(Vector3.up, forward) * x;
        MoveToDest(destLocal);
    }
    private async void MoveToDest(Vector3 destLocal)
    {
        const float duration = .35f;

        Vector3 dest = transform.TransformPoint(destLocal);

        diceModel.transform.DORotate(new Vector3(destLocal.z * 90, 0, -destLocal.x * 90), duration, RotateMode.WorldAxisAdd)
            .SetEase(Ease.OutExpo);
        await transform.DOLocalJump(dest, 1, 1, duration).SetEase(Ease.OutExpo).AsyncWaitForCompletion();

        if (_beginRecordPosition)
        {
            _lastPos = transform.position;
            _lastRotation = diceModel.rotation;
            _beginRecordPosition = false;
        }

        _canMove = true;
    }

    public void StepOnGround(int gemCount)
    {
        if (_isSpawning)
        {
            _isSpawning = false;
            return;
        }

        if (!_beginRecordPosition)
            _beginRecordPosition = true;

        if (gemCount == 0)
        {
            if (!_diceFaceControl.ChangeGroundFace(-1))
                Shatter();
        }
        else
        {
            _diceFaceControl.ChangeGroundFace(gemCount);
        }
    }

    private void Shatter()
    {
        GameObject shatteredObject = Instantiate(shatteredModel, transform.position, Quaternion.identity);
        int childCount = shatteredObject.transform.childCount;
        Vector3 explosionCenter = shatteredObject.transform.position + Vector3.up * .5f;

        for (int i = 0; i < childCount; i++)
        {
            shatteredObject.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(15, explosionCenter, 3);
        }

        Destroy(gameObject);
    }

    public void StartRespawn()
    {
        _isSpawning = true;
    }

    public void Respawn(float respawnHeight)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = _lastPos + Vector3.up * respawnHeight;
        diceModel.rotation = _lastRotation;
    }
}