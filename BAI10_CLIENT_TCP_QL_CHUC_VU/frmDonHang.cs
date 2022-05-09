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

                chon = 11;

                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                sw.WriteLine(chon);
                sw.Flush();



                //tạo mảng byte để nhận table chuc vu từ máy chủ
                byte[] data_donhang = new byte[1024 * 5000];

                //tạo mảng byte để nhận table nhan vien từ máy chủ
                byte[] data_sanpham = new byte[1024 * 5000];



                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_donhang);
                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_donhang = (DataTable)DeserializeData(data_donhang);

                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_sanpham);

                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_sanpham = (DataTable)DeserializeData(data_sanpham);



                cmbTenSanPham.DataSource = dt_sanpham;
                cmbTenSanPham.DisplayMember = "tensp";
                cmbTenSanPham.ValueMember = "masp";

                //dgDSSanPham.Columns["tensp"].HeaderText = "Tên sản phẩm";
                //đưa datatable vào dataGridView
                dgDSDonHang.DataSource = dt_donhang;





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

        public bool timDonHangTheoMaDH(string madhtim)
        {
            chon = 12;

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(madhtim);
            sw.Flush();

            int kq = int.Parse(sr.ReadLine());

            if (kq == 1)
                return true;
            else
                return false;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu có bị bỏ trống 
            if (txtMaDH.Text == "" || txtTenKH.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!");
                return;
            }
            // Kiểm tra mã hãng sản xuất có độ dài chuỗi hợp lệ hay không
            if (txtMaDH.Text.Length > 5)
            {
                MessageBox.Show("Mã hãng sản xuất tối đa 5 ký tự!");
                return;
            }
            if (timDonHangTheoMaDH(txtMaDH.Text) == true)
            {
                MessageBox.Show("Mã chức vụ đã tồn tại!");
                return;
            }

            chon = 13;
            string madh = txtMaDH.Text;
            string khachang = txtTenKH.Text;
            string tensp = cmbTenSanPham.SelectedValue.ToString();
            DateTime ngaylap = DateTime.Parse(dtmNgayLap.Text);
            float tongtien = float.Parse(txtTongTien.Text);
            float soluong = float.Parse(txtSoLuong.Text);


            //thêm chức vụ            
            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(madh);
            sw.WriteLine(khachang);
            sw.WriteLine(tensp);
            sw.WriteLine(ngaylap);
            sw.WriteLine(tongtien);
            sw.WriteLine(soluong);

            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDSDonHang.DataSource = dt;
        }
    }
}
