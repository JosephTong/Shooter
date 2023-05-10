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

    private int m_SentBotCount = 0;
    private LootLocationScriptable m_LootLocationScriptable;

    private void Start() {
        m_IncreaseBotBtn.onClick.AddListener(()=>{
            // TODO : check for bot owned and bot used 
            m_SentBotCount++;
            UpdateSentBotCount();
        });

        m_DecreaseBotBtn.onClick.AddListener(()=>{
            if(m_SentBotCount<=0){
                // low cap 0
                return;
            }
            // TODO : check for bot owned and bot used 
            m_SentBotCount--;
            UpdateSentBotCount();
        });

        m_CancelBtn.onClick.AddListener(()=>{
            m_Self.SetActive(false);
        });

        m_Self.SetActive(false);
    }

    public void SetLootDeail(LootLocationScriptable scriptable){
        m_LootLocationScriptable = scriptable;


        m_SentBotCount = 0;
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

    private void UpdateSentBotCount(){
        string text = $"x {m_SentBotCount}";
        m_BotCount.text = text;

        if(m_SentBotCount <=0){
            text = $"Sent at least one bot to start looting ";
            m_Saftness.text = text;
        }else{
            text = $"Saftness : {m_LootLocationScriptable.BaseSaftness}%";
            if(m_SentBotCount>1){
                string symbol = m_LootLocationScriptable.SaftnessGainPreExtraBot>=0?"+":"";
                text += $" {symbol}{m_LootLocationScriptable.SaftnessGainPreExtraBot *(m_SentBotCount-1) }%";
                text += $" ({m_LootLocationScriptable.BaseSaftness+m_LootLocationScriptable.SaftnessGainPreExtraBot *(m_SentBotCount-1)}%)";
            }
            m_Saftness.text = text;
        }


    }
}
