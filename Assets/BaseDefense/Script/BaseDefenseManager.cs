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
        //public Action CancelReload;
        public Action<int> GainAmmo;
        public Action SetAmmoToFull;
        public Action SetAmmoToZero;
        public Func<bool> IsFullClipAmmo;
    }

    public enum BaseDefenseStage{
        Shoot,
        SwitchWeapon,
        Reload
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

    [Header("Enemy Hp Bars")]
    [SerializeField] private Transform m_EnemyHpBarParent;
    public Transform EnemyHpBarParent { get { return m_EnemyHpBarParent; } }

    [Header("Wall")]
    [SerializeField] private HpBar m_WallHpBar;
    private float m_WallMaxHp = 1000;
    private float m_WallCurrentHp = 1000;
    private float m_TotalWallHpBarStayTime = 0;


    public Action m_ShootUpdatreAction = null;
    public Action m_SwitchWeaponUpdateAction = null;
    public Action m_ReloadUpdateAction = null;
    public Action m_UpdateAction = null;

    #region Change Game Stage From
    public Action m_GameStageChangeFromShootAction = null;
    public Action m_GameStageChangeFromSwitchWeaponAction = null;
    public Action m_GameStageChangeFromReloadAction = null;
    #endregion

    #region Change Game Stage To
    public Action m_GameStageChangeToShootAction = null;
    public Action m_GameStageChangeToSwitchWeaponAction = null;
    public Action m_GameStageChangeToReloadAction = null;
    #endregion


    private BaseDefenseStage m_GameStage = BaseDefenseStage.Shoot;
    public BaseDefenseStage GameStage {get { return m_GameStage; }}


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
        }
    }


    private void Start() {

        m_GameStageChangeFromReloadAction += CloseReloadPanel;
        m_QuitGameBtn.onClick.AddListener(()=>{
            Application.Quit();
        });
        m_WallCurrentHp = m_WallMaxHp;
        m_WallHpBar.m_HpBarFiller.fillAmount = m_WallCurrentHp / m_WallMaxHp;
    }

    private void Update() {
        switch (m_GameStage)
        {
            case BaseDefenseStage.Shoot:
                m_ShootUpdatreAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_SwitchWeaponUpdateAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_ReloadUpdateAction?.Invoke();
            break;
            default:
            break;
        }
        m_UpdateAction?.Invoke();
    }
    
    private void FixedUpdate()
    {
        // wall hp bar stay time
        if(m_TotalWallHpBarStayTime>0){
            m_TotalWallHpBarStayTime -= Time.deltaTime;
        }else{
            m_WallHpBar.m_CanvasGroup.alpha = 0;
        }
    }

    public void ChangeGameStage(BaseDefenseStage newStage){
        switch (m_GameStage)
        {
            case BaseDefenseStage.Shoot:
                m_GameStageChangeFromShootAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_GameStageChangeFromSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_GameStageChangeFromReloadAction?.Invoke();
            break;
            default:
            break;
        }

        switch (newStage)
        {
            case BaseDefenseStage.Shoot:
                m_GameStageChangeToShootAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_GameStageChangeToSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_GameStageChangeToReloadAction?.Invoke();
            break;
            default:
            break;
        }

        m_GameStage = newStage;
    }

    public void OnWallHit(float damage){
        m_WallHpBar.m_CanvasGroup.alpha = 1;
        m_WallCurrentHp-=damage;
        m_WallHpBar.m_HpBarFiller.fillAmount = m_WallCurrentHp / m_WallMaxHp;
        m_TotalWallHpBarStayTime = 2;
    }


    public static BaseDefenseManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<BaseDefenseManager>();
        }
        return m_Instance;
    }

    public void StartReload(GunReloadControllerConfig gunReloadConfig){
        m_ReloadController.gameObject.SetActive(true);
        m_ReloadController.StartReload( gunReloadConfig );
    }

    public void CloseReloadPanel(){
        m_ReloadController.gameObject.SetActive(false);

    }

    public void SwitchSelectedWeapon(GunScriptable gun){
        m_GunController.SetSelectedGun(gun);
    }
}
