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
            
            mousePos = GameStateManager.ToWorldPoint(Input.mousePosition, mainCam);

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
}
