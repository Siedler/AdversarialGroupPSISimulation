using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleCamera : MonoBehaviour {

    private Camera mainCamera;

    public float zoomSpeed = 20;
    public float targetOrtho;
    public float smoothSpeed = 10.0f;
    public float minOrtho = 2.0f;
    public float maxOrtho = 20.0f;

    public float cameraMovementFactor = 10f;

    private Agent _selectedAgent;

    void Start() {
        mainCamera = gameObject.GetComponent(typeof(Camera)) as Camera;

        targetOrtho = mainCamera.orthographicSize;

        _selectedAgent = null;

        AgentEventManager.current.OnAgentSelected += OnAgentSelected;
        AgentEventManager.current.OnAgentDespawn += OnAgentDespawn;
    }

    // Update is called once per frame
    void Update() {
        if (!IsAnAgentSelected()) {
            float axisX = Input.GetAxis ("Horizontal") * cameraMovementFactor * Time.deltaTime;
            float axisY = Input.GetAxis ("Vertical") * cameraMovementFactor * Time.deltaTime;

            transform.position += new Vector3(axisX, axisY, 0);
        } else {
            transform.position = new Vector3(_selectedAgent.transform.position.x, _selectedAgent.transform.position.y, transform.position.z);
            
            if (Input.GetKeyDown(KeyCode.Escape)) UnselectAgent();
        }

        // Scroll of camera
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        if (scroll != 0.0f) {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
        }

        mainCamera.orthographicSize = Mathf.MoveTowards (Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);

    }

    public void OnAgentSelected(Agent agent) {
        _selectedAgent = agent; 
    }
    
    
    private void OnAgentDespawn(Agent agent) {
        if (_selectedAgent != null && _selectedAgent == agent) {
            UnselectAgent();
        }
    }

    public void UnselectAgent() {
        _selectedAgent = null;
        AgentEventManager.current.DeselectAgent();
    }

    private bool IsAnAgentSelected() {
        return _selectedAgent != null;
    }
}
