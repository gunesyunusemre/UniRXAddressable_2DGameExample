using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region UniRX
using UniRx;
using UniRx.Triggers;
#endregion

public class SpawnManager : MonoBehaviour
{
    #region Variable

    #region Enemy Variable
    public Transform[] enemySpawnPoints;
    public List<GameObject> enemyGameObjects = new List<GameObject>();
    ReactiveProperty<bool> isSpawnEnemy = new ReactiveProperty<bool>(true);
    #endregion

    #region HealthObj Variable
    [SerializeField] private Transform[] healthSpawnPoints;
    public List<GameObject> healthGameObjects = new List<GameObject>();
    ReactiveProperty<bool> isSpawn = new ReactiveProperty<bool>(true);
    #endregion

    #region Player Variable
    public Transform playerSpawnPoint;
    public List<GameObject> player = new List<GameObject>();
    public bool isAlivePlayer = false;
    #endregion

    #endregion

    private void Start()
    {
        #region Spawn Player
        this.UpdateAsObservable()//Observable
            .Where(_ => player.Count <= 0)//Koşul
            .Subscribe(async (_) => //Observer
            {
                if (!isAlivePlayer)
                {
                    isAlivePlayer = true;
                    await AddressablesAssetLoader.InitAsset("SwordMan", player);                    
                }                
            });

        this.FixedUpdateAsObservable() //Observable
            .Where(_ => player !=null) //Koşul 1
            .Where(_ => player.Count != 0)// Koşul 2
            .Subscribe(_ => //Observer
            {

                if (player[0].GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Dynamic)
                {
                    player[0].transform.position = playerSpawnPoint.position;
                    player[0].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    player[0].GetComponent<SwordManHealth>().spawnManager = this;
                }
            });
        #endregion -------------------------------------------------------------------------------

        #region HealthObjSpawn
        this.FixedUpdateAsObservable()
            .Where(_ => healthGameObjects.Count < healthSpawnPoints.Length)
            .Subscribe((_) =>
            {
                isSpawn.Value = true;
            });

        this.FixedUpdateAsObservable()
            .Where(_ => healthGameObjects.Count >= healthSpawnPoints.Length)
            .Subscribe((_) =>
            {
                isSpawn.Value = false;
            });

        this.isSpawn
            .Where(x => x)
            .Delay(TimeSpan.FromSeconds(1f))
            .Subscribe(async(_) =>
            {                
                await AddressablesAssetLoader.InitAsset("Health", healthGameObjects);
                int i = UnityEngine.Random.Range(0, healthSpawnPoints.Length);
                healthGameObjects[healthGameObjects.Count-1].transform.position = healthSpawnPoints[i].position;
                healthGameObjects[healthGameObjects.Count-1].GetComponent<HealthBox>().spawnManager = this;
                isSpawn.Value = false;
            });
        #endregion---------------------------------------------------

        #region EnemiesSpawn
        this.FixedUpdateAsObservable()
            .Where(_ => enemyGameObjects.Count < enemySpawnPoints.Length)
            .Subscribe((_) =>
            {
                isSpawnEnemy.Value = true;
            });

        this.FixedUpdateAsObservable()
            .Where(_ => enemyGameObjects.Count >= enemySpawnPoints.Length)
            .Subscribe((_) =>
            {
                isSpawnEnemy.Value = false;
            });

        this.isSpawnEnemy
            .Where(x => x)
            .Delay(TimeSpan.FromSeconds(1f))
            .Subscribe(async (_) =>
            {
                await AddressablesAssetLoader.InitAsset("Enemy1", enemyGameObjects);
                int i = UnityEngine.Random.Range(0, enemySpawnPoints.Length);
                enemyGameObjects[enemyGameObjects.Count - 1].transform.position = enemySpawnPoints[i].position;
                //enemyGameObjects[enemyGameObjects.Count - 1].GetComponent<EnemyHealth>().enabled = true;
                enemyGameObjects[enemyGameObjects.Count - 1].GetComponent<EnemyHealth>().spawnManager = this;
                isSpawnEnemy.Value = false;
            });
        #endregion---------------------------------------------------

    }

}
