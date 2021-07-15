using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    [SerializeField] Vector2 rotationSpeed;
    public bool reverse;

    private Camera mainCamera;
    private Vector2 lastMousePosition;

    private Vector3 lastTargetPosition;
    private Vector3 toCameraPos = new Vector3(0.0f, 1.0f, 0.0f);
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        mainCamera.transform.position += playerObject.transform.position - lastTargetPosition;
        lastTargetPosition = playerObject.transform.position;

        //if (Input.GetMouseButtonDown(0))
        {
            //lastMousePosition = Input.mousePosition;
        }
        //else if (Input.GetMouseButton(0))
        {
            //float x = Input.GetAxis("Mouse X");
            //float y = -Input.GetAxis("Mouse Y");
            float x = Input.GetAxis("Horizontal2"); 
            float y = Input.GetAxis("Vertical2");
            if (reverse == true)
            {
                x = -x;
                //y = y;
            }
            else if (reverse == false)
            {
                //x = x;
                y = -y;
            }
            var newAngle = Vector3.zero;
            newAngle.x = x * rotationSpeed.x;
            newAngle.y = y * rotationSpeed.y;

            mainCamera.transform.RotateAround(playerObject.transform.position + toCameraPos, Vector3.up, newAngle.x);
            mainCamera.transform.RotateAround(playerObject.transform.position + toCameraPos, transform.right, newAngle.y);
            lastMousePosition = Input.mousePosition;
        }
    }
}
