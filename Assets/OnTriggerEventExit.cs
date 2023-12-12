using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerEventExit : MonoBehaviour
{
    public GameObject textUI;

    //private Collider
    //private Text text1;
    // Start is called before the first frame update
    void Start()
    {
        //text1 = textUI1.GetComponent<Text>();
        textUI.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Physics.OverlapSphere(this.transform.position, 10f);
    }
    private void OnTriggerEnter(Collider other)
    {
        //text1.text = "hello, world";
        textUI.SetActive(false);
        //Debug.Log(other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        textUI.SetActive(true);
        //Debug.Log(other.name+"aaaaa");
    }
}
