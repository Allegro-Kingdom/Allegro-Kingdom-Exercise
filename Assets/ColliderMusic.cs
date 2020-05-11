using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMusic : MonoBehaviour
{
    public MainScene.Type type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && type != MainScene.Type.None)
        {
            GameObject.Find("Main Camera").GetComponent<MainScene>().Insert(type);
        }
    }
}
