using System.Collections;
using System.Collections.Generic;
using ullScreenShockWaveControllerNameSpace;
using UnityEngine;

namespace ullScreenShockWaveControllerNameSpace
{
    [System.Serializable]
    public class FullScreenShockWaveConfig
    {
        public float WaveThiccness = 0.1f;
        public float WaveSize = 0.5f;
        public float WaveStrength = 5f;
        public float WaveSpeed = 0.1f;
    }
}

public class FullScreenShockWaveController : MonoBehaviour
{
    private const string HOW_TO_USE = " - FullScreenEffectCamera with the SAME camera setting as main camera is required\n" +
        " - Set FullScreenEffectCamera as the chile of main camera with position and rotation of zero\n" +
        " - Add new layer FullScreenEffect \n" +
        " - In FullScreenEffectCamera >> Rendering >> Culling Mask , remove FullScreenEffect\n" +
        " - In FullScreenEffectCamera >> OutPut >> Output Texture , Create and add new render texture\n" +
        " - In FullScreenShockWave , change layer to FullScreenEffect\n" +
        " - In FullScreenShockWave >> Additional Setting >> Sorting Layer , Change to FullScreenEffect\n" +
        " - Sorting Layer FullScreenEffect MUST be the highest layer which cover ALL other layer\n" +
        " - In ShockWaveFullScreen(material) , add the render texture in \"RenderTexture\"\n" +
        " - Try to match the border of FullScreenShockWave to be the same as FullScreenEffectCamera view area\n";
    [Header(HOW_TO_USE)]
    [SerializeField] private SpriteRenderer m_FullScreenSpriteRenderer;
    [SerializeField] private AnimationCurve m_StrengthReduceOverTimeCurve;
    /*
    [SerializeField][Range(0.01f, 1f)] private float m_WaveThiccness = 0.5f;
    [SerializeField][Range(0.01f, 1f)] private float m_WaveSize = 0.5f;
    [SerializeField][Range(-5f, 5f)] private float m_WaveStrength = 5f;
    [SerializeField][Range(0f, 3f)] private float m_WaveSpeed = 0.1f;*/
    private FullScreenShockWaveConfig m_Config = null;
    private Material FullScreenShockWave = null;

    private void Start()
    {
        FullScreenShockWave = m_FullScreenSpriteRenderer.material;

    }
    private void Update()
    {
        if (m_Config == null)
        {
            return;
        }
        if (FullScreenShockWave)
        {
            if (Mathf.Abs(FullScreenShockWave.GetFloat("_NormailzedTime")) > m_Config.WaveSize)
            {
                FullScreenShockWave.SetFloat("_Thiccness", m_Config.WaveThiccness);
                FullScreenShockWave.SetFloat("_Strength", m_Config.WaveStrength);
                FullScreenShockWave.SetFloat("_NormailzedTime", 0);
            }
            else
            {
                var normailzedTime = FullScreenShockWave.GetFloat("_NormailzedTime");
                FullScreenShockWave.SetFloat("_NormailzedTime",
                    normailzedTime + m_Config.WaveSpeed * Time.deltaTime);

                float strengthReduction = FullScreenShockWave.GetFloat("_Strength") >= 0 ? -1 : 1;
                FullScreenShockWave.SetFloat("_Strength",
                    Mathf.Lerp(m_Config.WaveStrength, 0, m_StrengthReduceOverTimeCurve.Evaluate(normailzedTime / m_Config.WaveSize)));

            }


        }
    }

    public void Init(FullScreenShockWaveConfig config)
    {
        m_Config = config;
        if (FullScreenShockWave)
        {
            FullScreenShockWave.SetFloat("_Thiccness", config.WaveThiccness);
            FullScreenShockWave.SetFloat("_Strength", config.WaveStrength);
            FullScreenShockWave.SetFloat("_NormailzedTime", 0);
        }
    }

}
