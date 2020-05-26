using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class FreeCamController : MonoBehaviour
{
    public float m_LookSpeedController = 120f;
    public float m_LookSpeedMouse = 10.0f;
    public float m_MoveSpeed = 10.0f;
    public float m_MoveSpeedIncrement = 2.5f;
    public float m_Turbo = 10.0f;

    string kMouseX = "Mouse X";
    string kMouseY = "Mouse Y";
    string kRightStickX = "Controller Right Stick X";
    string kRightStickY = "Controller Right Stick Y";
    string kVertical = "Vertical";
    string kHorizontal = "Horizontal";

    string kYAxis = "YAxis";
    string kSpeedAxis = "Speed Axis";

    bool freeCamOn;

    Camera[] cameras;
    public Canvas menu;
    public MenuFreeCam menuFreeCam;
    //public OrbitControl orbitControl;

    float lookSpeedController;
    float lookSpeedMouse;
    float moveSpeed;
    float moveSpeedIncrement;
    float turbo;

    private void Start()
    {
        cameras = GetComponentsInChildren<Camera>();
        foreach (Camera item in cameras)
        {
            item.enabled = false;
        }
        menu.gameObject.SetActive(false);
        lookSpeedController = m_LookSpeedController;
        lookSpeedMouse = m_LookSpeedMouse;
        moveSpeed = m_MoveSpeed;
        moveSpeedIncrement = m_MoveSpeedIncrement;
        turbo = m_Turbo;
    }




    void LateUpdate()
    {
        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.F) && !freeCamOn) || freeCamOn)
        {
            ControlFreeCamState(true);

            CameraControl();
        }
        else if((Input.GetKeyDown(KeyCode.Escape) && freeCamOn))
        {
            ControlFreeCamState(false);
        }

        if (!freeCamOn)
        {
            transform.position = PlayerController.s_instance.m_references.m_worldCamera.transform.position;
            transform.rotation = PlayerController.s_instance.m_references.m_worldCamera.transform.rotation;
        }

        if(freeCamOn && Input.GetKeyDown(KeyCode.F))
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        }
        if (menu.gameObject.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_LookSpeedController = Mathf.Lerp(0, lookSpeedController * 2, menuFreeCam.controlRotationSpeedSlider.value);
            m_LookSpeedMouse = Mathf.Lerp(0, lookSpeedMouse * 2, menuFreeCam.mouseRotationSpeedSlider.value);
            m_MoveSpeed = Mathf.Lerp(0, moveSpeed * 2, menuFreeCam.mouseSpeedSlider.value);
            m_MoveSpeedIncrement = Mathf.Lerp(0, moveSpeedIncrement * 2, menuFreeCam.mouseSpeedIncrementSlider.value);
            m_Turbo = Mathf.Lerp(0, turbo * 2, menuFreeCam.turboSlider.value);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //if(Input.GetKeyDown(KeyCode.O) && freeCamOn)
        //{
        //    RaycastHit _hit;
        //    Physics.Raycast(transform.position, transform.forward, out _hit, Mathf.Infinity);

        //    if(gameObjects.Count > 0)
        //    {
        //        Destroy(gameObjects[0].gameObject);
        //        gameObjects.RemoveAt(0);
        //    }

        //    GameObject go = Instantiate(orbitControl.objectToRotate, _hit.point, Quaternion.identity);
        //    gameObjects.Add(go);
        //}

    }
    List<GameObject> gameObjects = new List<GameObject>();
    void CameraControl()
    {
        float inputRotateAxisX = 0.0f;
        float inputRotateAxisY = 0.0f;
        if (Input.GetMouseButton(1))
        {
            inputRotateAxisX = Input.GetAxis(kMouseX) * m_LookSpeedMouse;
            inputRotateAxisY = Input.GetAxis(kMouseY) * m_LookSpeedMouse;
        }
        inputRotateAxisX += (Input.GetAxis(kRightStickX) * m_LookSpeedController * Time.deltaTime);
        inputRotateAxisY += (Input.GetAxis(kRightStickY) * m_LookSpeedController * Time.deltaTime);

        float inputChangeSpeed = Input.GetAxis(kSpeedAxis);
        if (inputChangeSpeed != 0.0f)
        {
            m_MoveSpeed += inputChangeSpeed * m_MoveSpeedIncrement;
            if (m_MoveSpeed < m_MoveSpeedIncrement) m_MoveSpeed = m_MoveSpeedIncrement;
        }

        float inputVertical = Input.GetAxis(kVertical);
        float inputHorizontal = Input.GetAxis(kHorizontal);
        float inputYAxis = Input.GetAxis(kYAxis);

        bool moved = inputRotateAxisX != 0.0f || inputRotateAxisY != 0.0f || inputVertical != 0.0f || inputHorizontal != 0.0f || inputYAxis != 0.0f;
        if (moved)
        {
            float rotationX = transform.localEulerAngles.x;
            float newRotationY = transform.localEulerAngles.y + inputRotateAxisX;

            // Weird clamping code due to weird Euler angle mapping...
            float newRotationX = (rotationX - inputRotateAxisY);
            if (rotationX <= 90.0f && newRotationX >= 0.0f)
                newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
            if (rotationX >= 270.0f)
                newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

            transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);

            float moveSpeed = Time.deltaTime * m_MoveSpeed;
            if (Input.GetMouseButton(1))
                moveSpeed *= Input.GetKey(KeyCode.LeftShift) ? m_Turbo : 1.0f;
            else
                moveSpeed *= Input.GetAxis("Fire1") > 0.0f ? m_Turbo : 1.0f;
            transform.position += transform.forward * moveSpeed * inputVertical;
            transform.position += transform.right * moveSpeed * inputHorizontal;
            transform.position += Vector3.up * moveSpeed * inputYAxis;
        }
    }

    void ControlFreeCamState(bool b)
    {
        freeCamOn = b;
        foreach (Camera item in cameras)
        {
            item.enabled = b;
        }
        PlayerController.s_instance.On_PlayerEnterInCinematicState(b);
        Cursor.visible = !b;
        if (b)
        {
            PlayerController.s_instance.m_references.playerCanvasGroupe.alpha = 0;
        }
        else
        {
            PlayerController.s_instance.m_references.playerCanvasGroupe.alpha = 1;
        }

        PlayerController.s_instance.m_references.m_worldCamera.gameObject.SetActive(!b);
        PlayerController.s_instance.m_references.m_gunCamera.gameObject.SetActive(!b);
    }
}

[Serializable] public class MenuFreeCam
{
    public Slider mouseRotationSpeedSlider;
    public Slider controlRotationSpeedSlider;
    [Space]
    public Slider mouseSpeedSlider;
    public Slider mouseSpeedIncrementSlider;
    [Space]
    public Slider turboSlider;
}

[Serializable]
public class OrbitControl
{
    public GameObject objectToRotate;
}
