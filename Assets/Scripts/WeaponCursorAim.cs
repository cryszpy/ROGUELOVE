using UnityEngine;

public class WeaponCursorAim : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    private Camera mainCam;
    
    private Vector3 mousePos;

    void Start() {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void FixedUpdate() {

        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;

            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }
}
