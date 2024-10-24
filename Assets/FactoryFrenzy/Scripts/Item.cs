using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

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
    private bool isLocked = false;

    private AudioSource audioSourceGrab;
    private AudioSource audioSourceDrop;

    void Start()
    {
        transform.localPosition = itemPos;
        transform.localRotation = itemRot;
        transform.localScale = itemScale;
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(PickUpItem);
        grabInteractable.selectExited.AddListener(PutDownItem);
        grabInteractable.activated.AddListener(lockItem);

        // add audio source
        audioSourceGrab = gameObject.AddComponent<AudioSource>();
        audioSourceGrab.clip = Resources.Load<AudioClip>("grab");
        audioSourceDrop = gameObject.AddComponent<AudioSource>();
        audioSourceDrop.clip = Resources.Load<AudioClip>("drop");
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
                // Change le matériau en transparent
                SetMaterialToTransparent(mat);

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
}
