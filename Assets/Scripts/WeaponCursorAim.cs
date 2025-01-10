using UnityEngine;

public class WeaponCursorAim : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    private Camera mainCam;
    
    private Vector3 mousePos;

    [SerializeField] private Weapon parent;

    void Start() {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public virtual void FixedUpdate() {

        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {
            
            mousePos = ToWorldPoint(Input.mousePosition);

            Vector3 rotation = mousePos - transform.position;

            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, rotZ);

            /* if (rotation.y > 0) {
                parent.spriteRenderer.sortingOrder = -1;
            } else {
                parent.spriteRenderer.sortingOrder = 1;
            } */
        }
    }

    private Vector2 ToWorldPoint(Vector3 input) {

        Vector2 inCamera;
        Vector2 pixelAmount;
        Vector2 worldPoint;

        inCamera.y = mainCam.orthographicSize * 2;
        inCamera.x = inCamera.y * Screen.width / Screen.height;

        pixelAmount.x = Screen.width / inCamera.x;
        pixelAmount.y = Screen.height / inCamera.y;

        worldPoint.x = ((input.x / pixelAmount.x) - (inCamera.x / 2) + mainCam.transform.position.x);
        worldPoint.y = ((input.y / pixelAmount.y) - (inCamera.y / 2) + mainCam.transform.position.y);

        return worldPoint;
    }
}
