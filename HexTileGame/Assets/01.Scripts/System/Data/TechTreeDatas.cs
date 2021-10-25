using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TechTreeDatas : MonoBehaviour
{
    [SerializeField]
    private SkillTreeNode[] skillTreeNodes;

    private List<SkillTreeNode> dataList = new List<SkillTreeNode>();

    private string filePath = "";
    private string fileName = "TestData.txt";

    public void LoadData()
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string dataString = sr.ReadToEnd();
            JsonUtility.FromJsonOverwrite(dataString, this);

#if UNITY_EDITOR
            Debug.Log(dataString);
#endif

            for (int i = 0; i < skillTreeNodes.Length; i++)
            {
                Debug.Log(skillTreeNodes[i].ResearchThingIdx);
                dataList.Add(skillTreeNodes[i]);
            }
        }
    }

    public SkillTreeNode GetDataByIdx(int idx)
    {
        return dataList.Find(x => x.Idx == idx);
    }

    /* 접근만 하면 되니까 주석.
    public void SaveData()
    {
        skillTreeNodes = new SkillTreeNode[] { new SkillTreeNode(), new SkillTreeNode() };
        string dataString = JsonUtility.ToJson(this);

        Debug.Log(dataString);

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(dataString);
        sw.Close();
    }
    */

    private void Awake()
    {
        filePath = Application.dataPath + "/" + fileName;

        LoadData();
    }
}
