using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileController : MonoBehaviour
{
    public GameObject[] photosInFile;
    public GameObject file;
    private int photoCount;

    //
    public GameObject firstPhoto;

    // Start is called before the first frame update
    void Start()
    {
        photosInFile = new GameObject[GameParameter.maxPhotoInFile];
        photoCount = 0;

        AddPhoto(firstPhoto);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPhoto(GameObject photo)
    {
        photosInFile[photoCount] = Instantiate(photo, file.transform);
        photosInFile[photoCount].transform.localPosition = new Vector3(
            photoCount / 4 - 1,
            (float)(1 - (photoCount % 4) * 0.5),
            0);
        photosInFile[photoCount].transform.localScale = new Vector3(
            0.3f,
            0.3f,
            0.3f);
        photosInFile[photoCount].GetComponent<SpriteRenderer>().sortingOrder = 21 + photoCount;

        photoCount++;
    }
}
