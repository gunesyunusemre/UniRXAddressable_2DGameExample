using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SwordManHealth : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D myBody;
    [SerializeField] Collider2D collider2d;
    public SpawnManager spawnManager;

    [SerializeField] private int startHealthValue = 100;
    private ReactiveProperty<int> health = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> Health { get { return health; } set { health = value; } }


    void Start()
    {
        health.Value = startHealthValue;

        this.health
            .Where(x => x<=0 )
            .Subscribe(_ =>
            {
                Debug.Log("I am Dead");
                anim.SetBool("Death", true);
                myBody.velocity = Vector2.zero;
                collider2d.enabled = false;
                myBody.isKinematic = true;
                spawnManager.player.Clear();
                spawnManager.isAlivePlayer = false;
                Destroy(gameObject);
            });

    }




}
