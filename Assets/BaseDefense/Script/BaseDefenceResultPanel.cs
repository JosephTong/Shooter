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
    private float m_BeforeBattleWallHp;

    private void Start() {
        RecordBeforeBattleData();
        m_Self.SetActive(false);
    }

    public void RecordBeforeBattleData(){
        m_BeforeBattleWallHp = MainGameManager.GetInstance().GetWallCurHp();
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

    public void ShowResult(bool isLose ){

        // before
        m_Wall.Before.text = ((int)m_BeforeBattleWallHp).ToString();

        m_Raw.Before.text = ((int)m_BeforeBattleResourceData.Raw).ToString();
        m_Scrap.Before.text = ((int)m_BeforeBattleResourceData.Scrap).ToString();
        m_Chem.Before.text = ((int)m_BeforeBattleResourceData.Chem).ToString();
        m_Electronic.Before.text = ((int)m_BeforeBattleResourceData.Electronic).ToString();
        m_Bot.Before.text = ((int)m_BeforeBattleResourceData.Bot).ToString();
        m_Heat.Before.text = ((int)m_BeforeBattleResourceData.Heat).ToString();

        // changes
        var afterBattleResourceData = MainGameManager.GetInstance().GetOwnedResources();

        float statValue = 0;
        statValue = MainGameManager.GetInstance().GetWallCurHp() - m_BeforeBattleWallHp;
        string textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Wall.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Raw - afterBattleResourceData.Raw;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Raw.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Scrap - afterBattleResourceData.Scrap;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Scrap.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Chem - afterBattleResourceData.Chem;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Chem.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Electronic - afterBattleResourceData.Electronic;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Electronic.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Bot - afterBattleResourceData.Bot;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Bot.Changes.text = textString;

        statValue = m_BeforeBattleResourceData.Heat - afterBattleResourceData.Heat;
        textString = statValue>=0?"+":"";
        textString += ((int)statValue).ToString();
        m_Heat.Changes.text = textString;



        // after
        m_Wall.After.text = ((int)MainGameManager.GetInstance().GetWallCurHp()).ToString();

        m_Raw.After.text = ((int)afterBattleResourceData.Raw).ToString();
        m_Scrap.After.text = ((int)afterBattleResourceData.Scrap).ToString();
        m_Chem.After.text = ((int)afterBattleResourceData.Chem).ToString();
        m_Electronic.After.text = ((int)afterBattleResourceData.Electronic).ToString();
        m_Bot.After.text = ((int)afterBattleResourceData.Bot).ToString();
        m_Heat.After.text = ((int)afterBattleResourceData.Heat).ToString();
    }
}
