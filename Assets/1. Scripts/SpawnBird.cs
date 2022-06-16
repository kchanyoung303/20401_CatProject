using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBird : MonoBehaviour
{
    //������ ���� ������Ʈ
    public GameObject monsterSpawner = null;

    //����Ȱ ���͵� ��Ƴ���
    public List<GameObject> monsters = new List<GameObject>();

    private int MonsterValue=0;

   
    //������ ���� �ִ��
    public int spawnMaxCnt = 50;

    //������ ���� ���� ��ǥ (x,z)��ġ
    public float rndPos;

    void Spawn()
    {
        //���� ���� ������ ���� �ִ�� ���� ũ�� ���ư�~
        if (monsters.Count > spawnMaxCnt)
        {
            return;
        }

        //������ ��ġ�� �����Ѵ�. �ʱ� ���̸� 1000 ������ .x,z�� ���� 
        Vector3 vecSpawn = new Vector3(this.transform.position.x+Random.Range(-rndPos, rndPos), 5f, this.transform.position.z+Random.Range(-rndPos, rndPos));

        //������ �ӽ� ���̿��� �Ʒ��������� Raycast�� ���� �������� ���� ���ϱ�
        Ray ray = new Ray(vecSpawn, Vector3.down);

        //Raycast ���� ��������
        RaycastHit raycastHit = new RaycastHit();
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            //Raycast ���̸� y������ �缳��
            vecSpawn.y = raycastHit.point.y;
        }

        //������ ���ο� ���͸� Instantiate�� clone�� �����.
        GameObject newMonster = Instantiate(monsterSpawner, vecSpawn, Quaternion.identity);
        MonsterValue += 1;

        //���� ��Ͽ� ���ο� ���͸� �߰�
        monsters.Add(newMonster);
    }

    private void Start()
    {
        //�ݺ������� ���͸� ����� InvokeRepeating
        InvokeRepeating("Spawn", 6f, 5f);
    }
}
