using UnityEngine;
using System.Collections;

public class KeepScreenSize : MonoBehaviour
{

    private float sizeModifier, cameraDistance;
    private RectTransform canvasTransform;
    private Vector3 tmpLocalScale = new Vector3();

    // Use this for initialization
    void Start()
    {
        canvasTransform = GetComponent<RectTransform>();
        // get distance between camera and 0,0,0. Use this info to calculate size modifier.
        cameraDistance = Vector3.Distance(Camera.main.transform.position, Vector3.zero);
        sizeModifier = cameraDistance / canvasTransform.localScale.x;
    }

    // Now that we have the modifier, use varying camera distance to calculate the size of canvas
    void LateUpdate()
    {
        cameraDistance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        tmpLocalScale.x = cameraDistance / sizeModifier;
        tmpLocalScale.y = tmpLocalScale.x;
        tmpLocalScale.z = tmpLocalScale.x;

        // Apply new scale
        canvasTransform.localScale = tmpLocalScale;
        // rotate canvas to be perpendicular to camera regardless of camera rotation.
        // Notice we don't use LookAt(camera). This would mirror the canvas.
        canvasTransform.rotation = Camera.main.transform.rotation;
    }
}