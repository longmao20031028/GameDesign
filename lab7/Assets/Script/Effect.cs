using UnityEngine;
using System.Collections; // 需要导入System.Collections来使用协程

public class Effect : MonoBehaviour
{
    

    // 播放特效
    public void PlayEffect(Vector3 position)
    {
        
            GameObject effectInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Smoke"), position, Quaternion.identity);
            StartCoroutine(DestroyEffectAfterTime(effectInstance));
        
    }

    // 协程，用于在一定时间后销毁特效
    private IEnumerator DestroyEffectAfterTime(GameObject effectInstance)
    {
        // 获取特效预制件的粒子系统组件
   
            // 如果没有粒子系统，等待一个默认的时间
            yield return new WaitForSeconds(0.6f); 
        
        // 销毁特效实例
        Destroy(effectInstance);
    }
}