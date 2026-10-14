using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonmanagerscript : MonoBehaviour
{
    Transform chapterTarget;
    bool movingtoChapters = false;
    Vector3 originalPos;
    Quaternion originalRot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject chapterObj = GameObject.Find("ChaptersTarget");
        chapterTarget = chapterObj.transform;
        originalPos = Camera.main.transform.position;
        originalRot = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingtoChapters)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, chapterTarget.position, Time.deltaTime * 2);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, chapterTarget.rotation, Time.deltaTime * 2);
        }
        else 
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, originalPos, Time.deltaTime * 2);
            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, originalRot, Time.deltaTime * 2);
        }
    }

    public void StartCameraChapter()
    {
        movingtoChapters = true;
    }

    public void StopCameraChapter() 
    {
        movingtoChapters = false;
    }


}
