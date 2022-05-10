﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAI10_CLIENT_TCP_QL_CHUC_VU
{
    public partial class frmSanPham : Form
    {
        public frmSanPham()
        {
            InitializeComponent();
        }
        StreamReader sr;
        StreamWriter sw;

        int chon = 0;
        private void frmSanPham_Load(object sender, EventArgs e)
        {
            try
            {
                //tạo luồng đọc và luồng ghi dữ liệu
                //sử dụng biến static ns của form Kết Nối
                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                chon = 20;

                sr = new StreamReader(frmKetNoi.ns);
                sw = new StreamWriter(frmKetNoi.ns);

                sw.WriteLine(chon);
                sw.Flush();



                //tạo mảng byte để nhận table chuc vu từ máy chủ
                byte[] data_hsx = new byte[1024 * 5000];

                //tạo mảng byte để nhận table nhan vien từ máy chủ
                byte[] data_sanpham = new byte[1024 * 5000];



                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_hsx);
                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_hsx = (DataTable)DeserializeData(data_hsx);

                //sử dụng biến static clientSockt của form Kết Nối
                frmKetNoi.clientSock.Receive(data_sanpham);

                //chuyển dữ liệu vừa nhận dạng mảng byte sang kiểu object rồi ép kiểu sang datatable
                DataTable dt_sanpham = (DataTable)DeserializeData(data_sanpham);
                dgDSSanPham.DataSource = dt_sanpham;
                dgDSSanPham.Columns["masp"].HeaderText = "Mã sản phẩm";
                dgDSSanPham.Columns["tensp"].HeaderText = "Tên sản phẩm";
                dgDSSanPham.Columns["mahsx"].HeaderText = "Tên hãn sản xuất";
                dgDSSanPham.Columns["cpu"].HeaderText = "CPU";
                dgDSSanPham.Columns["ram"].HeaderText = "RAM";
                dgDSSanPham.Columns["rom"].HeaderText = "ROM";
                dgDSSanPham.Columns["manhinh"].HeaderText = "Màn hình";
                dgDSSanPham.Columns["kichthuoc"].HeaderText = "Kích thước";
                dgDSSanPham.Columns["hedieuhanh"].HeaderText = "Hệ điều hành";
                dgDSSanPham.Columns["giaban"].HeaderText = "Giá bán";

                cboHSX.DataSource = dt_hsx;
                cboHSX.DisplayMember = "tenhsx";
                cboHSX.ValueMember = "mahsx";

                //dgDSSanPham.Columns["tensp"].HeaderText = "Tên sản phẩm";
                //đưa datatable vào dataGridView
                dgDSSanPham.DataSource = dt_sanpham;


                /*byte[] data_sanpham = new byte[1024 * 5000];
                frmKetNoi.clientSock.Receive(data_hsx);
                DataTable dt_hsx = (DataTable)DeserializeData(data_hsx);*/

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

        private void dgDSSanPham_Click(object sender, EventArgs e)
        {
            DataGridViewRow r = new DataGridViewRow();
            r = dgDSSanPham.SelectedRows[0];
            txtMaSP.Text = r.Cells["masp"].Value.ToString();
            txtTenSP.Text = r.Cells["tensp"].Value.ToString();
            txtCPU.Text = r.Cells["cpu"].Value.ToString();
            txtRAM.Text = r.Cells["ram"].Value.ToString();
            txtROM.Text = r.Cells["rom"].Value.ToString();
            txtManHinh.Text = r.Cells["manhinh"].Value.ToString();
            txtKichThuoc.Text = r.Cells["kichthuoc"].Value.ToString();
            txtHeDieuHanh.Text = r.Cells["hedieuhanh"].Value.ToString();
            txtGiaBan.Text = r.Cells["giaban"].Value.ToString();
            cboHSX.SelectedValue = r.Cells["mahsx"].Value;
        }

        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            /*//Viet cau lenh SQL cho tim kiem
            String sql = "SELECT * FROM tblMatHang";
            String dk = "";
            //Tim theo MaSP khac rong
            if (txtTimKiem.Text.Trim() != "")
            {
                dk += " masp like '%" + txtTimKiem.Text + "%'";
            }
            //kiem tra TenSP va MaSP khac rong
            if (txtTimKiem.Text.Trim() != "" && dk != "")
            {
                dk += " AND tensp like N'%" + txtTimKiem.Text + "%'";
            }
            //Tim kiem theo TenSP khi MaSP la rong
            if (txtTimKiem.Text.Trim() != "" && dk == "")
            {
                dk += " tensp like N'%" + txtTimKiem.Text + "%'";
            }
            //Ket hoi dk
            if (dk != "")
            {
                sql += " WHERE" + dk;
            }
            //Goi phương thức Load dữ liệu kết hợp điều kiện tìm kiếm
            //LoadDuLieu(sql);*/
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            /*string sql = "";
            SqlConnection con;//Khai báo đối tượng thực hiện kết nối đến cơ sở dữ liệu
            SqlCommand cmd;//Khai báo đối tượng thực hiện các câu lệnh truy vấn
            if (btnThem.Enabled == true)
            {
                //Kiểm tra xem ô nhập MaSP có bị trống không
                if (txtMaSP.Text.Trim() == "")
                {
                    errChiTiet.SetError(txtMaSP, "Bạn không để trống mã sản phẩm trường này!");
                    return;
                }
                else
                {
                    //Kiểm tra xem mã sản phẩm đã tồn tại chưa đẻ tránh việc insert mới bị lỗi  
                    sql = "Select Count(*) From sanpham Where masp ='" + txtMaSP.Text + "'";
                    cmd = new SqlCommand(sql, con);
                    int val = (int)cmd.ExecuteScalar();
                    if (val > 0)
                    {
                        errChiTiet.SetError(txtMaSP, "Mã sản phẩm trùng trong cơ sở dữ liệu");
                        return;
                    }
                    errChiTiet.Clear();
                }
                //Insert vao CSDL
                sql = "INSERT INTO sanpham(masp,tensp,mahsx,cpu,ram,rom,manhinh,hedieuhanh,kichthuoc,giaban)VALUES (";
                sql += "N'" + txtMaSP.Text + "',N'" + txtTenSP.Text + "',N'" + cboHSX.SelectedIndex + "','" + txtCPU.Text + "',N'" + txtRAM.Text + "',N'" + txtROM.Text + "',N'"  + txtManHinh.Text + "',N'" + txtHeDieuHanh.Text + "',N'" + txtKichThuoc.Text + "',N'" + txtGiaBan.Text + "')";
            }*/
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // kiểm tra mã có tồn tại
            if (txtMaSP.Text == "" || txtTenSP.Text == "" || txtRAM.Text == "" || txtCPU.Text == "" || txtRAM.Text == "" || txtROM.Text == "" || txtManHinh.Text == "" || txtHeDieuHanh.Text == "" || txtKichThuoc.Text == "" || txtGiaBan.Text == "")
            {
                MessageBox.Show("Vui lòng chọn mhập đủ thông tin!");
                return;
            }
            if (timSanPhamTheoMaSP(txtMaSP.Text) == false)
            {
                MessageBox.Show("Sản phẩm không tồn tại!");
                return;
            }
            chon = 22;
            string masp = txtMaSP.Text;
            string tensp = txtTenSP.Text;
            string mahsx = cboHSX.ValueMember;
            string cpu = txtCPU.Text;
            string ram = txtRAM.Text;
            string rom = txtROM.Text;
            string manhinh = txtManHinh.Text;
            string hedieuhanh = txtHeDieuHanh.Text;
            string kichthuoc = txtKichThuoc.Text;
            float giaban = float.Parse(txtGiaBan.Text);


            //thêm chức vụ

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(masp);
            sw.WriteLine(tensp);
            sw.WriteLine(mahsx);
            sw.WriteLine(cpu);
            sw.WriteLine(ram);
            sw.WriteLine(rom);
            sw.WriteLine(manhinh);
            sw.WriteLine(hedieuhanh);
            sw.WriteLine(kichthuoc);
            sw.WriteLine(giaban);
            sw.Flush();

            //tạo mảng byte để nhận dữ liệu từ máy chủ
            byte[] data = new byte[1024 * 5000];
            frmKetNoi.clientSock.Receive(data);

            //chuyển dữ liệu vừa nhận dạng mảng byte sang datatable
            DataTable dt = (DataTable)DeserializeData(data);

            //đưa datatable vào dataGridView
            dgDSSanPham.DataSource = dt;
        }
        public bool timSanPhamTheoMaSP(string masp)
        {
            chon = 21;

            sr = new StreamReader(frmKetNoi.ns);
            sw = new StreamWriter(frmKetNoi.ns);

            sw.WriteLine(chon);
            sw.WriteLine(masp);
            sw.Flush();

            int kq = int.Parse(sr.ReadLine());

            if (kq == 1)
                return true;
            else
                return false;
        }
    }
}
