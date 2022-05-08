using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BAI10_CLIENT_TCP_QL_CHUC_VU
{
    public partial class frmDonHang : Form
    {
        public frmDonHang()
        {
            InitializeComponent();
        }

        StreamReader sr;
        StreamWriter sw;

        int chon = 0;

        private void frmDonHang_Load(object sender, EventArgs e)
        {
            try
            {
                //tạo luồng đọc và luồng ghi dữ liệu
                //sử dụng biến static ns của form Kết Nối
                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                chon = 10;

                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                sw.WriteLine(chon);
                sw.Flush();


                //tạo mảng byte để nhận table chuc vu từ máy chủ
                //byte[] data_sanpham = new byte[1024 * 5000];

                //tạo mảng byte để nhận table nhan vien từ máy chủ
                byte[] data_donhang = new byte[1024 * 5000];


                /*//sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_sanpham);
                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_sanpham = (DataTable)DeserializeData(data_sanpham);*/

                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_donhang);
                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_donhang = (DataTable)DeserializeData(data_donhang);

                //đưa datatable vào dataGridView
                dgDSDonHang.DataSource = dt_donhang;

                /*cmbTenSanPham.DataSource = dt_sanpham;
                cmbTenSanPham.DisplayMember = "tensp";
                cmbTenSanPham.ValueMember = "masp";*/


                

                //clientSock.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //chuyển dữ liệu từ dạng mảng byte sang kiểu object
        public object DeserializeData(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }

        private void dgDSDonHang_Click(object sender, EventArgs e)
        {
            DataGridViewRow r = new DataGridViewRow();
            r = dgDSDonHang.SelectedRows[0];
            txtMaDH.Text = r.Cells["madh"].Value.ToString();
            txtTenKH.Text = r.Cells["khachhang"].Value.ToString();
            cmbTenSanPham.SelectedValue = r.Cells["masp"].Value;
            dtmNgayLap.Text = r.Cells["ngaylap"].Value.ToString();
            txtSoLuong.Text = r.Cells["soluong"].Value.ToString();
            txtTongTien.Text = r.Cells["tongtien"].Value.ToString();
        }
    }
}
