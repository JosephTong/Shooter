using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayTimeManager : MonoBehaviour
{
    public static DayTimeManager m_Instance = null;
    private int m_TotalBotUsed = 0;
    [SerializeField] private TMP_Text m_BotCountText;

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
