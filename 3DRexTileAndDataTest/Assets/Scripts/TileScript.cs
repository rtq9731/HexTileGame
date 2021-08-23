using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour, ITurnFinishObj
{

    [SerializeField] TileData data;

    public TileData Data
    {
        get { return data; }
        private set { }
    }

    PlayerScript Owner = null;

    private event Action g;

    public void Damage(int damage)
    {
        data.shield -= damage;
        if(data.shield < 0)
        {
            Owner = null;
        }
    }

    public void SetPosition(Vector3 pos)
    {
        data.Position = pos;
        gameObject.transform.position = pos;
    }

    public void ChangeOwner(PlayerScript newOwner)
    {
        Owner = newOwner;
    }

    public void TurnFinish()
    {
        if(g != null)
        {
            g();

            g = null;
        }

        Owner.AddResource(data.resource);
    }

    public void UpgradeTileAttackPower()
    {
        g += () => data.attackPower++;
        Owner.AddResource(-1);
    }

    public void FireMissile(TileScript attackTile)
    {
        attackTile.Damage(data.attackPower);
    }

}