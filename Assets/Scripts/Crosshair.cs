using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    [SerializeField] private Camera mainCam;

    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite empty;

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnEnable() {
        GameStateManager.EOnGamestateChange += CheckVisibility;
    }

    private void OnDisable() {
        GameStateManager.EOnGamestateChange -= CheckVisibility;
    }

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;

        if (!mainCam) {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = ToWorldPoint(Input.mousePosition);
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

    private void CheckVisibility() {

        switch (GameStateManager.GetState()) {
            case GAMESTATE.MAINMENU:
                spriteRenderer.sprite = empty;
                break;
            case GAMESTATE.MENU:
                spriteRenderer.sprite = empty;
                break;
            case GAMESTATE.PLAYING:
                spriteRenderer.sprite = normal;
                break;
            case GAMESTATE.PAUSED:
                spriteRenderer.sprite = empty;
                break;
            case GAMESTATE.GAMEOVER:
                spriteRenderer.sprite = empty;
                break;    
        }
    }
}
