using UnityEngine;
using System.Collections;

public class LoadGame : MonoBehaviour
{
    ArrayList mjList = new ArrayList();

    GameObject selectedMj;


    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < 10; i++)
        {
            GameObject mj = (GameObject)Instantiate(Resources.Load("tong1"));
            int x = i - 5;
            mj.transform.position = new Vector2(x, -3);
            selectedMj = mj;
            mjList.Add(mj);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject mj = (GameObject)Instantiate(Resources.Load("tong1"));
            int x = i - 5;
            mj.transform.position = new Vector2(x, -1);
            selectedMj = mj;
            mjList.Add(mj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //selectedMj.transform.Translate(new Vector3(0.5f,0,0));
        //Debug.Log("position: "+selectedMj.transform.position.x);
    }
}
