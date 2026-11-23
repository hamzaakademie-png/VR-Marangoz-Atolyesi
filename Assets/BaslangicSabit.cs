using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BaslangictaSabit : MonoBehaviour {
    void Start() {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        var tut = GetComponent<XRGrabInteractable>();
        tut.selectExited.AddListener(_ => {
            rb.isKinematic = false;
            rb.useGravity = true;
        });
    }
}