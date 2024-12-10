using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace 画像の変換
{
    public partial class size_pop : Form
    {
        private PictureBox pictureBox;
        public size_pop(Bitmap bitmap, int width, int height)
        {
            InitializeComponent();
            this.Size = new Size(width, height);
            pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 画像のサイズを調整
            pictureBox.Dock = DockStyle.Fill; // フォームいっぱいに表示

            // BitmapをPictureBoxに設定
            pictureBox.Image = new Bitmap(bitmap, new Size(width, height));

            // PictureBoxをフォームに追加
            this.Controls.Add(pictureBox);
            // 最大化ボタンを無効化
            this.MaximizeBox = false;

            // 最小化ボタンを無効化
            this.MinimizeBox = false;

            // フォームのボーダースタイルを変更（最大化不可）
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

        }

        private void size_pop_Load(object sender, EventArgs e)
        {
            
        }
    }
}
