using System;
using UnityEngine;

/// <summary>
/// To use this script, attach it to the main camera in your Unity scene. This will allow you to look around with the
/// mouse, and to move around with the keyboard (WASD by default) while testing your HoloLens app in Unity.
/// </summary>
public class HoloTestingCamera : MonoBehaviour
{
    public float SensitivityX = 8F;
    public float SensitivityY = 8F;
    public float MoveSpeed = 4;
    public float MinimumY = -89.99F;
    public float MaximumY = 89.99F;
    public bool AddReticle = true;
    public Material ReticleMaterial;
    public bool HideMouseCursor = true;
    public bool ClampForwardMotionToXZPlane = true;

    private Camera _camera;
    private float _rotationX;
    private float _rotationY;
    private Quaternion _originalRotation;

    //Replace this Action with something else if you don't want to call an object's OnSelect method when it's clicked on
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
        HandleMovement();
        HandleLookDirection();

        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            return;

        var targetedObject = hitInfo.collider.gameObject;
        ObjectClickedHandler(targetedObject);
    }

    private void HandleMovement()
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

    private void HandleLookDirection()
    {
            _rotationX += Input.GetAxis("Mouse X") * SensitivityX;
            _rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
            var xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
            var yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);
            transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
    }

    private void CreateReticle()
    {
        AddReticleCube(-.01f, 0, .01f, .005f);
        AddReticleCube(.01f, 0, .01f, .005f);
        AddReticleCube(0, .01f, .005f, .01f);
        AddReticleCube(0, -.01f, .005f, .01f);
    }

    private void AddReticleCube(float x, float y, float width, float height)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = gameObject.transform;
        cube.transform.localPosition = new Vector3(x, y, _camera.nearClipPlane+.1f);
        cube.transform.localScale = new Vector3(width, height, .01f);
        if (ReticleMaterial != null)
            cube.GetComponent<MeshRenderer>().material = ReticleMaterial;
    }
#endif
}
