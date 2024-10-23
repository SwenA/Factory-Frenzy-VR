using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovePlat : MonoBehaviour
{
    public GameObject MoveToSphere;
    
    void Start() {
        GetComponent<XRGrabInteractable>().selectExited.AddListener(onExited);
    }

    void onExited(SelectExitEventArgs args) 
    {
        if (MoveToSphere == null)
        {
            instanciateSphere(gameObject.transform.position);
        }
    }
    
    public void instanciateSphere(Vector3 position) 
    {
        // instanciate the default sphere
        MoveToSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        MoveToSphere.transform.position = position;
        MoveToSphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        // add Xr grab interactable to the sphere
        MoveToSphere.AddComponent<XRGrabInteractable>();
        MoveToSphere.GetComponent<Rigidbody>().isKinematic = true;
        MoveToSphere.GetComponent<Rigidbody>().useGravity = false;
    }

}
