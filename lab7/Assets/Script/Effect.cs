using UnityEngine;
using System.Collections; // ��Ҫ����System.Collections��ʹ��Э��

public class Effect : MonoBehaviour
{
    

    // ������Ч
    public void PlayEffect(Vector3 position)
    {
        
            GameObject effectInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Smoke"), position, Quaternion.identity);
            StartCoroutine(DestroyEffectAfterTime(effectInstance));
        
    }

    // Э�̣�������һ��ʱ���������Ч
    private IEnumerator DestroyEffectAfterTime(GameObject effectInstance)
    {
        // ��ȡ��ЧԤ�Ƽ�������ϵͳ���
   
            // ���û������ϵͳ���ȴ�һ��Ĭ�ϵ�ʱ��
            yield return new WaitForSeconds(0.6f); 
        
        // ������Чʵ��
        Destroy(effectInstance);
    }
}