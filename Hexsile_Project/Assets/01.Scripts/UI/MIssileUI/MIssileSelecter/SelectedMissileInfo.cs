using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMissileInfo : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Text textMissileWarheadType;
    [SerializeField] Text textMissileRange;
    [SerializeField] Text textInfoWarheadName;
    [SerializeField] Text textMissileInfo;

    private void OnEnable()
    {
        RefreshTextsToNull();
    }

    private void RefreshTextsToNull()
    {
        textMissileWarheadType.text = "���õ� �̻��� ����";
        textMissileRange.text = "";
        textInfoWarheadName.text = "";
        textMissileInfo.text = "";
    }

    public void RefreshTexts(MissileData data)
    {
        MissileWarheadData warheadData = MainSceneManager.Instance.GetWarheadData(data.WarheadType);
        textMissileWarheadType.text = $"������ �̻��� ź�� : {warheadData.Name}";
        textMissileRange.text = $"�̻����� ��Ÿ� : { data.MissileRange }";
        textInfoWarheadName.text = $"{warheadData.Name}�� Ư¡ : ";
        textMissileInfo.text = warheadData.Info;
    }
}