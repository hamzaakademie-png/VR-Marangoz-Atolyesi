using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Yontma : MonoBehaviour {
    public XRSocketInteractor yuva;
    public TextMeshPro yazi;
    public ParticleSystem talas;
    public AudioSource vurusSesi;
    public AudioSource bitisSesi;
    public int gerekenVurus = 5;
    [Range(0.2f, 0.8f)] public float urunBaslangicBoyu = 0.4f;

    int vurus;
    float sonVurus;
    bool bitiyor;
    Urun sonSecilen;
    Transform onizleme;
    Vector3 kutukOlcek, urunOlcek;

    void Start() {
        DurumYaz();
        yuva.selectEntered.AddListener(_ => { Sifirla(); bitiyor = false; DurumYaz(); });
        yuva.selectExited.AddListener(_ => {
            bool b = bitiyor;
            Sifirla();
            bitiyor = false;
            if (!b) DurumYaz();
        });
    }

    void Update() {
        if (Defter.Secilen == sonSecilen) return;
        sonSecilen = Defter.Secilen;
        Sifirla();
        DurumYaz();
    }

    void Sifirla() {
        vurus = 0;
        if (onizleme) Destroy(onizleme.gameObject);
        onizleme = null;
    }

    void DurumYaz() {
        if (Defter.Secilen == null)
            Yaz("1. Defterden bir ürün seç\n2. Kütüğü mengeneye koy\n3. Doğru aletle vur");
        else if (yuva.hasSelection)
            Yaz("Kütük hazır. " + Defter.Secilen.alet + " ile vur!");
        else
            Yaz("Seçilen ürün: " + Defter.Secilen.ad + "\nŞimdi kütüğü mengeneye koy.");
    }

    void OnTriggerEnter(Collider other) {
        Alet alet = other.GetComponentInParent<Alet>();
        if (alet != null) Vur(alet);
    }

    public void Vur(Alet alet) {
        if (alet == null || bitiyor) return;
        if (Time.time - sonVurus < 0.3f) return;
        sonVurus = Time.time;

        if (Defter.Secilen == null) { Yaz("Önce defterden bir ürün seç."); return; }
        if (!yuva.hasSelection) { Yaz("Önce kütüğü mengeneye koy."); return; }
        if (alet.aletAdi != Defter.Secilen.alet) {
            Yaz("Yanlış alet!\n" + Defter.Secilen.ad + " için gereken: " + Defter.Secilen.alet);
            return;
        }

        Transform kutuk = yuva.firstInteractableSelected.transform;
        if (vurus == 0) Baslat(kutuk);

        vurus++;
        float p = (float)vurus / gerekenVurus;

        if (talas) talas.Play();
        if (vurusSesi) vurusSesi.Play();
        ParcaFirlat(kutuk);

        Vector3 kHedef = new Vector3(
            kutukOlcek.x * (1f - 0.55f * p),
            kutukOlcek.y * (1f - 0.2f * p),
            kutukOlcek.z * (1f - 0.55f * p));
        StartCoroutine(Olcekle(kutuk, kHedef, 0.2f));
        if (onizleme)
            StartCoroutine(Olcekle(onizleme, urunOlcek * Mathf.Lerp(urunBaslangicBoyu, 1f, p), 0.2f));

        if (vurus < gerekenVurus) {
            Yaz(Defter.Secilen.ad + " yontuluyor... " + vurus + " / " + gerekenVurus);
        } else {
            bitiyor = true;
            StartCoroutine(Bitir(kutuk));
        }
    }

    void Baslat(Transform kutuk) {
        kutukOlcek = kutuk.localScale;
        if (!Defter.Secilen.model) return;

        GameObject g = Instantiate(Defter.Secilen.model, kutuk.position, Quaternion.identity);
        g.name = Defter.Secilen.ad;
        urunOlcek = Defter.Secilen.model.transform.localScale;
        g.transform.localScale = urunOlcek * urunBaslangicBoyu;

        foreach (var c in g.GetComponentsInChildren<Collider>()) c.enabled = false;
        var tut = g.GetComponent<XRGrabInteractable>();
        if (tut) tut.enabled = false;
        var rb = g.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        g.SetActive(true);
        onizleme = g.transform;
    }

    void ParcaFirlat(Transform kutuk) {
        var mr = kutuk.GetComponent<Renderer>();
        for (int i = 0; i < 5; i++) {
            var parca = GameObject.CreatePrimitive(PrimitiveType.Cube);
            parca.transform.position = kutuk.position + Random.insideUnitSphere * 0.04f;
            parca.transform.rotation = Random.rotation;
            parca.transform.localScale = Vector3.one * Random.Range(0.012f, 0.028f);
            if (mr) parca.GetComponent<Renderer>().sharedMaterial = mr.sharedMaterial;
            var prb = parca.AddComponent<Rigidbody>();
            prb.AddForce((Random.onUnitSphere + Vector3.up) * 0.5f, ForceMode.Impulse);
            Destroy(parca, 2.5f);
        }
    }

    IEnumerator Olcekle(Transform t, Vector3 hedef, float sure) {
        if (!t) yield break;
        Vector3 bas = t.localScale;
        float z = 0f;
        while (t && z < sure) {
            z += Time.deltaTime;
            t.localScale = Vector3.Lerp(bas, hedef, z / sure);
            yield return null;
        }
        if (t) t.localScale = hedef;
    }

    IEnumerator Bitir(Transform kutuk) {
        yield return new WaitForSeconds(0.3f);
        Transform urun = onizleme;
        onizleme = null;

        if (kutuk) Destroy(kutuk.gameObject);
        if (urun) {
            urun.localScale = urunOlcek;
            foreach (var c in urun.GetComponentsInChildren<Collider>()) c.enabled = true;
            var tut = urun.GetComponent<XRGrabInteractable>();
            if (tut) tut.enabled = true;
        }
        if (bitisSesi) bitisSesi.Play();
        Yaz("Tebrikler! " + Defter.Secilen.ad + " hazır.\nYeni ürün için başka bir kütük koy.");
        vurus = 0;
    }

    void Yaz(string metin) { if (yazi) yazi.text = metin; }
}