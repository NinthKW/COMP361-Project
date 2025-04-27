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
        this.GetComponent<Button>().onClick.AddListener(() => togglePopup());
        popupObj = Instantiate(popupPrefab, this.GetComponentInParent<Transform>());
       
        popupObj.GetComponent<Transform>().localPosition = new Vector3(250f, -250f);
        
        popupObj.SetActive(false);

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void togglePopup()
    {
        if (popupObj.activeSelf)
        {
            popupObj.SetActive(false);
        } else
        {
            popupObj.SetActive(true);
        }
        
    }
}
