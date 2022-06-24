using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDetector : MonoBehaviour
{
    //Ʈ���Ÿ� ���ؼ� ã���� �ϴ� ��ǥ �±׸� �����Ѵ�
    public string targetTag = string.Empty;

    //Ʈ���� �ȿ� ������ 
    private void OnTriggerEnter(Collider other)
    {
        //Ʈ���ſ� �浹�� ������Ʈ�� tag�� Ʈ�� �̳�
        //compareTag��
        // if(other.gameObject.tag ==  targetTag) ���� ���ϱ� ���� 
        if (other.gameObject.CompareTag(targetTag) == true)
        {
            //�ش� ������Ʈ�� ���� ����޽����� �߼���. �Լ���, ����޽��� ��� ��ġ , ����޽��� ���� ���� ���� �ɼ� �ʼ��� �ʼ��� �ƴϳ� ���� 
            gameObject.SendMessageUpwards("OnCkTarget", other.gameObject, SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false);
        }
        if(other.gameObject.CompareTag(targetTag)==false)
        {
            gameObject.SendMessageUpwards("OnCkTargetNon", other.gameObject, SendMessageOptions.DontRequireReceiver);

            gameObject.SetActive(true);
        }
    }
}