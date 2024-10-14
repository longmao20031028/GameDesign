using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ECS或MVC概念应用于小游戏
首先，将实体及其状态定义为博弈模型。
然后，给出一些可以修改游戏模型的组件/控件。
最后，游戏圈调用的系统处理程序OnGUI提供了一个游戏UI视图。
*/

public class GameZZZ : MonoBehaviour
{

    // Model
    private class GameModel
    {
        private GameView v;
        public int player = 1;
        public int nul = 0;
        public int bin = 2;
        public int goal = 3;
        public int wall = 4;

        public int goalx = 0;
        public int goaly = 4;
        public int goalx2 = 4;
        public int goaly2 = 0;

        public int[,] board = new int[5, 5];

        public void Init()
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    board[i, j] = nul;

            board[0, 0] = player;
            board[1, 0] = bin;
            board[2, 0] = bin;
            board[0, 3] = wall;
            board[1, 3] = wall;
            board[3, 0] = wall;
            board[2, 3] = wall;
            board[goalx, goaly] = goal;
            board[goalx2, goaly2] = goal;
        }


        private int[] dx = { 1, 0, -1, 0 };
        private int[] dy = { 0, 1, 0, -1 };

        public int MoveLeft(int x, int y) => Move(2, x, y);
        public int MoveRight(int x, int y) => Move(0, x, y);
        public int MoveUp(int x, int y) => Move(3, x, y);
        public int MoveDown(int x, int y) => Move(1, x, y);

        int Move(int direction, int x, int y)
        {
            int next = CheckNext(direction, x, y);
            switch (next)
            {
                case 0: // nothing
                    board[x, y] = nul;
                    board[x + dx[direction], y + dy[direction]] = player;
                    return 1;
                case 1: // box
                    int n = CheckNext(direction, x + dx[direction], y + dy[direction]);
                    if (n == 2 || n == 1) // next is obstacle or box
                    {
                        board[x, y] = bin;
                        board[x + dx[direction], y + dy[direction]] = player;
                    }
                    else if (n == 0) // next is empty
                    {
                        board[x, y] = nul;
                        board[x + dx[direction], y + dy[direction]] = player;
                        board[x + dx[direction] + dx[direction], y + dy[direction] + dy[direction]] = bin;   
                    }
                    return 1;
                case 2: // wall
                    return 0;
            }
            return 0;
        }

        int CheckNext(int direction, int x, int y)
        {
            if (x + dx[direction] < 0 || x + dx[direction] >= 5 || y + dy[direction] < 0 || y + dy[direction] >= 5)
                return 2; // wall
            if (board[x + dx[direction], y + dy[direction]] == nul)
                return 0; // nothing
            if (board[x + dx[direction], y + dy[direction]] == bin)
                return 1; // box
            if (board[x + dx[direction], y + dy[direction]] == goal)
                return 0; // goal
            return 2; // wall
        }


        public bool GameOver()
        {
            return board[goalx, goaly] == bin && board[goalx2, goaly2] == bin;
        }

    }

    // View
    private class GameView
    {
        public Texture2D boxImage;
        public Texture2D playerImage;
        public Texture2D binImage;
        public Texture2D goalImage;
        public Texture2D wallImage;

        public AudioSource audioSource;
        public AudioClip moveSound;

        // 创建并初始化一个 5x5 的二维数组，所有元素初始化为 0
        //用于显示完成的图案
        int[,] winMap = new int[5, 5]
        {
            { 0, 1, 2, 0, 2 },
            { 1, 0, 2, 0, 2 },
            { 1, 0, 2, 0, 2 },
            { 1, 0, 2, 0, 2 },
            { 1, 0, 2, 2, 0 }
        };



        public void Render(int[,] board, int playerX, int playerY, int goalX, int goalY, int goalX2, int goalY2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (board[i, j] == 0)
                    {
                        GUI.DrawTexture(new Rect(+120 + i * 70, 45 + j * 70, 75, 70), boxImage, ScaleMode.ScaleToFit);
                    }
                    else if (board[i, j] == 1)
                    {
                        GUI.DrawTexture(new Rect(120 + i * 70, 45 + j * 70, 70, 70), playerImage, ScaleMode.ScaleToFit);
                    }
                    else if (board[i, j] == 2)
                    {
                        GUI.DrawTexture(new Rect(120 + i * 70, 45 + j * 70, 70, 70), binImage, ScaleMode.ScaleToFit);
                    }
                    else if (board[i, j] == 4)
                    {
                        GUI.DrawTexture(new Rect(120 + i * 70, 45 + j * 70, 70, 70), wallImage, ScaleMode.ScaleToFit);
                    }
                }
            }
            if (board[goalX, goalY] != 2 && (playerX != goalX || playerY != goalY))
                GUI.DrawTexture(new Rect(120 + goalX * 70, 45 + goalY * 70, 70, 70), goalImage, ScaleMode.ScaleToFit);
            if (board[goalX2, goalY2] != 2 && (playerX != goalX2 || playerY != goalY2))
                GUI.DrawTexture(new Rect(120 + goalX2 * 70, 45 + goalY2 * 70, 70, 70), goalImage, ScaleMode.ScaleToFit);
        }
        public void WinRender()
        {

            GUI.Label(new Rect(120 + 437, 25 + 140, 70, 70), "Win!");

            GUIStyle yellowBoxStyle = new GUIStyle(GUI.skin.box);
            yellowBoxStyle.normal.background = MakeTex(2, 2, Color.yellow);

            GUIStyle orangeBoxStyle = new GUIStyle(GUI.skin.box);
            orangeBoxStyle.normal.background = MakeTex(2, 2, new Color(1, 0.5f, 0));

            for(int i = 0; i < 5; i++)
                for(int j = 0; j < 5; j++)
                {
                    switch (winMap[i,j])
                    {
                        case 0:
                            GUI.Box(new Rect(j*-4+120 + i * 70 + i * 3, 45 + j * 70 + j * 3, 70, 70), "");
                            break;
                        case 1:
                            GUI.Box(new Rect(j*-4+120 + i * 70 + i * 3, 45 + j * 70 + j * 3, 70, 70), "", orangeBoxStyle);
                            break;
                        case 2:
                            GUI.Box(new Rect(j*-4+120 + i * 70 + i * 3, 45 + j * 70 + j * 3, 70, 70), "", yellowBoxStyle);
                            break;
                        default:
                            break;
                    }
                }


        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color; // 填充颜色
            }
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

    }

    // Controller
    private GameModel model;
    private GameView view;

    void Start()
    {
        model = new GameModel();
        view = new GameView();
        view.boxImage = Resources.Load<Texture2D>("TV");
        view.playerImage = Resources.Load<Texture2D>("bangbu");
        view.binImage = Resources.Load<Texture2D>("Bin");
        view.goalImage = Resources.Load<Texture2D>("Goal");
        view.wallImage = Resources.Load<Texture2D>("wall");

        view.audioSource = gameObject.AddComponent<AudioSource>();
        view.moveSound = Resources.Load<AudioClip>("move");
        //view.audioSource.volume = 2f;

        model.Init();
    }

    void OnGUI()
    {
        GUI.Box(new Rect(90, 25, 600, 400), "Zenless Zone Zero");
        if (GUI.Button(new Rect(120 + 437, 25 + 25, 70, 70), "Restart"))
            model.Init();

        int playerX = 0, playerY = 0;

        if (!model.GameOver())
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (model.board[i, j] == model.player)
                    {
                        playerX = i;
                        playerY = j;
                    }
                }
            }

            view.Render(model.board, playerX, playerY, model.goalx, model.goaly, model.goalx2, model.goaly2);

            int isMove = 0;
            if (GUI.Button(new Rect(120 + 395, 25 + 220, 70, 70), "<"))
                isMove = model.MoveLeft(playerX, playerY);
            if (GUI.Button(new Rect(120 + 437, 25 + 300, 70, 70), "v"))
                isMove = model.MoveDown(playerX, playerY);
            if (GUI.Button(new Rect(120 + 480, 25 + 220, 70, 70), ">"))
                isMove = model.MoveRight(playerX, playerY);
            if (GUI.Button(new Rect(120 + 437, 25 + 140, 70, 70), "^"))
                isMove = model.MoveUp(playerX, playerY);
            if(isMove==1)
                view.audioSource.PlayOneShot(view.moveSound);
        }
        else
        {
            view.WinRender();
        }
    }

}
