using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;

    [Header("Grid")]
    [SerializeField] private GameObject m_WeaponGridPanel;
    [SerializeField] private GameObject m_WeaponGridPrefab;
    [SerializeField] private TMP_Text m_WeaponPageText;
    [SerializeField] private Button m_LastPageBtn;
    [SerializeField] private Button m_NextPageBtn;
    [SerializeField] private List<WeaponGrid> m_AllWeaponGrid = new List<WeaponGrid>();
    private int m_PageIndex = 0;




    [Header("Detail")]
    [SerializeField] private GameObject m_WeaponDetailPanel;
    [SerializeField] private Button m_QuitWeaponDetailBtn;
   // [SerializeField] private Button m_TryReloadBtn;
    [SerializeField] private Image m_WeaponDetailImage;
    [SerializeField] private TMP_Text m_WeaponDisplayName;
    [SerializeField] private TMP_Text m_DamagePerPellet;
    [SerializeField] private TMP_Text m_PelletPerShot;
    [SerializeField] private TMP_Text m_ClipSize;
    [SerializeField] private TMP_Text m_SemiAuto;
    [SerializeField] private TMP_Text m_FireRate;
    [SerializeField] private TMP_Text m_Accuracy;
    [SerializeField] private TMP_Text m_RecoilControl;
    [SerializeField] private TMP_Text m_Stability;
    [SerializeField] private TMP_Text m_Handling;



    private void Start(){
        DayTimeManager.GetInstance().m_ChangeFromWeapon += ()=>{
            m_Self.SetActive(false);
            };

        DayTimeManager.GetInstance().m_ChangeToWeapon += ()=>{
            m_WeaponDetailPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(true);
            SetActive();
            };
/*
        m_TryReloadBtn.onClick.AddListener(()=>{

        });*/

        m_QuitWeaponDetailBtn.onClick.AddListener(()=>{
            m_WeaponDetailPanel.SetActive(false);
            m_WeaponGridPanel.SetActive(true);
        });

        ClearInfo();
        m_Self.SetActive(false);


        var allWeaponOwnership = MainGameManager.GetInstance().GetAllWeaponOwnership();
        int index = 0;
        for (int i = m_PageIndex*12; index < m_AllWeaponGrid.Count; i++){
            if( i >= allWeaponOwnership.Count){
                // clear image and weapon name , but do not destory
                m_AllWeaponGrid[index].Init(null);
                index ++;
                continue;
            }
            // TODO : not own handling
            m_AllWeaponGrid[index].Init(allWeaponOwnership[i].Gun);
            index ++;
        }
    }

    public void SetActive(){
        ClearInfo();
        m_Self.SetActive(true);

    }

    public void SetInfo(GunScriptable gunScriptable){
        m_WeaponGridPanel.SetActive(false);
        m_WeaponDetailPanel.SetActive(true);
        m_WeaponDetailImage.sprite = gunScriptable.DisplaySprite;
        m_WeaponDisplayName.text = gunScriptable.DisplayName;
        m_DamagePerPellet.text = $"Damage per pellet : {gunScriptable.Damage}";
        m_PelletPerShot.text = $"Pellet per shot: {gunScriptable.PelletPerShot}";
        m_SemiAuto.text = $"Semi-Auto : {gunScriptable.IsSemiAuto.ToString() }";
        m_ClipSize.text = $"Clip size : {gunScriptable.ClipSize}";
        m_FireRate.text = $"Fire Rate : {gunScriptable.FireRate}";
        m_Accuracy.text = $"Accuracy : {gunScriptable.Accuracy}";
        m_RecoilControl.text = $"Recoil Control : {gunScriptable.RecoilControl}";
        m_Stability.text = $"Stability : {gunScriptable.Stability}";
        m_Handling.text = $"Handling : {gunScriptable.Handling}";

    }

    public void ClearInfo(){
        m_WeaponDisplayName.text = "- - -";
        m_DamagePerPellet.text = $"Damage per pellet : - - -";
        m_PelletPerShot.text = $"Pellet per shot: - - -";
        m_ClipSize.text = $"Clip size : - - -";
        m_FireRate.text = $"Fire Rate : - - -";
        m_Accuracy.text = $"Accuracy : - - -";
        m_RecoilControl.text = $"Recoil Control : - - -";
        m_Stability.text = $"Stability : - - -";
        m_Handling.text = $"Handling : - - -";
    }
}
