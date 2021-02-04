using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading;

public class SwordManController : MonoBehaviour
{
    [SerializeField] private float runSpeed=0, jumpForce=0;
    private float jumpHeight = 0.4f;
    private float moveInput;

    private bool facingRight=true;

    [SerializeField] private Vector3 range;

    [SerializeField] private Animator anim = null;
    [SerializeField] private Rigidbody2D myBody=null;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        this.FixedUpdateAsObservable()
            .Where(_ => myBody.bodyType == RigidbodyType2D.Dynamic)
            .Subscribe(_ =>
            {
                moveInput = Input.GetAxisRaw("Horizontal") * runSpeed;
                anim.SetFloat("Speed", Mathf.Abs(moveInput));
                myBody.velocity = new Vector2(moveInput, myBody.velocity.y);

                if (moveInput > 0 && !facingRight || moveInput < 0 && facingRight)
                    Flip();

            });

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_=>
            {
                Collider2D bottomHit = Physics2D.OverlapBox(groundCheck.position, range, 0, groundLayer);

                if (myBody.velocity.y > 0)
                {
                    myBody.velocity = new Vector2(myBody.velocity.x, myBody.velocity.y * jumpHeight);
                }

                if (bottomHit == null)
                    return;

                if (bottomHit.gameObject.tag == "Ground")
                {
                    myBody.velocity = new Vector2(myBody.velocity.x, jumpForce);
                    anim.SetBool("Jump", true);
                }

            });

        this.OnCollisionEnter2DAsObservable()
            .Where(x => x.gameObject.tag == "Ground")
            .Subscribe(_ => anim.SetBool("Jump", false) );
            
    }


    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }


}
