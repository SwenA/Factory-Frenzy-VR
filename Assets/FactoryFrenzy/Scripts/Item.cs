using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform slot;
    [SerializeField] public Vector3 itemPos;
    [SerializeField] public Quaternion itemRot;
    [SerializeField] public Vector3 itemScale;
    XRGrabInteractable grabInteractable;
    public bool isInSlot = true;
    private bool canBeDestroyed = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = itemPos;
        transform.localRotation = itemRot;
        transform.localScale = itemScale;
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(PickUpItem);
        grabInteractable.selectExited.AddListener(PutDownItem);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickUpItem(SelectEnterEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        if (pickedItem.GetComponent<Item>().isInSlot == true)
        {
            Debug.Log("Picked up item: " + pickedItem.name);
            pickedItem.transform.localScale = Vector3.one;
            pickedItem.GetComponent<Item>().isInSlot = false;
            GameObject nvlobj = Instantiate(gameObject, slot, false);
            nvlobj.GetComponent<Item>().isInSlot = true;
        }
    }

    public void PutDownItem(SelectExitEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        if (pickedItem != null)
        {
            pickedItem.tag = "LevelObject";
            if(canBeDestroyed)
            {
                Destroy(pickedItem);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<InventoryManager>() != null)
        {
            canBeDestroyed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<InventoryManager>() != null)
        {
            canBeDestroyed = false;
        }
    }
}
