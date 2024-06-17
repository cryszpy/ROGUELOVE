using UnityEngine;

public class WeaponCursorAim : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public Camera mainCam;
    
    public Vector3 mousePos;

    void Start() {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void FixedUpdate() {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
