using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance = null;

    [SerializeField] MissileWarhead missileWarhead;
    [SerializeField] MissileEngine missileEngine;

    private string filePath = "";
    private string fileName = "TestData.txt";

    public MissileWarheadData GetWarheadData(MissileTypes.MissileWarheadType type)
    {
        return missileWarhead.dataList.Find(x => x.TYPE == type);
    }

    public MissileWarheadData GetWarheadByIdx(int idx)
    {
        return missileWarhead.dataArray[idx];
    }

    public MissileEngineData GetEngineData(MissileTypes.MissileEngineType type)
    {
        return missileEngine.dataList.Find(x => x.TYPE == type);
    }

    public MissileEngineData GetEngineDataByIdx(int idx)
    {
        return missileEngine.dataArray[idx];
    }

    private void Awake()
    {
        Instance = this;

        Debug.Log(Application.dataPath);
        filePath = Application.dataPath + "/" + fileName;

        SkillTreeNode skillTreeNode = new SkillTreeNode();
        string dataString = JsonUtility.ToJson(skillTreeNode);

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(dataString);
        sw.Close();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public TileInfoScript InfoPanel;

    [SerializeField] public MissileManager missileManager;
    [SerializeField] public FogOfWarManager fogOfWarManager;
    [SerializeField] public GameObject tileVcam;
    [SerializeField] public TileChecker tileChecker;
    [SerializeField] public UITopBar uiTopBar;
    [SerializeField] public MissileEffectPool effectPool;

    public float TileZInterval = 0.875f;
    public float TileXInterval = 1f;
    public uint turnCnt = 0;

    public string PlayerName = "COCONUT";

    List<PlayerScript> players = new List<PlayerScript>();
    public List<PlayerScript> Players { get { return players; } set { players = value; } }

    [SerializeField] PersonPlayer player = null;

    private void Start()
    {
        InfoPanel = FindObjectOfType<TileInfoScript>();
        UIStackManager.RemoveUIOnTopWithNoTime();
    }

    public PlayerScript GetPlayer()
    {
        return player;
    }

    public bool CheckTurnFinish()
    {
        if(players.Find(x => x.IsTurnFinish == false))
        {
            return false;
        }
        else
        {
            players.ForEach(x => x.StartNewTurn()); // 다음 턴으로 넘기면서 True 반환
            turnCnt++;
            uiTopBar.UpdateTexts();

            return true;
        }
    }

    public void StartGame()
    {
        foreach (var item in players)
        {
            item.AddTile(TileMapData.Instance.GetRandTile());
        }
    }

    public void LoadedGame()
    {

    }
}
