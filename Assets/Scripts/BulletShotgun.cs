using System.Collections;
using UnityEngine;

public class BulletShotgun : BulletScript
{
    [SerializeField] protected float velocityReduction;

    public override void Start() {
        base.Start();

        StartCoroutine(BulletStall());
        Debug.Log(rb.linearVelocity.magnitude);
    }

    public virtual IEnumerator BulletStall() {
        bool checker = false;

        while (!checker) {

            if (Mathf.Abs(rb.linearVelocity.x) <= 0.05f && Mathf.Abs(rb.linearVelocity.y) <= 0.05f) {
                checker = true;
                break;
            }

            rb.linearVelocity *= 1 - velocityReduction;

            yield return new WaitForSeconds(.02f);
        }

        rb.linearVelocity = Vector2.zero;

        DestroyBullet();
    }
}
