using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Provider;

public class Item : MonoBehaviour
{
    [SerializeField]
    private Transform slot;

    [SerializeField]
    public Vector3 itemPos;

    [SerializeField]
    public Quaternion itemRot;

    [SerializeField]
    public Vector3 itemScale;
    XRGrabInteractable grabInteractable;
    public bool isInSlot = true;
    private bool canBeDestroyed = false;
    private InputDevice device;
    private bool rotationActivated = false;
    private bool smartPlacementActivated = false; 
    private Vector3 itemLastRotation;
    private Vector3 itemLastPosition;
    // Start is called before the first frame update
    private bool isLocked = false;

    private AudioSource audioSourceGrab;
    private AudioSource audioSourceDrop;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        transform.localPosition = itemPos;
        transform.localRotation = itemRot;
        transform.localScale = itemScale;
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(PickUpItem);
        grabInteractable.selectExited.AddListener(PutDownItem);
        grabInteractable.activated.AddListener(SmartPlacement);
        grabInteractable.activated.AddListener(lockItem);

        audioSourceGrab = gameObject.AddComponent<AudioSource>();
        audioSourceGrab.clip = Resources.Load<AudioClip>("grab");
        audioSourceDrop = gameObject.AddComponent<AudioSource>();
        audioSourceDrop.clip = Resources.Load<AudioClip>("drop");
    }

    // Update is called once per frame
    void Update()
    {
        if (grabInteractable.isSelected)
        {
            itemLastPosition = gameObject.transform.position;
            itemLastRotation = gameObject.transform.eulerAngles;
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
        Vector3 itemRotation = transform.rotation.eulerAngles;
        itemRotation.x = Mathf.Round(itemRotation.x / 10) * 10;
        itemRotation.y = Mathf.Round(itemRotation.y / 10) * 10;
        itemRotation.z = Mathf.Round(itemRotation.z / 10) * 10;
        transform.rotation = Quaternion.Euler(itemRotation);
        transform.Rotate(Vector3.up, rotation);
        yield return new WaitForSeconds(0.1f);
        
    }

    IEnumerator RotateItemY(float rotation)
    {
        // Debug.Log("Rotating item");
        Vector3 itemRotation = transform.rotation.eulerAngles;
        itemRotation.x = Mathf.Round(itemRotation.x / 10) * 10;
        itemRotation.y = Mathf.Round(itemRotation.y / 10) * 10;
        itemRotation.z = Mathf.Round(itemRotation.z / 10) * 10;
        transform.rotation = Quaternion.Euler(itemRotation);
        transform.Rotate(Vector3.right, rotation);
        yield return new WaitForSeconds(0.1f);
        
    }

    public void lockItem(ActivateEventArgs args)
    {
        // Check if the activating controller is the left hand
        if (args.interactorObject.transform.parent.name == "Left Controller")
        {
            Debug.Log("Locked item: " + gameObject.name);
            isLocked = !isLocked;

            if (isLocked)
            {
                grabInteractable.trackPosition = false;
                grabInteractable.trackRotation = false;
                SetGlobalOpacity(0.7f);
            }
            else
            {
                grabInteractable.trackPosition = true;
                grabInteractable.trackRotation = true;
                SetGlobalOpacity(1.0f);
            }
        }
    }

    #region Design
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
                if (opacity < 1.0f)
                {
                    // Change le matériau en transparent
                    SetMaterialToTransparent(mat);
                }
                else
                {
                    // Change le matériau en opaque
                    SetMaterialToOpaque(mat);
                }

                // Vérifie si le shader du matériau possède la propriété "_BaseColor" (ou "_Color")
                if (mat.HasProperty("_BaseColor") || mat.HasProperty("_Color"))
                {
                    // Récupère la couleur actuelle du matériau
                    Color baseColor = mat.HasProperty("_BaseColor")
                        ? mat.GetColor("_BaseColor")
                        : mat.GetColor("_Color");

                    // Modifie l'alpha (opacité)
                    baseColor.a = opacity;

                    // Applique la nouvelle couleur avec l'alpha modifié
                    if (mat.HasProperty("_BaseColor"))
                    {
                        mat.SetColor("_BaseColor", baseColor);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        mat.SetColor("_Color", baseColor);
                    }
                }
            }
        }
    }

    // Fonction pour changer le mode de rendu du matériau en transparent
    void SetMaterialToTransparent(Material mat)
    {
        // Change le mode de rendu en Transparent avec Alpha Blending
        mat.SetFloat("_Surface", 1); // Surface Type : Transparent
        mat.SetFloat("_Blend", 0); // Blending Mode : Alpha
        mat.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0); // Désactiver l'écriture dans le Z-buffer
        mat.SetFloat("_AlphaClip", 0); // Alpha Clipping désactivé

        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_BLENDMODE_ALPHA");
        mat.DisableKeyword("_ALPHATEST_ON"); // Désactiver l'alpha test
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Désactiver le mode Alpha premultiplied

        // En fonction de tes paramètres, la face de rendu est "Front" seulement
        mat.SetFloat("_Cull", (int)UnityEngine.Rendering.CullMode.Back);

        // Configurer la file d'attente de rendu pour transparent
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    // Fonction pour changer le mode de rendu du matériau en opaque
    void SetMaterialToOpaque(Material mat)
    {
        // Rétablir le mode de rendu en opaque
        mat.SetFloat("_Surface", 0); // Surface Type : Opaque
        mat.SetFloat("_Blend", 0); // Blending Mode : Opaque
        mat.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One); // Source blend
        mat.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero); // Destination blend
        mat.SetFloat("_ZWrite", 1); // Activer l'écriture dans le Z-buffer
        mat.SetFloat("_AlphaClip", 1); // Alpha Clipping activé

        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHATEST_ON"); // Activer le test alpha
        mat.DisableKeyword("_BLENDMODE_ALPHA"); // Désactiver le blending alpha
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Désactiver le mode Alpha premultiplied

        // Rendre seulement la face avant (Cull Front)
        mat.SetFloat("_Cull", (int)UnityEngine.Rendering.CullMode.Back); // Garder "Front"

        // File d'attente de rendu pour opaque
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        // Paramètres additionnels selon ton image
        mat.SetFloat("_ReceiveShadows", 1); // Activer les ombres
    }

    #endregion

    

    public void PickUpItem(SelectEnterEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        if (pickedItem.GetComponent<Item>().isInSlot == true)
        {
            // Debug.Log("Picked up item: " + pickedItem.name);
            pickedItem.transform.localScale = Vector3.one;
            pickedItem.GetComponent<Item>().isInSlot = false;
            GameObject nvlobj = Instantiate(gameObject, slot, false);
            nvlobj.GetComponent<Item>().isInSlot = true;
        }
        audioSourceGrab.Play();
    }

    public void PutDownItem(SelectExitEventArgs args)
    {
        GameObject pickedItem = args.interactableObject.transform.gameObject;
        if (pickedItem != null)
        {
            pickedItem.tag = "LevelObject";
            if (canBeDestroyed)
            {
                Destroy(pickedItem);
            }
        }
        audioSourceDrop.Play();
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

    void SmartPlacement (ActivateEventArgs args)
    {
        if (args.interactorObject.transform.parent.name == "Right Controller")
        {
            Vector3 itemRotation = transform.rotation.eulerAngles;
            itemRotation.x = Mathf.Round(itemRotation.x / 10) * 10;
            itemRotation.y = Mathf.Round(itemRotation.y / 10) * 10;
            itemRotation.z = Mathf.Round(itemRotation.z / 10) * 10;
            transform.rotation = Quaternion.Euler(itemRotation);
            smartPlacementActivated = !smartPlacementActivated;
            if (!smartPlacementActivated)
            {
                grabInteractable.trackRotation = true;
            }
            else {
                grabInteractable.trackRotation = false;
            }
            Debug.Log("Smart Placement activated: " + smartPlacementActivated);
        }
    }
}
