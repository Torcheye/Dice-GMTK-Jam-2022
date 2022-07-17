using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform diceModel;
    public GameObject shatteredModel;
    public float respawnHeight;
    public bool isMenu;

    private CameraControl _cameraControl;
    private bool _canMove = true;
    private DiceFaceControl _diceFaceControl;
    private Vector3 _lastPos;
    private Quaternion _lastRotation;
    private bool _isSpawning;
    private bool _beginRecordPosition;

    private void Awake()
    {
        _diceFaceControl = diceModel.GetComponent<DiceFaceControl>();
        _cameraControl = FindObjectOfType<CameraControl>();
        _isSpawning = true;
        _lastPos = transform.position;
        _lastRotation = diceModel.rotation;
    }

    private void Update()
    {
        HandleMove(_cameraControl.GetDirection());

        if (transform.position.y < 0 && !_isSpawning)
        {
            _isSpawning = true;
        }
    }

    private void HandleMove(Vector3 forward)
    {
        if (!_canMove || _isSpawning)
            return;

        float x = 0, z = 0;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            x = 1;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            x = -1;
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            z = 1;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            z = -1;

        if (Mathf.Abs(x) + Mathf.Abs(z) <= 0)
            return;

        _canMove = false;

        Vector3 destLocal = forward * z + Vector3.Cross(Vector3.up, forward) * x;
        MoveToDest(destLocal);
    }
    private async void MoveToDest(Vector3 destLocal)
    {
        const float duration = .35f;
        GameManager.Instance.PlaySoundWoosh();

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

    public void StepOnGround(int gemCount, Transform destFloor = null)
    {
        GameManager.Instance.PlaySoundFootstep();

        if (_isSpawning)
        {
            _isSpawning = false;
            return;
        }

        if (!_beginRecordPosition)
            _beginRecordPosition = true;

        if (isMenu)
        {
            if (destFloor != null)
                StartCoroutine(Success(destFloor));
            return;
        }

        if (gemCount == 0)
        {
            if (!_diceFaceControl.ChangeGroundFace(-1))
            {
                Shatter();
            }
            else
            {
                if (destFloor != null)
                {
                    StartCoroutine(Success(destFloor));
                }
            }
        }
        else
        {
            _diceFaceControl.ChangeGroundFace(gemCount);
        }
    }

    private IEnumerator Success(Transform destFloor)
    {
        destFloor.transform.DOMoveY(10, 3);
        destFloor.transform.DORotate(new Vector3(0, 720, 0), 3, RotateMode.WorldAxisAdd);
        transform.DOMoveY(10, 3);
        transform.DORotate(new Vector3(0, 720, 0), 3, RotateMode.WorldAxisAdd);
        yield return new WaitForSeconds(3);
        GameManager.Instance.NextLevel();
    }

    private void Shatter()
    {
        GameManager.Instance.PlaySoundShatter();
        GameManager.Instance.ShakeScreen();

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
        if (_isSpawning)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = _lastPos + Vector3.up * respawnHeight;
        diceModel.rotation = _lastRotation;
    }
}