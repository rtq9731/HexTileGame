using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPartSelector : MonoBehaviour
{
    [SerializeField] Transform unlockedPartParent = null;
    [SerializeField] GameObject infoPanelPrefab = null;
    [SerializeField] Button btnOk = null;
    [SerializeField] Sprite noneSprite = null;

    public PanelMissileMaker panelMissileMaker;

    List<GameObject> partPanelPool = new List<GameObject>();

    [Header("About MissilePart Info")]
    [SerializeField] Image partIcon = null;
    [SerializeField] Text textPartName = null;
    [SerializeField] Text textPartATK = null;
    [SerializeField] Text textPrice = null;
    [SerializeField] Text textMakeTurn = null;
    [SerializeField] Text textWeight = null;
    [SerializeField] Text textPartInfo = null;

    [SerializeField] private PersonPlayer player = null;

    public void InitPanelInNull(PanelMissileMaker.partType part)
    {
        transform.parent.gameObject.SetActive(true);

        partPanelPool.ForEach(x => x.gameObject.SetActive(false));

        switch (part)
        {
            case PanelMissileMaker.partType.Body:
                foreach (var item in player.UnlockedBodyIdx)
                {
                    GetNewInfoPanel(out PanelPartinfo element);
                    element.partSelector = this;
                    element.InitPanelPartInfo(MainSceneManager.Instance.GetMissileBodyByIdx(item));
                }
                break;
            case PanelMissileMaker.partType.Engine:
                foreach (var item in player.UnlockedEngineIdx)
                {
                    GetNewInfoPanel(out PanelPartinfo element);
                    element.partSelector = this;
                    element.InitPanelPartInfo(MainSceneManager.Instance.GetEngineDataByIdx(item));
                }
                break;
            case PanelMissileMaker.partType.Warhead:
                foreach (var item in player.UnlockedWarheadIdx)
                {
                    GetNewInfoPanel(out PanelPartinfo element);
                    element.partSelector = this;
                    element.InitPanelPartInfo(MainSceneManager.Instance.GetWarheadByIdx(item));
                }
                break;
            default:
                break;
        }

        partIcon.sprite = noneSprite;
        textPartName.text = "선택된 부품 없음";
        textPrice.text = "";
        textPartATK.text = "";
        textWeight.text = "";
        textMakeTurn.text = "";
        textPartInfo.text = "";
    }

    public void InitPartText(MissileWarheadData warhead)
    {
        btnOk.onClick.AddListener(() =>
        {
            panelMissileMaker.SetMissileBluePrintPart(warhead.TYPE);
            transform.parent.gameObject.SetActive(false);
            btnOk.onClick.RemoveAllListeners();
        });
        partIcon.sprite = MainSceneManager.Instance.GetWarheadSprite(warhead.TYPE);
        Debug.Log(MainSceneManager.Instance.GetWarheadSprite(warhead.TYPE));

        textPartATK.transform.parent.gameObject.SetActive(true);

        textPartName.text = warhead.Name;
        textPartATK.text = $"탄두의 공격력 : {warhead.Atk}";
        textPrice.text = $"탄두의 가격 : {warhead.Price}";
        textWeight.text = $"탄두의 무게 : {warhead.Weight}";
        textMakeTurn.text = $"제작에 걸리는 시간 : {warhead.Makingtime} 턴";
        textPartInfo.text = warhead.Info;
    }

    public void InitPartText(MissileEngineData engine)
    {
        btnOk.onClick.AddListener(() =>
        {
            panelMissileMaker.SetMissileBluePrintPart(engine.TYPE);
            transform.parent.gameObject.SetActive(false);
            btnOk.onClick.RemoveAllListeners();
        });
        partIcon.sprite = MainSceneManager.Instance.GetEngineSprite(engine.TYPE);
        Debug.Log(MainSceneManager.Instance.GetEngineSprite(engine.TYPE));

        textPartATK.transform.parent.gameObject.SetActive(false);

        textPartName.text = engine.Name;
        textPrice.text = $"엔진의 가격 : {engine.Price}";
        textMakeTurn.text = $"제작에 걸리는 시간 : {engine.Makingtime} 턴";
        textWeight.text = $"감당 가능한 무게 : {engine.Weight}";
        textPartInfo.text = engine.Info;
    }

    public void InitPartText(BodyData body)
    {
        btnOk.onClick.AddListener(() =>
        {
            panelMissileMaker.SetMissileBluePrintPart(body.TYPE);
            transform.parent.gameObject.SetActive(false);
            btnOk.onClick.RemoveAllListeners();
        });
        partIcon.sprite = MainSceneManager.Instance.GetBodySprite(body.TYPE);
        Debug.Log(MainSceneManager.Instance.GetBodySprite(body.TYPE));

        textPartATK.transform.parent.gameObject.SetActive(false);

        textPartName.text = body.Name;
        textPrice.text = $"선체의 가격 : {body.Price}";
        textMakeTurn.text = $"제작에 걸리는 시간 : {body.Makingtime} 턴";
        textWeight.text = $"추가 사거리 : {body.Morerange}";
        textPartInfo.text = body.Info;
    }

    private GameObject GetNewInfoPanel(out PanelPartinfo element)
    {
        GameObject result = null;
        element = null;
        if (partPanelPool.Count > 1)
        {
            result = partPanelPool.Find(x => !x.activeSelf);
            if (result != null)
            {
                element = result.GetComponent<PanelPartinfo>();
            }
            else
            {
                result = Instantiate(infoPanelPrefab, unlockedPartParent);
                partPanelPool.Add(result);
                element = result.GetComponent<PanelPartinfo>();
            }
        }
        else
        {
            result = Instantiate(infoPanelPrefab, unlockedPartParent);
            partPanelPool.Add(result);
            element = result.GetComponent<PanelPartinfo>();
        }

        result.SetActive(true);
        return result;
    }

    private void OnDisable()
    {
        if (UIStackManager.GetTopUI() == transform.parent.gameObject)
        {
            UIStackManager.RemoveUIOnTop();
        }
    }
}
