using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_CurrentHp = 100;
    [SerializeField] private float m_MaxHp = 100;
    [SerializeField] private Image m_HpBar;


    [SerializeField][Range(1, 50)] private float m_MoveSpeed = 5;
    [SerializeField][Range(0.1f, 10)] private float m_LeftRightSwayAmount = 3;
    [SerializeField][Range(1, 50)] private float m_LeftRightSwaySpeed = 5;
    [SerializeField] private float m_CurrentDistance = 100;
    [SerializeField] private float m_MaxDistance = 100;
    [SerializeField] private float m_YPosClosest = -9.5f;
    [SerializeField] private float m_YPosFarthest = 4f;
    [SerializeField] private float m_ScaleClosest = 5;
    [SerializeField] private float m_ScaleFarthest = 0.5f;
    [SerializeField] private AnimationCurve m_NormalizedMoveToScale;
    private bool m_IsRotationCloseWise = true;

    private void Start() {
        this.transform.localEulerAngles += Vector3.forward * m_LeftRightSwayAmount * Random.Range(-1f,1f);
        m_CurrentHp = m_MaxHp;
        UpdateHpBar();
        
    }

    private void FixedUpdate()
    {
        if(m_CurrentDistance<=0){
            m_CurrentDistance = 0;
            this.transform.localScale = m_ScaleClosest * Vector3.one;
            this.transform.position = new Vector3(
                this.transform.position.x,
                m_YPosClosest,
                this.transform.position.z);
            this.transform.localEulerAngles = Vector3.zero;
        }else{
            m_CurrentDistance -= m_MoveSpeed * Time.deltaTime;

            this.transform.localScale = Vector3.Lerp( m_ScaleClosest * Vector3.one , m_ScaleFarthest * Vector3.one , 
                m_NormalizedMoveToScale.Evaluate( m_CurrentDistance / m_MaxDistance) );
            this.transform.position = new Vector3(
                this.transform.position.x,
                Mathf.Lerp(m_YPosClosest , m_YPosFarthest , m_NormalizedMoveToScale.Evaluate( m_CurrentDistance / m_MaxDistance ) ),
                this.transform.position.z
            );

            if (m_IsRotationCloseWise)
            {
                this.transform.localEulerAngles += Vector3.forward * Time.deltaTime * m_LeftRightSwaySpeed;
                if (this.transform.localEulerAngles.z > m_LeftRightSwayAmount && 
                    this.transform.localEulerAngles.z < 180 )
                {
                    m_IsRotationCloseWise = false;
                }
            }
            else
            {
                this.transform.localEulerAngles -= Vector3.forward * Time.deltaTime * m_LeftRightSwaySpeed;
                if (this.transform.localEulerAngles.z < 0 - m_LeftRightSwayAmount || 
                    (this.transform.localEulerAngles.z < 360 - m_LeftRightSwayAmount && this.transform.localEulerAngles.z > m_LeftRightSwayAmount ) )
                {
                    m_IsRotationCloseWise = true;
                }
            }
        }

    }

    private void UpdateHpBar(){
        m_HpBar.fillAmount = m_CurrentHp / m_MaxHp;
    }

    public float GetDistance(){
        return m_CurrentDistance;
    }

    public void OnDamage(float changes)
    {
        m_CurrentHp -= changes;
        if (m_CurrentHp <= 0)
        {
            Destroy(this.gameObject);
            return;
        }
        UpdateHpBar();
    }

    private void OnDestroy()
    {

    }
}
