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
    public partial class frmHangSanXuat: Form
    {
        public frmHangSanXuat()
        {
            InitializeComponent();
        }

        StreamReader sr;
        StreamWriter sw;

        int chon = 0;

        private void frmHangSanXuat_Load(object sender, EventArgs e)
        {
            try
            {
                //tạo luồng đọc và luồng ghi dữ liệu
                //sử dụng biến static ns của form Kết Nối
                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                chon = 6;

                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                sw.WriteLine(chon);
                sw.Flush();

                //tạo mảng byte để nhận table hang san xuat từ máy chủ
                byte[] data_hangsanxuat = new byte[1024 * 5000];

                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_hangsanxuat);
                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_hangsanxuat = (DataTable)DeserializeData(data_hangsanxuat);


                //đưa datatable vào dataGridView
                dgDanhSachHang.DataSource = dt_hangsanxuat;

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

        // Tìm hãng sản xuất theo mã
        public bool timHangSanXuatTheoMaHSX(string mahsxtim)
        {
            chon = 5;

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(mahsxtim);
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
            if (txtMaHSX.Text == "" || txtTenHSX.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ dữ liệu!");
                return;
            }
            // Kiểm tra mã hãng sản xuất có độ dài chuỗi hợp lệ hay không
            if (txtMaHSX.Text.Length > 5)
            {
                MessageBox.Show("Mã hãng sản xuất tối đa 5 ký tự!");
                return;
            }
            if (timHangSanXuatTheoMaHSX(txtMaHSX.Text) == true)
            {
                MessageBox.Show("Mã chức vụ đã tồn tại!");
                return;
            }

            chon = 1;
            string mahsx = txtMaHSX.Text;
            string tenhsx = txtTenHSX.Text;

            //thêm chức vụ            
            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(mahsx);
            sw.WriteLine(tenhsx);
            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDanhSachHang.DataSource = dt;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // kiểm tra mã có tồn tại
            if (txtMaHSX.Text == "")
            {
                MessageBox.Show("Vui lòng chọn mã hãng sản xuất!");
                return;
            }
            chon = 2;
            string mahsx = txtMaHSX.Text;

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(mahsx);
            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDanhSachHang.DataSource = dt;
        }

        private void dgDanhSachHang_Click_1(object sender, EventArgs e)
        {
            DataGridViewRow r = new DataGridViewRow();
            r = dgDanhSachHang.SelectedRows[0];
            txtMaHSX.Text = r.Cells["mahsx"].Value.ToString();
            txtTenHSX.Text = r.Cells["tenhsx"].Value.ToString();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // kiểm tra mã có tồn tại
            if (txtMaHSX.Text == "" || txtTenHSX.Text == "")
            {
                MessageBox.Show("Vui lòng chọn mhập đủ thông tin!");
                return;
            }
            if (timHangSanXuatTheoMaHSX(txtMaHSX.Text) == false)
            {
                MessageBox.Show("Mã hãng sản xuất không tồn tại!");
                return;
            }
            chon = 3;
            string mahsx = txtMaHSX.Text;
            string tenhsx = txtTenHSX.Text;

            //thêm chức vụ

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(mahsx);
            sw.WriteLine(tenhsx);
            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDanhSachHang.DataSource = dt;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            chon = 4;
            string tenhsxtim = txtTimKiem.Text;

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(tenhsxtim);

            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDanhSachHang.DataSource = dt;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
