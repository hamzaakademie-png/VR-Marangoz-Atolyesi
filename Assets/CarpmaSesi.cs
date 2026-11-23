using UnityEngine;
public class CarpmaSesi : MonoBehaviour {
    public AudioSource ses;
    void Awake() { if (!ses) ses = GetComponent<AudioSource>(); }
    void OnCollisionEnter(Collision c) {
        Debug.Log(name + " çarptı, hız: " + c.relativeVelocity.magnitude);
        if (c.relativeVelocity.magnitude > 0.3f && ses) ses.Play();
    }
}