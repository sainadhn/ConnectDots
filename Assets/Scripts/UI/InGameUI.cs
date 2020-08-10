using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : UIScreen
{
    Text flows;
    Text moves;
    Text pipe;
    // Start is called before the first frame update
    void Awake()
    {
        flows = transform.Find("Flows").GetComponent<Text>();
        moves = transform.Find("Moves").GetComponent<Text>();
        pipe = transform.Find("Pipe").GetComponent<Text>();

        transform.Find("Level").GetComponent<Text>().text = "Level " + GameManager.Instance.loadedLevel;
        
    }

    // Update is called once per frame
    void Update()
    {
        flows.text = string.Format("{0}/{1}", GameManager.Instance.pipesConnected.ToString(), GameManager.Instance.colorsInPuzzle);
        moves.text = GameManager.Instance.noOfMoves.ToString();
        pipe.text = string.Format("{0} %", GameManager.Instance.gridsFilledPercent);
    }
}
