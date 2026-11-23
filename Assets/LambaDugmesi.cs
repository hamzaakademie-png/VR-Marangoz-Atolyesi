using UnityEngine;
public class LambaDugmesi : MonoBehaviour {
    public Light lamba;
    public Renderer ampul;
    public AudioSource tikSesi;
    public Color parlak = new Color(4f, 3.2f, 2f);
    void Awake() { if (!tikSesi) tikSesi = GetComponent<AudioSource>(); }
    public void Degistir() {
        lamba.enabled = !lamba.enabled;
        if (ampul) ampul.material.SetColor("_EmissionColor", lamba.enabled ? parlak : Color.black);
        if (tikSesi) tikSesi.Play();
    }
}