using UnityEngine;

public class WeaponFlipDirection : MonoBehaviour
{

    [SerializeField] private Weapon parent;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Flips weapon sprite depending on mouse orientation to character
        if (gameObject.transform.rotation.z > 0.7f || gameObject.transform.rotation.z < -0.7f) {
            parent.sprite.flipY = true;
        } else {
            parent.sprite.flipY = false;
        }
    }
}
