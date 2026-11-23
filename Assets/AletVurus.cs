using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AletVurus : MonoBehaviour {
    public float menzil = 0.45f;

    void Start() {
        GetComponent<XRGrabInteractable>().activated.AddListener(_ => Vur());
    }

    void Vur() {
       var yontma = FindAnyObjectByType<Yontma>();
        if (yontma == null) return;
        if (Vector3.Distance(transform.position, yontma.transform.position) > menzil) return;
        yontma.Vur(GetComponent<Alet>());
    }
}