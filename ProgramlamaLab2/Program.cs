using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.CompilerServices;

// Test projesinin internal üyelere erişebilmesi için
[assembly: InternalsVisibleTo("WordHeapProject.Tests")]

namespace WordHeapProject
{
    // ══════════════════════════════════════════════════════
    //  Heap içinde tutulacak düğüm (node) yapısı
    // ══════════════════════════════════════════════════════
    public class HeapNode
    {
        public string Word  { get; set; }
        public int    Count { get; set; }

        public HeapNode(string word)
        {
            Word  = word;
            Count = 1;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    //  2 Anahtarlı Özel Min-Heap
    //
    //  Anahtar 1 → Kelimenin ilk harfi   (A < B < C … alfabetik artan)
    //  Anahtar 2 → Frekans               (büyük frekans önce gelir)
    //  Bağ kırıcı → Kelime alfabetik sırası (a < b < c …)
    // ══════════════════════════════════════════════════════════════════════
    public class CustomHeap
    {
        private readonly List<HeapNode> _heap = new List<HeapNode>();

        // Verbose mod açıkken her adım ekrana yazılır
        public bool Verbose { get; set; } = false;

        public bool IsEmpty()  => _heap.Count == 0;
        public int  Size()     => _heap.Count;

        // ── Kelime işle: yeniyse ekle, varsa güncelle ──
        public void ProcessWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return;

            // Türkçe kültürüyle küçük harfe çevir (I→ı, İ→i vb.)
            word = word.ToLower(new CultureInfo("tr-TR"));

            int idx = FindIndex(word);

            if (idx != -1)
            {
                _heap[idx].Count++;

                if (Verbose)
                {
                    Print.Info($"  GÜNCELLEME  \"{word}\" → frekans: {_heap[idx].Count}  " +
                               $"(indeks {idx} → yukarı kaydırılıyor)");
                }

                HeapifyUp(idx);
            }
            else
            {
                _heap.Add(new HeapNode(word));
                int newIdx = _heap.Count - 1;

                if (Verbose)
                {
                    Print.Info($"  YENİ KELIME \"{word}\" → indeks {newIdx} 'e eklendi, yukarı kaydırılıyor");
                }

                HeapifyUp(newIdx);
            }

            if (Verbose)
            {
                Print.Dim($"  Heap durumu: [{string.Join(" | ", _heap.ConvertAll(n => $"{n.Word}:{n.Count}"))}]");
            }
        }

        // ── En yüksek öncelikli düğümü çek ──
        public HeapNode ExtractMin()
        {
            if (_heap.Count == 0) return null;

            HeapNode root = _heap[0];
            int last = _heap.Count - 1;
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);

            if (_heap.Count > 0) HeapifyDown(0);

            return root;
        }

        // ─────────────────── Özel istatistikler ───────────────────

        // En yüksek frekanslı kelimeyi kopyasını döndür (heap'i bozmaz)
        public HeapNode PeekTopFrequency()
        {
            // En yüksek frekans: ikinci anahtara göre tüm heap'i tara
            if (_heap.Count == 0) return null;
            HeapNode best = _heap[0];
            foreach (var node in _heap)
                if (node.Count > best.Count) best = node;
            return best;
        }

        // ─────────────────── Yardımcı metotlar ───────────────────

        private int FindIndex(string word)
        {
            for (int i = 0; i < _heap.Count; i++)
                if (_heap[i].Word == word) return i;
            return -1;
        }

        // ── 2-Anahtarlı Karşılaştırma ──
        // Dönüş < 0  →  a önce gelir (daha yüksek öncelik)
        // Dönüş > 0  →  b önce gelir
        private static int Compare(HeapNode a, HeapNode b)
        {
            char ca = a.Word[0];
            char cb = b.Word[0];

            // Anahtar 1: İlk harf – alfabetik artan (A önce)
            if (ca != cb)
                return ca.CompareTo(cb);

            // Anahtar 2: Frekans – azalan (büyük frekans önce)
            if (a.Count != b.Count)
                return b.Count.CompareTo(a.Count);

            // Bağ kırıcı: Kelime alfabetik artan
            return string.Compare(a.Word, b.Word, StringComparison.Ordinal);
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (Compare(_heap[i], _heap[parent]) < 0)
                {
                    if (Verbose)
                        Print.Dim($"    ↑ Swap: [{i}]\"{_heap[i].Word}\" ↔ [{parent}]\"{_heap[parent].Word}\"");
                    Swap(i, parent);
                    i = parent;
                }
                else break;
            }
        }

        private void HeapifyDown(int i)
        {
            int n = _heap.Count;
            while (true)
            {
                int best  = i;
                int left  = 2 * i + 1;
                int right = 2 * i + 2;

                if (left  < n && Compare(_heap[left],  _heap[best]) < 0) best = left;
                if (right < n && Compare(_heap[right], _heap[best]) < 0) best = right;

                if (best == i) break;
                Swap(i, best);
                i = best;
            }
        }

        private void Swap(int x, int y)
        {
            HeapNode tmp = _heap[x];
            _heap[x]     = _heap[y];
            _heap[y]     = tmp;
        }
    }

    // ══════════════════════════════════════════════════════
    //  Renkli terminal çıktısı için yardımcı sınıf
    // ══════════════════════════════════════════════════════
    public static class Print
    {
        public static void Header(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Success(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Warning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Dim(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Row(string word, int count, int maxCount)
        {
            // Frekansa göre renk: yüksek=yeşil, orta=sarı, düşük=beyaz
            if      (count == maxCount)        Console.ForegroundColor = ConsoleColor.Green;
            else if (count >= maxCount * 0.5)  Console.ForegroundColor = ConsoleColor.Yellow;
            else                               Console.ForegroundColor = ConsoleColor.White;

            // Görsel frekans çubuğu (max 20 karakter)
            int barLen = Math.Max(1, (int)Math.Round(count / (double)maxCount * 20));
            string bar = new string('█', barLen);

            Console.WriteLine($"  {word,-22} {count,5}   {bar}");
            Console.ResetColor();
        }

        public static void Prompt(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(text);
            Console.ResetColor();
        }
    }

    // ══════════════════════════════════════════════════════
    //  Ana Program
    // ══════════════════════════════════════════════════════
    public class Program
    {
        // Token'dan sadece harfleri ayıkla (noktalama, rakam vb. at)
        // NOT: Test projesinden erişilebilmesi için "internal" yapıldı.
        internal static string CleanWord(string token)
        {
            var sb = new StringBuilder(token.Length);
            foreach (char c in token)
                if (char.IsLetter(c)) sb.Append(c);
            return sb.ToString();
        }

        // ── Dosyayı oku, heap'i doldur, sonuçları yazdır ──
        public static async Task FindWordCountAsync(string filePath, bool verbose)
        {
            filePath = filePath.Trim();

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Dosya bulunamadı: \"{filePath}\"");

            var heap = new CustomHeap { Verbose = verbose };

            string[] lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            // ── Dosya bilgisi ──
            Console.WriteLine();
            Print.Header("┌─────────────────────────────────────────┐");
            Print.Header($"│  Dosya  : {Path.GetFileName(filePath),-31}│");
            Print.Header($"│  Satır  : {lines.Length,-31}│");
            Print.Header("└─────────────────────────────────────────┘");

            if (verbose)
            {
                Console.WriteLine();
                Print.Warning("  ── VERBOSE MOD: Her adım gösteriliyor ──");
                Console.WriteLine();
            }

            int totalWords   = 0;
            int uniqueWords  = 0;

            foreach (string line in lines)
            {
                string[] tokens = line.Split(
                    new[] { ' ', '\t', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (string token in tokens)
                {
                    string clean = CleanWord(token);
                    if (!string.IsNullOrEmpty(clean))
                    {
                        // Verbose modda yeni/güncelleme ayrımı için heap size'ı takip et
                        int sizeBefore = heap.Size();
                        heap.ProcessWord(clean);
                        if (heap.Size() > sizeBefore) uniqueWords++;
                        totalWords++;
                    }
                }
            }

            // ── İstatistik özeti ──
            HeapNode topNode = heap.PeekTopFrequency();
            Console.WriteLine();
            Print.Success($"  ✔ Toplam kelime (tekrar dahil) : {totalWords}");
            Print.Success($"  ✔ Benzersiz kelime sayısı      : {uniqueWords}");
            if (topNode != null)
                Print.Success($"  ✔ En sık geçen kelime         : \"{topNode.Word}\" ({topNode.Count} kez)");

            // ── Sonuç tablosu ──
            int maxCount = topNode?.Count ?? 1;

            Console.WriteLine();
            Print.Header("  ┌──────────────────────────────────────────────────┐");
            Print.Header("  │   KELİME               FREKANS   GÖRSEL DAĞILIM  │");
            Print.Header("  ├──────────────────────────────────────────────────┤");

            if (heap.IsEmpty())
            {
                Print.Warning("  │   (Geçerli kelime bulunamadı)                    │");
            }
            else
            {
                while (!heap.IsEmpty())
                {
                    HeapNode node = heap.ExtractMin();
                    Print.Row(node.Word, node.Count, maxCount);
                }
            }

            Print.Header("  └──────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        // ── Giriş noktası ──
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // ── Karşılama ekranı ──
            Console.Clear();
            Print.Header("╔══════════════════════════════════════════════╗");
            Print.Header("║     TXT Kelime Frekans Analizi  —  Heap      ║");
            Print.Header("║     2 Anahtarlı Min-Heap Uygulaması          ║");
            Print.Header("╚══════════════════════════════════════════════╝");
            Console.WriteLine();
            Print.Dim("  Komutlar:");
            Print.Dim("    <dosya adı>          → Normal mod");
            Print.Dim("    <dosya adı> -v       → Verbose mod (adım adım)");
            Print.Dim("    q  /  ç               → Çıkış");
            Console.WriteLine();

            while (true)
            {
                Print.Prompt("▶ Dosya adı veya tam yolu: ");
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                string lower = input.ToLower(CultureInfo.InvariantCulture);
                if (lower == "q" || lower == "ç" || lower == "c")
                {
                    Console.WriteLine();
                    Print.Success("  Programdan çıkılıyor... İyi çalışmalar!");
                    Console.WriteLine();
                    break;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Print.Warning("  Lütfen bir dosya adı giriniz.\n");
                    continue;
                }

                // Verbose flag kontrolü
                bool verbose = false;
                string filePath = input;

                if (input.EndsWith(" -v", StringComparison.OrdinalIgnoreCase))
                {
                    verbose  = true;
                    filePath = input[..^3].Trim();
                }

                // .txt uzantısını otomatik ekle
                if (!filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    filePath += ".txt";

                try
                {
                    await FindWordCountAsync(filePath, verbose);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine();
                    Print.Error($"  [HATA] {ex.Message}");
                    Console.WriteLine();
                }
                catch (UnauthorizedAccessException)
                {
                    Print.Error("  [HATA] Dosyaya erişim izni yok.\n");
                }
                catch (Exception ex)
                {
                    Print.Error($"  [BEKLENMEYEN HATA] {ex.Message}\n");
                }
            }
        }
    }
}
