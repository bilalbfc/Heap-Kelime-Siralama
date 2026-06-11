# 📊 2-Anahtarlı Özel Heap ile Kelime Frekansı Analizi

Bu proje, verilen bir `.txt` dosyasındaki kelimelerin frekanslarını (dosyada kaçar kez geçtiklerini) sayan ve bu verileri **2 ayrı anahtarla çalışan özel bir Min-Heap** veri yapısı kullanarak sıralayan bir konsol uygulamasıdır. 

Kocaeli Sağlık ve Teknoloji Üniversitesi,  **Programlama Laboratuvarı II - Proje 3** isterleri doğrultusunda geliştirilmiştir.

---

## 🚀 Özellikler

* **Özel Veri Yapısı:** Hazır sıralama algoritmaları veya kütüphane koleksiyonları yerine tamamen dinamik olarak yönetilen, 2 anahtarlı özel bir Heap yapısı kullanılmıştır.
* **Çift Anahtarlı Öncelik Yönetimi:**
    * **Anahtar 1 (Birincil):** Kelimenin ilk harfi (A'dan Z'ye alfabetik artan sıra).
    * **Anahtar 2 (İkincil):** Kelimenin frekansı (Aynı harfle başlayan kelimeler için en yüksek frekanstan en düşüğe doğru azalan sıra).
    * *Bağ Kırıcı:* Hem ilk harfi hem frekansı aynı olan kelimeler için kelimenin kendi alfabetik sırası baz alınır.
* **Gelişmiş Terminal Arayüzü:** Renklendirilmiş çıktı yönetimi ve kelime dağılımlarını görselleştiren grafiksel frekans çubukları.
* **Adım Adım İzleme (Verbose Mode):** `-v` parametresiyle çalıştırıldığında, dosyadan okunan her kelimenin Heap ağacına eklenme, güncellenme ve yukarı kaydırılma (`HeapifyUp`) adımlarını anlık olarak terminale basar.
* **Türkçe Karakter Desteği:** `tr-TR` kültürü (Culture Info) entegrasyonu sayesinde `I-ı` ve `İ-i` gibi Türkçe karakter dönüşümleri hatasız gerçekleştirilir.

---

## 📐 Heap Çalışma Mantığı ve Yapısı

Program, dosyadan okunan her kelime için Heap yapısını dinamik olarak günceller. Eğer kelime Heap içinde zaten varsa frekansı artırılıp yukarı kaydırılır (`HeapifyUp`); ilk defa karşılaşıldıysa ağacın sonuna eklenip öncelik sırasına göre konumlandırılır.

Aşağıdaki şemada, `Kocaeli, Konya, Van, Ankara, Konya, Sivas, Adana, Adana` kelime dizisinin sisteme sırayla girilmesiyle oluşan 2 anahtarlı Heap ağacının adım adım evrimi gösterilmektedir:

<p align="center">
  <img src="https://raw.githubusercontent.com/EmirT41/Heap-Kelime-Siralama/main/`heap_step_by_step.jpg.png" alt="Heap Adım Adım Çalışma Mantığı" width="700"/>
</p>

*Not: Yukarıdaki görsel ödev dokümanında yer alan resmi Heap ağacı simülasyonunu temsil etmektedir.*

---

## 🛠️ Kurulum ve Çalıştırma

Proje **.NET 8.0** SDK kullanılarak geliştirilmiştir. Çalıştırmak için bilgisayarınızda .NET 8.0 sürümünün yüklü olduğundan emin olun.

1. Depoyu klonlayın veya zip olarak indirin:
   ```bash
   git clone [https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git)
   cd YOUR_REPO_NAME

2. Projeyi derleyin:
   ```bash
   dotnet build

3. Uygulamayı çalıştırın:
   ```bash
   dotnet run

## 📊 Örnek Girdi ve Çıktı

1. ornek.txt İçeriği:
Elma armut elma kiraz
armut kiraz kiraz elma
Üzüm elma üzüm

2. Terminal Çıktısı:
┌─────────────────────────────────────────┐
│  Dosya  : ornek.txt                     │
│  Satır  : 3                             │
└─────────────────────────────────────────┘

  ✔ Toplam kelime (tekrar dahil) : 11
  ✔ Benzersiz kelime sayısı      : 4
  ✔ En sık geçen kelime         : "elma" (4 kez)

  ┌──────────────────────────────────────────────────┐
  │   KELİME               FREKANS   GÖRSEL DAĞILIM  │
  ├──────────────────────────────────────────────────┤
  │   armut                    2     ██████████      │
  │   elma                     4     ████████████████████
  │   kiraz                    3     ███████████████ │
  │   üzüm                     2     ██████████      │
  └──────────────────────────────────────────────────┘
   


       
