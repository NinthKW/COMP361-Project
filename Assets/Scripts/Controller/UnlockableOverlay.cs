using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.UI;

public class UnlockableOverlay : MonoBehaviour
{
    public Base building;
    public GameObject popupPrefab;
    private GameObject popupObj; 

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() => showPopup());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void showPopup()
    {

    }
}
