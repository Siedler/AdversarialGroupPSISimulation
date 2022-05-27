using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera : MonoBehaviour {

    private Camera mainCamera;

    public float zoomSpeed = 20;
    public float targetOrtho;
    public float smoothSpeed = 10.0f;
    public float minOrtho = 2.0f;
    public float maxOrtho = 20.0f;

    public float camerMovementFactor = 10f;

    void Start() {
        mainCamera = gameObject.GetComponent(typeof(Camera)) as Camera;

        targetOrtho = mainCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update() {
        float axisX = Input.GetAxis ("Horizontal") * camerMovementFactor * Time.deltaTime;
        float axisY = Input.GetAxis ("Vertical") * camerMovementFactor * Time.deltaTime;
    
        transform.position += new Vector3(axisX, axisY, 0);
    
        // Scroll of camera
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        if (scroll != 0.0f) {
             targetOrtho -= scroll * zoomSpeed;
             targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
        }

        mainCamera.orthographicSize = Mathf.MoveTowards (Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
    }
}
