using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] photoSeries;
    public GameObject parentTransform;

    public GameObject[,] photoInstances;

    // Start is called before the first frame update
    void Start()
    {
        photoInstances = new GameObject[RFIBParameter.stageCol, RFIBParameter.stageRow];

        for (int i = 0; i < RFIBParameter.stageCol; i++)
        {
            for (int j = 0; j < RFIBParameter.stageRow; j++)
            {
                photoInstances[i, j] = Instantiate(photoSeries[i * RFIBParameter.stageRow + j], parentTransform.transform);
                photoInstances[i, j].transform.localPosition = new Vector3(
                    i * GameParameter.stageGap,
                    j * GameParameter.stageGap,
                    0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
