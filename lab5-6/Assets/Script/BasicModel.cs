using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dpGame
{
    public class Director : System.Object
    {
        // 单例模式，只能有一个实例，通过getInstance访问
        private static Director _instance;

        // 管理着SceneController，通过它来间接管理场景中的对象
		public SceneController currentSceneController { get; set; }

		public static Director getInstance() 
        {
			if (_instance == null) 
            {
				_instance = new Director ();
			}
			return _instance;
		}
    }

    public interface SceneController
    {
		void loadResources ();
	}

    public interface Interaction 
    {
        void MoveBoat();
        void Restart();
        void moveCharacters(CharacterController chr);
    }

    public class Move : MonoBehaviour {
        readonly float speed = 20;
        public int state; // 0->不运动， 1->从岸边到船上，2->到达目的地
        Vector3 dest;
        Vector3 tmp;
        void Update()
        {
            // 从岸上到船的上空
            if (state == 1) 
            {
                transform.position = Vector3.MoveTowards (transform.position, tmp, speed * Time.deltaTime);
                if (transform.position == tmp) 
                {
                    state = 2;
                }
            } 
            else if (state == 2) 
            {
                transform.position = Vector3.MoveTowards (transform.position, dest, speed * Time.deltaTime);
                if (transform.position == dest) {
                    state = 0;
                }
            }
        }
        
        public void setDest(Vector3 new_dest) 
        {
            dest = new_dest;
            tmp = new_dest;
            state = 1;
            // 判断dest位置是否在船上（人比船高）
            if (this.transform.position.y > dest.y) 
            {
                tmp.y = this.transform.position.y;
            }
            else if (this.transform.position.y < dest.y) 
            {
                tmp.x = this.transform.position.x;
                tmp.y += 0.8f;
            }
            else 
            {
                state = 2;
            }
        }

        public void init() 
        {
            state = 0;
        }
    }

    public class CharacterController {
        GameObject character;
        ClickEvent click;
        public Move move;
        int state; // Boat = 1, bank = 0;
        int man; //Devil = 0, Priest = 1;
        int tag;

        BankController bankController;

        public CharacterController(string chr, int tag) 
        {
            this.tag = tag;
            if (chr == "priest") 
            {
                character = Object.Instantiate(Resources.Load("Prefabs/Xing", 
                    typeof(GameObject)), Vector3.zero, Quaternion.AngleAxis(180, Vector3.up)) 
                    as GameObject;
                character.transform.rotation = Quaternion.Euler(0, 180, 0);//保险起见，重新设置一下角度
                man = 1;
            }
            else 
            {
                character = Object.Instantiate(Resources.Load("Prefabs/King", 
                    typeof(GameObject)), Vector3.zero, Quaternion.AngleAxis(180, Vector3.up)) 
                    as GameObject;
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
                man = 0;
            }
            move = character.AddComponent(typeof(Move)) as Move;
            click = character.AddComponent(typeof(ClickEvent)) as ClickEvent;
            click.setChrController(this);
        }

        public void setName(string name) 
        {
            character.name = name;
        }

        public void setPosition(Vector3 position) 
        {
            character.transform.position = position;
        }


        public string getName() 
        {
            return character.name;
        }

        public int getTag() 
        {
            return tag;
        }

        public string getPerson() 
        {
            return man == 0 ? "Devil" : "Priest";
        }
        public BankController getBank() 
        {
            return bankController;
        }
        public int getState() 
        {
            return state;
        }
        
        public GameObject getObj() 
        {
            return this.character;
        }

        public void moveToBoat(BoatController boatController)  
        {
            bankController = null;
            character.transform.parent = boatController.getObj().transform;
			state = 1;
        }

        public void moveToBank(BankController bankController)  
        {
            this.bankController = bankController;
            character.transform.parent = null;
			state = 0;
        }

        public void init() 
        {
            move.init();
            bankController = (Director.getInstance ().currentSceneController 
                as Controller).leftBank;
			moveToBank (bankController);
			setPosition (bankController.getPos(tag));
			bankController.moveToBank (this);

        }
    }

    public class SoundController
    {
        public AudioClip clip;

        public void LoadSound()
        {
            clip = Resources.Load<AudioClip>("Audio/move");
        }

        public void PlaySound()
        {
            Debug.Log($"PlaySound");
            AudioSource.PlayClipAtPoint(clip, new Vector3(26.84f, 15.5f, -6));
        }
    }



    public class BankController 
    {
        GameObject bank;
        Vector3 leftBank = new Vector3(15,-2,0);
        Vector3 rightBank = new Vector3(30,-2,0);
        Vector3[] LchrPosition;
        Vector3[] RchrPosition;
        int LR;
        CharacterController[] character;

        GameObject cloud;
        Vector3 cloudPosition = new Vector3(33,15.5f,3.8f);
        GameObject bg;
        Vector3 bgPosition = new Vector3(18,10.4f,21);
        

        public BankController(string LR) 
        {
            cloud = Object.Instantiate(Resources.Load("Prefabs/clouds",
                    typeof(GameObject)), cloudPosition, Quaternion.identity, null)
                    as GameObject;
            bg = Object.Instantiate(Resources.Load("Prefabs/Bg",
                    typeof(GameObject)), bgPosition, Quaternion.identity, null)
                    as GameObject;
            LchrPosition = new Vector3[] 
            {
                new Vector3(17,11.9f,5.3f), 
                new Vector3(18,11.9f,5.3f), 
                new Vector3(19,11.9f,5.3f), 
				new Vector3(20,11.9f,5.3f), 
                new Vector3(21,11.9f,5.3f), 
                new Vector3(22,11.9f,5.3f)
            };
            RchrPosition = new Vector3[] 
            {
                new Vector3(31,11.9f,5.3f), 
                new Vector3(32,11.9f,5.3f), 
                new Vector3(33,11.9f,5.3f), 
				new Vector3(34,11.9f,5.3f),
                new Vector3(35,11.9f,5.3f), 
                new Vector3(36,11.9f,5.3f)
            };
            character = new CharacterController[6];
            if (LR == "left") 
            {
				bank = Object.Instantiate (Resources.Load ("Prefabs/Building", 
                    typeof(GameObject)), leftBank, Quaternion.AngleAxis(270, Vector3.up), null) 
                    as GameObject;
				bank.name = "left";
				this.LR = 0;
			} 
            else 
            {
				bank = Object.Instantiate (Resources.Load ("Prefabs/Building", 
                    typeof(GameObject)), rightBank, Quaternion.AngleAxis(270, Vector3.up), null) 
                    as GameObject;
				bank.name = "right";
				this.LR = 1;
			}
        }

        public void moveToBank(CharacterController chr) 
        {
            int index = chr.getTag();
            character[index] = chr;
        }
        public Vector3 getPos(int tag) 
        {
            if (bank.name == "left")
                return LchrPosition[tag];
            else 
                return RchrPosition[tag];
        }
        public CharacterController outOfBank(int tag) 
        {
            if (character[tag] != null) 
            {
                CharacterController tmp = character[tag];
                character[tag] = null;
                return tmp;
            }
            else 
                return null;
        }

        public int getLR() 
        {
            return LR;
        }

        public int[] getCount() 
        {
            int[] count = {0, 0};
            for (int i = 0; i < character.Length; i ++) 
            {
                if (character[i] == null) 
                {
                    continue;
                }
                else if (character[i].getPerson() == "Devil") 
                {
                    count[0] ++;
                }
                else
                {
                    count[1] ++;
                }
            }
            return count;
        }

        public void init() 
        {
            character = new CharacterController[6];
        }
    }

    //船控制器
    public class BoatController
    {
        GameObject boat;

        Vector3 start = new Vector3(23.9f, 10.2f, 5);
        Vector3 end = new Vector3(29.8f, 10.2f, 5);
        Vector3[] seatPos;
        int LR ;
        CharacterController[] seat =  new CharacterController[2];

        public BoatController() {
            LR = 0;
            seatPos = new Vector3[] 
            {
                new Vector3(23.2f, 11.7f, 5.5f), 
                new Vector3(24.2f, 11.7f, 5.5f)
            };
            
            boat = Object.Instantiate(Resources.Load("Prefabs/Bin", typeof(GameObject)), 
                start, Quaternion.AngleAxis(270, Vector3.up)) as GameObject;
            boat.name = "boat";
            boat.AddComponent(typeof(ClickEvent));

        }

        public Vector3 boatMovePos() 
        {
            LR = LR == 0 ? 1 : 0;
            if (LR == 0) 
                return start;
            else 
                return end;
            
        }
        public Vector3 getSeat() 
        {
            if (seat[0] == null) 
            {
                if (this.getLR() == 1) 
                {
                    Vector3 tmp = seatPos[0] + new Vector3(5.9f, 0, 0);
                    return tmp;
                }
                return seatPos[0];
            }
            else if (seat[1] == null) 
            {
                if (this.getLR() == 1) 
                {
                    Vector3 tmp = seatPos[1] + new Vector3(5.9f, 0, 0);
                    return tmp;
                }
                return seatPos[1]; 
            }
            return new Vector3();
        }
        public bool moveToBoat(CharacterController chr) 
        {
            if (seat[0] == null) 
            {
                seat[0] = chr;
                return true;
            }
            else if (seat[1] == null) 
            {
                seat[1] = chr;
                return true; 
            }
            return false;
        }

        public CharacterController outOfBoat(int tag) 
        {
            if (seat[0] != null && seat[0].getTag() == tag) 
            {
                CharacterController res = seat[0];
                seat[0] = null;
                return res;
            }
            else if (seat[1] != null && seat[1].getTag() == tag) 
            {
                CharacterController res = seat[1];
                seat[1] = null;
                return res;
            }
            return null;
        }
        public GameObject getObj() 
        {
            return boat;
        }
        
        public bool isEmpty() 
        {
            return (seat[0] == null && seat[1] == null);
        }

        public bool isFull() 
        {
            return (seat[0] != null && seat[1] != null);
        }

        public int getLR() 
        {
            return LR;
        }

        public int[] getCount() 
        {
            int[] count = {0, 0};
            for (int i = 0; i < 2; i ++) 
            {
                if (seat[i] != null && seat[i].getPerson() == "Priest") 
                {
                    count[1] ++;
                }
                else if (seat[i] != null && seat[i].getPerson() == "Devil") 
                {                    
                    count[0] ++;
                }
            }
            return count;
        }
        public void init() 
        {
            if (LR == 1) 
                boatMovePos();
            boat.transform.position = start;
            seat = new CharacterController[2];
        }
    }

    
}
