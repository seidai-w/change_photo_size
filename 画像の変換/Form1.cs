using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Reflection;


namespace 画像の変換
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Bitmap bitmap = new Bitmap(@""))
            {
                // ポップアップフォームを表示
                var popupForm = new size_pop(bitmap, 150, 150);
                popupForm.ShowDialog(); // モーダルで表示
            }
            //string inputFilePath = @"C:\path\to\input\image.jpg";  // 変換したい元の画像パス
            //string outputFilePath = @"C:\path\to\output\image.png"; // 出力する画像のパス
            //int newWidth = 800;  // 新しい幅
            //int newHeight = 600; // 新しい高さ

            //// 画像の拡張子とサイズを変換するメソッドを呼び出し
            //ConvertImageFormat(inputFilePath, outputFilePath, newWidth, newHeight);
        }

        static void ConvertImageFormat(string inputPath, string outputPath, int width, int height)
        {
            try
            {
                // 画像を読み込む
                using (Image image = Image.FromFile(inputPath))
                {
                    // 新しいサイズにリサイズ
                    using (Bitmap resizedImage = new Bitmap(image, new Size(width, height)))
                    {
                        // 出力先のディレクトリが存在しない場合は作成
                        string directory = Path.GetDirectoryName(outputPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        // 画像を指定したフォーマットで保存（例：PNG）
                        resizedImage.Save(outputPath, GetImageFormat(outputPath));
                        Console.WriteLine($"画像が変換されて保存されました: {outputPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
            }
        }

        // 出力パスの拡張子に基づいて適切なImageFormatを返す
        static ImageFormat GetImageFormat(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".tiff" => ImageFormat.Tiff,
                _ => throw new NotSupportedException($"この拡張子はサポートされていません: {extension}")
            };
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel1.BackColor = Color.FromArgb(50, 255, 0, 0);
        }

        private void panel1_DragOver(object sender, DragEventArgs e)
        {
            // ドラッグ対象が許可されている場合、カーソルをデフォルトに設定
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // コピーが許可された場合
                this.Cursor = Cursors.Default;   // デフォルトのカーソルに変更
                panel1.BackColor = Color.FromArgb(50, 255, 0, 0); // 背景色を変更
            }
            else
            {
                e.Effect = DragDropEffects.None; // 不許可の場合
                this.Cursor = Cursors.No; // 不可カーソルに変更
                panel1.BackColor = Color.FromArgb(50, 255, 0, 0); // 背景色を変更
            }
        }

        private void panel1_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.No;
            panel1.BackColor = SystemColors.Control;
        }

    }
}
