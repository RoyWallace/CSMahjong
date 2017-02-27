using UnityEngine;
using System.Collections;

public class ClickEvent : MonoBehaviour
{


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        transform.Translate(new Vector2(0, 0.5f));
    }


}
