using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootResultPanelNameSpace;

namespace LootResultPanelNameSpace
{
    [System.Serializable]
    public class LootResultResourcesText
    {
        public TMP_Text Raw;
        public TMP_Text Scrap;
        public TMP_Text Chem;
        public TMP_Text Electronic;
        public TMP_Text Bot;
        public TMP_Text Heat;
    }
}

public class LootResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private Image m_DetailImage;
    [SerializeField] private TMP_Text m_LocationName;
    [SerializeField] private TMP_Text m_Safeness;
    [SerializeField] private TMP_Text m_SendBot;
    [SerializeField] private TMP_Text m_ResultText;
    [SerializeField] private LootResultResourcesText m_Gain;
    [SerializeField] private Button m_NextBtn;

    private void Start() {
        m_NextBtn.onClick.AddListener(()=>{
            DayTimeManager.GetInstance().OnClickResultPanelNextBtn();
        });

        m_Self.SetActive(false);
    }

    public void Init(LootLocationScriptable scriptable , int botCount){
        m_Self.SetActive(true);
        m_DetailImage.sprite = scriptable.DetailImage;
       m_LocationName.text = $"Location : {scriptable.DisplayName }";
       float extraSafeness = scriptable.SaftnessGainPreExtraBot * Mathf.Min(0,botCount-1 );
       float totalSafeness = scriptable.BaseSafeness + extraSafeness;
       m_LocationName.text = $"Safeness : {totalSafeness}%";
       m_SendBot.text = $"SendBot : {botCount}";

        float safenessNeeded = Random.Range(0f,100f);
        bool isSuccess = safenessNeeded<=totalSafeness;
        string targetResultText = isSuccess?"Return with loots":"No one return";

       m_ResultText.text = $"SendBot : {targetResultText}";
       m_ResultText.color = isSuccess?Color.green:Color.red;
       
       if(isSuccess){
            float seedRandom = Random.Range(0f,1f);
            m_Gain.Raw.text = $"+{Mathf.Lerp(scriptable.MinRawMaterial,scriptable.MaxRawMaterial,seedRandom )}";
            seedRandom = Random.Range(0f,1f);
            m_Gain.Scrap.text = $"+{Mathf.Lerp(scriptable.MinScrapMaterial,scriptable.MaxScrapMaterial,seedRandom )}";
            seedRandom = Random.Range(0f,1f);
            m_Gain.Chem.text = $"+{Mathf.Lerp(scriptable.MinChemMaterial,scriptable.MaxChemMaterial,seedRandom )}";
            seedRandom = Random.Range(0f,1f);
            m_Gain.Electronic.text = $"+{Mathf.Lerp(scriptable.MinElectronicMaterial,scriptable.MaxElectronicMaterial,seedRandom )}";
            m_Gain.Bot.text = "+0";
            m_Gain.Heat.text = $"+{scriptable.HeatGainOnLoot}";

            // TODO : sent resources changes to daytime manager , NOT main game manager , we need to know how much resourecs owed brefore chnages
       }else{
            m_Gain.Raw.text =  "+0";
            m_Gain.Scrap.text =  "+0";
            m_Gain.Chem.text =  "+0";
            m_Gain.Electronic.text =  "+0";
            m_Gain.Bot.text = $"-{botCount}";
            m_Gain.Heat.text = $"+0";
       }
    }
}
