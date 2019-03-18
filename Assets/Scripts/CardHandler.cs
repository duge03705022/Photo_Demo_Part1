using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour
{
    public RFIBManager rFIBManager;
    public GameController gameController;

    //待改善
    //public LevelControllerStage1 levelControllerStage1;

    #region Card Parameter
    public GameObject fileParentTransform;
    public GameObject[] file;

    private GameObject[,] cardInstance;
    private string[,] lastBlockId;
    private bool[,] hasPlaced;

    # endregion

    // Start is called before the first frame update
    void Start()
    {
        cardInstance = new GameObject[RFIBParameter.stageCol, RFIBParameter.stageRow];
        hasPlaced = new bool[RFIBParameter.stageCol, RFIBParameter.stageRow];
        lastBlockId = new string[RFIBParameter.stageCol, RFIBParameter.stageRow];

        for (int i = 0; i < RFIBParameter.stageCol; i++)
        {
            for (int j = 0; j < RFIBParameter.stageRow; j++)
            {
                for (int k = 0; k < RFIBParameter.maxHight; k++)
                {
                    hasPlaced[i, j] = false;
                    lastBlockId[i, j] = "0000";
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCards();
    }

    private void updateCards()
    {
        for (int i = 0; i < RFIBParameter.stageCol; i++)
        {
            for (int j = 0; j < RFIBParameter.stageRow; j++)
            {
                if (lastBlockId[i, j] != rFIBManager.blockId[i, j, 0])
                {
                    if (rFIBManager.blockId[i, j, 0] != "0000")
                    {
                        PlaceCard(i, j);
                    }
                    else
                    {
                        DestroyCard(i, j);
                    }

                    lastBlockId[i, j] = rFIBManager.blockId[i, j, 0];
                }
            }
        }
    }

    private void PlaceCard(int x, int y)
    {
        cardInstance[x, y] = file[RFIBParameter.SearchCard(rFIBManager.blockId[x, y, 0])];
        cardInstance[x, y].SetActive(true);
        cardInstance[x, y].transform.localPosition = new Vector3(
            x * GameParameter.stageGap,
            y * GameParameter.stageGap,
            0);

        hasPlaced[x, y] = true;
    }

    private void DestroyCard(int x, int y)
    {
        Destroy(cardInstance[x, y]);
        cardInstance[x, y] = null;
        hasPlaced[x, y] = false;
    }
}
