﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableSlot : VR_Interactable_UI
{

	[SerializeField] private Image image;
	[SerializeField] private Text stack;
	private int index;
	private VR_Interactable_Object pendingItem;


	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		GetComponent<BoxCollider>().isTrigger = true;

	}

    public int Index
    {
        get { return this.index; }
        set { this.index = value; }
    }

	protected override void Update()
	{
		base.Update();
		DisplayInfo();
	}


	private void DisplayInfo()
	{
		if (GetReferredSlot().StoredItem != null)
		{
			image.color = new Color(1, 1, 1, 1);
			stack.color = new Color(1, 1, 1, 1);
			//temp = image.color;
			//temp.a = 1;
			//image.color = temp;
			image.sprite = GetReferredSlot().StoredItem.Icon;
			stack.text = GetReferredSlot().CurrentStack.ToString();


        }
        else
        {
            image.color = new Color(0, 0, 0, 0);
            stack.color = new Color(0, 0, 0, 0);
        }



    }

    // To get the index of the referenced slot,
    // we take the (page number) * (number of slots per page) - (number of slots  - current index) - 1
    private Inventory.InventorySlot GetReferredSlot()
    {
        int referencedIndex = (StoragePanel.Instance.CurrentPageNumber) * (StoragePanel.Instance.NumberOfSlotsPerPage) + index;
        // - (StoragePanel.Instance.NumberOfSlotsPerPage - (index + 1)) - 1;

        return StoragePanel.Instance._Inventory.InventoryItemsArr[referencedIndex];



    }


	}

	// To get the index of the referenced slot,
	// we take the (page number) * (number of slots per page) - (number of slots  - current index) - 1
	private Inventory.InventorySlot GetReferredSlot()
	{
		int referencedIndex = (StoragePanel.Instance.CurrentPageNumber) * (StoragePanel.Instance.NumberOfSlotsPerPage) + index;
		// - (StoragePanel.Instance.NumberOfSlotsPerPage - (index + 1)) - 1;

        Inventory.InventorySlot temp = GetReferredSlot();

        if (temp.StoredItem != null)
        {
            temp.CurrentStack--;

			GenericItem instanceInteractable = Instantiate(temp.StoredItem.ObjectReference).GetComponent<GenericItem>();

            instanceInteractable.OnTriggerPress(currentInteractingController);
            if (temp.CurrentStack < 1)
            {
                temp.EmptySlot();
            }
        }


    }


    private void AddToSlot(IStorable item)
    {
        Inventory.InventorySlot temp = GetReferredSlot();

        if (temp.StoredItem == null)
        {

            temp.StoredItem = item;
            temp.CurrentStack++;
            currentInteractingController = null;

        }
        else if (temp.StoredItem.ItemID == item.ItemID)
        {
            if (temp.CurrentStack < temp.StoredItem.MaxStackSize)
            {

                temp.CurrentStack++;
            }
            else
            {
                //show red outline
            }
        }



    }

	}



	private void RemoveFromSlot()
	{

		Inventory.InventorySlot temp = GetReferredSlot();

            RemoveFromSlot();
        }
    }

    protected override void OnTriggerRelease()
    {

        if (currentInteractingController.UI == this)
        {

            GenericItem g = currentInteractingController.GetComponentInChildren<GenericItem>();
            Debug.Log(g);
            if (g)
            {
                ItemData d = ItemManager.Instance.GetItemData(g.ItemID);
                
                if (d != null && (GetReferredSlot().StoredItem == null || GetReferredSlot().StoredItem.ItemID == d.ItemID))
                {
                    currentInteractingController.SetModelActive(true);
                    AddToSlot(d);
                    Destroy(g.gameObject);
                }

            }

        }
    }

			instanceInteractable.OnTriggerPress(currentInteractingController);

			if (temp.CurrentStack < 1)
			{
				temp.EmptySlot();
			}
		}


	}


	private void AddToSlot(IStorable item)
	{
		Inventory.InventorySlot temp = GetReferredSlot();

		if (temp.StoredItem == null)
		{

			temp.StoredItem = item;
			temp.CurrentStack++;
			currentInteractingController = null;

		}
		else if (temp.StoredItem.ItemID == item.ItemID)
		{
			if (temp.CurrentStack < temp.StoredItem.MaxStackSize)
			{

				temp.CurrentStack++;
			}
			else
			{
				//show red outline
			}
		}



	}

	//protected override void OnTriggerEnter(Collider other)
	//{
	//    storagePanel.NumberOfHoveredSlots++;
	//}

	//protected override void OnTriggerExit(Collider other)
	//{
	//    storagePanel.NumberOfHoveredSlots--;
	//}



	protected override void OnTriggerPress()
	{
		if (currentInteractingController.UI == this && !currentInteractingController.InteractedObject)
		{
			base.OnTriggerPress();
			RemoveFromSlot();
		}
	}

	protected override void OnTriggerRelease()
	{
		
		if (currentInteractingController.UI == this)
		{
			Debug.Log("hittttt");
			GenericItem g = null;
			if (currentInteractingController.InteractedObject is GenericItem)
			{
				 g = currentInteractingController.InteractedObject as GenericItem;
			}
			Debug.Log(g);
			if (g)
			{
				ItemData d = ItemManager.Instance.GetItemData(g.ItemID);

				if (d != null && (GetReferredSlot().StoredItem == null || GetReferredSlot().StoredItem.ItemID == d.ItemID))
				{
					currentInteractingController.SetModelActive(true);
					AddToSlot(d);
					Destroy(g.gameObject);
				}

			}
			base.OnTriggerRelease();

		}
	}

}

	public int Index
	{
		get { return this.index; }
		set { this.index = value; }
	}
		}
		else
		{
			image.color = new Color(0, 0, 0, 0);
			stack.color = new Color(0, 0, 0, 0);
		}
		return StoragePanel.Instance._Inventory.InventoryItemsArr[referencedIndex];
		if (temp.StoredItem != null)
		{
			temp.CurrentStack--;
			GenericItem instanceInteractable = Instantiate(temp.StoredItem.ObjectReference, currentInteractingController.transform.position,
				currentInteractingController.transform.rotation)
				.GetComponent<GenericItem>();