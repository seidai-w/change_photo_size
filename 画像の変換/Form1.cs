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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


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
                    if (user_choice.Checked && checkBox3.Checked)
                    {
                        if (width.Text == "" || height.Text == "")
                        {
                            MessageBox.Show("横か縦の値が空欄になっています", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        int _width = 0;
                        int _height = 0;
                        bool success = int.TryParse(width.Text, out _width);

                        if (!success || _width < 1)
                        {
                            return;
                        }
                        success = int.TryParse(width.Text, out _height);

                        if (!success || _height < 1)
                        {
                            return;
                        }
                        var sizes = resized_size(tmps[i], _width, _height, width_priority.Checked);
                        if (sizes[0] == 0 || sizes[1] == 0)
                        {
                            return;
                        }
                        if (!resize_photo(tmps[i], save_path, sizes[0], sizes[1], (_width - sizes[0] < 0 || _height - sizes[1] < 0)))
                        {
                            flags = true;
                        }


                    }
                    else
                    {
                        var tmp = Get_size(tmps[i]);
                        if (!(bool)tmp[0])
                        {
                            return;
                        }

                        if (!resize_photo(tmps[i], save_path, (int)tmp[1], (int)tmp[2], (bool)tmp[3]))
                        {
                            flags = true;
                        }
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
                label11.Visible = true;
                listBox2.Visible = true;
                button9.Visible = true;
                button10.Visible = true;
            }
            else
            {
                listBox1.Items.Clear();
                label6.Visible = false;
                listBox1.Visible = false;
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                label11.Visible = false;
                listBox2.Visible = false;
                button9.Visible = false;
                button10.Visible = false;
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
            }
            else
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
            if (extent.Contains(Path.GetExtension(file[0])))
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
            if (!user_choice.Checked)
            {
                label13.Visible = false;
                label14.Visible = false;
                height.Text = "";
                width.Text = "";
                height.Visible = false;
                width.Visible = false;
                checkBox3.Visible = false;
                button6.Visible = false;
                if (tabControl1.SelectedIndex == 0)
                {
                    groupBox4.Visible = true;
                }
            }
            else
            {
                label13.Visible = true;
                label14.Visible = true;
                height.Visible = true;
                width.Visible = true;
                if (tabControl1.SelectedIndex == 0)
                {
                    checkBox3.Visible = true;
                }
                button6.Visible = true;
                if (tabControl1.SelectedIndex == 0) {
                    groupBox4.Visible = true;
                }
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
        private bool resize_photo(string origin, string save_path, int width, int height, bool begger)
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

        private void button6_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (textBox4.Text != "")
                {
                    try
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(textBox4.Text))
                        {
                            width.Text = image.Width.ToString();
                            height.Text = image.Height.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("画像の読み込みに失敗しました: " + ex.Message.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                if (textBox5.Text != "")
                {
                    try
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromFile(textBox5.Text))
                        {
                            width.Text = image.Width.ToString();
                            height.Text = image.Height.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("画像の読み込みに失敗しました: " + ex.Message.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        private void width_TextChanged(object sender, EventArgs e)
        {
            int _width = 0;
            if (tabControl1.SelectedIndex == 0)
            {
                if (textBox4.Text != "")
                {
                    if (!string.IsNullOrEmpty(textBox4.Text) && checkBox3.Checked && width.Focused)
                    {
                        bool success = int.TryParse(width.Text, out _width);

                        if (!success || _width < 1)
                        {
                            return;
                        }
                        var tmp = get_oligin_size(textBox4.Text);
                        if (tmp[0].Key == 0)
                        {
                            return;
                        }
                        else
                        {
                            height.Text = roun(tmp[0].Value * _width, tmp[0].Key).ToString();
                        }
                    }
                }
            }
            else
            {

            }
        }

        private List<KeyValuePair<int, int>> get_oligin_size(string path)
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();

            try
            {
                if (extent.Contains(Path.GetExtension(path)) && File.Exists(path))
                {
                    using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
                    {
                        list.Add(new KeyValuePair<int, int>(image.Width, image.Height));
                    }
                    return list;
                }
                list.Add(new KeyValuePair<int, int>(0, 0));
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("画像の読み込みに失敗しました: " + ex.Message.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                list.Add(new KeyValuePair<int, int>(0, 0));
                return list;
            }
        }

        private void height_TextChanged(object sender, EventArgs e)
        {
            int _height = 0;
            if (tabControl1.SelectedIndex == 0)
            {
                if (textBox4.Text != "")
                {
                    if (!string.IsNullOrEmpty(textBox4.Text) && checkBox3.Checked && height.Focused)
                    {
                        bool success = int.TryParse(height.Text, out _height);

                        if (!success || _height < 1)
                        {
                            return;
                        }
                        var tmp = get_oligin_size(textBox4.Text);
                        if (tmp[0].Key == 0)
                        {
                            return;
                        }
                        else
                        {
                            width.Text = roun(tmp[0].Key * _height, tmp[0].Value).ToString();
                        }
                    }
                }
            }
            else
            {

            }
        }

        private List<int> resized_size(string path, int width, int height, bool width_priority)
        {
            var list = new List<int>() { 0, 0 };
            if (!width_priority)
            {
                var tmp = get_oligin_size(path);
                if (tmp[0].Key == 0)
                {
                    return list;
                }
                else
                {
                    list[1] = height;
                    list[0] = roun(tmp[0].Key * height, tmp[0].Value);
                    return list;
                }


            }
            else
            {
                var tmp = get_oligin_size(path);
                if (tmp[0].Key == 0)
                {
                    return list;
                }
                else
                {
                    list[0] = width;
                    list[1] = roun(tmp[0].Value * width, tmp[0].Key);
                    return list;
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                groupBox4.Visible = false;
            }
            else { 
                groupBox4.Visible= true;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_DragDrop(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox5.BackColor = SystemColors.Control;
            // ファイルが渡されていなければ、何もしない
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (extent.Contains(Path.GetExtension(file[0])))
            {
                textBox5.Text = file[0].ToString();
                if (checkBox2.Checked)
                {
                    string save_path = get_path(textBox5.Text);
                    if (save_path == "")
                    {
                        return;
                    }
                    List<int> tmp = size_check();
                    if (tmp[0] == 0)
                    {
                        return;
                    }
                    if (resize_photo_back(textBox5.Text, save_path, (int)tmp[1], (int)tmp[2]))
                    {
                        textBox5.Text = "";
                        MessageBox.Show("変換が完了しました", "結果");
                    }
                }
            }
        }

        private void textBox5_DragEnter(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox5.BackColor = Color.Red;
        }

        private void textBox5_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            textBox5.BackColor = SystemColors.Window;
        }

        private void textBox5_DragOver(object sender, DragEventArgs e)
        {
            // ドラッグ対象が許可されている場合、カーソルをデフォルトに設定
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // コピーが許可された場合
                this.Cursor = Cursors.Default;   // デフォルトのカーソルに変更
                textBox5.BackColor = Color.Red;
            }
            else
            {
                e.Effect = DragDropEffects.None; // 不許可の場合
                this.Cursor = Cursors.No; // 不可カーソルに変更
                textBox5.BackColor = Color.Red; // 背景色を変更
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                groupBox4.Visible = true;
                checkBox3.Visible = true;
            }
            else if (tabControl1.SelectedIndex == 1) 
            {
                groupBox4.Visible= false;
                checkBox3.Visible = false;
            }
        }

        public static Bitmap ResizeWithPadding(System.Drawing.Image image, int width, int height)
        {
            // 目的のサイズのBitmapを作成（白色背景）
            Bitmap resizedImage = new Bitmap(width, height);

            // 背景を白色に設定
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.Clear(Color.White); // 背景色を白に設定

                // 元の画像の比率に合わせてリサイズ
                float aspectRatio = (float)image.Width / image.Height;
                int newWidth = width;
                int newHeight = height;

                // 画像の比率に合わせてリサイズ
                if (width / aspectRatio <= height)
                {
                    newWidth = width;
                    newHeight = (int)(width / aspectRatio);
                }
                else
                {
                    newHeight = height;
                    newWidth = (int)(height * aspectRatio);
                }

                // リサイズ後の画像を中央に描画
                int x = (width - newWidth) / 2;
                int y = (height - newHeight) / 2;

                // リサイズした画像を中央に描画
                g.DrawImage(image, x, y, newWidth, newHeight);
            }

            return resizedImage;
        }

        private bool resize_photo_back(string origin, string save_path, int width, int height)
        {
            try
            {
                // 画像を読み込む
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(origin))
                {
                    using (System.Drawing.Image changed_image = ResizeWithPadding(image, width, height)) { 
                        //MessageBox.Show($"{save_path}\r\n横:{width}  縦:{height}\r\n新");
                        // 画像を指定したフォーマットで保存（例：PNG）
                        changed_image.Save(save_path, GetImageFormat(comboBox1.SelectedItem.ToString()));
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
        
        private List<int> size_check()
        {
            List<int> size_ = new List<int>() { 0, 0, 0 };
            if (radioButton30.Checked)
            {
                size_[0] = 1;
                size_[1] = 1280;
                size_[2] = 960;
            }
            else if (radioButton29.Checked)
            {
                size_[0] = 1;
                size_[1] = 1024;
                size_[2] = 768;
            }
            else if (radioButton28.Checked)
            {
                size_[0] = 1;
                size_[1] = 640;
                size_[2] = 480;
            }
            else if (radioButton27.Checked)
            {
                size_[0] = 1;
                size_[1] = 300;
                size_[2] = 225;
            }
            else if (user_choice.Checked)
            {
                bool success = int.TryParse(width.Text, out int tmp);

                if (!success || tmp < 1)
                {
                    return size_;
                }
                size_[1] = tmp;
                tmp = 0;
                success = int.TryParse(height.Text, out tmp);

                if (!success || tmp < 1)
                {
                    return size_;
                }
                size_[2] = tmp;
                size_[0] = 1;
            }
            return size_;

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                string save_path = get_path(textBox5.Text);
                if (save_path == "")
                {
                    return;
                }
                List<int> tmp = size_check();
                if (tmp[0] == 0)
                {
                    return;
                }
                if (resize_photo_back(textBox5.Text, save_path, (int)tmp[1], (int)tmp[2]))
                {
                    textBox5.Text = "";
                    MessageBox.Show("変換が完了しました", "結果");
                }

            }
        }
        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel2.BackColor = Color.FromArgb(50, 255, 0, 0);
        }

        private void panel2_DragOver(object sender, DragEventArgs e)
        {
            // ドラッグ対象が許可されている場合、カーソルをデフォルトに設定
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy; // コピーが許可された場合
                this.Cursor = Cursors.Default;   // デフォルトのカーソルに変更
                panel2.BackColor = Color.FromArgb(50, 255, 0, 0); // 背景色を変更
            }
            else
            {
                e.Effect = DragDropEffects.None; // 不許可の場合
                this.Cursor = Cursors.No; // 不可カーソルに変更
                panel2.BackColor = Color.FromArgb(50, 255, 0, 0); // 背景色を変更
            }
        }
        private void panel2_DragDrop(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel2.BackColor = SystemColors.Control;
            var flags = false;
            if (!checkBox2.Checked)
            {
                // ファイルが渡されていなければ、何もしない
                if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

                // 渡されたファイルに対して処理を行う
                foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    listBox2.Items.Add(filePath);
                }
            }
            else
            {
                var tmps = (string[])e.Data.GetData(DataFormats.FileDrop);
                progressBar2.Maximum = tmps.Length;
                for (int i = 0; i < tmps.Length; i++)
                {
                    string save_path = get_path(tmps[i]);
                    if (save_path == "")
                    {
                        flags = true;
                        break;
                    }
                    List<int> tmp = size_check();
                    if (tmp[0] == 0)
                    {
                        flags = true;
                        break;
                    }
                    if (!resize_photo_back(tmps[i], save_path, (int)tmp[1], (int)tmp[2]))
                    {
                        flags = true;
                    }

                    progressBar2.Value = i + 1;
                }
                if (flags)
                {
                    MessageBox.Show("変換できないものがありました。", "結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("すべて完了しました", "結果");
                }
                progressBar2.Value = 0;
            }
        }

        private void panel2_DragLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            panel2.BackColor = SystemColors.Control;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int tmps = listBox2.Items.Count;
            if (tmps == 0)
            {
                return;
            }
            bool flags = false;
            progressBar2.Maximum = tmps;
            for (int i = 0; i < tmps; i++)
            {
                string item = listBox2.Items[i].ToString();
                string save_path = get_path(item);
                if (save_path == "")
                {
                    flags = true;
                    break;
                }
                List<int> tmp = size_check();
                if (tmp[0] == 0)
                {
                    flags = true;
                    break;
                }
                if (!resize_photo_back(item, save_path, (int)tmp[1], (int)tmp[2]))
                {
                    flags = true;
                }

                progressBar2.Value = i + 1;
            }
            if (flags)
            {
                MessageBox.Show("変換できないものがありました。", "結果", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("すべて完了しました", "結果");
            }
            progressBar2.Value = 0;
            listBox2.Items.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
