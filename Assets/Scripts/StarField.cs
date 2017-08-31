using UnityEngine;
using System.Collections;

public class StarField : MonoBehaviour {

    public float backgroundDistance = 10000;
    public float smallStarsDistance = 5000;
    public float mediumStarsDistance = 2500;
    public float bigStarsDistance = 1000;

    private Material starsMaterial;
    private Camera cam;
    private float lastCameraSize = 0f;
    private float lastCameraAspect = 0f;

    // Use this for initialization
    void Awake() {

        cam = GetComponentInParent<Camera>();
        starsMaterial = GetComponent<MeshRenderer>().material;

        if (!cam || !starsMaterial)
        {
            Debug.Log("Camera or material is not set");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (lastCameraAspect != cam.aspect || lastCameraSize != cam.orthographicSize)
            updateSize();
    }

    void LateUpdate()
    {
        starsMaterial.SetTextureOffset("_Background", new Vector2(cam.transform.position.x, cam.transform.position.y) / backgroundDistance);
        starsMaterial.SetTextureOffset("_SmallStars", new Vector2(cam.transform.position.x, cam.transform.position.y) / smallStarsDistance);
        starsMaterial.SetTextureOffset("_MediumStars", new Vector2(cam.transform.position.x, cam.transform.position.y) / mediumStarsDistance);
        starsMaterial.SetTextureOffset("_BigStars", new Vector2(cam.transform.position.x, cam.transform.position.y) / bigStarsDistance);
    }

    private void updateSize()
    {
        lastCameraAspect = cam.aspect;
        lastCameraSize = cam.orthographicSize;

        if (cam.aspect > 1)
            transform.localScale = new Vector3(lastCameraSize * cam.aspect, lastCameraSize * cam.aspect, 1);
        else
            transform.localScale = new Vector3(lastCameraSize, lastCameraSize, 1);
    }
}
