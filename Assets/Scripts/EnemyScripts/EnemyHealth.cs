using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public SpawnManager spawnManager;

    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D myBody;
    [SerializeField] Collider2D collider2d;

    [SerializeField] private int startHealthValue = 100;
    //UniRX Variable this variable behave Observable
    private ReactiveProperty<int> health = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> Health { get { return health; } set { health = value; } }

    public ReactiveProperty<bool> isDead = new ReactiveProperty<bool>(false);


    void Start()
    {
        health.Value = startHealthValue;

        this.health//Observable
            .Where(x => x <= 0)//Koşul
            .Subscribe(_ =>//Observer
            {
                anim.SetBool("Death", true);
                myBody.velocity = Vector2.zero;
                collider2d.enabled = false;
                myBody.isKinematic = true;
                isDead.Value = true;
            });

        this.isDead//Observable
            .Where(x => x)//Koşul
            .Delay(TimeSpan.FromSeconds(2f))//Gecikme
            .Subscribe(_ =>//Observer
            {
                AddressablesAssetLoader.ClearAsset(this.gameObject, spawnManager.enemyGameObjects);
            });

    }

}
