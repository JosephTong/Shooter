using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPanelController : MonoBehaviour
{
    [SerializeField] private GameObject m_Self;
    [SerializeField] private TMP_Text m_WeaponDisplayName;
    [SerializeField] private TMP_Text m_DamagePerPellet;
    [SerializeField] private TMP_Text m_PelletPerShot;
    [SerializeField] private TMP_Text m_ClipSize;
    [SerializeField] private TMP_Text m_FireRate;
    [SerializeField] private TMP_Text m_Accuracy;
    [SerializeField] private TMP_Text m_RecoilControl;
    [SerializeField] private TMP_Text m_Stability;
    [SerializeField] private TMP_Text m_Handling;

    private void Start(){
        ClearInfo();
        m_Self.SetActive(false);
    }

    public void SetActive(){
        ClearInfo();
        m_Self.SetActive(true);

    }

    public void SetInfo(GunScriptable gunScriptable){
        m_WeaponDisplayName.text = gunScriptable.DisplayName;
        m_DamagePerPellet.text = $"Damage per pellet : {gunScriptable.Damage}";
        m_PelletPerShot.text = $"Pellet per shot: {gunScriptable.PelletPerShot}";
        m_ClipSize.text = $"Clip size : {gunScriptable.ClipSize}";
        m_FireRate.text = $"Fire Rate : {gunScriptable.FireRate}";
        m_Accuracy.text = $"Accuracy : {gunScriptable.Accuracy}";
        m_RecoilControl.text = $"Recoil Control : {gunScriptable.RecoilControl}";
        m_Stability.text = $"Stability : {gunScriptable.Stability}";
        m_Handling.text = $"Handling : {gunScriptable.Handling}";

    }

    public void ClearInfo(){
        m_WeaponDisplayName.text = "---";
        m_DamagePerPellet.text = $"Damage per pellet : ---";
        m_PelletPerShot.text = $"Pellet per shot: ---";
        m_ClipSize.text = $"Clip size : ---";
        m_FireRate.text = $"Fire Rate : ---";
        m_Accuracy.text = $"Accuracy : ---";
        m_RecoilControl.text = $"Recoil Control : ---";
        m_Stability.text = $"Stability : ---";
        m_Handling.text = $"Handling : ---";
    }
}
