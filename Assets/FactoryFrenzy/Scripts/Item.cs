using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Provider;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform slot;
    [SerializeField] public Vector3 itemPos;
    [SerializeField] public Quaternion itemRot;
    [SerializeField] public Vector3 itemScale;
    XRGrabInteractable grabInteractable;
    public bool isInSlot = true;
    private bool canBeDestroyed = false;
    private InputDevice device;
    private bool rotationActivated = false;
    // Start is called before the first frame update
    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
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
        if (grabInteractable.isSelected)
        {
            bool primary2DAxisClick;
            Vector2 primary2DAxisValue;
            device.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out primary2DAxisClick);
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisValue);

            if (primary2DAxisClick && !rotationActivated)
            {
                // Debug.Log("Primary 2D Axis Click found: " + primary2DAxisClick);
                // Debug.Log("Primary 2D Axis Click: " + primary2DAxisClick + ", Rotation Activated: " + !rotationActivated);
                Debug.Log(primary2DAxisValue.x);
                rotationActivated = true;
                SmartPlacement(true);
                if (primary2DAxisValue.x > 0.5f)
                {
                    StartCoroutine(RotateItemX(10f));
                }
                else if (primary2DAxisValue.x < -0.5f)
                {
                    StartCoroutine(RotateItemX(-10f));
                }
                if (primary2DAxisValue.y > 0.5f)
                {
                    StartCoroutine(RotateItemY(10f));
                }
                else if (primary2DAxisValue.y < -0.5f)
                {
                    StartCoroutine(RotateItemY(-10f));
                }
            }
            if (!primary2DAxisClick && rotationActivated)
            {
                rotationActivated = false;
            }
        }
    }

    IEnumerator RotateItemX(float rotation)
    {
        // Debug.Log("Rotating item");
        
        transform.Rotate(Vector3.up, rotation);
        yield return new WaitForSeconds(0.1f);
        
    }

    IEnumerator RotateItemY(float rotation)
    {
        // Debug.Log("Rotating item");
        
        transform.Rotate(Vector3.right, rotation);
        yield return new WaitForSeconds(0.1f);
        
    }

    public void PickUpItem(SelectEnterEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        SmartPlacement(false);
        if (pickedItem.GetComponent<Item>().isInSlot == true)
        {
            // Debug.Log("Picked up item: " + pickedItem.name);
            pickedItem.transform.localScale = Vector3.one;
            pickedItem.GetComponent<Item>().isInSlot = false;
            GameObject nvlobj = Instantiate(gameObject, slot, false);
            nvlobj.GetComponent<Item>().isInSlot = true;
        }
    }

    public void PutDownItem(SelectExitEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        grabInteractable.trackPosition = false;
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

    void SmartPlacement (bool isActivated)
    {
        if (!isActivated)
        {
            grabInteractable.trackPosition = true;
            grabInteractable.trackRotation = true;
        }
        else {
            grabInteractable.trackPosition = false;
            grabInteractable.trackRotation = false;
        }
        Debug.Log("Smart Placement activated: " + isActivated);
    }
}
