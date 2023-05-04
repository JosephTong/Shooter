using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BaseDefenseNameSpace;

namespace BaseDefenseNameSpace
{
    public class SpawnUIObjectForReloadPhaseConfig
    {
        public GameObject Prefab;
        public Vector2 Position;
    }

    public class GunReloadControllerConfig
    {
        public GunReloadScriptable ReloadScriptable;
        public Action CancelReload;
        public Action<int> GainAmmo;
        public Action SetAmmoToFull;
        public Action SetAmmoToZero;
        public Func<bool> IsFullClipAmmo;
    }
}

public class BaseDefenseManager : MonoBehaviour
{
    public static BaseDefenseManager m_Instance = null;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private GunController m_GunController;
    [SerializeField] private GunReloadController m_ReloadController;
    [SerializeField] private SwitchWeaponController m_SwitchWeaponController;
    [SerializeField] private Button m_QuitGameBtn;

    public List<EnemyController> m_Enemies = new List<EnemyController>();

    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
        }
    }

    private void Start() {

        StartCoroutine(SpawnEnemy());
        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });
    }


    private IEnumerator SpawnEnemy(){
        while (this.gameObject.activeSelf)
        {
            m_EnemySpawnController.SpawnEnemy();
            yield return new WaitForSeconds(10);
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
