using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Doorway : MonoBehaviour
{
    private WalkerGenerator map;

    [SerializeField] private CinemachineVirtualCamera cam;
    
    public Transform cameraLookAt;

    [SerializeField] private Animator animator;

    [SerializeField] private SceneInfo sceneInfo;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        StartCoroutine(CameraSway());
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        // If collided with the player, start next level
        if (collider.CompareTag("Player")) {

            // If there are unopened or missed chests in completed level—
            if (map.spawnedChests.Count > 0) {

                // Increase chance to spawn big chest
                foreach (var chest in map.spawnedChests) {
                    PlayerController.AddBigChestChance(0.2f);
                }
            }

            // Start first level if coming out of tutorial
            if (GameStateManager.tutorialEnabled) {
                GameStateManager.tutorialEnabled = false;
                TransitionManager.StartLeaf(1 + sceneInfo.sceneOffset);
            } 
            // Next level
            else {
                GameStateManager.NextLevel();
            }
        } 
    }

    private IEnumerator CameraSway() {
        var gameState = GameStateManager.GetState();
        GameStateManager.SetState(GAMESTATE.MENU);
        cam.Follow = this.transform;

        yield return new WaitForSeconds(3.5f);
        
        if (!GameStateManager.tutorialEnabled) {
            cam.Follow = cameraLookAt;
        }
        WalkerGenerator.doneWithLevel = true;
        GameStateManager.SetState(gameState);

        GameStateManager.EOnDoorwaySpawn?.Invoke();
    }

    public void TriggerAnimOpen() {
        animator.SetTrigger("Open");
    }

    public UnityEngine.Object Create(UnityEngine.Object original, Vector3 position, Quaternion rotation, GameObject player, WalkerGenerator gen) {
        GameObject door = Instantiate(original, position, rotation) as GameObject;
        
        if (door.TryGetComponent<Doorway>(out var doorway)) {
            doorway.map = gen;
            doorway.cameraLookAt = player.transform;
            Debug.Log("Doorway Spawned!");
            return door;
        } else {
            Debug.LogError("Could not find Doorway script or extension of such on this Object.");
            return null;
        }
    }
}
