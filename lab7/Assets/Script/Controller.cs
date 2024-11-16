using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour, SceneController, ISSActionCallback, Interaction
{
    public IActionManager actionManager;
    public DiskFactory diskFactory;
    public ActionMode mode = ActionMode.NONE;

    public Sound sound;
    public Effect effect;
    public ScoreManager scoreManager;
    public UI ui;
    public int trial = 10;
    public float time = 0;
    public int round = 1;
    public int maxRound = 4;
    public int state = 0;

    public Queue<GameObject> diskQueue = new Queue<GameObject>();
    public int used = 0;

    private void Awake()
    {
        Director director = Director.getInstance();
        director.currentSceneController = this;

        this.gameObject.AddComponent<DiskFactory>();
        this.gameObject.AddComponent<ScoreManager>();
        diskFactory = Singleton<DiskFactory>.Instance;
        ui = gameObject.AddComponent<UI>() as UI;
        scoreManager = Singleton<ScoreManager>.Instance;
        effect = gameObject.AddComponent<Effect>();
        sound = gameObject.AddComponent<Sound>();
    }

    private void Update()
    {
        if (mode == ActionMode.NONE) return;


        if (state <= 0 || state == 2)
        {
            return;
        }
        if (trial == 0 && round >= maxRound)
        {
            time += Time.deltaTime;
            if (time > 3)
            {
                SetState(-2);
                time = 0;
            }
            return;
        }
        if (trial == 0 && state == 1)
        {
            state = 2;
        }

        if (trial == 0 && state == 3)
        {
            round = (round + 1);
            if (round > maxRound)
            {
                return;
            }
            trial = 10;
            state = 1;
        }

        if (time > 1.5 && used == 0)
        {
            Debug.Log("Throw!");
            if (Singleton<ScoreManager>.Instance.checkGame() == false)
            {
                Debug.Log("gg");
                SetState(-1);
                return;
            }
            ThrowDisk();
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
        }
    }


    public void ThrowDisk()
    {
        Debug.Log("ThrowDisk");
        int tmp = Random.Range(0, round);
        int num = 0;
        if (tmp < 0.9)
        {
            diskQueue.Enqueue(diskFactory.GetDisk(round));
            num = 1;
        }
        else// if (tmp < 2)
        {
            diskQueue.Enqueue(diskFactory.GetDisk(round));
            diskQueue.Enqueue(diskFactory.GetDisk(round));
            num = 2;
        }
        //else
        //{
        //    diskQueue.Enqueue(diskFactory.GetDisk(round));
        //    diskQueue.Enqueue(diskFactory.GetDisk(round));
        //    diskQueue.Enqueue(diskFactory.GetDisk(round));
        //    num = 3;
        //}
        used = num;
        for (int i = 0; i < num; i++)
        {
            GameObject disk = diskQueue.Dequeue();
            Vector3 position = new Vector3(0, 0, 0);
            float y = UnityEngine.Random.Range(1f, 3f);
            position = new Vector3(-disk.GetComponent<Disk>().target.x * 7, y, 0);
            disk.transform.position = position;

            disk.SetActive(true);

            actionManager.fly(disk, disk.GetComponent<Disk>().target, 
                    disk.GetComponent<Disk>().speed);
        }
        trial--;
    }

    public void hit(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);

        RaycastHit hit;
        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(ray);
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    RaycastHit hit = hits[i];
        if(Physics.Raycast(ray,out hit))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);  // 打印命中的物体名称

            if (hit.collider.gameObject.GetComponent<Disk>() != null
               && hit.collider.gameObject.GetComponent<Disk>().hit != 1)
            {
                hit.collider.gameObject.GetComponent<Disk>().hit ++;
                if (hit.collider.gameObject.GetComponent<Disk>().hit >= 1)
                {
                    effect.PlayEffect(hit.point);
                    
                    scoreManager.Hit(hit.collider.gameObject);
                    hit.collider.gameObject.transform.position = new Vector3(0, -40, 0);
                    sound.playSound(2);
                }
                
                return;
            }
        }
    }

    public int GetRound()
    {
        return round;
    }
    public void LoadResources() { }
    public void SSActionEvent(SSAction action) { }
    public int GetScore()
    {
        return Singleton<ScoreManager>.Instance.getScore();
    }
    public int GetState() 
    { 
        return state;
    }
    public void setMode(ActionMode m)
    {
        mode = m;
        if (mode == ActionMode.PHYSICS)
        {
            Debug.Log("phy");
            if (gameObject.GetComponent<PhysicsManager>() == null)
                actionManager = gameObject.AddComponent<PhysicsManager>() as PhysicsManager;
            else
                actionManager = gameObject.GetComponent<PhysicsManager>() as PhysicsManager;
        }
    }
    public void SetState(int a)
    {
        state = a;
    }

    public void Restart()
    {
        trial = 10;
        round = 1;
        time = 0;
    }

    
}

