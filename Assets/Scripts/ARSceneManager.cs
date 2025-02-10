using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARSceneManager : MonoBehaviour
{
    public GameObject ObjectPrefab;
    public GameObject SpawnContainer;
    public ARCameraBackground CameraBackground;
    public Image ARToogleImage;
    private bool enableCamera = true;
    public GameManager GameManager;

    private GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private Vector2 touchPosition;

    // Pitch zoom
    private Vector2 firstTouch;
    private Vector2 secondTouch;
    private float currentDistance;
    private float previousDistance;
    private bool firstPitch = true;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Tap to spawn object
        if (Input.touchCount > 0 && spawnedObject == null)
        {
            if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitpose = hits[0].pose;
                Vector3 fieldPosition = hitpose.position;
                fieldPosition.y += 0.1f;
                spawnedObject = Instantiate(ObjectPrefab, fieldPosition, hitpose.rotation);
                spawnedObject.transform.SetParent(SpawnContainer.transform);
                raycastManager.enabled = false;
                planeManager.enabled = false;

                Transform fieldA = spawnedObject.transform.Find("Team A");
                Transform fieldB = spawnedObject.transform.Find("Team B");
                if (fieldA != null)
                {
                    GameManager.Goal_A = fieldA.transform.Find("Goal").gameObject;
                }
                if (fieldB != null)
                {
                    GameManager.Goal_B = fieldB.transform.Find("Goal").gameObject;
                }
                GameManager.objectContainer = spawnedObject;
                GameManager.fieldScalePercentage = spawnedObject.transform.localScale.x;
            }
        }

        // Pitch to zoom object scale
        if (Input.touchCount > 1 && spawnedObject != null)
        {
            firstTouch = Input.GetTouch(0).position;
            secondTouch = Input.GetTouch(1).position;
            currentDistance = secondTouch.magnitude-firstTouch.magnitude;
            if (firstPitch)
            {
                previousDistance = currentDistance;
                firstPitch = false;
            }
            if (currentDistance != previousDistance)
            {
                Vector3 scaleValue = spawnedObject.transform.localScale * (currentDistance / previousDistance);
                spawnedObject.transform.localScale = scaleValue;
                previousDistance = currentDistance;
                GameManager.fieldScalePercentage = spawnedObject.transform.localScale.x / GameManager.fieldScalePercentage;
            }
        }
        else
        {
            firstPitch = true;
        }

        // Swipe to rotate object
        if (Input.touchCount == 1 && spawnedObject != null)
        {
            Touch touch = Input.GetTouch(0);

            // Only detect horizontal swipe (movement in x-axis)
            if (touch.phase == TouchPhase.Moved)
            {
                float rotationSpeed = 0.2f; // You can adjust the rotation speed to your preference
                float deltaX = touch.deltaPosition.x;

                // Rotate the object based on the swipe direction (deltaX)
                spawnedObject.transform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
            }
        }
    }

    public void ToggleBackground()
    {
        Debug.Log("Toogle : "+enableCamera);
        if (enableCamera)
        {
            enableCamera = false;
            ARToogleImage.rectTransform.localPosition = new Vector3(20, 0, 0);
        }
        else
        {
            enableCamera = true;
            ARToogleImage.rectTransform.localPosition = new Vector3(-20, 0, 0);
        }

        if (CameraBackground != null)
        {
            CameraBackground.enabled = enableCamera;
        }
        GameManager.audioManager.PlaySFX("click");
    }
}
