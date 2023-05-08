using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;
using System.Linq;
using EZCameraShake;
using BaseDefenseNameSpace;

public class GunController : MonoBehaviour
{
    [SerializeField] private GunScriptable m_SelectedGun;
    [SerializeField] private SpriteRenderer m_FPSImage;


    [Header("Drag to aim")]
    //[SerializeField][Range(0.1f, 2)] private float m_AimSensitivity = 0.5f;
    [SerializeField][Range(1.1f, 5)] private float m_CrossHairMaxSize = 4f;
    [SerializeField][Range(0.1f, 1f)] private float m_CrossHairMinSize = 0.5f;
    [SerializeField][Range(0.1f, 5)] private float m_CrossHairRadius = 0.5f;
    [SerializeField] private Button2D m_AimBtn;
    [SerializeField] private Transform m_CrossHair;
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;


    // the area the player can shoot 
    // prevent crosshair going out of bound
    [Header("CrossHair out of bound prevention")]
    [SerializeField] private Vector2Int m_FieldSize;
    [SerializeField] private Vector3 m_FieldCenter;

    [Header("Aim effect for camera")]
    [SerializeField] private Camera m_MainCamera;
    private Vector3 m_MainCameraStartPos;
    private Vector3 m_AimDirection = Vector3.zero;
    private float m_AimDistanceNormalized = 0;
    private float m_FieldCenterToCornerDistance = 10;

    [Header("Aim effect for gun")]
    [SerializeField] private Transform m_GunModel;


    [Header("Accracy")]
    private float m_CurrentAccruacy = 0;
    private Vector3 m_MousePreviousPos = Vector3.zero;

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI m_AmmoText;
    [SerializeField] private List<WeaponToBeSwitch> m_WeaponsToBeSwitch = new List<WeaponToBeSwitch>();
    private float m_CurrentAmmo;
    private Dictionary<string,float> m_GunsClipAmmo = new Dictionary<string, float>(); // how many ammo left on gun when switching

    [Header("Shooting")]
    [SerializeField] private GameObject m_ShotPointPrefab; // indicate where the shot land 
    [SerializeField] private AudioSource m_ShootAudioSource;
    [SerializeField] private Button2D m_ShootBtn;
    private float m_CurrentShootCoolDown = 0; // must be 0 or less to shoot 
    private Coroutine m_SemiAutoShootCoroutine = null;

    [Header("Reload")]
    [SerializeField] private Button m_ReloadBtn;




    private void Start()
    {
        BaseDefenseManager.GetInstance().m_ShootUpdatreAction += UpdateCrossHair;
        BaseDefenseManager.GetInstance().m_UpdateAction += ShootCoolDown;
        MainGameManager.GetInstance().AddNewAudioSource(m_ShootAudioSource);

        m_SemiAutoShootCoroutine = null;
        this.transform.position = new Vector3(8, -4.5f, 0) + m_FieldCenter;
        m_MainCamera.transform.position = new Vector3(m_FieldCenter.x, m_FieldCenter.y, -10);
        m_FieldCenterToCornerDistance = Mathf.Sqrt(m_FieldSize.y / 2 * m_FieldSize.y / 2 + m_FieldSize.x / 2 * m_FieldSize.x / 2);
        m_MainCameraStartPos = m_MainCamera.transform.position;
        foreach (var item in m_WeaponsToBeSwitch)
        {
            m_GunsClipAmmo.Add(item.m_Gun.DisplayName,item.m_Gun.ClipSize);
        }

        m_ReloadBtn.onClick.AddListener(()=>{
            if(IsFullClipAmmo())
                return;
                
            GunReloadControllerConfig gunReloadConfig = new GunReloadControllerConfig{
                ReloadScriptable = m_SelectedGun.ReloadScriptable,
                GainAmmo = GainAmmo,
                SetAmmoToFull = SetClipAmmoToFull,
                SetAmmoToZero = SetClipAmmoToZero,
                IsFullClipAmmo = IsFullClipAmmo,
            };
            BaseDefenseManager.GetInstance().StartReload(gunReloadConfig);
        });





        m_AimBtn.onDown.AddListener(() =>
        {
            m_MousePreviousPos = Input.mousePosition;
            m_AimDragMouseStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrossHair.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });

        m_AimBtn.onExit.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });

        m_ShootBtn.onDown.AddListener(() =>
        {
            
            if(m_CurrentAmmo<=0){
                m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
            }else{
                m_SemiAutoShootCoroutine = null;
                if( m_SemiAutoShootCoroutine == null && m_SelectedGun.IsSemiAuto){
                    m_SemiAutoShootCoroutine = StartCoroutine( SemiAutoShoot() );
                    return;
                }
                Shoot();
            }
                
        });
        m_ShootBtn.onUp.AddListener(() =>
        {
            if(m_SemiAutoShootCoroutine!= null){
                StopCoroutine(m_SemiAutoShootCoroutine);
                m_SemiAutoShootCoroutine = null;
            }
        });

        m_ShootBtn.onExit.AddListener(() =>
        {
            if(m_SemiAutoShootCoroutine!= null){
                StopCoroutine(m_SemiAutoShootCoroutine);
                m_SemiAutoShootCoroutine = null;
            }
        });

        ChangeAmmoCount(m_SelectedGun.ClipSize, true);
        m_FPSImage.sprite = m_SelectedGun.FPSSprite;
    }

    public void UpdateCrossHair(){
        // aim
        if (m_AimDragMouseStartPos != Vector2.zero)
        {
            m_AimDragMouseEndPos = Input.mousePosition;

            Vector3 offset = MainGameManager.GetInstance().m_AimSensitivity * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) / 15;
            m_CrossHair.position = m_CrossHairDragStartPos + offset;

            // accrucy lose for moving
            float mouseMoveAmound = Vector3.Distance(m_MousePreviousPos, Input.mousePosition) /
                (Mathf.Sqrt(Screen.height * Screen.height + Screen.width * Screen.width) / 2) * 1000;

            if (mouseMoveAmound == 0)
            {
                // draging but not moving , gain accruacy over time
                AccruacyGainOvertime();
            }
            else
            {
                m_CurrentAccruacy -= Time.deltaTime * mouseMoveAmound * (100 - m_SelectedGun.Stability);
                m_MousePreviousPos = Input.mousePosition;
            }


            CrossHairOutOfBoundPrevention();

            // aim to camera effect
            m_AimDirection = (m_CrossHair.position - m_FieldCenter).normalized;
            m_AimDistanceNormalized = Vector3.Distance(m_CrossHair.position, m_FieldCenter) /
                m_FieldCenterToCornerDistance;

            // aim to weapon effect
            m_MainCamera.transform.position = m_MainCameraStartPos + m_AimDirection * m_AimDistanceNormalized;
            float gunScaleX = 0.1f * ((m_CrossHair.position.x - m_FieldCenter.x) / (m_FieldSize.x / 2));
            m_GunModel.localScale = new Vector3(1 - gunScaleX, 1, 1);

            float gunRotationZ = -5 * (m_CrossHair.position.y - m_FieldCenter.y) / (m_FieldSize.y / 2);
            m_GunModel.localEulerAngles = new Vector3(0, 0, gunRotationZ);
        }
        else
        {
            AccruacyGainOvertime();
        }
        if (m_CurrentAccruacy < 0)
            m_CurrentAccruacy = 0;

        float targetCrossHairScale = Mathf.InverseLerp(100,0,m_CurrentAccruacy);
        targetCrossHairScale = Mathf.Lerp(m_CrossHairMinSize,m_CrossHairMaxSize,targetCrossHairScale);
        m_CrossHair.localScale = Vector3.one * targetCrossHairScale;
    }


    private void ShootCoolDown()
    {
        // fire rate
        m_CurrentShootCoolDown -= Time.deltaTime;
    }

    public void SetSelectedGun(GunScriptable gun){
        m_GunsClipAmmo[m_SelectedGun.DisplayName] = m_CurrentAmmo;
        m_SelectedGun = gun;

        m_CurrentAccruacy = m_SelectedGun.Accuracy;
        m_SemiAutoShootCoroutine = null;
        ChangeAmmoCount(m_GunsClipAmmo[m_SelectedGun.DisplayName], true);
        m_FPSImage.sprite = m_SelectedGun.FPSSprite;
    }
    private void GainAmmo(int changes){
        ChangeAmmoCount(changes,false);
    }

    private bool IsFullClipAmmo(){
        return m_CurrentAmmo >= m_SelectedGun.ClipSize;
    }

    private void SetClipAmmoToZero(){
        ChangeAmmoCount(0,true);
    }

    private void SetClipAmmoToFull(){
        ChangeAmmoCount(m_SelectedGun.ClipSize,true);
    }
    private IEnumerator SemiAutoShoot(){
        while (m_CurrentAmmo>0)
        {        
            Shoot();
            yield return null;
        }
        
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
    }


    private void Shoot()
    {
        if (m_CurrentShootCoolDown > 0)
            return;

        // shake camera
        CameraShaker.Instance.ShakeOnce( m_SelectedGun.CameraShakeStrength , m_SelectedGun.CameraShakeAmount,0.1f,0.1f );

        m_ShootAudioSource.PlayOneShot(m_SelectedGun.ShootSound);

        // move cross hair up 
        m_CrossHair.position += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(0f,0.5f),0) 
            *Mathf.Lerp ( 0, m_FieldSize.y/Camera.main.orthographicSize, (100 - m_SelectedGun.RecoilControl )/ 100 );
        CrossHairOutOfBoundPrevention();

        float targetCrossHairScale = Mathf.InverseLerp(100,0,m_CurrentAccruacy);
        float targetRadiusScale = Mathf.Lerp(0,m_CrossHairMaxSize-m_CrossHairMinSize,targetCrossHairScale);
        for (int j = 0; j < m_SelectedGun.PelletPerShot; j++)
        {        

            // random in a square
            // TODO : change to in cricle in future
            Vector3 accuracyOffset = new Vector3( 
                Random.Range(-1f,1f) * targetRadiusScale * m_CrossHairRadius ,
                Random.Range(-1f,1f) * targetRadiusScale * m_CrossHairRadius ,
                0
            );
            var shotPoint = Instantiate(m_ShotPointPrefab);
            shotPoint.transform.position = m_CrossHair.position + accuracyOffset;
            Destroy(shotPoint,1);

            RaycastHit2D[] hits = Physics2D.RaycastAll(m_CrossHair.position + accuracyOffset, Vector2.zero);
            List<EnemyBodyPart> hitedEnemy = new List<EnemyBodyPart>();
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.TryGetComponent<EnemyBodyPart>(out var enemyBodyPart);         
                if(enemyBodyPart != null){
                    hitedEnemy.Add(enemyBodyPart);
                }
            }
            if(hitedEnemy.Count>0){
                var sortedEnemies = hitedEnemy.OrderBy(x => x.GetDistance()).ToList();
                sortedEnemies[0].OnHit(m_SelectedGun.Damage);
            }
        }
        


        m_CurrentAccruacy -= (100 - m_SelectedGun.RecoilControl );

        m_CurrentShootCoolDown = 1 / m_SelectedGun.FireRate;
        ChangeAmmoCount(-1, false);
    }

    private void ChangeAmmoCount(float num, bool isSetAmmoCount = false)
    {
        if (isSetAmmoCount)
        {
            m_CurrentAmmo = num;
        }
        else
        {
            m_CurrentAmmo += num;
        }
        if(m_CurrentAmmo >m_SelectedGun.ClipSize){
            m_CurrentAmmo =m_SelectedGun.ClipSize;
        }

        m_AmmoText.text = $"{m_CurrentAmmo} / {m_SelectedGun.ClipSize}";
    }


    private void AccruacyGainOvertime()
    {
        if (m_CurrentAccruacy < m_SelectedGun.Accuracy)
        {
            m_CurrentAccruacy += Time.deltaTime * m_SelectedGun.Handling;
        }
        else
        {
            m_CurrentAccruacy = m_SelectedGun.Accuracy;
        }
    }


    private void CrossHairOutOfBoundPrevention()
    {
        if (m_CrossHair.position.x < m_FieldCenter.x - m_FieldSize.x / 2)
        {
            // Left out of bound
            m_CrossHair.position = new Vector3(m_FieldCenter.x - m_FieldSize.x / 2, m_CrossHair.position.y, 0);
        }

        if (m_CrossHair.position.x > m_FieldCenter.x + m_FieldSize.x / 2)
        {
            // Right out of bound
            m_CrossHair.position = new Vector3(m_FieldCenter.x + m_FieldSize.x / 2, m_CrossHair.position.y, 0);
        }


        if (m_CrossHair.position.y > m_FieldCenter.y + m_FieldSize.y / 2)
        {
            // Top out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, m_FieldCenter.y + m_FieldSize.y / 2, 0);
        }


        if (m_CrossHair.position.y < m_FieldCenter.y - m_FieldSize.y / 2)
        {
            // Down out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, m_FieldCenter.y - m_FieldSize.y / 2, 0);
        }
    }


    private void OnDestroy() {
        
    }
}
