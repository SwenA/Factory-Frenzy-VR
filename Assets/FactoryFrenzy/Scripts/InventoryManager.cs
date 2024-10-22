using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InventoryManager : MonoBehaviour
{
    public GameObject InvisibleSlot;
    private int nbSlots = 0;
    private int nbSlotsPerPage = 9;
    private int currentPage = 0;
    private int nbPages = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        nbSlots = gameObject.transform.childCount;
        nbPages = Mathf.CeilToInt((float)nbSlots / nbSlotsPerPage);
        int remainingSlots = nbSlotsPerPage - nbSlots%nbSlotsPerPage;
        for (int i = 0; i < remainingSlots; i++)
        {
            Instantiate(InvisibleSlot, transform, false);
        }
        UpdatePage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPage()
    {
        currentPage++;
        if (currentPage >= nbPages)
        {
            currentPage = 0;
        }
        UpdatePage();
    }

    public void PreviousPage()
    {
        currentPage--;
        if (currentPage < 0)
        {
            currentPage = nbPages - 1;
        }
        UpdatePage();
    }

    public void UpdatePage()
    {
        for (int i = 0; i < nbSlots; i++)
        {
            GameObject slot = transform.GetChild(i).gameObject;
            slot.SetActive(i >= currentPage * nbSlotsPerPage && i < (currentPage + 1) * nbSlotsPerPage);
        }
    }
}
