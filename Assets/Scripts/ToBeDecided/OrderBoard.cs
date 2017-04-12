﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderBoard : MonoBehaviour {

    public static OrderBoard Instance;
    public GameObject order;

    private GameObject canvas;
    private GameObject panel;
    [HideInInspector]
    public const int maxNumberOfOrders = 12;
    private int currentNumberOfOrders;


    public int CurrentNumberOfOrders
    {
        get { return this.currentNumberOfOrders; }
    }

    void Awake()
    {
        Instance = this;
    }


	// Use this for initialization
	void Start () {

        canvas = transform.Find("Canvas").gameObject;
        panel = canvas.transform.Find("RequestPanel").gameObject;

        if (panel == null)
            Debug.Log("RequestPanel missing!");

	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.I))
        {
            SpawnOnBoard();

            
        }

        Debug.Log("orders: "+ currentNumberOfOrders);
        
	}
    

    public void SpawnOnBoard(/*Order o*/)
    {
        if(currentNumberOfOrders < maxNumberOfOrders)
        {
            GameObject g = Instantiate(order, panel.transform);
            GenerateOrderInfo(g);
            currentNumberOfOrders++;
        }

        
    }

    private void GenerateOrderInfo(GameObject g)
    {
        GameObject paper = g.transform.Find("Paper").gameObject;
        Text orderName = paper.transform.Find("OrderName").GetComponent<Text>();
        Text orderCost = paper.transform.Find("OrderCost").GetComponent<Text>();
        Text orderDuration = paper.transform.Find("OrderDuration").GetComponent<Text>();


    }

    // TO DO
    //public void SpawnOnBoard(Order r)
    //{
    //    Instantiate(order, panel.transform);

    //}
}
