using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    GameObject Head;
    GameObject LeftArm;
    GameObject RightArm;
    GameObject LeftLeg;
    GameObject LeRightLeg;
    GameObject Tail;


    public Minion SetHead(GameObject H)
    {
        Head = H;

        return this;
    }
    public Minion SetLeftArm(GameObject LA)
    {
        LeftArm = LA;

        return this;
    }
    public Minion SetRightArm(GameObject RA)
    {
        RightArm = RA;

        return this;
    }
    public Minion SetLeftLeg(GameObject LL)
    {
        LeftLeg = LL;

        return this;
    }
    public Minion SetRightLeg(GameObject RL)
    {
        LeRightLeg = RL;

        return this;
    }
    public Minion SetTail(GameObject T)
    {
        Tail = T;

        return this;
    }

}

