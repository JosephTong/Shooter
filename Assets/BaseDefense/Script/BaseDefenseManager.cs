using System.Collections;
using System.Collections.Generic;
using GunReloadControllerNameSpase;
using UnityEngine;

public class BaseDefenseManager : MonoBehaviour
{
    public static BaseDefenseManager m_Instance = null;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private GunController m_GunController;
    [SerializeField] private GunReloadController m_ReloadController;
    [SerializeField] private SwitchWeaponController m_SwitchWeaponController;

    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
        }
    }

    public static BaseDefenseManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<BaseDefenseManager>();
        }
        return m_Instance;
    }

    public void StartReload(GunReloadControllerConfig gunReloadConfig){
        m_ReloadController.gameObject.SetActive(true);
        m_ReloadController.InIt( gunReloadConfig );
    }

    public void CloseReloadPanel(){
        m_ReloadController.gameObject.SetActive(false);

    }

    public void SwitchSelectedWeapon(GunScriptable gun){
        m_GunController.SetSelectedGun(gun);
    }
}
