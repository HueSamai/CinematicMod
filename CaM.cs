using UnityEngine;
using UnityEngine.InputSystem;
using InputModule;
using CementTools;
using System.Collections;
using UnityEngine.SceneManagement;
using Femur;

// Token: 0x02000005 RID: 5
public class CinematicMod : CementMod
{
    private bool locked;
    private GameObject virtualCamera;

    private GameObject UI;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    private GameObject camera;

    private bool camenabler;

    private float speed = 25f;

    private float mouseSensitivity = 10f;

    private bool usingCustomCam;

    public CinematicKeybindManager keybindManager;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnload;
        StartCoroutine(WaitToStart());
    }


    public IEnumerator WaitToStart()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Menu");
        camera = Camera.main.gameObject;
        keybindManager = new CinematicKeybindManager(modFile);
        InputManager.onInputHeld(keybindManager.GetForwardKeyPath()).bind(MoveForward);
        InputManager.onInputHeld(keybindManager.GetBackwardKeyPath()).bind(MoveBackward);
        InputManager.onInputHeld(keybindManager.GetRightKeyPath()).bind(MoveRight);
        InputManager.onInputHeld(keybindManager.GetLeftKeyPath()).bind(MoveLeft);
        InputManager.onInputHeld(keybindManager.GetUpKeyPath()).bind(MoveUp);
        InputManager.onInputHeld(keybindManager.GetDownKeyPath()).bind(MoveDown);
        InputManager.onInput(keybindManager.GetToggleCamKeyPath()).bind(ToggleCam);
        InputManager.onInput(keybindManager.GetLockCamKeyPath()).bind(ToggleLock);
        InputManager.onInput(keybindManager.GetToggleKillVolumes()).bind(
            CementModSingleton.Get<RemoveKillVolume>().ToggleKillVolumes
        );
    }

    private void OnSceneUnload(Scene _)
    {
        cameraPosition = camera.transform.position;
        cameraRotation = camera.transform.rotation;
    }

    private void OnSceneChanged(Scene _, LoadSceneMode __)
    {
        StartCoroutine(WaitToAdjustCamera());
    }

    private IEnumerator WaitToAdjustCamera()
    {
        yield return new WaitUntil(() => virtualCamera != null);
        yield return new WaitUntil(() => UI != null);
        if (usingCustomCam)
        {
            EnableCustomCamera();
        }
    }

    private void EnableCustomCamera()
    {
        virtualCamera.SetActive(false);
        UI.SetActive(false);
        camera.transform.position = cameraPosition;
        camera.transform.rotation = cameraRotation;
    }

    private void DisableCustomCamera()
    {
        virtualCamera.SetActive(true);
        UI.SetActive(true);
        cameraPosition = camera.transform.position;
        cameraRotation = camera.transform.rotation;
    }

    private void MoveForward(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position += speed * Time.deltaTime * camera.transform.forward;
    }

    private void MoveUp(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position += speed * Time.deltaTime * camera.transform.up;
    }

    private void MoveDown(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position -= speed * Time.deltaTime * camera.transform.up;
    }

    private void MoveBackward(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position -= speed * Time.deltaTime * camera.transform.forward;
    }

    private void MoveRight(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position += speed * Time.deltaTime * camera.transform.right;
    }

    private void MoveLeft(Actor _)
    {
        if (!usingCustomCam || locked)
        {
            return;
        }

        camera.transform.position -= speed * Time.deltaTime * camera.transform.right;
    }

    private void ToggleLock(Actor _)
    {
        locked = !locked;
    }

    private void ToggleCam(Actor _)
    {
        usingCustomCam = !usingCustomCam;
        if (usingCustomCam)
        {
            EnableCustomCamera();
        }
        else
        {
            DisableCustomCamera();
        }   
    }

    public void Update()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GameObject.Find("VirtualCamera");
        }

        if (UI == null)
        {
            UI = GameObject.Find("UI");
        }

        if (!usingCustomCam || locked)
        {
            return;
        }

        if (Mouse.current.rightButton.isPressed)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float mouseY = -Mouse.current.delta.y.ReadValue();

            float newYRot = camera.transform.eulerAngles.y + mouseX * mouseSensitivity * Time.deltaTime;
            float xRot = camera.transform.eulerAngles.x;
            if (xRot > 180)
            {
                xRot -= 360;
            }

            float newXRot = Mathf.Clamp(xRot + mouseY * mouseSensitivity * Time.deltaTime, -90f, 90f);

            camera.transform.eulerAngles = new Vector3(newXRot, newYRot, 0);
        }
    }

    private void LateUpdate()
    {
        if (usingCustomCam)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    
}