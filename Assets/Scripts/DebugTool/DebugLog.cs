using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Meta.XR.Util;

public class DebugLog : MonoBehaviour
{
    public static DebugLog Instance;
    [SerializeField]private TMP_Text Message;
    [SerializeField]private GameObject Text;
    private string priviousMessage = "";
    private int count;
    private bool isOn = false;

    private void Awake()
    {
        Instance = this;
        Text.SetActive(isOn);
        
    }
    public void Log(string args){
        if(args == priviousMessage){
        }
        Message.text += " " + args;
    }
    
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
            {
                isOn = !isOn;
                Text.SetActive(isOn);
            }
    }
}
