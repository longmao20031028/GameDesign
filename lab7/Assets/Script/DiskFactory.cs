using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public class DiskFactory : MonoBehaviour
{
    Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    public int[] randMode = new int[4] { 8, 13, 18, 23 };//用于调整不同难度生成不同飞碟的概率。
    static GameData gameData;
    private void Start()
    {
        if (gameData == null)
        {
            gameData = Resources.Load<GameData>("data");
            if (gameData == null)
            {
                Debug.LogError("Failed to load GameData");
            }
        } 
    }

    public GameObject GetDisk(int round)
    {
        GameObject newDisk = null;
        int t = 0;

        //根据不同难度，调整概率决定生成不同类型的disk
        int diff = 0;
        Random.InitState((int)System.DateTime.Now.Ticks);
        switch (round)
        {
            case 1: diff = Random.Range(0, randMode[0]); break;
            case 2: diff = Random.Range(0, randMode[1]); break;
            case 3: diff = Random.Range(0, randMode[2]); break;
            case 4: diff = Random.Range(0, randMode[3]); break;
        }

        //Debug.Log("diff:" + diff);
        if (diff < randMode[0] )
        {
            t = 0;
        }
        else if (diff < randMode[1] )
        {
            t = 1;
        }
        else if (diff < randMode[2] )
        {
            t = 2;
        }
        else if (diff < randMode[3] )
        {
            t = 3;
        }
        name = t.ToString();
        float RanX = 0;
        

        if (pool.ContainsKey(name) && pool[name].Count > 0)
        {
            //Debug.Log("Get:" + name + ":" + pool[name].Count.ToString());
            newDisk = pool[name].Dequeue();
            newDisk.GetComponent<Disk>().nTag = name;
        }
        else
        {
            if (gameData.InfoList.Count >= t)
                newDisk = Instantiate(gameData.InfoList[t].diskPrefabs, 
                    Vector3.zero, Quaternion.identity);
            else
                newDisk = Instantiate(Resources.Load<GameObject>("Prefabs/" + name),
                    Vector3.zero, Quaternion.identity);
            if (newDisk != null)
            {
                if (!pool.ContainsKey(name))
                {
                    pool[name] = new Queue<GameObject>();
                }
                newDisk.AddComponent<Disk>();
                newDisk.GetComponent<Disk>().nTag = name;
            }
            else
            {
                Debug.LogError("\"Prefab not found for name: \" + name");
                return null;
            }
            //Debug.Log("newDisk");
        }

        //设置速度和血量
        if (gameData.InfoList.Count >= t)
        {
            newDisk.GetComponent<Disk>().speed = gameData.InfoList[t].spd;
            newDisk.GetComponent<Disk>().hit = 1 - gameData.InfoList[t].hp;
        }
        else
        {
            newDisk.GetComponent<Disk>().hit = 0;
            newDisk.GetComponent<Disk>().speed = 5.0f;
        }

        RanX = Random.Range(-1f, 1f) < 0 ? -2 : 2;
        if (RanX > 0)
        {
            newDisk.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            newDisk.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        newDisk.GetComponent<Disk>().target = new Vector3(RanX, 1, 0);

        

        newDisk.SetActive(true);

        return newDisk;
    }

 

    public void Recycle(GameObject disk)
    {
        string tag = disk.GetComponent<Disk>().nTag;
        disk.gameObject.SetActive(false);
        pool[tag].Enqueue(disk.gameObject);
        //Debug.Log("Recycle" + tag);
    }
}

public class Disk : MonoBehaviour
{
    public Vector3 pos;
    public float speed;
    public Vector3 target;
    public int hit = 0;
    public string nTag;//nameTag
}

