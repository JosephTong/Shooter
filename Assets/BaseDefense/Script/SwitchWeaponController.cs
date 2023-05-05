using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using UnityEngine;
using EZCameraShake;
public class SwitchWeaponController : MonoBehaviour
{
    [SerializeField] private Button2D m_LookDownBtn;
    [SerializeField] private Button2D m_LookUpBtn;
    [SerializeField] private Transform m_CameraParent;
    [SerializeField] private CameraShaker m_CameraShaker;
    [SerializeField] private GameObject m_ShootPanel;
    //[SerializeField] private GunController m_GunController;
    private float TimePassAfterlookDown = 0; // temporatory solution to accidently select shotgun when look down 

    private void Start() {
        m_LookDownBtn.onDown.AddListener(()=>{
            m_CameraShaker.enabled = false;
            m_CameraParent.position= new Vector3(0,-13.5f,0);
            m_ShootPanel.SetActive(false);
            m_LookUpBtn.gameObject.SetActive(true);
            TimePassAfterlookDown = 0;
        });

        m_LookUpBtn.onDown.AddListener(()=>{
            m_CameraParent.position= new Vector3(0,0,0);
            m_ShootPanel.SetActive(true);
            m_LookUpBtn.gameObject.SetActive(false);
            m_CameraShaker.enabled = true;
        });

        m_LookUpBtn.gameObject.SetActive(false);
    }    
    
    private void Update() {

        // select weapon
        if(Input.GetMouseButtonDown(0) && !m_ShootPanel.activeSelf && TimePassAfterlookDown>0.1f){
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.TryGetComponent<WeaponToBeSwitch>(out var weaponToBeSwitch);         
                if(weaponToBeSwitch != null){
                    SwitchWeapon(weaponToBeSwitch.m_Gun);
                    m_LookUpBtn.onDown.Invoke();
                    return;
                }
            }
        }
        TimePassAfterlookDown+=Time.deltaTime;
    }

    private void SwitchWeapon(GunScriptable gun){
        BaseDefenseManager.GetInstance().SwitchSelectedWeapon(gun);
    }
}
