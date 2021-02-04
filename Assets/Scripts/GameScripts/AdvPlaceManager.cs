using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class AdvPlaceManager : MonoBehaviour
{

    [SerializeField] private Transform[] places;
    private List<GameObject> logos = new List<GameObject>();
    private bool isFirstOpen = true;


    void Start()
    {

        this.UpdateAsObservable()//Observable
            .Where(_ => isFirstOpen)//Koşul
            .Subscribe(async(_) =>//Observer
            {
                isFirstOpen = false;
                await AddressablesAssetLoader.InitAsset("Logos", logos);

                if (logos.Count >= places.Length)
                {
                    for (int i = places.Length; i < logos.Count; i++)
                    {
                        AddressablesAssetLoader.ClearAsset(logos[i], logos);
                    }

                    for (int i = 0; i < places.Length; i++)
                    {
                        logos[i].transform.position = places[i].transform.position;
                    }
                }
                else if (places.Length > logos.Count)
                {
                    for (int i = 0; i < logos.Count ; i++)
                    {

                        logos[i].transform.position = places[i].transform.position;
                    }
                }
            });
        
            


    }

}
