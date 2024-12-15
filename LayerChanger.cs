using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerChanger : MonoBehaviour
{
    public void ChangeToOutline(GameObject objectToChange)
    {
        int layer = LayerMask.NameToLayer("Outline");
        ChangeLayer(objectToChange, layer);
    }

    public void ChangeToDefault(GameObject objectToChange)
    {
        int layer = LayerMask.NameToLayer("Default");
        ChangeLayer(objectToChange, layer);

    }

    private void ChangeLayer(GameObject objectToChange, int layer)
    {
        #if UNITY_EDITOR
        Debug.Log("Changing " + objectToChange.name + "'s layer to " + layer);
        #endif

        objectToChange.layer = layer;
        foreach (Transform tChild in objectToChange.transform)
        {
            tChild.gameObject.layer = layer;

            Transform tSubChild = tChild.GetComponentInChildren<Transform>();
            if (tSubChild != null)
                ChangeLayer(tChild.gameObject, layer);
        }

    }
}
