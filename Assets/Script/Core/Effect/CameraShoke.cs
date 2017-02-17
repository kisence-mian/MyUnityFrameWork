using UnityEngine;
using System.Collections;

public class CameraShoke : MonoBehaviour {



    public Transform camTransform;      // 抖动目标的transform
    public float n_shake = 0f;        //持续抖动的时长
    public float n_shakeAmount = 0.15f;       //振幅越大抖动越厉害
    public float n_decreaseFactor = 1f;       //震动速度

    private Vector3 m_v3_randomOffest = new Vector3(-0.1f,0,0.1f); //左右偏移修正
    private Vector3 m_v3_weight = Vector3.one;// 震动方向权重

    public void Init(GameObject l_go_camera )
    {
        camTransform = l_go_camera.transform;


    }

    /// <summary>
    /// 触发震动
    /// </summary>
    /// <param name="时长"></param>
    /// <param name="振幅"></param>
    /// <param name="震速"></param>
    /// <param name="偏移"></param>
    public void Shoke(float l_n_shokeTime, float l_n_amount, float l_n_decreaseFactor, Vector3 l_v3_randomOffest, Vector3 l_v3_weight)
    {
        n_shake = l_n_shokeTime;
        n_shakeAmount = l_n_amount;
        n_decreaseFactor = l_n_decreaseFactor;
        m_v3_randomOffest = l_v3_randomOffest;
        m_v3_weight = l_v3_weight;
    }

    public void UpdateShoke()
    {
        if (n_shake > 0)
        {
            Vector3 l_v3_randomValue = Random.insideUnitSphere * n_shakeAmount + m_v3_randomOffest;

            float weight = m_v3_weight.x + m_v3_weight.y + m_v3_weight.z;
            if (weight == 0)
            {
                return;
            }
            l_v3_randomValue.x *= (m_v3_weight.x / weight);
            l_v3_randomValue.y *= (m_v3_weight.y / weight);
            l_v3_randomValue.z *= (m_v3_weight.z / weight);

            //l_v3_randomValue.y = 0; //禁止y轴方向的震动，
            camTransform.localPosition =  l_v3_randomValue;

            n_shake -= Time.deltaTime * n_decreaseFactor;
        }

        else
        {
            camTransform.localPosition = Vector3.zero;
            n_shake = 0;

        }
    }
}
