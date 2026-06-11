using System;
using System.Collections.Generic;
using Xunit;
using WordHeapProject;

namespace WordHeapProject.Tests
{
    // ══════════════════════════════════════════════════════
    //  CustomHeap (2 Anahtarlı Heap) Testleri
    // ══════════════════════════════════════════════════════
    public class CustomHeapTests
    {
        // ── Yeni heap boş olmalı ──
        [Fact]
        public void YeniHeap_BosOlmali()
        {
            var heap = new CustomHeap();

            Assert.True(heap.IsEmpty());
            Assert.Equal(0, heap.Size());
        }

        // ── Tek kelime eklenince heap boş olmamalı ve frekans 1 olmalı ──
        [Fact]
        public void TekKelimeEkle_FrekansBirOlmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("elma");

            Assert.False(heap.IsEmpty());
            Assert.Equal(1, heap.Size());

            HeapNode node = heap.ExtractMin();
            Assert.Equal("elma", node.Word);
            Assert.Equal(1, node.Count);
        }

        // ── Aynı kelime birden fazla kez eklenince frekans artmalı ──
        [Fact]
        public void AyniKelimeTekrarEklenince_FrekansArtmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("elma");
            heap.ProcessWord("elma");
            heap.ProcessWord("elma");

            // Heap boyutu 1 olmalı (tek düğüm, sadece sayaç arttı)
            Assert.Equal(1, heap.Size());

            HeapNode node = heap.ExtractMin();
            Assert.Equal("elma", node.Word);
            Assert.Equal(3, node.Count);
        }

        // ── Büyük/küçük harf duyarsızlık (Türkçe karakterler dahil) ──
        [Theory]
        [InlineData("Elma", "elma")]
        [InlineData("ELMA", "elma")]
        [InlineData("İSTANBUL", "istanbul")]   // İ -> i (tr-TR kuralı)
        [InlineData("Üzüm", "üzüm")]
        public void BuyukKucukHarf_AyniKelimeOlarakSayilmali(string input1, string expectedLower)
        {
            var heap = new CustomHeap();
            heap.ProcessWord(input1);
            heap.ProcessWord(expectedLower);

            // İkisi de aynı kelimeye indirgenmeli -> tek düğüm, frekans 2
            Assert.Equal(1, heap.Size());

            HeapNode node = heap.ExtractMin();
            Assert.Equal(expectedLower, node.Word);
            Assert.Equal(2, node.Count);
        }

        // ── Anahtar 1: Farklı ilk harfler -> alfabetik sırayla çıkmalı ──
        [Fact]
        public void FarkliIlkHarfler_AlfabetikSirayaGoreCikmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("zebra");
            heap.ProcessWord("elma");
            heap.ProcessWord("armut");
            heap.ProcessWord("kiraz");

            var sira = new List<string>();
            while (!heap.IsEmpty())
                sira.Add(heap.ExtractMin().Word);

            Assert.Equal(new List<string> { "armut", "elma", "kiraz", "zebra" }, sira);
        }

        // ── Anahtar 2: Aynı ilk harf -> frekansa göre azalan sıralanmalı ──
        [Fact]
        public void AyniIlkHarf_FrekansaGoreAzalanSiralanmali()
        {
            var heap = new CustomHeap();

            // "elma" 1 kez, "ekmek" 3 kez
            heap.ProcessWord("elma");
            heap.ProcessWord("ekmek");
            heap.ProcessWord("ekmek");
            heap.ProcessWord("ekmek");

            HeapNode first  = heap.ExtractMin();
            HeapNode second = heap.ExtractMin();

            // Frekansı yüksek olan (ekmek:3) önce gelmeli
            Assert.Equal("ekmek", first.Word);
            Assert.Equal(3, first.Count);

            Assert.Equal("elma", second.Word);
            Assert.Equal(1, second.Count);
        }

        // ── Bağ kırıcı: Aynı harf + aynı frekans -> alfabetik sıralanmalı ──
        [Fact]
        public void AyniHarfAyniFrekans_AlfabetikSiralanmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("elma");
            heap.ProcessWord("elbise");
            heap.ProcessWord("ekmek");
            // Üçü de "e" ile başlıyor ve frekansları 1

            var sira = new List<string>();
            while (!heap.IsEmpty())
                sira.Add(heap.ExtractMin().Word);

            Assert.Equal(new List<string> { "ekmek", "elbise", "elma" }, sira);
        }

        // ── Karma senaryo: Birden fazla harf + farklı frekanslar ──
        [Fact]
        public void KarisikSenaryo_DogruSiraylaCikmali()
        {
            var heap = new CustomHeap();

            // "Elma armut elma kiraz / armut kiraz kiraz elma / Üzüm elma üzüm"
            string[] kelimeler =
            {
                "elma", "armut", "elma", "kiraz",
                "armut", "kiraz", "kiraz", "elma",
                "üzüm", "elma", "üzüm"
            };

            foreach (var kelime in kelimeler)
                heap.ProcessWord(kelime);

            var sonuc = new List<(string Word, int Count)>();
            while (!heap.IsEmpty())
            {
                var node = heap.ExtractMin();
                sonuc.Add((node.Word, node.Count));
            }

            var beklenen = new List<(string, int)>
            {
                ("armut", 2),
                ("elma", 4),
                ("kiraz", 3),
                ("üzüm", 2),
            };

            Assert.Equal(beklenen, sonuc);
        }

        // ── Boş heap'te ExtractMin null dönmeli ──
        [Fact]
        public void BosHeapte_ExtractMin_NullDonmeli()
        {
            var heap = new CustomHeap();
            HeapNode node = heap.ExtractMin();

            Assert.Null(node);
        }

        // ── Boş/whitespace kelimeler heap'e eklenmemeli ──
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BosVeyaWhitespaceKelime_HeapeEklenmemeli(string kelime)
        {
            var heap = new CustomHeap();
            heap.ProcessWord(kelime);

            Assert.True(heap.IsEmpty());
        }

        // ── PeekTopFrequency: en yüksek frekanslı kelimeyi doğru bulmalı ──
        [Fact]
        public void PeekTopFrequency_EnSikKelimeyiBulmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("zebra");
            heap.ProcessWord("zebra");
            heap.ProcessWord("zebra");
            heap.ProcessWord("armut");

            HeapNode top = heap.PeekTopFrequency();

            Assert.Equal("zebra", top.Word);
            Assert.Equal(3, top.Count);

            // Heap'i bozmamalı (boyut hala 2)
            Assert.Equal(2, heap.Size());
        }

        // ── Tüm kelimeler çekildikten sonra heap tekrar boş olmalı ──
        [Fact]
        public void TumElemanlarCekildiktenSonra_HeapBosOlmali()
        {
            var heap = new CustomHeap();
            heap.ProcessWord("a");
            heap.ProcessWord("b");
            heap.ProcessWord("c");

            heap.ExtractMin();
            heap.ExtractMin();
            heap.ExtractMin();

            Assert.True(heap.IsEmpty());
            Assert.Null(heap.ExtractMin());
        }
    }

    // ══════════════════════════════════════════════════════
    //  CleanWord (Noktalama Temizleme) Testleri
    // ══════════════════════════════════════════════════════
    public class CleanWordTests
    {
        [Theory]
        [InlineData("merhaba", "merhaba")]
        [InlineData("merhaba.", "merhaba")]
        [InlineData("merhaba!", "merhaba")]
        [InlineData("(merhaba)", "merhaba")]
        [InlineData("\"merhaba\",", "merhaba")]
        [InlineData("123", "")]
        [InlineData("kelime123", "kelime")]
        [InlineData("...", "")]
        [InlineData("Çiçek!", "Çiçek")]
        [InlineData("güneş.", "güneş")]
        public void CleanWord_NoktalamaVeRakamlariTemizler(string girdi, string beklenen)
        {
            string sonuc = Program.CleanWord(girdi);
            Assert.Equal(beklenen, sonuc);
        }

        [Fact]
        public void CleanWord_BosString_BosDonmeli()
        {
            Assert.Equal("", Program.CleanWord(""));
        }
    }
}
