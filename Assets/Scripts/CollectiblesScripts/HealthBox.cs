using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class HealthBox : MonoBehaviour
{
    public SpawnManager spawnManager;

    [SerializeField] private int increaseHealthValue;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float coolDown;

    Collider2D playerCollider;
    GameObject player;
    ReactiveProperty<bool> isPlayer = new ReactiveProperty<bool>(false);

    private void Start()
    {

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                playerCollider = Physics2D.OverlapBox(transform.position, new Vector2(0.35f, 0.45f), playerLayer);
                if (playerCollider.gameObject.tag=="Player")
                {
                    isPlayer.Value = true;
                    player = playerCollider.gameObject;
                    player.gameObject.GetComponent<SwordManHealth>().Health.Value += increaseHealthValue;
                    AddressablesAssetLoader.ClearAsset(this.gameObject, spawnManager.healthGameObjects);
                }                    
                else
                    isPlayer.Value = false;
            });

    }


}
