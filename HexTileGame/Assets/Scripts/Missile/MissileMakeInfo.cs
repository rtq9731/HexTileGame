﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileMakeInfo : MonoBehaviour
{
    [SerializeField] Text textMissileWarhead;
    [SerializeField] Text textMissileMakeTime;

    public void SetData(MissileData data)
    {
        textMissileWarhead.text = $"{data.warheadType} 생산 중";
        textMissileMakeTime.text = $"{data.turnForMissileReady} 턴 뒤에 생산";
    }
}