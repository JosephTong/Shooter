using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MainGameNameSpace;

public class BaseDefenceResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Button m_NextBtn;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Wall;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Raw;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Scrap;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Chem;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Electronic;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Bot;
    [SerializeField] private BaseDefenceResultPanelRowContent m_Heat;
    private ResourcesRecord m_BeforeBattleResourceData;
    [SerializeField]private float m_BeforeBattleRWallHp;

    private void Start() {
        RecordBeforeBattleData();
        m_Self.SetActive(false);
    }

    public void RecordBeforeBattleData(){
        m_BeforeBattleRWallHp = MainGameManager.GetInstance().GetWallCurHp();
        var beforeBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();
        m_BeforeBattleResourceData = new ResourcesRecord{
            Raw = beforeBattleResourceData.Raw,
            Scrap = beforeBattleResourceData.Scrap,
            Chem = beforeBattleResourceData.Chem,
            Electronic = beforeBattleResourceData.Electronic,
            Bot = beforeBattleResourceData.Bot,
            Heat = beforeBattleResourceData.Heat
        };
    }
}
