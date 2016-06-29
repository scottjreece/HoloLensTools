using System;
using UnityEngine;
using System.Collections;

public class HoloTestingCamera : MonoBehaviour
{
    public float SensitivityX = 8F;
    public float SensitivityY = 8F;
    public float MinimumX = -360F;
    public float MaximumX = 360F;
    public float MinimumY = -89.99F;
    public float MaximumY = 89.99F;
    public bool AddReticle = true;
    public Material ReticleMaterial;
    public float MoveSpeed = 4;
    public bool HideMouseCursor = true;
    public bool ClampForwardMotionToXZPlane = true;

    private Camera _camera;
    private float _rotationX;
    private float _rotationY;
    private Quaternion _originalRotation;

    public static Action<GameObject> ObjectClickedHandler = x => x.SendMessageUpwards("OnSelect");

#if !UNITY_EDITOR
    void Update(){}
    void Start(){}
#else

    void Start()
    {
        _camera = GetComponent<Camera>();
        _originalRotation = transform.localRotation;

        if (AddReticle)
            CreateReticle();

        if (HideMouseCursor)
            Cursor.visible = false;
    }

    void Update()
    {
        DoMove();
        DoLook();

        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            var targetedObject = hitInfo.collider.gameObject;
            ObjectClickedHandler(targetedObject);
        }
    }

    private void DoMove()
    {
        var forwardDistance = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
        var horizontalDistance = Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;
        var verticalDistance = Input.GetAxis("Jump") * MoveSpeed * Time.deltaTime;

        if (ClampForwardMotionToXZPlane)
        {
            var forward = _camera.transform.forward;
            forward.y = 0;
            forward.Normalize();
            gameObject.transform.Translate(forward*forwardDistance, Space.World);
        }
        else
        {
            gameObject.transform.Translate(Vector3.forward*forwardDistance);
        }

        gameObject.transform.Translate(Vector3.right * horizontalDistance, Space.Self);
        gameObject.transform.Translate(Vector3.up * verticalDistance, Space.World);
    }

    private void DoLook()
    {
            // Read the mouse input axis
            _rotationX += Input.GetAxis("Mouse X") * SensitivityX;
            _rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
            _rotationX = ClampAngle(_rotationX, MinimumX, MaximumX);
            _rotationY = ClampAngle(_rotationY, MinimumY, MaximumY);
            var xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
            var yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);
            transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
    }

    private void CreateReticle()
    {
        AddCube(-.01f, 0, .01f, .005f);
        AddCube(.01f, 0, .01f, .005f);
        AddCube(0, .01f, .005f, .01f);
        AddCube(0, -.01f, .005f, .01f);
    }

    private void AddCube(float x, float y, float width, float height)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = gameObject.transform;
        cube.transform.localPosition = new Vector3(x, y, _camera.nearClipPlane+.1f);
        cube.transform.localScale = new Vector3(width, height, .01f);
        if (ReticleMaterial != null)
            cube.GetComponent<MeshRenderer>().material = ReticleMaterial;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
#endif
}
