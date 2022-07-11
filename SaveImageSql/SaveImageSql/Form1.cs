using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveImageSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Load += Form1_Load;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void InsertImage(string fileName, byte[] image)
        {
            using (var connection = new SqlConnection("Data Source=192.168.180.210;Initial Catalog=RecycledAppTEST;User ID=readmin;Password=Recycle123$;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                var param = new DynamicParameters();
                param.Add("@fileName", fileName);
                param.Add("@image", image);
                var result = connection.Execute("Insert into tblImages (FileName, Image) values (@fileName,@image)", param);
            }
        }

        private void LoadData()
        {
            List<ImageModel> result;
            using (var connection = new SqlConnection("Data Source=192.168.180.210;Initial Catalog=RecycledAppTEST;User ID=readmin;Password=Recycle123$;Connect Timeout=30"))
            {
                result = connection.Query<ImageModel>("select * from tblImages").ToList();
                gridData.DataSource = result;
            }
        }

        private byte[] convertImageToByte(Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private Image ConvertByteArrayToimage(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //"Text files (*.txt)|*.txt|All files (*.*)|*.*"'
            using (var ofd = new OpenFileDialog() { Filter = "*Image file (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*", Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //display image to picture box
                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                    txtFilename.Text = ofd.FileName;
                    InsertImage(ofd.FileName, convertImageToByte(pictureBox1.Image));

                    LoadData();
                }
            }
        }

        private void gridData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTable dt = gridData.DataSource as DataTable;

            List< ImageModel> res = (List< ImageModel>)gridData.DataSource;

            if (res != null)
            {
                
                pictureBox1.Image = ConvertByteArrayToimage((byte[])res[e.RowIndex].Image);
            }
        }
    }

    public class ImageModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] Image { get; set; }

    }
}
