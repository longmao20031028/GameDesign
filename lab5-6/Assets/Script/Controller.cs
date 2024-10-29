using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dpGame;

public class Controller : MonoBehaviour, SceneController, Interaction{
    public bool forbid = false;
    float speed = 15; 
    public BankController leftBank;
    public BankController rightBank;
    public BoatController boat;
    public dpGame.CharacterController[] characters;
    public UserGUI u;
    public Judger judger;

    public SoundController soundController;
    public ScenceActionManager actionManager;
    CCSequenceAction characterMove;
    SSMoveToAction boatMove;

    void Awake() 
    {
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		u = gameObject.AddComponent <UserGUI>() as UserGUI;
        actionManager = gameObject.AddComponent<ScenceActionManager>() as ScenceActionManager;
        judger = gameObject.AddComponent<Judger>() as Judger;
		characters = new dpGame.CharacterController[6];
        soundController = new SoundController();
        loadResources ();
	}
    public void loadResources() 
    {
		leftBank = new BankController ("left");
		rightBank = new BankController ("right");
		boat = new BoatController ();
        soundController.LoadSound();

		for (int i = 0; i < 3; i ++) {
            dpGame.CharacterController tmp = new dpGame.CharacterController("devil", i);
            tmp.setPosition(leftBank.getPos(i));
            tmp.moveToBank(leftBank);
            leftBank.moveToBank(tmp);
            characters[i] = tmp;
        }

        for (int i = 0; i < 3; i ++) 
        {
            dpGame.CharacterController tmp = new dpGame.CharacterController("priest", i + 3);
            tmp.setPosition(leftBank.getPos(i+3));
            tmp.moveToBank(leftBank);
            leftBank.moveToBank(tmp);
            characters[i+3] = tmp;
        }

    }

    public void MoveBoat() {
        if (u.status ==0 ) return;
        if (forbid) return;
        if (! boat.isEmpty()) 
        {
            soundController.PlaySound();
            actionManager.moveBoat(boat.getObj(), boat.boatMovePos(), speed);
        }
        u.status = judger.checkGame();
    }

    public void moveCharacters (dpGame.CharacterController chr) 
    {
        if (u.status ==0 ) return;
        if (forbid) return;
        if (chr.getState() == 1) 
        {
            BankController bank;
            if (boat.getLR() == 0) 
            {
                bank = leftBank;
            }
            else 
            {
                bank = rightBank;
            }
            soundController.PlaySound();
            boat.outOfBoat(chr.getTag());
            chr.moveToBank(bank);

            actionManager.moveCharacter(chr.getObj(), bank.getPos(chr.getTag()), speed);
            bank.moveToBank(chr);
        }
        else {
            BankController bank = chr.getBank();
            
            if (boat.getLR() == bank.getLR()) 
            {
                if (!boat.isFull()) 
                {
                    soundController.PlaySound();
                    bank.outOfBank(chr.getTag());
                    chr.moveToBoat(boat);

                    actionManager.moveCharacter(chr.getObj(), boat.getSeat(), speed);
                    boat.moveToBoat(chr);
                }
            }
        }
        u.status = judger.checkGame();
    }

    public void Restart() 
    {
        boat.init();
        leftBank.init();
        rightBank.init();
        for (int i = 0; i < characters.Length; i ++) 
        {
            characters[i].init();
        }
    }
}