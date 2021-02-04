using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressablesAssetLoader
{
    //Asenkron olarak oluşturulmuş bir methodtur.
    public static async Task InitAsset<T>(string assetNameOrLabel, List<T> createdObjs)
        where T : Object
    {
        //Addressables varlıkların lokasyonları belirlenir.
        var locations = await Addressables.LoadResourceLocationsAsync(assetNameOrLabel).Task;
        foreach (var location in locations)
        {
            //Bu lokasyonlar ile varlıklarımız oluşturulur.
            createdObjs.Add(await Addressables.InstantiateAsync(location).Task as T);
        } 
    }

    public static void ClearAsset<T>(T go, List<T> Assets)
    {
        Addressables.Release(go);
        Assets.Remove(go);
    }

    public static void ClearAsset<T>(List<T> AssetList)
    {
        foreach (var asset in AssetList)
        {
            Addressables.Release(asset);
        }
        AssetList.Clear();
    }


}
