using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LootDetailsPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_DetailImage; 
    [SerializeField] private TMP_Text m_HeatGain; 
    [SerializeField] private TMP_Text m_LocationName; 
    [SerializeField] private TMP_Text m_Raw; 
    [SerializeField] private TMP_Text m_Scrap;
    [SerializeField] private TMP_Text m_Chem;
    [SerializeField] private TMP_Text m_Electronic;
    [SerializeField] private TMP_Text m_Saftness;
    [SerializeField] private TMP_Text m_BotCount;
    [SerializeField] private Button m_ConfirmBtn;
    [SerializeField] private Button m_CancelBtn;
    [SerializeField] private Button m_IncreaseBotBtn;
    [SerializeField] private Button m_DecreaseBotBtn;


    private int m_LocationId = 0;
    private int m_NewSentBotCount = 0;

    private int m_OriginalLootBotCount = 0; // loot bot count on a location before hand
    private LootLocationScriptable m_LootLocationScriptable;

    private void Start() {
        m_IncreaseBotBtn.onClick.AddListener(()=>{
            // GetTotalBotUsed() is the bot use count before any of this panel changes confirm
            if( DayTimeManager.GetInstance().GetTotalBotUsed() + m_NewSentBotCount - m_OriginalLootBotCount >=
                  MainGameManager.GetInstance(). GetOwnedBotCount() ){
                return;
            }
            m_NewSentBotCount++;
            UpdateSentBotCount();
        });

        m_DecreaseBotBtn.onClick.AddListener(()=>{
            if(m_NewSentBotCount<=0){
                // low cap 0
                return;
            }
            m_NewSentBotCount--;
            UpdateSentBotCount();
        });

        m_ConfirmBtn.onClick.AddListener(OnClickConfirm);

        m_CancelBtn.onClick.AddListener(()=>{
            m_Self.SetActive(false);
        });

        m_Self.SetActive(false);
    }

    public void SetLootDeail(LootLocationScriptable scriptable, int locationId, int originalLootBotCount){
        m_LootLocationScriptable = scriptable;
        m_LocationId = locationId;

        m_OriginalLootBotCount = originalLootBotCount;
        m_NewSentBotCount = m_OriginalLootBotCount;
        m_Self.SetActive(true);
        string text = $"{scriptable.DisplayName}";
        m_LocationName.text = text;

        text = $"Heat gain on loot : {scriptable.HeatGainOnLoot }";
        m_HeatGain.text = text;

        text = $"{scriptable.MinRawMaterial}~{scriptable.MaxRawMaterial}";
        m_Raw.text = text;
        text = $"{scriptable.MinScrapMaterial}~{scriptable.MaxScrapMaterial}";
        m_Scrap.text = text;
        text = $"{scriptable.MinChemMaterial}~{scriptable.MaxChemMaterial}";
        m_Chem.text = text;
        text = $"{scriptable.MinElectronicMaterial}~{scriptable.MaxElectronicMaterial}";
        m_Electronic.text = text;

        m_DetailImage.sprite = scriptable.DetailImage;

        UpdateSentBotCount();
    }

    private void OnClickConfirm(){
        DayTimeManager.GetInstance().GetLootLocationController(m_LocationId).SetLootBotCount(m_NewSentBotCount);
        DayTimeManager.GetInstance().ChangeBotUsedCount( m_NewSentBotCount - m_OriginalLootBotCount );
        // GetTotalBotUsed() is the bot use count before any of this panel changes confirm
        DayTimeManager.GetInstance().SetUsedBotCountText( DayTimeManager.GetInstance().GetTotalBotUsed() + m_NewSentBotCount - m_OriginalLootBotCount );
        m_Self.SetActive(false);
    }

    private void UpdateSentBotCount(){
        string text = $"x {m_NewSentBotCount}";
        m_BotCount.text = text;

        if(m_NewSentBotCount <=0){
            text = $"Sent at least one bot to start looting ";
            m_Saftness.text = text;
        }else{
            text = $"Safeness : {m_LootLocationScriptable.BaseSafeness}%";
            if(m_NewSentBotCount>1){
                string symbol = m_LootLocationScriptable.SaftnessGainPreExtraBot>=0?"+":"";
                text += $" {symbol}{m_LootLocationScriptable.SaftnessGainPreExtraBot *(m_NewSentBotCount-1) }%";
                text += $" ({m_LootLocationScriptable.BaseSafeness+m_LootLocationScriptable.SaftnessGainPreExtraBot *(m_NewSentBotCount-1)}%)";
            }
            m_Saftness.text = text;
        }

            // GetTotalBotUsed() is the bot use count before any of this panel changes confirm
        DayTimeManager.GetInstance().SetUsedBotCountText( DayTimeManager.GetInstance().GetTotalBotUsed() + m_NewSentBotCount - m_OriginalLootBotCount );
    }
}
