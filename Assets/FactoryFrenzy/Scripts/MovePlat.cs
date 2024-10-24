using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MovePlat : MonoBehaviour
{
    public GameObject MoveToSphere;
    public GameObject SpherePrefab;
    
    void Start() {
        GetComponent<XRGrabInteractable>().selectExited.AddListener(onExited);
        if (MoveToSphere != null)
        {   
            GetComponent<LineSnapper>().endPoint = MoveToSphere.transform;
        }
        else
        {
            // deactivates the line snapper
            GetComponent<LineSnapper>().enabled = false;
            // deactivates the line renderer
            GetComponent<LineRenderer>().enabled = false;
        }
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
        MoveToSphere = Instantiate(SpherePrefab, position, Quaternion.identity);

        // activate the line snapper
        GetComponent<LineSnapper>().enabled = true;
        // activate the line renderer
        GetComponent<LineRenderer>().enabled = true;

        // set the sphere as the new target
        GetComponent<LineSnapper>().endPoint = MoveToSphere.transform;
        
    }

}
