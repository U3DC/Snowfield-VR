﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AdventurerFSM))]
public class AdventurerAI : AI
{
    private List<Relation> actorRelations = new List<Relation>();
    [SerializeField]
    [Range(1, 99)]
    [Tooltip("How fast this guy finish quests 1 slowest")]
    private float efficiency = 20;
    [SerializeField]
    private List<Equipment> inventory = new List<Equipment>();
    private QuestBook questBook;

    protected override void Awake()
    {
        base.Awake();
        AddJob(JobType.ADVENTURER);
        GetSlots();
    }

    protected virtual void Start()
    {
        questBook = new QuestBook();
    }

    public QuestBook QuestBook
    {
        get
        {
            return questBook;
        }
    }
    public void EquipRandomWeapons()
    {
        foreach (Equipment equip in inventory)
        {
            if (equip.Slot == EquipSlot.EquipmentSlotType.LEFTHAND || equip.Slot == EquipSlot.EquipmentSlotType.RIGHTHAND)
                ChangeWield(Instantiate(equip));
        }
    }

    protected void GetSlots()
    {
        EquipSlot[] equipmentSlots = GetComponentsInChildren<EquipSlot>();
        foreach (EquipSlot slot in equipmentSlots)
        {
            switch (slot.CurrentType)
            {
                case EquipSlot.EquipmentSlotType.LEFTHAND:
                    leftHand = slot;
                    break;
                case EquipSlot.EquipmentSlotType.RIGHTHAND:
                    rightHand = slot;
                    break;
            }
        }
    }

    //public override void Interact(Actor actor)
    //{
    //    if (actor is Player)
    //        isConversing = true;
    //}
    //public void Interact()
    //{
    //    //IsConversing = true;

    //    //foreach (QuestEntryGroup<StoryQuest> group in questBook.StoryQuests)
    //    //{
    //    //    QuestEntry<StoryQuest> quest = questBook.GetCompletableQuest(group);
    //    //    if (quest != null)
    //    //    {
    //    //        Debug.Log("hit");
    //    //        UIManager.Instance.Instantiate(UIType.OP_YES_NO, quest.Quest.Name, quest.Quest.Dialog.Title, transform.position, Player.Instance.gameObject);
    //    //    }
    //    //}


    //}

    public override void ChangeWield(Equipment item)
    {
        switch (item.Slot)
        {
            case EquipSlot.EquipmentSlotType.LEFTHAND:
                if (leftHand.Item != null)
                    Destroy(leftHand.Item);
                leftHand.Item = item;
                leftHand.Item.Equip(leftHand.transform);

                break;

            case EquipSlot.EquipmentSlotType.RIGHTHAND:
                if (rightHand.Item != null)
                    Destroy(rightHand.Item);
                rightHand.Item = item;
                rightHand.Item.Equip(rightHand.transform);
                break;
        }
    }

    public bool GotLobang()
    {
        foreach (QuestEntryGroup<StoryQuest> group in questBook.StoryQuests)
        {
            if (questBook.GetCompletableQuest(group) != null || questBook.GetStartableQuest(group) != null)
                return true;
        }
        return false;
    }

    public void GetLobang()
    {
        
        foreach (QuestEntryGroup<StoryQuest> group in questBook.StoryQuests)
        {
            QuestEntry<StoryQuest> completableQuest = questBook.GetCompletableQuest(group);
            if (completableQuest != null)
            {
                OptionPane op = UIManager.Instance.Instantiate(UIType.OP_OK, "Quest", "Complete Quest: " + completableQuest.Quest.Name, transform.position, Player.Instance.transform, transform);
                op.SetEvent(OptionPane.ButtonType.Ok, CompleteQuestDelegate);
                StartCoroutine(HandleQuest(completableQuest, op));
                return;
            }

            QuestEntry<StoryQuest> startableQuest = questBook.GetStartableQuest(group);

            if (startableQuest != null)
            {
                OptionPane op = UIManager.Instance.Instantiate(UIType.OP_YES_NO, "Quest", "Start Quest: " + startableQuest.Quest.Name, transform.position, Player.Instance.transform, transform);
                op.SetEvent(OptionPane.ButtonType.Yes, StartQuestYESDelegate);
                op.SetEvent(OptionPane.ButtonType.No, StartQuestNODelegate);
                StartCoroutine(HandleQuest(startableQuest, op));
                return;
            }
        }
    }



    protected System.Collections.IEnumerator HandleQuest(QuestEntry<StoryQuest> parameter, OptionPane op)
    {
        isInteracting = true;
        currentQuestMethod = null;
        while (true)
        {
            if(currentQuestMethod != null)
            {
                currentQuestMethod(parameter);
                break;
            }
            else if (!isInteracting)
            {
                if (op)
                    op.ClosePane();
                break;
            }
            
            yield return new WaitForEndOfFrame();
        }
        isInteracting = false;
    }

    protected Action<QuestEntry<StoryQuest>> currentQuestMethod = null;

    public void StartQuestYESDelegate()
    {
        currentQuestMethod = StartQuest;
    }

    public void StartQuestNODelegate()
    {
        currentQuestMethod = RejectQuest;
    }

    public void CompleteQuestDelegate()
    {
        currentQuestMethod = EndQuest;
    }

    public void StartQuest(QuestEntry<StoryQuest> hunt)
    {
        hunt.StartQuest((int)(((hunt.Quest.RequiredLevel + 1) / (float)GetJob(JobType.ADVENTURER).Level) * (100 - efficiency)));
    }

    public void RejectQuest(QuestEntry<StoryQuest> hunt)
    {
        hunt.Checked = true;
    }

    public void EndQuest(QuestEntry<StoryQuest> hunt)
    {
        GetJob(JobType.ADVENTURER).GainExperience(hunt.Quest.Experience);
        QuestBook.RequestNextQuest(hunt);
    }

    public bool QuestProgress()
    {
        return questBook.QuestProgess();
    }

    public override void Interact(Actor actor)
    {
        StartCoroutine((currentFSM as AdventurerFSM).Interact(actor));
    }

    public void ResetQuestOnNewTownVisit()
    {
        questBook.ResetChecked();
    }

}
