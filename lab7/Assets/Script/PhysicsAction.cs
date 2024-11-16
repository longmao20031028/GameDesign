
using UnityEngine;

public class PhysicsAction : SSAction
{
    public Vector3 forces;
    private bool once = true;

    public static PhysicsAction GetSSAction(Vector3 target, float speed)
    {
        PhysicsAction action = ScriptableObject.CreateInstance<PhysicsAction>();

        action.forces = target * speed;
        return action;
    }

    public override void FixedUpdate()
    {
        if (once)
        {
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.gameObject.GetComponent<Rigidbody>().AddForce(forces, ForceMode.Impulse);
            once = false;
        }
        if (this.transform.position.y <= -20 || this.transform.position.y>15)
        {
            this.destroy = true;
            if (this.transform.position.y > -30)
            {
                Singleton<ScoreManager>.Instance.Miss();
            }
            this.callBack.SSActionEvent(this);
        }
    }
    public override void Update() { }
    public override void Start() { }

}
