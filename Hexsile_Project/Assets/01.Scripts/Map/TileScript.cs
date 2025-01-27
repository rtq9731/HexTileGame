using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TileScript : MonoBehaviour, ITurnFinishObj
{
    TileData data = new TileData();

    public TileData Data
    {
        get { return data; }
        set { data = value; }
    }

    PlayerScript owner = null;

    public PlayerScript Owner
    {
        get { return owner; }
    }

    public void Damage(MissileTypes.MissileWarheadType warhead)
    {
        DoMissileHitAct(warhead, this);
        data.Shield -= MainSceneManager.Instance.GetWarheadData(warhead).Atk;

        if (data.Shield <= 0)
        {
            ChangeOwner(null);
            data.Shield = data.MaxShield;
        }
    }

    public void Damage(int damage)
    {
        if (this.data.type == (TileType.Lake | TileType.Lake) || owner == null)
            return;

        data.Shield -= damage;
        MainSceneManager.Instance.effectPool.PlayEffectOnTile(this);

        if (data.Shield <= 0)
        {
            ChangeOwner(null);
        }
    }

    private void DoMissileHitAct(MissileTypes.MissileWarheadType warhead, TileScript hitTile)
    {
        List<TileScript> tilesInMoreHitRange = MainSceneManager.Instance.tileChecker.FindTilesInRange(hitTile, 1);
        switch (warhead)
        {
            case MissileTypes.MissileWarheadType.WideDamageTypeWarhead1:
                DamageMoreTile(tilesInMoreHitRange, 2, MainSceneManager.Instance.GetWarheadData(warhead).Atk);
                break;
            case MissileTypes.MissileWarheadType.WideDamageTypeWarhead2:
                DamageMoreTile(tilesInMoreHitRange, 4, MainSceneManager.Instance.GetWarheadData(warhead).Atk);
                break;
            case MissileTypes.MissileWarheadType.ContinuousTypeWarhead1:
                ResourceLoss(1, 2);
                break;
            case MissileTypes.MissileWarheadType.ContinuousTypeWarhead2:
                ResourceLoss(3, 3);
                break;
            case MissileTypes.MissileWarheadType.MoreWideTypeWarhead:
                DamageMoreTile(tilesInMoreHitRange, 6, MainSceneManager.Instance.GetWarheadData(warhead).Atk);
                break;
            case MissileTypes.MissileWarheadType.WideContinuousTypeWarhead:
                ResourceLoss(3, 3);
                EffectMoreTile(tilesInMoreHitRange, 3, 2, 3);
                break;
            case MissileTypes.MissileWarheadType.WideDamageTypeWarhead:
                DamageMoreTile(tilesInMoreHitRange, 3, MainSceneManager.Instance.GetWarheadData(warhead).Atk);
                break;
            case MissileTypes.MissileWarheadType.HellFireTypeWarhead:
                DamageMoreTile(tilesInMoreHitRange, 5, MainSceneManager.Instance.GetWarheadData(warhead).Atk);
                EffectMoreTile(tilesInMoreHitRange, 5, 2, 2);
                break;
            default:
                break;
        }
    }

    private void DamageMoreTile(List<TileScript> tilesInMoreHitRange, int moreHitTileCnt, int damage)
    {
        tilesInMoreHitRange = GetTilesCanFire(tilesInMoreHitRange);

        if(tilesInMoreHitRange.Count < 1)
        {
            return;
        }

        for (int i = 0; i < moreHitTileCnt; i++)
        {

            if(tilesInMoreHitRange.Count < 1)
            {
                return;
            }

            TileScript randTile = tilesInMoreHitRange[UnityEngine.Random.Range(0, tilesInMoreHitRange.Count)];

            tilesInMoreHitRange.Remove(randTile); // 중복 타격을 막기 위함
            Debug.Log(randTile.owner);

            randTile.Damage(damage);

        }
    }

    private void EffectMoreTile(List<TileScript> tilesInMoreHitRange, int moreHitTileCnt, int resourceLoss, int turnForFinish)
    {
        tilesInMoreHitRange = GetTilesCanFire(tilesInMoreHitRange);

        if (tilesInMoreHitRange.Count < 1)
        {
            return;
        }

        for (int i = 0; i < moreHitTileCnt; i++)
        {
            if (tilesInMoreHitRange.Count < 1)
            {
                return;
            }

            int a = i;
            TileScript randTile = tilesInMoreHitRange[UnityEngine.Random.Range(0, tilesInMoreHitRange.Count)];

            tilesInMoreHitRange.Remove(randTile); // 이미 적용된 타일을 치움 ( 중복 효과 적용을 막기 위함 )

            ResourceLoss(resourceLoss, turnForFinish);
        }
    }

    private void ResourceLoss(int resourceLoss, int turn)
    {
        this.data.ResourceLoss = resourceLoss;
        this.data.ResourceLossTurn = turn;
    }

    private List<TileScript> GetTilesCanFire(List<TileScript> tiles)
    {
        List<TileScript> results = new List<TileScript>();
        tiles.FindAll(x =>
        {
            if (x.owner == null || // 주인 있슴
            x.owner == MainSceneManager.Instance.GetPlayer() || // 주인이 플레이어가 아님.
            x.data.type == (TileType.Ocean | TileType.Lake)) // 강이나 호수 아님
            {
                return false;
            }
            else
            {
                return true;
            }
        }).ForEach(x => results.Add(x));

        return results;
    }

    public void SetPosition(Vector3 pos)
    {
        data.Position = pos;
        gameObject.transform.position = pos;
    }

    public void ChangeOwner(PlayerScript newOwner)
    {
        if (owner != null)
        {
            owner.TurnFinishAction -= TurnFinish;
        }

        Color tileColor = Color.white;
        if (newOwner != null && newOwner != owner)
        {
            tileColor = newOwner != MainSceneManager.Instance.GetPlayer() ? (newOwner as AIPlayer).Data.PlayerColor : (newOwner as PersonPlayer).PlayerData.PlayerColor;

            MeshRenderer meshBoder = GetComponent<TilePrefabScript>().meshBoder;
            if (meshBoder != null)
            {
                meshBoder.material.color = tileColor;
            }
            owner = newOwner;

            if(!owner.OwningTiles.Contains(this))
            {
                owner.OwningTiles.Add(this);
                owner.TurnFinishAction += TurnFinish;
            }
        }
        else if(newOwner == null)
        {
            if (owner == null)
            {
                return;
            }

            owner.OwningTiles.Remove(this);

            if (owner != MainSceneManager.Instance.GetPlayer()) // 만약 AI라면
            {
                (owner as AIPlayer).CheckIsDie();
            }

            owner = newOwner;

            MeshRenderer meshBoder = GetComponent<TilePrefabScript>().meshBoder;
            if (meshBoder != null)
            {
                meshBoder.material.color = tileColor;
            }

        }

        data.Shield = data.MaxShield;
    }

    public void TurnFinish()
    {
        if(owner == null)
        {
            return;
        }

        if(data.ResourceLossTurn <= 0)
        {
            data.ResourceLoss = 0;
        }

        if(owner.GetType() == typeof(PersonPlayer))
        (owner as PersonPlayer).AddResource(data.Resource - data.ResourceLoss);
    }

    public void BuyTile(PersonPlayer owner)
    {
        if (owner.ResourceTank >= data.Price)
        {

            if (this.owner == owner)
            {
                return;
            }

            owner.AddTile(this);
            owner.AddResource(-data.Price);
            MainSceneManager.Instance.InfoPanel.RefreshTexts(this);
        }
    }

    //public void FireMissile(TileScript attackTile)
    //{
    //    attackTile.Damage(data.attackPower);
    //}

    public void SelectTile(GameObject tileVcam)
    {
        if (data.Position != transform.position)
        {
            return;
        }

        DOTween.Complete(this.transform);
        DOTween.Kill(tileVcam.transform);
        this.transform.DOMoveY(this.transform.position.y + 0.3f, 0.5f);
        transform.DOLocalRotate(new Vector3(0, 360, 0), 0.3f);
        tileVcam.transform.DOMove(new Vector3(transform.position.x + 1, 2.5f, transform.position.z - 1), 0.3f);
        tileVcam.SetActive(true);
    }

    public void RemoveSelect(GameObject tileVcam)
    {
        tileVcam.SetActive(false);
        transform.DOMove(data.Position, 0.3f);
    }

    public bool IsInEffect()
    {
        return data.IsInEffect;
    }
}
