using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DayTimeManager : MonoBehaviour
{
    public static DayTimeManager m_Instance = null;
    [SerializeField] private TMP_Text m_BotCountText;
    [SerializeField] private LootResultPanel m_LootResultPanel;
    [SerializeField] private Button m_finializeBtn;
    private int m_CurrentShowResultIndex = 0;
    private List<LootLocationController> m_AllLocationWithBot = new List<LootLocationController>();

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
        DayTimeManager.GetInstance().SetUsedBotCountText();
        m_finializeBtn.onClick.AddListener(()=>{
            DayTimeManager.GetInstance().OnClickResultPanelNextBtn();
        });
    }

    public int GetTotalBotUsed(){
        m_AllLocationWithBot = m_AllSpawnedLootLocation.Where(x=>x.Value.GetLootBotCount()>0).Select(x=>x.Value).ToList();
        int ans = 0;
        for (int i = 0; i < m_AllLocationWithBot.Count; i++)
        {
            ans += m_AllLocationWithBot[i].GetLootBotCount();
        }
        return ans;
    }

    public LootLocationController GetLootLocationController(int id){
        return m_AllSpawnedLootLocation[id];
    }

    public void OnClickResultPanelNextBtn(){
        if(m_CurrentShowResultIndex>=m_AllLocationWithBot.Count){
            m_LootResultPanel.TurnOff();
            // TODO : show total resources changes
            return;
        }
        m_LootResultPanel.Init(
            m_AllLocationWithBot[m_CurrentShowResultIndex].GetScriptable(),
            m_AllLocationWithBot[m_CurrentShowResultIndex].GetLootBotCount());
        m_CurrentShowResultIndex ++;
    }

    public bool IsLocationIdUsed(int id){
        return m_AllSpawnedLootLocation.ContainsKey(id);
    }

    public void SetUsedBotCountText(){
        m_BotCountText.text = $"{GetTotalBotUsed()} / { MainGameManager.GetInstance().GetOwnedBotCount() }";

    }

    public void StoreLootLocationController(int id,LootLocationController lootLocationController){
        m_AllSpawnedLootLocation.Add(id,lootLocationController);
    }
    

}
