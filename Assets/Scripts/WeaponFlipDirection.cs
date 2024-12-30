using UnityEngine;

public class WeaponFlipDirection : MonoBehaviour
{

    [SerializeField] private Weapon parent;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GameStateManager.GetState() == GAMESTATE.PLAYING) {

            // Flips weapon sprite depending on mouse orientation to character
            if (gameObject.transform.rotation.z > 0.7f || gameObject.transform.rotation.z < -0.7f) {
                
                // Flip weapon sprite
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, -1, gameObject.transform.localScale.z);

                // Flip spawn position sprite (muzzle flash)
                if (parent.spawnPos.TryGetComponent<SpriteRenderer>(out var renderer)) {
                    renderer.flipY = true;
                }
            } else {
                // Flip weapon sprite
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 1, gameObject.transform.localScale.z);

                // Flip spawn position sprite (muzzle flash)
                if (parent.spawnPos.TryGetComponent<SpriteRenderer>(out var renderer)) {
                    renderer.flipY = false;
                }
            }
        }
    }
}
