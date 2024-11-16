using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Director : System.Object
{ 
    //µ¥ÀýÄ£Ê½
    private static Director _instance;
    public SceneController currentSceneController { get; set; }

    public static Director getInstance()
    {
        if (_instance == null)
            _instance = new Director();
        return _instance;
    }
}

public interface SceneController 
{
    void LoadResources();
}

public interface Interaction
{
    void hit(Vector3 pos);
    void Restart();
    int GetScore();
    int GetState();
    void SetState(int s);
    void setMode(ActionMode m);
    int GetRound();
}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                    " is needed in the scene, but there is none.");
                }
            }
            return instance;
        }
    }
}