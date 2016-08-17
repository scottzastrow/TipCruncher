/*
 * 
 * Copyright (c) 2015 All Rights Reserved, VERGOSOFT LLC
 * Title: TipCruncher
 * Author: Scott Zastrow
 * 
 */
using System;
using System.Text; //XML Serialization
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Xml; //XML Serialization
using System.IO; //XML Serialization
using System.Runtime.Serialization; //XML Serialization
using System.Runtime.Serialization.Formatters; //XML Serialization
using System.Runtime.Serialization.Formatters.Binary; //XML Serialization


[System.Serializable] //XML Serialization

public class FileManager : MonoBehaviour
{

    public XmlDocument xDoc = new XmlDocument(); //XML Serialization
    public string getXML; //XML Serialization
    public XmlNode dataNode; //XML Serialization
    private Boolean decOff;
    [SerializeField]
    private Boolean deleteOldData;
    public Text tipText;
    public Text enterSubTotal;
    public Text outPutText;
    public Text noInputText;
    public Text supTotalText;
    public Text tipTotalText;
    public Text totalTotalText;
    public Text tipTitleText;
    public Text totalTitleText;
    public Text splitTotalText;
    public Text splitTittle;
    public Text splitSubTotalText;
    public Text splitTipText;
    public GameObject tipCalPanel;
    public GameObject tipTotalPanel;
    public GameObject splitPanel;
    public GameObject tipBKPanel;
    public GameObject donatePanel;
    public Toggle defaultToggle;
    public Toggle secondToggle;
    public Toggle thirdToggle;
    public Button One;
    public Button Two;
    public Button Three;
    public Button Four;
    public Button Five;
    public Button Six;
    public Button Seven;
    public Button Eight;
    public Button Nine;
    public Button Zero;
    public Button decPoint;
    public Slider tipSlider;
    public Slider roundTipSlider;
    public Slider roundTotalSlider;
    public Slider splitSlider;
    private Decimal subTotal;
    private Decimal tipSource;
    private Decimal theTip;
    private Decimal theTotal;
    private Decimal splitValue;
    private int decCount;
    public static int showIAP;
    public static Boolean currentlyIAP;

    void Start()
    {
        deleteOldFile();
        currentlyIAP = false;
        tipCalPanel.SetActive(true);
        tipCalPanel.transform.SetAsLastSibling();
        tipTotalPanel.SetActive(false);
        donatePanel.SetActive(false);
        splitSlider.value = 1;
        openXML();
        dataNode = xDoc.SelectSingleNode("Root/data"); //Puts the <data> node from the xDoc into a single variable.
        showIAP = int.Parse(dataNode.Attributes["showiap"].Value);
        tipSlider.value = int.Parse(dataNode.Attributes["tip"].Value);
        try
        {
            roundTipSlider.value = int.Parse(dataNode.Attributes["roundtip"].Value);
        }
        catch
        {
            roundTipSlider.value = 1;
        }
        roundTotalSlider.value = int.Parse(dataNode.Attributes["roundtotal"].Value);
        //print("In AIP " + currentlyIAP + " // Start IAP? " + showIAP + " // Stored IAP " + (int.Parse(dataNode.Attributes["showiap"].Value)).ToString());
        decOff = false;
        decCount = 0;
        enterSubTotal.text = "";
        outPutText.text = "";
        noInputText.text = "0.00";
		cancelAmt();
        tipBKPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, ((670 - Screen.height))+120);
        tipBKPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (670 - Screen.height));
	}
	void Update()
	{
        if (tipTotalPanel.activeInHierarchy)
        {
            //theTip = subTotal * ((Decimal.Parse((tipSlider.value).ToString()))/100);
            tipTitleText.text = Math.Round(((theTip / subTotal) * 100)).ToString("#,##0") + "% Tip:";

            if (roundTotalSlider.value == 1 & splitSlider.value == 1)
            {
                roundTotal();
            }
        }
    }

    public void openXML() //XML Serialization
    {
        if (File.Exists(Application.persistentDataPath + "/tipcruncher.tc"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream tcFile = File.Open(Application.persistentDataPath + "/tipcruncher.tc", FileMode.Open);
            getXML = ((string)bf.Deserialize(tcFile));
            xDoc.Load(new StringReader(getXML));
            //print("Internal file was opened. " + xDoc.InnerXml);
            tcFile.Close();
        }
        else
        {
            xDoc.Load(Application.dataPath + "/Raw/data.txt"); //Pulls from Streaming Assets
            saveXML(xDoc);
            //print(xDoc.InnerXml + " File had to load from text file.");
        }
    }

    public void saveXML(XmlDocument xmlDoc) //XML Serialization
    {
        StringBuilder myXmlDoc = new StringBuilder(xmlDoc.InnerXml);
        getXML = myXmlDoc.ToString();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream tcFile = File.Create(Application.persistentDataPath + "/tipcruncher.tc");
        bf.Serialize(tcFile, getXML);
        tcFile.Close();
    }

    public void updatexDoc() //XML Serialization
    {
        if (dataNode != null)
        {
            dataNode.Attributes["tip"].Value = tipSlider.value.ToString();
            dataNode.Attributes["roundtip"].Value = roundTipSlider.value.ToString();
            dataNode.Attributes["roundtotal"].Value = roundTotalSlider.value.ToString();
            dataNode.Attributes["showiap"].Value = showIAP.ToString();
        }

    }

    void OnEnable()
    {
        One.onClick.AddListener(delegate { inputSubTotal("1"); });
        Two.onClick.AddListener(delegate { inputSubTotal("2"); });
        Three.onClick.AddListener(delegate { inputSubTotal("3"); });
        Four.onClick.AddListener(delegate { inputSubTotal("4"); });
        Five.onClick.AddListener(delegate { inputSubTotal("5"); });
        Six.onClick.AddListener(delegate { inputSubTotal("6"); });
        Seven.onClick.AddListener(delegate { inputSubTotal("7"); });
        Eight.onClick.AddListener(delegate { inputSubTotal("8"); });
        Nine.onClick.AddListener(delegate { inputSubTotal("9"); });
        Zero.onClick.AddListener(delegate { inputSubTotal("0"); });
        decPoint.onClick.AddListener(delegate { inputSubTotal("."); });
    }

    void OnApplicationPause()
    {
        if (currentlyIAP == false)
        {
            tipCalPanel.transform.SetAsLastSibling();
            cancelAmt();
            tipCalPanel.SetActive(true);
            tipTotalPanel.SetActive(false);
            donatePanel.SetActive(false);
            splitSlider.value = 1;
            if (dataNode != null)
            {
                cancelAmt();
                updatexDoc();
                saveXML(xDoc);
            }
        }
        else if (currentlyIAP == true)
        {
            tipTotalPanel.SetActive(true);
            tipTotalPanel.transform.SetAsLastSibling();
            tipCalPanel.SetActive(false);
            donatePanel.SetActive(false);
            updatexDoc();
            saveXML(xDoc);
        }
    }

    public void cancelAmt()
    {
        enterSubTotal.text = "";
        outPutText.text = "";
        noInputText.text = "0.00";
        decOff = false;
        decCount = 0;
    }

   public void inputSubTotal(string decNumber)
    {
        try
        {
            if (((enterSubTotal.text).ToString()).Length >= 5 & decOff == false & decNumber != ".")
            {
                decNumber = "";
            }

            if (((enterSubTotal.text).ToString()).Length < 1 & decNumber == "0")
            {
                decNumber = "";
                enterSubTotal.text = "";
                outPutText.text = "";
                noInputText.text = "0.00";
                decCount = 0;
            }

            if (decOff == false)
            {

                if (decNumber == ".")
                {
                    decOff = true;
                }
            }
            else if (decOff == true)
            {
                if (decNumber == ".")
                    decNumber = "";
                else
                    decCount += 1;

                if (decCount > 2)
                    decNumber = "";

            }

            enterSubTotal.text = enterSubTotal.text + decNumber.ToString();

            outPutText.text = (Decimal.Parse(enterSubTotal.text)).ToString("#,###.#0");
            //print("Decimal Count: " + decCount);
            noInputText.text = "";
        }
        catch
        {
            decNumber = "";
            enterSubTotal.text = "";
            outPutText.text = "";
            noInputText.text = "0.00";
            decCount = 0;
        }
    }
   public void roundTip()
   {
       subTotal = Decimal.Parse(enterSubTotal.text);

       if (roundTipSlider.value == 0)
       {
           dotheMath();
           doTheText();
           updatexDoc();
           saveXML(xDoc);
       }
       else if (roundTipSlider.value == 1)
       {
           roundTotalSlider.value = 0;
           theTip = tipSource / 100 * subTotal;
           Decimal dTip = Math.Round(theTip);
           theTip = Decimal.Parse(dTip.ToString());
           theTotal = theTip + subTotal;
           doTheText();
           updatexDoc();
           saveXML(xDoc);
       }
       splitTheBill();

   }
   public void roundTotal()
   {
        if (roundTotalSlider.value == 1 & splitSlider.value == 1)
        {
            roundTipSlider.value = 0;
            theTip = tipSource / 100 * subTotal;
            theTotal = Math.Round(theTip + subTotal);
            //print("theTip: "+ theTip + "theTotal: "+ theTotal);
            theTip = theTotal - subTotal;
            //print("theTip2: " + theTip + "theTotal2: " + theTotal);
            doTheText();
            updatexDoc();
            saveXML(xDoc);
        }
        else if (roundTotalSlider.value == 1 & splitSlider.value != 1)
        {
            roundTipSlider.value = 0;
            dotheMath();
            //Run Round Split Method
            roundSplit();
            doTheText();
            updatexDoc();
            saveXML(xDoc);
        }
        else if (roundTotalSlider.value == 0)
        {
            dotheMath();
            doTheText();
            updatexDoc();
            saveXML(xDoc);
        }
       splitTheBill();
   }

    public void roundSplit()
    {
        dotheMath();
        splitValue = Math.Round(theSplit(Decimal.Parse((splitSlider.value).ToString())));
        splitTotalText.text = splitValue.ToString("#,##0.00");
        theTotal = splitValue * Decimal.Parse((splitSlider.value).ToString());
        theTip = theTotal - subTotal;
        tipTotalText.text = theTip.ToString("#,##0.00");
        totalTotalText.text = theTotal.ToString("#,##0.00");
    }

   public void splitTheBill()
   {
       //print("SplitBill begin " + splitSlider.value);
       if (splitSlider.value == 1)
       {
           splitPanel.SetActive(false);
        }
       else
       {
           splitPanel.SetActive(true);
           splitTittle.text = "Totals for 1 out of " + splitSlider.value + ":";
           if(roundTotalSlider.value == 1)
                roundSplit();

            //Where Split Text is run a method for split text
            splitTotalText.text = (theSplit(Decimal.Parse((splitSlider.value).ToString()))).ToString("#,##0.00");
            splitSubTotalText.text = ((Decimal.Parse((supTotalText.text))) / (Decimal.Parse((splitSlider.value).ToString()))).ToString("#,##0.00");
            splitTipText.text = (Decimal.Parse(splitTotalText.text) - Decimal.Parse(splitSubTotalText.text)).ToString();
       }
       updatexDoc();
       saveXML(xDoc);
   }
    
   public decimal theSplit(decimal split)
   {

        split = theTotal / split;

        return split;
   }

   public void dotheMath()
   {

            theTip = tipSource / 100 * subTotal;
            theTotal = theTip + subTotal;

   }
   public void doTheText()
   {
       supTotalText.text = subTotal.ToString("#,##0.00");
       tipTotalText.text = theTip.ToString("#,##0.00");
       totalTotalText.text = theTotal.ToString("#,##0.00");
       //tipTitleText.text = (((tipSlider.value).ToString()) + "% Tip:");
   }
    public void calculateTip()
   {
       tipSource = Decimal.Parse(tipSlider.value.ToString());
       if (enterSubTotal.text != null)
       {
           try
           {
               roundTip();
               doTheText();
               enterTotal();
           }
           catch
           {
               print("You didn't enter any numbers!!!");
           }
       }

   }
   public void tip()
   {
       tipText.text = (tipSlider.value).ToString() + "%";
   }



   public void deleteXML() //XML Serialization
   {
       if (File.Exists(Application.persistentDataPath + "/tipcruncher.tc"))
       {
           File.Delete(Application.persistentDataPath + "/tipcruncher.tc");
           openXML();
       }
   }

    public void deleteOldFile()
    {
        if (File.Exists(Application.persistentDataPath + "/data.tc"))
        {
            File.Delete(Application.persistentDataPath + "/data.tc");
        }
    }

    public void enterTotal()
    {
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(1, 4);

        if (randomNumber == 3 & showIAP == 0)
        {
            enterDonate();
        }
        else
        {
            tipTotalPanel.SetActive(true);
            tipTotalPanel.transform.SetAsLastSibling();
            tipCalPanel.SetActive(false);
            donatePanel.SetActive(false);
            //print("Enter Total " + splitSlider.value);

            if (roundTipSlider.value == 1)
            {
                roundTip();
            }
            else if (roundTotalSlider.value == 1)
            {
                roundTotal();
            }
            //splitTheBill();
        }
    }
    public void enterfromDonate()
    {
        tipTotalPanel.SetActive(true);
        tipTotalPanel.transform.SetAsLastSibling();
        tipCalPanel.SetActive(false);
        donatePanel.SetActive(false);

        //print("Enter Total " + splitSlider.value);

        if (roundTipSlider.value == 1)
        {
            roundTip();
        }
        else if (roundTotalSlider.value == 1)
        {
            roundTotal();
        }
        splitTheBill();
        updatexDoc();
        saveXML(xDoc);
        StartCoroutine(waitForIAP());
    }

    public void enterKeyPad()
    {
        cancelAmt();
        tipCalPanel.SetActive(true);
        tipCalPanel.transform.SetAsLastSibling();
        tipTotalPanel.SetActive(false);
        donatePanel.SetActive(false);
        splitSlider.value = 1;
    }
    public void reenterKeyPad()
    {
        tipCalPanel.SetActive(true);
        tipCalPanel.transform.SetAsLastSibling();
        tipTotalPanel.SetActive(false);
        donatePanel.SetActive(false);
        updatexDoc();
        saveXML(xDoc);
    }
    public void enterDonate()
    {
        defaultToggle.isOn = true;
        secondToggle.isOn = false;
        thirdToggle.isOn = false;
        donatePanel.SetActive(true);
        donatePanel.transform.SetAsLastSibling();
        tipCalPanel.SetActive(false);
        tipTotalPanel.SetActive(false);
        updatexDoc();
        saveXML(xDoc);
    }

    IEnumerator waitForIAP()
    {
        yield return new WaitForSeconds(10f);
        currentlyIAP = false;
        //print("Waited Seconds before changing currentlyIAP to " + currentlyIAP);
    }

    public void quitApp()
    {
        updatexDoc();
        saveXML(xDoc);
        Application.Quit();
    }
}
