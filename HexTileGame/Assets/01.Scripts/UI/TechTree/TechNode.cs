using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TechNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int researchIdx;
    [SerializeField] Button myBtn = null;

    SkillTreeNode data = null;

    private void Start()
    {
            
    }

    private void OnClickCallPanelInput()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}