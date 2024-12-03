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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;


namespace 画像の変換
{
    public partial class Form1 : Form
    {
        List<string> extent = new List<string>()
        {
            ".jpeg", ".jpg", ".png", ".gif", ".ico", ".tiff", ".bmp"
        };

        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            // 最小化ボタンを無効化
            this.MinimizeBox = false;
            hide_choice(false);
            foreach (var ext in extent)
            {
                comboBox1.Items.Add(ext);
            }
            comboBox1.SelectedItem = comboBox1.Items[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (extent.Contains(Path.GetExtension(textBox4.Text)) && File.Exists(textBox4.Text))
            {
                var tmp = Get_size(textBox4.Text);
                if (!(bool)tmp[0])
                {
                    return;
                }
                
                using (Bitmap bitmap = new Bitmap(textBox4.Text))
                {
                    // ポップアップフォームを表示
                    var popupForm = new size_pop(bitmap, (int)tmp[1], (int)tmp[2]);
                    popupForm.ShowDialog(); // モーダルで表示
                }
            }
        }
        /// <summary>
        /// ファイルパスを出力
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        private string get_path(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                MessageBox.Show("ファイルが存在しません", "情報", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }

            //出力先の取得
            string outputPath = "";
            if (radioButton1.Checked)
            {
                outputPath = Path.GetDirectoryName(inputPath);
            }
            else if (radioButton2.Checked && textBox1.Text != "")
            {
                if (textBox1.Text.EndsWith("\\"))
                {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                }
                outputPath = textBox1.Text;
            }
            else
            {
                MessageBox.Show("保存先のディレクトリが入力されていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            string file_name = Path.GetFileNameWithoutExtension(inputPath);
            string baseFileName = textBox2.Text + file_name + textBox3.Text + comboBox1.SelectedItem.ToString();
            // ナンバリングを追加したファイル名を生成
            string save_name = GenerateUniqueFileName(outputPath, baseFileName);
            string save_path = $"{outputPath}\\{save_name}";
            return save_path;
        }

        /// <summary>
        /// 画像のリサイズ
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns ret[0]="成功したか"></returns>
        /// <returns ret[1]=width></returns>
        /// <returns ret[2]=heigit></returns>
        /// <returns ret[3]=元のサイズよりも大きいか></returns>
        List<object> Get_size(string inputPath)
        {
            List<object> result = new List<object>() { false, 0, 0, false };
            int _width = 0;
            int _height = 0;
            int origin_width = 0;
            int origin_height = 0;
            bool begger = false;
            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(inputPath))
                {
                    origin_width = image.Width;
                    origin_height = image.Height;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("画像の読み込みに失敗しました: " + ex.Message.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return result;
            }


            //サイズの取得
            if (user_choice.Checked)
            {
                if (width.Text == "")
                {
                    MessageBox.Show("横のサイズが入力されていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                else
                {
                    bool success = int.TryParse(width.Text, out _width);

                    if (!success || _width < 1)
                    {
                        MessageBox.Show("横のサイズが正常に入力されていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }

                }

                if (height.Text == "")
                {
                    MessageBox.Show("縦のサイズが入力されていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                else
                {
                    bool success = int.TryParse(height.Text, out _height);

                    if (!success || _height < 1)
                    {
                        MessageBox.Show("縦のサイズが正常に入力されていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                if (_width > origin_width || _height > origin_height)
                {
                    begger = true;
                }
            }
            else if (radioButton30.Checked)
            {
                //1280×960
                if (width_priority.Checked)
                {
                    _width = 1280;
                    int height_tmp = 960;
                    var tmp = roun(origin_height * _width, origin_width);
                    if (tmp != 0 && tmp == height_tmp)
                    {
                        _height = height_tmp;
                    } else if ( tmp != 0 && tmp != height_tmp)
                    {
                        _height = tmp;
                    } else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else if (height_priority.Checked)
                {
                    _height = 960;
                    int width_tmp = 1280;
                    var tmp = roun(origin_width * _height, origin_height);
                    if (tmp != 0 && tmp == width_tmp)
                    {
                        _width = width_tmp;
                    }
                    else if (tmp != 0 && tmp != width_tmp)
                    {
                        _width = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else
                {
                    MessageBox.Show("優先のチェックが入っていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                if (_width > origin_width || _height > origin_height)
                {
                    begger = true;
                }
            }
            else if (radioButton29.Checked)
            {
                //1024×768
                if (width_priority.Checked)
                {
                    _width = 1024;
                    int height_tmp = 768;
                    var tmp = roun(origin_height * _width, origin_width);
                    if (tmp != 0 && tmp == height_tmp)
                    {
                        _height = height_tmp;
                    }
                    else if (tmp != 0 && tmp != height_tmp)
                    {
                        _height = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else if (height_priority.Checked)
                {
                    _height = 768;
                    int width_tmp = 1024;
                    var tmp = roun(origin_width * _height, origin_height);
                    if (tmp != 0 && tmp == width_tmp)
                    {
                        _width = width_tmp;
                    }
                    else if (tmp != 0 && tmp != width_tmp)
                    {
                        _width = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else
                {
                    MessageBox.Show("優先のチェックが入っていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                if (_width > origin_width || _height > origin_height)
                {
                    begger = true;
                }
            }
            else if (radioButton28.Checked)
            {
                //640×480
                if (width_priority.Checked)
                {
                    _width = 640;
                    int height_tmp = 480;
                    var tmp = roun(origin_height * _width, origin_width);
                    if (tmp != 0 && tmp == height_tmp)
                    {
                        _height = height_tmp;
                    }
                    else if (tmp != 0 && tmp != height_tmp)
                    {
                        _height = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else if (height_priority.Checked)
                {
                    _height = 480;
                    int width_tmp = 640;
                    var tmp = roun(origin_width * _height, origin_height);
                    if (tmp != 0 && tmp == width_tmp)
                    {
                        _width = width_tmp;
                    }
                    else if (tmp != 0 && tmp != width_tmp)
                    {
                        _width = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else
                {
                    MessageBox.Show("優先のチェックが入っていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                if (_width > origin_width || _height > origin_height)
                {
                    begger = true;
                }
            }
            else if (radioButton27.Checked)
            {
                //300×225
                if (width_priority.Checked)
                {
                    _width = 300;
                    int height_tmp = 225;
                    var tmp = roun(origin_height * _width, origin_width);
                    if (tmp != 0 && tmp == height_tmp)
                    {
                        _height = height_tmp;
                    }
                    else if (tmp != 0 && tmp != height_tmp)
                    {
                        _height = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else if (height_priority.Checked)
                {
                    _height = 225;
                    int width_tmp = 300;
                    var tmp = roun(origin_width * _height, origin_height);
                    if (tmp != 0 && tmp == width_tmp)
                    {
                        _width = width_tmp;
                    }
                    else if (tmp != 0 && tmp != width_tmp)
                    {
                        _width = tmp;
                    }
                    else
                    {
                        MessageBox.Show("サイズの変換に失敗しました", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return result;
                    }
                }
                else
                {
                    MessageBox.Show("優先のチェックが入っていません", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return result;
                }
                if (_width > origin_width || _height > origin_height)
                {
                    begger = true;
                }
            }
            result[0] = true;
            result[1] = _width;
            result[2] = _height;
            result[3] = begger;
            return result;


            //try
            //{
            //    // 画像を読み込む
            //    using (Image image = Image.FromFile(inputPath)) 
            //    {
            //            // 新しいサイズにリサイズ
            //            using (Bitmap resizedImage = new Bitmap(image, new Size(_width, _height)))
            //            {

            //                if (!Directory.Exists(outputPath))
            //                {
            //                    Directory.CreateDirectory(outputPath);
            //                }
            //                if (begger)
            //                {
            //                    // Graphicsオブジェクトを作成
            //                    using (Graphics g = Graphics.FromImage(resizedImage))
            //                    {
            //                        // 補完モードの設定：高品質の拡大を指定
            //                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //                        g.DrawImage(image, 0, 0, _width, _height);
            //                    }
            //                }
            //                string file_name = Path.GetFileNameWithoutExtension(inputPath);
            //            string baseFileName = textBox2.Text + file_name + textBox3.Text + comboBox1.SelectedItem.ToString();

            //            // ナンバリングを追加したファイル名を生成
            //            string save_name = GenerateUniqueFileName(outputPath, baseFileName);
            //            string save_path = $"{outputPath}\\{save_name}";
            //            MessageBox.Show($"{save_path}\r\n横:{_width}  縦:{_height}");
            //            // 画像を指定したフォーマットで保存（例：PNG）
            //                //resizedImage.Save(save_path, GetImageFormat(comboBox1.SelectedItem.ToString()));
            //            return true;
            //            }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"エラー: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return false;
            //}
        }


        /// <summary>
        /// 割り算を行い、四捨五入して結果を返す
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        private int roun(int numerator, int denominator)
        {
            double result = numerator / denominator;
            int roundedResult = (int)Math.Round(result);  // 四捨五入してintにキャスト
            return roundedResult;
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
                ".ico" => ImageFormat.Icon,
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
        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel1.BackColor = SystemColors.Control;
            var flags = false;
            if (!checkBox2.Checked)
            {
                // ファイルが渡されていなければ、何もしない
                if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

                // 渡されたファイルに対して処理を行う
                foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    listBox1.Items.Add(filePath);
                }
            }
            else
            {
                var tmps = (string[])e.Data.GetData(DataFormats.FileDrop);
                progressBar1.Maximum = tmps.Length;
                for (int i = 0; i < tmps.Length; i++)
                {
                    string save_path = get_path(tmps[i]);
                    if (save_path == "")
                    {
                        return;
                    }
                    var tmp = Get_size(tmps[i]);
                    if (!(bool)tmp[0])
                    {
                        return;
                    }

                    if (!resize_photo(tmps[i], save_path, (int)tmp[1], (int)tmp[2], (bool)tmp[3]))
                    {
                        flags = true;
                    }
                    progressBar1.Value = i + 1;
                }
                if (flags)
                {
                    MessageBox.Show("変換できないものがありました。", "結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else
                {
                    MessageBox.Show("すべて完了しました", "結果");
                }
                progressBar1.Value = 0;
            }
        }

        private void panel1_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel1.BackColor = SystemColors.Control;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                hide_choice(true);
            }
            else
            {
                hide_choice(false);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                hide_choice(true);
            }
            else
            {
                hide_choice(false);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                label6.Visible = true;
                listBox1.Visible = true;
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            }
            else
            {
                listBox1.Items.Clear();
                label6.Visible = false;
                listBox1.Visible = false;
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void hide_choice(bool chck)
        {
            if (chck)
            {
                textBox1.Visible = true;
                button4.Visible = true;
            } else
            {
                textBox1.Text = "";
                textBox1.Visible = false;
                button4.Visible = false;
            }
        }

        /// <summary>
        /// フォルダ参照ダイアログを呼び出してパスを取得する
        /// </summary>
        /// <param name="select_path">初期パス(in)/選択パス(out)</param>
        /// <returns>true/false</returns>
        private bool GetDirNameFromOpenFileDialog(ref string select_path)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "フォルダを選択してください。";
                dlg.FileName = "SelectFolder";
                dlg.Filter = "Folder|.";
                dlg.CheckFileExists = false;

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    //開く以外を選択された場合はfalseを返す。
                    return false;
                }

                select_path = System.IO.Path.GetDirectoryName(dlg.FileName);

            }

            return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //初期フォルダ名を設定する場合はここにセットする
            string dir = "";

            //フォルダ参照ダイアログを開く、キャンセルの場合は関数から抜ける
            if (GetDirNameFromOpenFileDialog(ref dir) == false) return;

            //取得したフォルダ名をテキストボックスにセットする
            textBox1.Text = dir;
        }

        private void textBox4_DragDrop(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox4.BackColor = SystemColors.Control;
            // ファイルが渡されていなければ、何もしない
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (extent.Contains( Path.GetExtension(file[0])))
            {
                textBox4.Text = file[0].ToString();
                if (checkBox2.Checked)
                {
                    string save_path = get_path(textBox4.Text);
                    if (save_path == "")
                    {
                        return;
                    }
                    var tmp = Get_size(textBox4.Text);
                    if (!(bool)tmp[0])
                    {
                        return;
                    }

                    if (resize_photo(textBox4.Text, save_path, (int)tmp[1], (int)tmp[2], (bool)tmp[3]))
                    {
                        textBox4.Text = "";
                        MessageBox.Show("変換が完了しました", "結果");
                    }
                }
            }
        }

        private void textBox4_DragEnter(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox4.BackColor = Color.Red;
        }

        private void textBox4_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox4.BackColor = SystemColors.Control;
        }

        private void textBox4_DragOver(object sender, DragEventArgs e)
        {
            // ドラッグ対象が許可されている場合、カーソルをデフォルトに設定
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // コピーが許可された場合
                this.Cursor = Cursors.Default;   // デフォルトのカーソルに変更
                textBox4.BackColor = Color.Red;
            }
            else
            {
                e.Effect = DragDropEffects.None; // 不許可の場合
                this.Cursor = Cursors.No; // 不可カーソルに変更
                textBox4.BackColor = Color.Red; // 背景色を変更
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();

            // デフォルトのフォルダを指定する
            ofDialog.InitialDirectory = @"C:\\";

            //ダイアログのタイトルを指定する
            ofDialog.Title = "ファイルを選択してください";
            ofDialog.Filter = "Image Files|*.jpeg;*.jpg;*.png;*.gif;*.ico;*.tiff;*.bmp";
            ofDialog.FilterIndex = 2;
            ofDialog.RestoreDirectory = true;

            //ダイアログを表示する
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = ofDialog.FileName;
            }

            // オブジェクトを破棄する
            ofDialog.Dispose();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!user_choice.Checked) {
                label13.Visible = false;
                label14.Visible = false;
                height.Text = "";
                width.Text = "";
                height.Visible = false;
                width.Visible = false;
                checkBox3.Visible = false;
                button6.Visible = false;
                groupBox4.Visible = true;
            } else
            {
                label13.Visible = true;
                label14.Visible = true;
                height.Visible = true;
                width.Visible = true;
                checkBox3.Visible = true;
                button6.Visible = true;
                groupBox4.Visible = false;
            }
        }
        //呼び出し↓↓
        // 基本のファイル名と拡張子
        //string baseFileName = "image.jpg";
        //string directory = @"C:\path\to\your\directory"; // ファイルが保存されているディレクトリ

        //// ナンバリングを追加したファイル名を生成
        //string newFileName = GenerateUniqueFileName(directory, baseFileName);

        //// 結果を表示
        //Console.WriteLine("New file name: " + newFileName);
        // 新しいユニークなファイル名を生成するメソッド
        static string GenerateUniqueFileName(string directory, string baseFileName)
        {
            string filePath = Path.Combine(directory, baseFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFileName);
            string extension = Path.GetExtension(baseFileName);

            int counter = 1;

            // ファイルが存在する限りナンバリングを追加して新しい名前を生成
            while (File.Exists(filePath))
            {
                string newFileName = $"{fileNameWithoutExtension}({counter}){extension}";
                filePath = Path.Combine(directory, newFileName);
                counter++;
            }

            return Path.GetFileName(filePath); // 最終的な新しいファイル名を返す
        }

        private void width_KeyPress(object sender, KeyPressEventArgs e)
        {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void height_KeyPress(object sender, KeyPressEventArgs e)
        {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                string save_path = get_path(textBox4.Text);
                if (save_path == "")
                {
                    return;
                }
                var tmp = Get_size(textBox4.Text);
                if (!(bool)tmp[0])
                {
                    
                    return;
                }

                if (resize_photo(textBox4.Text, save_path, (int)tmp[1], (int)tmp[2], (bool)tmp[3]))
                {
                    textBox4.Text = "";
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="save_path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="begger"></param>
        /// <returns></returns>
        private bool resize_photo( string origin, string save_path, int width, int height, bool begger)
        {
            try
            {
                // 画像を読み込む
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(origin))
                {
                    // 新しいサイズにリサイズ
                    using (Bitmap resizedImage = new Bitmap(image, new Size(width, height)))
                    {

                        if (begger)
                        {
                            // Graphicsオブジェクトを作成
                            using (Graphics g = Graphics.FromImage(resizedImage))
                            {
                                // 補完モードの設定：高品質の拡大を指定
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(image, 0, 0, width, height);
                            }
                        }
                        //MessageBox.Show($"{save_path}\r\n横:{width}  縦:{height}\r\n新");
                        // 画像を指定したフォーマットで保存（例：PNG）
                        resizedImage.Save(save_path, GetImageFormat(comboBox1.SelectedItem.ToString()));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラー: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int tmps = listBox1.Items.Count;
            if (tmps == 0)
            {
                return;
            }
            bool flags = false;
            progressBar1.Maximum = tmps;
            for (int i = 0; i < tmps; i++)
            {
                string item = listBox1.Items[i].ToString();
                string save_path = get_path(item);
                if (save_path == "")
                {
                    return;
                }
                var tmp = Get_size(item);
                if (!(bool)tmp[0])
                {
                    return;
                }

                if (!resize_photo(item, save_path, (int)tmp[1], (int)tmp[2], (bool)tmp[3]))
                {
                    flags = true;
                }
                progressBar1.Value = i + 1;
            }
            if (flags)
            {
                MessageBox.Show("変換できないものがありました。", "結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("すべて完了しました", "結果");
            }
            progressBar1.Value = 0;
            listBox1.Items.Clear();
        }

    }
}
