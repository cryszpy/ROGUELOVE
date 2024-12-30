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
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = mousePos;
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
