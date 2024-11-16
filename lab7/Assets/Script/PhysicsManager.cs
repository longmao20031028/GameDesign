using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : ActionManager,IActionManager ,ISSActionCallback
{
    PhysicsAction action;
    Controller controller;
    
    private void Start()
    {
        controller = Director.getInstance().currentSceneController as Controller;
        controller.actionManager = this;
    }

    public void fly(GameObject disk, Vector3 target, float speed)
    {
        action = PhysicsAction.GetSSAction(target, speed);
        if (disk.GetComponent<Rigidbody>()==null)
        {
            disk.AddComponent<Rigidbody>();
            disk.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        this.RunAction(disk, action, this);
    }

    public void SSActionEvent(SSAction action)
    {
        //Debug.Log("action recycle");
        Singleton<DiskFactory>.Instance.Recycle(action.gameObject); ;
        controller.used--;
    }
}


