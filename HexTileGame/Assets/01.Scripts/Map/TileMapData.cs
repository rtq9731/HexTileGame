using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileMapData : MonoBehaviour
{
    private static TileMapData instance;

    [SerializeField]
    private List<TileScript> tileList = new List<TileScript>();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static TileMapData Instance
    {
        get { return instance; }
    }

    public void ResetTileList()
    {
        tileList = new List<TileScript>();
    }

    public TileScript GetTile(int num)
    {
        return tileList[num];
    }

    public void RemoveTileOnList(TileScript item)
    {
        tileList.Remove(item);
    }

    public void SetTileData(TileScript item)
    {
        tileList.Add(item);
    }

    public List<TileScript> GetAllTiles()
    {
        return tileList;
    }

    public void ResetColorAllTile()
    {
        foreach (var item in tileList)
        {
            item.GetComponent<MeshRenderer>().material.color = item.Owner != null ? item.Owner.playerColor : Color.white;
        }
    }

    public List<TileScript> GetEndTile(int size)
    {
        List<TileScript> result = new List<TileScript>();
        for (int i = 0; i < size; i++) 
        {
            TileScript tmp = tileList.Find(x =>
            !result.Contains(x)
            && MainSceneManager.Instance.tileChecker.FindTilesInRange(x, 1).Count <= 3);

            if(tmp != null)
            {
                result.Add(tmp);
            }
        }

        return result;
    }
}
