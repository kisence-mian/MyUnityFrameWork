using UnityEngine;
using System.Collections;

public class CameraShoke : MonoBehaviour {



    public Transform camTransform;      // 抖动目标的transform
    public float shake = 0f;        //持续抖动的时长
    public float shakeAmount = 0.15f;       //振幅越大抖动越厉害
    public float decreaseFactor = 1f;       //震动速度

    private Vector3 l_v3_randomValue;
    private Vector3 m_v3_randomOffest = new Vector3(-0.1f,0,0.1f); //左右偏移修正
    private bool m_b_FromLeft;//攻击来自右侧


    public void Init(GameObject l_go_camera )
    {
        camTransform = l_go_camera.transform;


    }

    /// <summary>
    /// 触发震动（震动的速度、幅度，以及不同的来自左右的伤害的震动应该方向不同）
    /// </summary>
    /// <param name="l_n_shokeTime"></param>
    public void Shoke(float l_n_shokeTime)
    {
        shake = l_n_shokeTime;
        if (Random.Range(0, 1000) > 500)
        {
            m_b_FromLeft = true;
            //Debug.Log("假设攻击来自左侧");
        }
        else
        {
            m_b_FromLeft = false;
            //Debug.Log("假设攻击来自右侧");
        }
    }

    public void UpdateShoke(Vector3 l_v3_originalPos)
    {
        if (shake > 0)
        {
            if (m_b_FromLeft)
            {
                l_v3_randomValue = Random.insideUnitSphere * shakeAmount + m_v3_randomOffest;
                
            }
            else
            {
                l_v3_randomValue = Random.insideUnitSphere * shakeAmount - m_v3_randomOffest;
                
            }
            
            l_v3_randomValue.y = 0; //禁止y轴方向的震动，
            camTransform.localPosition = l_v3_originalPos + l_v3_randomValue;

            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shake = 0f;
            camTransform.localPosition = l_v3_originalPos;
        }
    }
}
