using UnityEngine;
using TMPro;

[System.Serializable]
public class Urun {
    public string ad;
    public string alet;
    [TextArea] public string aciklama;
    public GameObject model;
    public Sprite resim;
}

public class Defter : MonoBehaviour {
    public Urun[] urunler;
    public TMP_Text baslik, aletYazisi, aciklamaYazisi, sayfaYazisi, durumYazisi;
    public UnityEngine.UI.Image resimAlani;
    public AudioSource sayfaSesi;
    public static Urun Secilen;
    int sayfa = 0;

        void Awake() { Secilen = null; }
    void Start() { Goster(); }
    public void Sonraki() { sayfa = (sayfa + 1) % urunler.Length; Goster(); Ses(); }
    public void Onceki() { sayfa = (sayfa - 1 + urunler.Length) % urunler.Length; Goster(); Ses(); }
    public void Sec() { Secilen = urunler[sayfa]; Goster(); Ses(); }

    void Ses() { if (sayfaSesi) sayfaSesi.Play(); }

    void Goster() {
        Urun u = urunler[sayfa];
        baslik.text = u.ad;
        aletYazisi.text = "Gerekli alet: " + u.alet;
        aciklamaYazisi.text = u.aciklama;
        sayfaYazisi.text = (sayfa + 1) + " / " + urunler.Length;
        durumYazisi.text = (Secilen == u) ? "Seçili ürün" : "";
        if (resimAlani) { resimAlani.sprite = u.resim; resimAlani.enabled = u.resim != null; }
    }
}