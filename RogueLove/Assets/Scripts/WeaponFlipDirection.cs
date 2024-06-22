using UnityEngine;

public class WeaponFlipDirection : MonoBehaviour
{

    [SerializeField] private Weapon parent;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Flips weapon sprite depending on mouse orientation to character
        if (gameObject.transform.rotation.z > 0.7f || gameObject.transform.rotation.z < -0.7f) {
            
            // Flip weapon sprite
            parent.sprite.flipY = true;

            // Flip spawn position sprite (muzzle flash)
            if (parent.spawnPos.TryGetComponent<SpriteRenderer>(out var renderer)) {
                renderer.flipY = true;
            }
        } else {
            // Flip weapon sprite
            parent.sprite.flipY = false;

            // Flip spawn position sprite (muzzle flash)
            if (parent.spawnPos.TryGetComponent<SpriteRenderer>(out var renderer)) {
                renderer.flipY = false;
            }
        }
    }
}
