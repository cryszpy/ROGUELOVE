using UnityEngine;

public class yeah : MonoBehaviour
{
    private void Start() {
        TransitionManager.EndLeaf(true);

        Cursor.visible = true;
    }
}
