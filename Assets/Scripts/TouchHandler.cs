using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchHandler : MonoBehaviour
{
    public RFIBManager rFIBManager;
    public GameController gameController;
    public FileController fileController;

    # region Touch Parameter
    private bool ifTouch;
    private int touchCount;
    private int touchTime;
    private int notTouchTime;
    private int nowTouchX;
    private int nowTouchY;
    private int[] touchPosX;
    private int[] touchPosY;

    public GameObject[] selectPrefab;
    public GameObject selectParent;
    private int selectCount;
    public GameObject[] selectInstances;

    private int[] sourcePosX;
    private int[] sourcePosY;
    private int targetPosX;
    private int targetPosY;

    private bool startProcess;

    private GameObject[] copyInstances;
    public GameObject copyParent;

    # endregion

    // Start is called before the first frame update
    void Start()
    {
        ifTouch = false;
        touchCount = 0;
        touchTime = 0;
        notTouchTime = 0;
        nowTouchX = -1;
        nowTouchY = -1;
        touchPosX = new int[RFIBParameter.maxTouch];
        touchPosY = new int[RFIBParameter.maxTouch];
        ResetTouchPos();

        sourcePosX = new int[RFIBParameter.maxTouch];
        sourcePosY = new int[RFIBParameter.maxTouch];
        selectCount = 0;
        selectInstances = new GameObject[RFIBParameter.maxTouch];
        ResetSelect();

        copyInstances = new GameObject[RFIBParameter.maxTouch];
    }
    
    // Update is called once per frame
    void Update()
    {
        SenseTouch();

        //Debug.Log(string.Format("[{0}, {1}] ({2}, {3}) - {4} {5} {6} - {7})",
        //    nowTouchX.ToString(),
        //    nowTouchY.ToString(),
        //    (nowTouchX / 3),
        //    (nowTouchY / 3),
        //    ifTouch.ToString(),
        //    touchTime,
        //    notTouchTime,
        //    touchCount));

        if (startProcess)
        {
            StartCoroutine(CopyPhoto());
            startProcess = false;
        }

        KeyPressed();
    }

    private void SenseTouch()
    {
        for (int i = 0; i < RFIBParameter.touchCol; i++)
        {
            for (int j = 0; j < RFIBParameter.touchRow; j++)
            {
                if (rFIBManager.touchBlock[i, j] && nowTouchX == -1 && nowTouchY == -1)
                {
                    nowTouchX = i;
                    nowTouchY = j;
                    ifTouch = true;

                    touchPosX[touchCount] = nowTouchX;
                    touchPosY[touchCount] = nowTouchY;
                    touchCount++;
                    notTouchTime = 0;
                    SetSelect(i / 3, j / 3);
                }
                else if (!rFIBManager.touchBlock[i, j] && ifTouch && i == nowTouchX && j == nowTouchY)
                {
                    nowTouchX = -1;
                    nowTouchY = -1;
                    ifTouch = false;
                    touchTime = 0;
                }
            }
        }

        if (ifTouch)
        {
            touchTime++;
        }
        else
        {
            notTouchTime++;
        }

        if (notTouchTime > RFIBParameter.notTouchGap)
        {
            //string str = "";
            //for (int i = 0; i < touchCount; i++)
            //{
            //    str += string.Format("({0} {1}) ", touchPosX[i], touchPosY[i]);
            //}
            //if (touchCount != 0)
            //{
            //    Debug.Log(str);
            //}

            ResetTouchPos();
            touchCount = 0;
        }
    }

    private void SetSelect(int x, int y)
    {
        if (touchCount == 1)
        {
            selectInstances[selectCount] = Instantiate(selectPrefab[0], selectParent.transform);
            selectInstances[selectCount].transform.localPosition = new Vector3(
                x * GameParameter.stageGap,
                y * GameParameter.stageGap,
                0);

            sourcePosX[selectCount] = x;
            sourcePosY[selectCount] = y;
            selectCount++;
        }
        else if (touchCount == 2 && touchPosX[0] == touchPosX[1] && touchPosY[0] == touchPosY[1])
        {
            Destroy(selectInstances[selectCount - 1]);
            selectInstances[selectCount - 1] = Instantiate(selectPrefab[1], selectParent.transform);
            selectInstances[selectCount - 1].transform.localPosition = new Vector3(
                x * GameParameter.stageGap,
                y * GameParameter.stageGap,
                0);

            targetPosX = x;
            targetPosY = y;

            startProcess = true;
        }
    }

    private void ResetTouchPos()
    {
        for (int i = 0; i < RFIBParameter.maxTouch; i++)
        {
            touchPosX[i] = -1;
            touchPosY[i] = -1;
        }
    }

    public void ResetSelect()
    {
        for (int i = 0; i < selectCount; i++)
        {
            sourcePosX[i] = -1;
            sourcePosY[i] = -1;

            Destroy(selectInstances[i]);
        }

        targetPosX = -1;
        targetPosY = -1;

        selectCount = 0;
        startProcess = false;
    }

    IEnumerator CopyPhoto()
    {
        for (int i = 0; i < selectCount - 1; i++)
        {
            copyInstances[i] = Instantiate(gameController.photoSeries[sourcePosX[i] * RFIBParameter.stageRow + sourcePosY[i]], copyParent.transform);
            copyInstances[i].transform.localPosition = new Vector3(
                    sourcePosX[i] * GameParameter.stageGap,
                    sourcePosY[i] * GameParameter.stageGap,
                    0);
            copyInstances[i].GetComponent<SpriteRenderer>().sortingOrder = 40;
            StartCoroutine(MovePhoto(i, sourcePosX[i], sourcePosY[i], targetPosX, targetPosY));
            yield return new WaitForSeconds(0.5f);
        }

        //
        yield return new WaitForSeconds(GameParameter.moveTime + 0.2f);
        ResetSelect();
    }

    IEnumerator MovePhoto(int instanceId, int fromX, int fromY, int toX, int toY)
    {
        for (int i = 0; i < GameParameter.moveStep; i++)
        {
            copyInstances[instanceId].transform.localPosition += new Vector3(
                (toX - fromX) * GameParameter.stageGap / GameParameter.moveStep,
                (toY - fromY) * GameParameter.stageGap / GameParameter.moveStep,
                0f);
            copyInstances[instanceId].transform.localScale -= new Vector3(
                0.7f / GameParameter.moveStep,
                0.7f / GameParameter.moveStep,
                0.7f / GameParameter.moveStep);
            yield return new WaitForSeconds(GameParameter.moveTime / GameParameter.moveStep);
        }
        
        fileController.AddPhoto(gameController.photoSeries[fromX * RFIBParameter.stageRow + fromY]);
        Destroy(copyInstances[instanceId]);
    }

    private void KeyPressed()
    {
        if (Input.GetKey("a"))
        {
            rFIBManager.touchBlock[10, 4] = true;
        }
        else
        {
            rFIBManager.touchBlock[10, 4] = false;
        }

        if (Input.GetKey("s"))
        {
            rFIBManager.touchBlock[13, 4] = true;
        }
        else
        {
            rFIBManager.touchBlock[13, 4] = false;
        }

        if (Input.GetKey("d"))
        {
            rFIBManager.touchBlock[16, 4] = true;
        }
        else
        {
            rFIBManager.touchBlock[16, 4] = false;
        }

        if (Input.GetKey("t"))
        {
            rFIBManager.touchBlock[22, 7] = true;
        }
        else
        {
            rFIBManager.touchBlock[22, 7] = false;
        }

        if (Input.GetKeyUp("z"))
        {
            ResetSelect();
        }
    }
}
