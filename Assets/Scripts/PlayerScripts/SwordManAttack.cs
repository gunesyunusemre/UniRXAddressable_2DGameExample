using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SwordManAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float attackRange;
    [SerializeField] private int damage;

    ReactiveProperty<int> killCount = new ReactiveProperty<int>(0);//UniRX variable
    [SerializeField] private int goalKillCount;

    private void Start()
    {
        this.UpdateAsObservable()//Observable
            .Where(_ => Input.GetKeyDown(KeyCode.J))//Koşul
            .Subscribe(_=>//Observer
            {
                anim.SetBool("SwordAttack", true);

                Collider2D[] attackEnemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemyLayer);

                foreach (var enemy in attackEnemies)
                {
                    enemy.GetComponent<EnemyHealth>().Health.Value -= damage;
                    enemy.GetComponent<Animator>().SetTrigger("Hit");

                    if (enemy.GetComponent<EnemyHealth>().Health.Value <= 0)
                    {
                        killCount.Value++;
                    }
                }

            });


        this.killCount//Observable
            .Where(x => x >= 3)//Koşul
            .Subscribe(_ => Addressables.LoadSceneAsync("NextScene"));//Observer


    }

    private void SwordAttack()
    {
        anim.SetBool("SwordAttack", false);
    }
}
