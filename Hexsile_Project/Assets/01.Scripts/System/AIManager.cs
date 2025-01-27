using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIManager : MonoBehaviour
{
    public int aiPlayerCount = 3;

    public List<AIPlayer> aiPlayers = new List<AIPlayer>();

    private static AIManager instance = null;

    private uint curTurnCnt = 0;

    public static AIManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
        aiPlayers = GetComponentsInChildren<AIPlayer>().ToList();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public AIData[] GetAIDatas()
    {
        List<AIData> datas = new List<AIData>();
        aiPlayers.ForEach(x => datas.Add(x.Data));
        return datas.ToArray();
    }

    public void InitAllAI()
    {
        List<Color> aiColorSetList = GameManager.Instance.colorSet.ToList().FindAll(x => x != MainSceneManager.Instance.GetPlayer().PlayerData.PlayerColor);
        for (int i = 0; i < aiPlayers.Count; i++)
        {
            aiPlayers[i].Data = new AIData();
            aiPlayers[i].Data.PlayerName = $"AI {i + 1}";
            aiPlayers[i].Data.PlayerColor = aiColorSetList[Random.Range(0, aiColorSetList.Count)];
            aiColorSetList.Remove(aiPlayers[i].Data.PlayerColor);
        }
    }

    public void LoadAllAI(SaveData curSave)
    {
        for (int i = 0; i < curSave.aiPlayers.Length; i++)
        {
            aiPlayers[i].Data = curSave.aiPlayers[i];
        }
    }

    public void LoadStage(SaveData curSave)
    {
        curTurnCnt = curSave.turnCnt;
        LoadAllAI(curSave);

        foreach (var item in aiPlayers)
        {
            TileMapData.Instance.GetAllTiles().FindAll(x => item.Data.TileNums.Contains(x.Data.tileNum)).ForEach(x => item.AddTile(x));
        }

        var onlineAIPlayers = from result in aiPlayers
                              where result.OwningTiles.Count >= 1
                              select result;

        aiPlayers.FindAll(x => !onlineAIPlayers.Contains(x)).ForEach(x => x.Data.IsGameOver = true);

        MainSceneManager.Instance.GetPlayer().TurnFinishAction += CheckAndAttackPlayer;
    }

    public void StartStage(int mapSize)
    {
        curTurnCnt = 0;

        InitAllAI();
        MainSceneManager.Instance.GetPlayer().TurnFinishAction += CheckAndAttackPlayer;
        foreach (var item in aiPlayers) // 초기에 구석자리 땅 주는 부분
        {

            if (aiPlayers.FindAll(x => x.OwningTiles.Count >= 1).Count >= aiPlayerCount)
            {
                break;
            }

            List<TileScript> tiles = TileMapData.Instance.GetEndTile(6);
            tiles = tiles.FindAll(x => x.Owner == null);
            item.AddTile(tiles[Random.Range(0, tiles.Count)]);
        }

        var onlineAIPlayers = from result in aiPlayers
                              where result.OwningTiles.Count >= 1
                              select result;

        aiPlayers.FindAll(x => !onlineAIPlayers.Contains(x)).ForEach(x => x.Data.IsGameOver = true);

        foreach (var item in onlineAIPlayers) // 구석자리 땅이 모두 배분 된 후 나머지 땅을 AI들끼리 나눈다.
        {
            MainSceneManager.Instance.tileChecker.FindTilesInRange(item.OwningTiles[0], mapSize - 1).ForEach(x =>
            {
                if (x.Owner != null)
                    return;

                item.AddTile(x);
            });
        }
    }

    public string CheckAttackTurn()
    {
        if (0 == curTurnCnt)
        {
            return $"AI가 20 턴 뒤에 공격할 예정입니다.";
        }

        if (0 < curTurnCnt && curTurnCnt <= 20)
        {
            if (curTurnCnt % 20 == 0)
            {
                return "다른 세력들이 공격합니다!!";
            }

            return $"AI가 {20 - curTurnCnt % 20} 턴 뒤에 공격할 예정입니다.";
        }
        else
        {
            if (curTurnCnt % 5 == 0)
            {
                return "다른 세력들이 공격합니다!!";
            }

            return $"AI가 {5 - curTurnCnt % 5} 턴 뒤에 공격할 예정입니다.";
        }
    }

    public void CheckAndAttackPlayer()
    {
        curTurnCnt++;
        MainSceneManager.Instance.noticeAttackPanel.Refresh(CheckAttackTurn());
        if (curTurnCnt <= 20)
        {
            if (curTurnCnt % 20 == 0)
            {
                AttackRandomTileByEachAI();
            }
        }
        else
        {
            if (curTurnCnt % 5 == 0)
            {
                AttackRandomTileByEachAI();
            }
        }
    }

    private void AttackRandomTileByEachAI()
    {
        List<AIPlayer> attackableAI = new List<AIPlayer>();

        List<AIPlayer> onlineAI = aiPlayers.FindAll(x => x.OwningTiles.Count >= 1);
        foreach (var item in onlineAI)
        {
            if(item.CanAttack(out List<TileScript> attackableTiles))
            {
                attackableAI.Add(item);
            }
        }

        foreach (var item in attackableAI)
        {

            if (item.OwningTiles.Count >= 3)
            {
                for (int i = 0; i < item.OwningTiles.Count / 3; i++)
                {
                    AIRandAttack(item);
                }
            }
            else
            {
                AIRandAttack(item);
            }
        }
    }

    private void AIRandAttack(AIPlayer ai)
    {
        List<TileScript> attackableTiles = new List<TileScript>();
        ai.Data.OwningTiles = ai.Data.OwningTiles.Where(item => item != null).ToList();

        foreach (var itemTile in ai.OwningTiles)
        {
            if(itemTile.IsInEffect()) // 지속형 탄두의 영향 하인가?
            {
                continue;
            }

            MainSceneManager.Instance.tileChecker.FindTilesInRange(itemTile, 1).FindAll(x => x.Owner == MainSceneManager.Instance.GetPlayer()).ForEach(x => attackableTiles.Add(x));
        }

        attackableTiles = attackableTiles.Distinct().ToList();

        MainSceneManager.Instance.missileManager.fireMissileFromStartToTarget(ai.OwningTiles.Find(x => x.Owner == ai),
            new MissileData(MissileTypes.MissileEngineType.commonEngine, MissileTypes.MissileWarheadType.CommonTypeWarhead, MissileTypes.MissileBody.Orign),
            attackableTiles[Random.Range(0, attackableTiles.Count)], out GameObject missileObj);
    }
}
