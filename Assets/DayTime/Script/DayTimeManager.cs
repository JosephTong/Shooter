using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DayTimeManager : MonoBehaviour
{
    public static DayTimeManager m_Instance = null;
    private int m_TotalBotUsed = 0;
    [SerializeField] private TMP_Text m_BotCountText;
    [SerializeField] private LootResultPanel m_LootResultPanel;
    [SerializeField] private Button m_finializeBtn;
    private int m_CurrentShowResultIndex = 0;
    private List<(LootLocationScriptable,int)> m_AllLocationWithBot = new List<(LootLocationScriptable, int)>();

    private Dictionary<int,LootLocationController> m_AllSpawnedLootLocation = new Dictionary<int, LootLocationController>();
    public static DayTimeManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<DayTimeManager>();
        }
        return m_Instance;
    }


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
            return;
        }
    }

    private void Start() {
        DayTimeManager.GetInstance().SetUsedBotCountText(0);
        m_finializeBtn.onClick.AddListener(()=>{
            DayTimeManager.GetInstance().OnClickResultPanelNextBtn();
        });
    }

    public int GetTotalBotUsed(){
        return m_TotalBotUsed;
    }

    public void ChangeBotUsedCount(int changes){
        m_TotalBotUsed += changes;
    }

    public LootLocationController GetLootLocationController(int id){
        return m_AllSpawnedLootLocation[id];
    }

    public void OnClickResultPanelNextBtn(){
        if(m_CurrentShowResultIndex>=m_AllLocationWithBot.Count){
            // TODO : show total resources changes
            return;
        }
        m_LootResultPanel.Init(
            m_AllLocationWithBot[m_CurrentShowResultIndex].Item1,
            m_AllLocationWithBot[m_CurrentShowResultIndex].Item2);
        m_CurrentShowResultIndex ++;
    }

    public bool IsLocationIdUsed(int id){
        return m_AllSpawnedLootLocation.ContainsKey(id);
    }

    public void SetUsedBotCountText(int usedBotCount){
        m_BotCountText.text = $"{usedBotCount} / { MainGameManager.GetInstance().GetOwnedBotCount() }";

    }

    public void StoreLootLocationController(int id,LootLocationController lootLocationController){
        m_AllSpawnedLootLocation.Add(id,lootLocationController);
    }
    

}
