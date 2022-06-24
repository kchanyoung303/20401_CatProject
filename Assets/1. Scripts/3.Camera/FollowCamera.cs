using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowCamera : MonoBehaviour
{
    public Transform target;        // ����ٴ� Ÿ�� ������Ʈ�� Transform

    private Transform tr;                // ī�޶� �ڽ��� Transform

    public GameObject MiniMap;
    void Start()
    {
        tr = GetComponent<Transform>();
    }
    void Update()
    {
        SizeUpMap();
    }
    void LateUpdate()
    {
        tr.position = new Vector3(target.position.x, tr.position.y, target.position.z);
        tr.LookAt(target);
    }
    void SizeUpMap()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            MiniMap.SetActive(true);
        }
        if(Input.GetKeyUp(KeyCode.M))
        {
            MiniMap.SetActive(false);
        }
    }
}
