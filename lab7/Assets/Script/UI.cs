using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UI : MonoBehaviour
{
    private Interaction interaction;
    //public int state = 0;
    private float Height = Screen.height;
    private float Width = Screen.width;
    bool flag = true;
    float time = 0;
    //private int Sorce = 0;
    GUIStyle btn_style;
    GUIStyle style1;
    GUIStyle style2;
    GUIStyle style3;

    static GameData gameData;

    public Texture2D crosshairTexture;
    public Texture2D PinkInfoTexture;
    public Texture2D BlueInfoTexture;
    public Texture2D RedInfoTexture;
    public Texture2D GoldInfoTexture;
    void Start()
    {
        interaction = Director.getInstance().currentSceneController as Interaction;
        Debug.Log((Width - 200) + "," + (Height - 400));

        btn_style = new GUIStyle("button");
        btn_style.fontSize = 75;

        style1 = new GUIStyle();
        style1.fontSize = 75;

        style2 = new GUIStyle();
        style2.fontSize = 75;
        style2.normal.textColor = Color.white;

        style3 = new GUIStyle();
        style3.fontSize = 49;
        style3.normal.textColor = Color.white;
        
        crosshairTexture = Resources.Load<Texture2D>("Crosshair");
        PinkInfoTexture = Resources.Load<Texture2D>("PinkInfo");
        RedInfoTexture = Resources.Load<Texture2D>("RedInfo");
        BlueInfoTexture = Resources.Load<Texture2D>("BlueInfo");
        GoldInfoTexture = Resources.Load<Texture2D>("GoldInfo");

        if (gameData == null)
        {
            gameData = Resources.Load<GameData>("data");
            if (gameData == null)
            {
                Debug.LogError("Failed to load GameData");
            }
        }
    }

    void OnGUI()
    {
        
        GUI.Box(new Rect(0, Height - 150, Width, 150), "");
        GUI.Box(new Rect(0, 0, Width, 150), "");

        
        switch(interaction.GetState())
        {
            case -1:
                Cursor.visible = true;
                GG();
                return;
            case -2:
                Cursor.visible = true;
                Win();
                return;
            case -5:
                Cursor.visible = true;
                Dir();
                return;
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 pos = Input.mousePosition;
            interaction.hit(pos);
        }
        if(flag)
        {
            Cursor.visible = true;
            Menu();
        }
        else
        {
            GameStart();
        }
        if (!flag && interaction.GetState() == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 95, 100, 50), "Next Round!", style2);
            time += Time.deltaTime;
            if (time > 3.5)
            {
                interaction.SetState(3);
                time = 0;
            }
        }
    }

    //游戏未开始的初始菜单
    private void Menu()
    {
        
        if (GUI.Button(new Rect(Width / 2 - 280, Height / 2 - 90, 560, 180), "S t a r t",btn_style))
        {
            interaction.Restart();
            interaction.setMode(ActionMode.PHYSICS);
            interaction.SetState(1);
            flag = false;
        }

        if (GUI.Button(new Rect(Width / 2 - 280, Height / 2 + 110, 560, 180), "Bestiary", btn_style))
        {
            interaction.SetState(-5);
        }

    }
    private void GameStart()
    {
        //分数
        
        GUI.Label(new Rect(Width-380, 20, 280, 80), "分数：" + Singleton<ScoreManager>.Instance.getScore().ToString(), style2);
        Vector3 mousePos = Input.mousePosition;

        GUI.Label(new Rect(Width / 2 - 100, 20, 280, 80), "轮次：" + interaction.GetRound(),style2);
        

        //重新开始按钮
        if (GUI.Button(new Rect(100, 20, 320, 80), "Home",btn_style))
        {
            Singleton<ScoreManager>.Instance.Reset();
            interaction.Restart();
            flag = true;
            interaction.SetState(0);
        }

        // 计算准心的中心位置
        float crossSize = 25;
        float crosshairWidth = crosshairTexture.width * crossSize / 100;
        float crosshairHeight = crosshairTexture.height * crossSize / 100;

        // 在屏幕坐标上绘制准心
        GUI.DrawTexture(new Rect
            (mousePos.x - crosshairWidth / 2,
            Height - mousePos.y - crosshairHeight / 2 , crosshairWidth , crosshairHeight ), crosshairTexture);

        //隐藏鼠标
        Cursor.visible = false;
    }

    private void GG()
    {
        GUI.Label(new Rect(Width/2-100, Height/2 - 200, 100, 50), "Game Over!", style1);
        GUI.Label(new Rect(Width / 2- 100, Height / 2 - 100, 100, 50), 
            "分数：" + Singleton<ScoreManager>.Instance.getScore().ToString(), style1);
        if (GUI.Button(new Rect(Width / 2 - 100, Height / 2, 320, 80), "Home", btn_style))
        {
            Singleton<ScoreManager>.Instance.Reset ();
            interaction.Restart();
            interaction.SetState(0);
            flag= true;
        }
    }

    private void Win()
    {
        GUI.Label(new Rect(Width / 2 - 130, Height / 2 - 200, 100, 50), "Finished!", style1);
        GUI.Label(new Rect(Width / 2 - 160, Height / 2 - 100, 110, 40), 
            "分数 " + Singleton<ScoreManager>.Instance.getScore().ToString(), style1);
        if (GUI.Button(new Rect(Width / 2 - 160, Height / 2, 320, 70), "Home", btn_style))
        {
            Singleton<ScoreManager>.Instance.Reset();
            interaction.Restart();
            interaction.SetState(0);
            flag= true;
        }
    }

    private void Dir()
    {
        GUI.Box(new Rect(100, 200, Width - 200, Height - 400), "");
        GUI.DrawTexture(new Rect(150, 250, 490, 490),PinkInfoTexture);
        GUI.DrawTexture(new Rect(200 + 490, 250, 490, 490), BlueInfoTexture);
        GUI.DrawTexture(new Rect(250 + 2 * 490, 250, 490, 490), RedInfoTexture);
        GUI.DrawTexture(new Rect(300 + 3 * 490, 250, 490, 490), GoldInfoTexture);
        if (GUI.Button(new Rect(100, 20, 320, 80), "Back", btn_style))
        {
            interaction.SetState(0);
        }

        for (int i = 0; i<4; i++)
        {
            GUI.Label(new Rect(150+ i * 490 + 120 + i * 60, 300 + 490, 490, 100), gameData.InfoList[i].Name,style2);

            string text = gameData.InfoList[i].Desc;
            string[] lines = text.Split('`'); // 假设文本中使用\n作为换行符
            int lineNumber = 0;
            foreach (string line in lines)
            {
                GUI.Label(new Rect(150 + i * 490 + i*70, 450 + 490 +  75 * (int)lineNumber, 490, 75), line,style3);
                lineNumber++;
            }

            //GUI.TextArea(new Rect(150 + i * 490, 450 + 490, 490, 490), ,style2);
        }

        //GUI.contentColor = originalColor;
    }
}

