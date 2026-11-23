using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AletEtiketi : MonoBehaviour {
    public float yukseklik = 0.12f;
    TextMeshPro etiket;
    bool uzerinde;

    void Awake() {
        var alet = GetComponent<Alet>();
        var go = new GameObject(name + "_Etiket");
        etiket = go.AddComponent<TextMeshPro>();
        etiket.text = alet ? alet.aletAdi : name;
        etiket.fontSize = 0.6f;
        etiket.alignment = TextAlignmentOptions.Center;
        etiket.color = Color.white;
        etiket.outlineWidth = 0.25f;
        etiket.outlineColor = Color.black;
        etiket.rectTransform.sizeDelta = new Vector2(1f, 0.2f);
        go.SetActive(false);

        var x = GetComponent<XRBaseInteractable>();
        x.hoverEntered.AddListener(_ => Goster(true));
        x.hoverExited.AddListener(_ => Goster(false));
    }

    void Goster(bool acik) {
        uzerinde = acik;
        if (etiket) etiket.gameObject.SetActive(acik);
    }

    void LateUpdate() {
        if (!uzerinde || !etiket) return;
        etiket.transform.position = transform.position + Vector3.up * yukseklik;
        var cam = Camera.main;
        if (cam) etiket.transform.rotation =
            Quaternion.LookRotation(etiket.transform.position - cam.transform.position);
    }

    void OnDestroy() {
        if (etiket) Destroy(etiket.gameObject);
    }
}