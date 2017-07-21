﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OrderSlip : VR_Interactable_UI
{

    private string o_name;
    private int reward;
    private int duration;
    private Sprite image;
    private Action<bool, OrderSlip> callback;
    private Text durationText;
    private Order order;
    private AdventurerAI ai;
    [SerializeField] GameObject detailPane;
    OptionPane currentOP;

    public AdventurerAI OrderedAI
    {
        get { return this.ai; }
        set { this.ai = value; }
    }

    GameObject slip;

    public int Reward
    {
        get
        {
            return reward;
        }
    }
    // Use this for initialization
    public void StartOrder(Order o, Action<bool, OrderSlip> _callback)
    {
        order = o;

        o_name = order.Name;
        reward = order.GoldReward;
        duration = order.Duration;
        callback = _callback;

        slip = transform.Find("Slip").gameObject;
        slip.transform.Find("OrderName").GetComponent<Text>().text = o_name;
        slip.transform.Find("OrderCost").GetComponent<Text>().text = reward.ToString();
        durationText = slip.transform.Find("OrderDuration").GetComponent<Text>();
        slip.transform.Find("OrderImage").GetComponent<Image>().sprite = o.Sprite;
        slip.gameObject.SetActive(false);
        StartCoroutine(OrderCoroutine());
    }

    public void ShowOrderInformation()
    {
        slip.gameObject.SetActive(true);
    }



    private IEnumerator OrderCoroutine()
    {
        while (true)
        {
            durationText.text = duration.ToString();
            yield return new WaitForSeconds(1);
            duration--;
            if (duration <= 0)
            {
                OrderEnd(false);
            }
        }
    }



    private void OrderEnd(bool success)
    {
        StopAllCoroutines();
        callback(success, this);
        OrderBoard.Instance.RemoveFromBoard(this);
    }

    Weapon interactingWeapon;

    private void DisplayOptions()
    {
        OptionPane op = UIManager.Instance.InstantiateOptions(transform.position, Player.Instance.transform, transform);
        interactingWeapon = currentInteractingController.GetComponentInChildren<Weapon>();
        currentOP = op;
        op.SetEvent(OptionPane.ButtonType.Yes, TryConfirmOrder);
        op.SetEvent(OptionPane.ButtonType.No, SpawnDetailsPanel);
        op.SetEvent(OptionPane.ButtonType.Cancel, CloseOptions);
        
    }

    private void CloseOptions()
    {
        Debug.Log("Closing option");
        if (currentOP)
        {
            currentOP.ClosePane();
            currentOP.Destroy();
        }
    }

    private void TryConfirmOrder()
    {

        Debug.Log("try to confirm");
       

        if (interactingWeapon)
        {
            if (interactingWeapon.ItemID == order.ItemID)
            {
                OrderEnd(true);
                GameManager.Instance.AddPlayerGold(reward);
                Destroy(interactingWeapon.gameObject);
                currentInteractingController.Model.SetActive(true);
                Debug.Log("correcy");
            }

        }
        else
        {
            Debug.Log("wrong?");
        }

    }

    private void SpawnDetailsPanel()
    {
        Debug.Log("spawn detail");
        string desc = "Name: " + o_name;


        OptionPane op = UIManager.Instance.InstantiateDetailPane(detailPane, desc, reward.ToString(), transform.position, Player.Instance.transform, transform);
        op.SetEvent(OptionPane.ButtonType.Ok, CloseOptions);

        currentOP.ClosePane();
    }



    protected override void OnTriggerPress()
    {

        if (currentInteractingController.UI == this)
        {
            DisplayOptions();
        }

    }


    protected override void OnControllerEnter()
    {
        base.OnControllerEnter();

    }

    protected override void OnControllerExit()
    {
        base.OnControllerExit();
    }







}
