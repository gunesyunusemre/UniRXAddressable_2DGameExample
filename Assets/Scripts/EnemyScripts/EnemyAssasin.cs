using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnemyAssasin : MonoBehaviour
{
    [Header("My Element")]
    [SerializeField] private Rigidbody2D myBody;
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyHealth myHealthScript;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float minX, maxX;
    [SerializeField] private float distance;
    [SerializeField] private int direction;
    private bool patrol;
    private Transform playerPos = null;

    [Header("Attack")]
    [SerializeField] private Transform attackPos;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int damage;
    private bool detect;


    void Start()
    {
        maxX = transform.position.x + (distance / 2);
        minX = maxX - distance;

        this.UpdateAsObservable()
            .Where(_ => playerPos==null)
            .Where(_ => GameObject.FindGameObjectWithTag("Player") != null)
            .Subscribe(_ => playerPos = GameObject.FindGameObjectWithTag("Player").transform);


        this.UpdateAsObservable()
            .Where(_ => GameObject.FindGameObjectWithTag("Player") != null)
            .Where(_ => Vector3.Distance(transform.position, playerPos.position) <= 3f)
            .Subscribe(_=>
            {
                patrol = false;
                anim.SetBool("Patrol", false);
            });

        this.UpdateAsObservable()
            .Where(_ => GameObject.FindGameObjectWithTag("Player") != null)
            .Where(_ => Vector3.Distance(transform.position, playerPos.position) > 3f)
            .Subscribe(_ =>
            {
                patrol = true;
                anim.SetBool("Patrol", true);
            });

        this.FixedUpdateAsObservable()
            .Where(_ => !myHealthScript.isDead.Value)
            .Where(_ => patrol)
            .Subscribe(_ =>
            {
                detect = false;
                anim.SetBool("Attack", false);
                transform.localScale = new Vector2(direction*0.15f,transform.localScale.y);

                switch (direction)
                {
                    case -1:
                        if (transform.position.x > minX)
                            myBody.velocity = new Vector2(-moveSpeed, myBody.velocity.y);
                        else
                            direction = 1;
                        break;
                    case 1:
                        if (transform.position.x < maxX)
                            myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);
                        else
                            direction = -1;
                        break;
                }
            });

        this.FixedUpdateAsObservable()
            .Where(_ => GameObject.FindGameObjectWithTag("Player") != null)
            .Where(_ => playerPos != null)
            .Where(_ => !patrol)
            .Subscribe(_ => 
            {
                if (Vector2.Distance(playerPos.position, transform.position) >= 0.35f)
                {
                    anim.SetBool("Attack", false);
                    if (!detect)
                    {
                        detect = true;
                        anim.SetTrigger("Detect");
                    }

                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Rogue_run_01"))
                        return;

                    Vector3 playerDir = (playerPos.position - transform.position).normalized;

                    if (playerDir.x > 0)
                    {
                        myBody.velocity = new Vector2(moveSpeed + 0.4f, myBody.velocity.y);
                        transform.localScale = new Vector2(1* 0.15f, transform.localScale.y);
                    }                        
                    else
                    {
                        myBody.velocity = new Vector2(-1 * (moveSpeed + 0.4f), myBody.velocity.y);
                        transform.localScale = new Vector2(-1 * 0.15f, transform.localScale.y);
                    }
                        

                }
                else if (Vector2.Distance(playerPos.position, transform.position) < 0.35f)
                {
                    myBody.velocity = new Vector2(0, myBody.velocity.y);
                    anim.SetBool("Attack", true);
                    detect = false;
                }
            });

    }

    public void Attack()
    {
        Collider2D attackPlayer = Physics2D.OverlapCircle(attackPos.position, attackRange, playerLayer);
        if (attackPlayer != null)
        {
            if (attackPlayer.tag == "Player")
            {
                attackPlayer.GetComponent<SwordManHealth>().Health.Value -= damage;
            }
        }
    }

}
