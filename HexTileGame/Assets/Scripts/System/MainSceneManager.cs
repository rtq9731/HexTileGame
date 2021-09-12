using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    public static MainSceneManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public TileInfoScript InfoPanel;

    [SerializeField] GameObject vcamForUIMoving;
    [SerializeField] GameObject CvsOtherUI;

    [SerializeField] public GameObject tileVcam;
    [SerializeField] public GameObject techPanel;
    [SerializeField] public TileChecker tileChecker;

    public float TileZInterval = 0.875f;
    public float TileXInterval = 1f;

    public string PlayerName = "COCONUT";

    List<PlayerScript> players = new List<PlayerScript>();
    public List<PlayerScript> Players { get { return players; } set { players = value; } }

    private void Start()
    {
        InfoPanel = FindObjectOfType<TileInfoScript>();
        UIStackManager.RemoveUIOnTopWithNoTime();
    }

    public void StartGame()
    {
        foreach (var item in players)
        {
            Debug.Log(item);
            item.AddTile(TileMapData.Instance.GetRandTile());
        }
    }

    public void CallTechPanel()
    {
        ChangeCameraForUI();
    }

    private IEnumerator ChangeCameraForUI()
    {
        vcamForUIMoving.SetActive(true);
        while (true)
        {

        }
    }
}
