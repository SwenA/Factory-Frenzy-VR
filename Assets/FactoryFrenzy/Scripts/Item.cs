using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private bool isLocked = false;

    void Start()
    {
        transform.localPosition = itemPos;
        transform.localRotation = itemRot;
        transform.localScale = itemScale;
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(PickUpItem);
        grabInteractable.selectExited.AddListener(PutDownItem);
        grabInteractable.activated.AddListener(lockItem);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void lockItem(ActivateEventArgs args)
    {
        // Check if the activating controller is the left hand
        if ( args.interactorObject.transform.parent.name == "Left Controller" )
        {
            Debug.Log("Locked item: " + gameObject.name);
            isLocked = !isLocked;

            if (isLocked)
            {
                grabInteractable.trackPosition = false;
                grabInteractable.trackRotation = false;
                SetGlobalOpacity(0.5f);
            }
            else
            {
                grabInteractable.trackPosition = true;
                grabInteractable.trackRotation = true;
                SetGlobalOpacity(1.0f);
            }
        }
    }

        // Fonction qui ajuste l'opacité de tous les matériaux d'un parent et de ses enfants
    void SetGlobalOpacity(float opacity)
    {
        // Récupère tous les renderers de l'objet parent et de ses enfants
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            // Pour chaque renderer, récupère tous les matériaux
            foreach (Material mat in renderer.materials)
            {
                // Vérifie si le shader du matériau possède la propriété "_GlobalOpacity"
                if (mat.HasProperty("Global Opacity"))
                {
                    // Change la valeur de l'opacité globale
                    mat.SetFloat("Global Opacity", opacity);
                }
            }
        }
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
