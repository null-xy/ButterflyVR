using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnTriggerEventSceneChange : MonoBehaviour
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
        SceneManager.LoadScene("NatureTest2_UV");
    }

    private void OnTriggerExit(Collider other)
    {
        textUI.SetActive(false);
        //Debug.Log(other.name+"aaaaa");
    }
}
