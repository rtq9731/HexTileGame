using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, ITurnFinishObj
{
    protected Action start = () => { };

    protected List<TileScript> tileInSight = new List<TileScript>();

    protected List<int> unlockedWarheadIdx = new List<int>();
    public List<int> UnlockedWarheadIdx
    {
        get { return unlockedWarheadIdx; }
    }

    protected List<int> unlockedEngineIdx = new List<int>();
    public List<int> UnlockedEngineIdx
    {
        get { return unlockedEngineIdx; }
    }

    [SerializeField] protected string myName = "NULL";
    public Color playerColor;

    public string MyName
    {
        get { return myName; }
    }

    public Action TurnFinishAction;

    protected List<TileScript> owningTiles = new List<TileScript>();
    public List<TileScript> OwningTiles
    {
        get { return owningTiles; }
    }

    protected List<MissileData> missileInMaking = new List<MissileData>();
    public  List<MissileData> MissileInMaking
    {
        get { return missileInMaking; }
    }

    protected List<MissileData> missileReadyToShoot = new List<MissileData>();
    public List<MissileData> MissileReadyToShoot
    {
        get { return missileReadyToShoot; }
    }

    protected bool isTurnFinish = false;
    public bool IsTurnFinish { get { return isTurnFinish; } }

    protected int resouceTank = 0;
    public int ResourceTank { get { return resouceTank; } }

    protected void Awake()
    {
        TurnFinishAction = () => { }; // �׼� �ʱ�ȭ
        TurnFinishAction += () => {
            missileInMaking.ForEach(x => x.TurnForMissileReady--);
            missileInMaking.FindAll(x => x.TurnForMissileReady <= 0).ForEach(x => {
                missileInMaking.Remove(x);
                missileReadyToShoot.Add(x);
                });
            };
    }

    protected void Start()
    {
        for (int i = 0; i < 11; i++)
        {
            unlockedWarheadIdx.Add(i);
        }

        for (int i = 0; i < 5; i++)
        {
            unlockedEngineIdx.Add(i);
        }

        MainSceneManager.Instance.Players.Add(this);
        MainSceneManager.Instance.uiTopBar.UpdateTexts();
    }

    public void StartNewTurn()
    {
        isTurnFinish = false;
    }

    public void AddResource(int resource)
    {
        resouceTank += resource;
        MainSceneManager.Instance.uiTopBar.UpdateTexts();
    }

    public void TurnFinish()
    {
        if (isTurnFinish)
        {
            return;
        }
        else
        {
            isTurnFinish = true;

            TurnFinishAction();
            owningTiles.ForEach(x => x.TurnFinish());
            MainSceneManager.Instance.CheckTurnFinish();
        }
    }

    public void AddTile(TileScript tile)
    {
        if(tile.Owner != this)
        {
            tile.ChangeOwner(this);
        }

        if (MainSceneManager.Instance.GetPlayer() == this)
        {
            MainSceneManager.Instance.fogOfWarManager.RemoveCloudOnTile(tile);
            MainSceneManager.Instance.tileChecker.FindTilesInRange(tile, 1).ForEach(x => MainSceneManager.Instance.fogOfWarManager.RemoveCloudOnTile(x));
        }

        // �ߺ� �ȵǰ� �ϱ� ����.
        owningTiles.Remove(tile);
        owningTiles.Add(tile);
    }
}