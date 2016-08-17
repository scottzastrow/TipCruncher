/*
 * 
 * Copyright (c) 2015 All Rights Reserved, VERGOSOFT LLC
 * Title: TipCruncher
 * Author: Scott Zastrow
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System.Collections;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    public Toggle defaultToggle;
    public Toggle secondToggle;
    public Toggle thirdToggle;
    public Text pOne;
    public Text pTwo;
    public Text pThree;

    private string[] theProducts = new string[2];

    void Awake()
    {
        IAPBuild();
    }

    void IAPBuild()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        try
        {
            builder.AddProduct("TC1", ProductType.Consumable, new IDs
        {
            {"TipCruncher Tip $.99", MacAppStore.Name}
        }
                );
            builder.AddProduct("TC5", ProductType.Consumable, new IDs
        {
            { "TipCruncher Tip $4.99", MacAppStore.Name}
        }
        );
            builder.AddProduct("TC10", ProductType.Consumable, new IDs
        {
            { "TipCruncher Tip $9.99", MacAppStore.Name}
        }
        );
        }
        catch
        {
            print("Builder Failed to Add Products.");
        }
        try
        {
            UnityPurchasing.Initialize(this, builder);
        }
        catch
        {
            print("Builder Failed to Initialize Products.");
        }
    }

    public void TipUs()
    {
        FileManager.currentlyIAP = true;
        if (defaultToggle.isOn == true)
            controller.InitiatePurchase("TC1");
        else if (secondToggle.isOn == true)
            controller.InitiatePurchase("TC5");
        else if (thirdToggle.isOn == true)
            controller.InitiatePurchase("TC10");
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
        
        foreach (var product in controller.products.all)
        {
            if (product.availableToPurchase)
            {
                Debug.Log(product.metadata.localizedTitle);
                Debug.Log(product.metadata.localizedDescription);
                Debug.Log(product.metadata.localizedPriceString);
            }
        }
        //print("TC1 is " + controller.products.WithID("TC1").metadata.localizedTitle);
        try
        {
            pOne.text = "Donate " + controller.products.WithID("TC1").metadata.localizedPriceString;
            pTwo.text = "Donate " + controller.products.WithID("TC5").metadata.localizedPriceString;
            pThree.text = "Donate " + controller.products.WithID("TC10").metadata.localizedPriceString;
        }
        catch
        {
            pOne.text = "Donate $.99 (USD)";
            pTwo.text = "Donate $4.99 (USD)";
            pThree.text = "Donate $9.99 (USD)";
        }

        //print("**********************************Unity is Ready to Make a Purchase*****************************");
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        //print("Initialization Error! Unity IAP encounters an unrecoverable initialization error.");
        print("Reason for Error: " + error);
        FileManager.currentlyIAP = false;
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        FileManager.showIAP = 1;
        
        //print("IAPManager Show IAP: " + FileManager.showIAP);
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        if (p == PurchaseFailureReason.PurchasingUnavailable)
        {
            // IAP may be disabled in device settings.
        }
        print("Purchase Failed.");
        //FileManager.currentlyIAP = false;
    }



}
