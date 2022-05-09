﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Data.SqlClient;

namespace BAI10_SERVER_TCP_QL_CHUC_VU
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }
        Socket server = null;
        Socket clientSock;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;

        string s;
        SqlConnection KetNoi;
        int chon = 0;
        int chon_loadform = 0;
        private void Server_Load(object sender, EventArgs e)
        {

        }
        public void MoKetNoi()
        {
            //   s = @"Data Source=DESKTOP-T84NIPD\MAYAO;Initial Catalog=QLNV;Integrated Security=True; User ID=sa;password=123456";
            s = @"Data Source=DESKTOP-BV0HRRC\SQLEXPRESS;Initial Catalog=QLLaptop;Integrated Security=True";
            //s = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=QLNV;Integrated Security=True";
            //s = @"Data Source=MAY1\SQLEXPRESS;Initial Catalog=QLNV;Integrated Security=True";

            KetNoi = new SqlConnection(s);
            KetNoi.Open();
        }

        public static void DongKetNoi(SqlConnection KetNoi)
        {
            KetNoi.Close();
        }

        private void btn_start_server_Click(object sender, EventArgs e)
        {
            btn_start_server.Text = "In process...";
            Application.DoEvents();

            // biến ipEnd sẽ chứa địa chỉ ip của tiến trình server trên mạng
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 2021);


            // server sử dụng đồng thời hai socket: 
            // một socket để chờ nghe kết nối, một socket để gửi/nhận dữ liệu

            // socket thứ nhất làm nhiệm vụ chờ kết nối từ Client
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            // yêu cầu hệ điều hành cho phép chiếm dụng cổng tcp 2021
            // server sẽ nghe trên tất cả các mạng mà máy tính này kết nối tới
            // chỉ cần gói tin tcp đến cổng 2021, tiến trình server sẽ nhận được
            server.Bind(ipEnd);
            server.Listen(100);

            //while này đảm server sẵn sàng nhận nhiều kết nối từ nhiều client và server chạy liên tục
            while(true)
            {
                //có thể tạo chuyển code trong while này sang class thread
                //để thực hiện TCP song song
                
                // Socket thứ hai làm nhiệm vụ gửi/nhận dữ liệu
                // socket này được tạo ra bởi lệnh Accept
                clientSock = server.Accept();

                //để gởi nhận dữ liệu giữa server và client có thể dùng lệnh clientSock.Send(data), clientSock.Receive(data); 
                //hoặc sử dụng luồng stream bằng lệnh NetworkStream ns = new NetworkStream(clientSock);
                
                //tạo các luồng truyền dữ liệu
                ns = new NetworkStream(clientSock);
                //tạo luồng đọc dữ liệu
                sr = new StreamReader(ns);
                //tạo luồng ghi dữ liệu
                sw = new StreamWriter(ns);

                //mở kết nối csdl sql server
                MoKetNoi();

                //while này duy trì liên tục phục vụ các yêu cầu liên tục
                //của 1 client như đăng nhập, thêm, cập nhật, xóa, tìm kiếm,....
                while (true)
                {
                    //nhận biến chọn từ client để xử lý các sự kiện như load dữ liệu, thêm, xóa, cập nhật 
                    chon = int.Parse(sr.ReadLine());
/*
                    //chọn = 1 là sự kiện khi client nhấn nút thêm
                    if (chon == 0)
                    {
                        //tạo datatable lấy dữ liệu từ sql server
                        DataTable table = getdataChucVu();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table));
                    }

                    //chọn = 1 là sự kiện khi client nhấn nút thêm
                    if (chon == 1)
                    {
                        //nhận macv, tencv, hspc từ client
                        string macv = sr.ReadLine();
                        string tencv = sr.ReadLine();
                        float hspc = float.Parse(sr.ReadLine());

                        DataTable table1 = themchucvudata(macv, tencv, hspc);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table1));
                    }

                    //chọn = 2 là sự kiện khi client nhấn nút xóa
                    if (chon == 2)
                    {
                        string macv = sr.ReadLine();

                        DataTable table2 = xoachucvudata(macv);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table2));

                    }

                    //chọn = 3 là sự kiện khi client nhấn nút cập nhật
                    if (chon == 3)
                    {
                        string macv = sr.ReadLine();
                        string tencv = sr.ReadLine();
                        float hspc = float.Parse(sr.ReadLine());

                        DataTable table3 = capnhatchucvudata(macv, tencv, hspc);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table3));
                    }

                    //chọn = 4 là sự kiện khi client nhấn nút tìm
                    if (chon == 4)
                    {
                        string tencvtim = sr.ReadLine();

                        DataTable table1 = timtenchucvudata(tencvtim);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table1));
                    }

                    //chọn = 5 tìm chức vụ theo macv
                    if (chon == 5)
                    {
                        string macvtim = sr.ReadLine();
                        int kq = timmachucvudata(macvtim);

                        sw.WriteLine(kq);
                        sw.Flush();
                    }


                    //chọn = 6 lấy danh sách chức vụ
                    if (chon == 6)
                    {
                        //tạo datatable lấy dữ liệu table chức vụ từ sql server
                        DataTable table6 = getdataChucVu();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table6));

                        //tạo datatable lấy dữ liệu table nhân viên từ sql server
                        DataTable table7 = getdataNhanVien();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table7));

                    }*/

                    //chọn = 7 lấy danh sách nhân viên, kiểm tra đăng nhập
                    if (chon == 7)
                    {
                       
                        string tendangnhap = sr.ReadLine();
                        string matkhau = sr.ReadLine();
                      
                        int kq = kiemtradangnhap(tendangnhap, matkhau);

                        sw.WriteLine(kq);
                        sw.WriteLine(tendangnhap);
                        sw.Flush();

                    }

                    #region HangSanXuat
                    if (chon == 6)
                    {
                        //tạo datatable lấy dữ liệu từ sql server
                        DataTable table = getdataHangSanXuat();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table));
                    }

                    //chọn = 5 tìm chức vụ theo macv
                    if (chon == 5)
                    {
                        string mahsxtim = sr.ReadLine();
                        int kq = timmahangsanxuatdata(mahsxtim);

                        sw.WriteLine(kq);
                        sw.Flush();
                    }

                    if (chon == 1)
                    {
                        //nhận macv, tencv, hspc từ client
                        string mahsx = sr.ReadLine();
                        string tenhsx = sr.ReadLine();

                        DataTable table1 = themhangsanxuatdata(mahsx, tenhsx);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table1));
                    }

                    //chọn = 2 là sự kiện khi client nhấn nút xóa
                    if (chon == 2)
                    {
                        string mahsx = sr.ReadLine();

                        DataTable table2 = xoahangsanxuatdata(mahsx);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table2));

                    }

                    //chọn = 3 là sự kiện khi client nhấn nút cập nhật
                    if (chon == 3)
                    {
                        string mahsx = sr.ReadLine();
                        string tenhsx = sr.ReadLine();

                        DataTable table3 = capnhathangsanxuatdata(mahsx, tenhsx);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table3));
                    }

                    //chọn = 4 là sự kiện khi client nhấn nút tìm
                    if (chon == 4)
                    {
                        string tenhsxtim = sr.ReadLine();

                        DataTable table1 = timtenhangsanxuatdata(tenhsxtim);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table1));
                    }

                    #endregion

                    #region DonHang
                    //chọn = 10 là sự kiện khi client nhấn nút thêm
                    if (chon == 10)
                    {
                        //tạo datatable lấy dữ liệu table chức vụ từ sql server
                        DataTable table6 = getdataSanPham();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table6));

                        //tạo datatable lấy dữ liệu table nhân viên từ sql server
                        DataTable table7 = getdataSanPham();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table7));
                    }

                    if (chon == 11)
                    {
                        string madhtim = sr.ReadLine();
                        int kq = timdonhangtheomadata(madhtim);

                        sw.WriteLine(kq);
                        sw.Flush();
                    }

                    if (chon == 12)
                    {
                        //nhận macv, tencv, hspc từ client
                        string madh = sr.ReadLine();
                        string khachhang = sr.ReadLine();
                        string masp = sr.ReadLine();
                        float soluong = float.Parse(sr.ReadLine());
                        float tongtien = float.Parse(sr.ReadLine());
                        DateTime ngaylap = DateTime.Parse(sr.ReadLine());

                        DataTable table1 = themdonhangdata(madh, khachhang, masp, ngaylap, tongtien, soluong);

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table1));
                    }

                    #endregion

                    #region SanPham
                    //chọn = 11 là sự kiện khi client nhấn nút thêm
                    if (chon == 11)
                    {
                        //tạo datatable lấy dữ liệu table chức vụ từ sql server
                        DataTable table11 = getdataHangSanXuat();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table11));

                        //tạo datatable lấy dữ liệu table nhân viên từ sql server
                        DataTable table12 = getdataSanPham();

                        //chuyển datatable sang dạng mảng byte --> rồi gởi sang client
                        clientSock.Send(SerializeData(table12));

                    }
                    #endregion
                }
            }    
            
            DongKetNoi(KetNoi);
            server.Close();
            clientSock.Close();
            btn_start_server.Text = "&Start Server";
            Application.DoEvents();
        }

        //chuyển datatable sang dạng mảng byte
        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        /*
        private DataTable getdataChucVu()
        {    
            string sTruyVan = "select * from chucvu";  

            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        private DataTable getdataNhanVien()
        {
            string sTruyVan = "select * from nhanvien";

            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
        private DataTable themchucvudata(string macv, string tencv, float hspc)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"insert into chucvu values(N'{0}',N'{1}',N'{2}')", macv, tencv,hspc);
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if(kt == true)
            {
                string sTruyVan2 = "select * from chucvu";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt);               
            }            
            return dt;    
        }

        private DataTable xoachucvudata(string macv)
        {
            bool kt = false;
            DataTable dt2 = new DataTable();
            string sTruyVan1 = string.Format(@"delete from chucvu where macv='{0}'", macv);
            //MessageBox.Show(sTruyVan1.ToString());
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan1, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from chucvu";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt2);
            }
            return dt2;

        }

        private DataTable capnhatchucvudata(string macv, string tencv, float hspc)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"update chucvu set tencv=N'{0}',hsphucap='{1}' where macv='{2}'", tencv, hspc, macv);  
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from chucvu";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt);
            }
            return dt;

        }
        private DataTable timtenchucvudata(string tencvtim)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from chucvu where tencv like '%{0}%'", tencvtim);
            //MessageBox.Show("sql: "+sTruyVan);
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);
            
            return dt;

        }

        private int timmachucvudata(string macvtim)
        {
            int kq = 1;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from chucvu where macv = '{0}'", macvtim);          
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);
            
            int dem = dt.Rows.Count;
           // MessageBox.Show("so dong = "+dem);
            if (dem > 0)
                return 1;
            else
                return 0;
        }*/

        private int kiemtradangnhap(string tendangnhap, string matkhau)
        {
            int kq = 1;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from nguoidung where ten = '{0}' and matkhau = '{1}'", tendangnhap, matkhau);
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);

            int dem = dt.Rows.Count;

            if (dem > 0)
                return 1;
            else
                return 0;
        }

        // Hãng sản xuất
        #region GetDataHangSanXuat
        private DataTable getdataHangSanXuat()
        {
            string sTruyVan = "select * from hangsanxuat";

            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
        #endregion

        #region ThemHangSanXuat
        private DataTable themhangsanxuatdata(string mahsx, string tenhsx)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"insert into hangsanxuat values(N'{0}',N'{1}')", mahsx, tenhsx);
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from hangsanxuat";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt);
            }
            return dt;
        }
        #endregion

        #region TimHangSanXuatTheoMa
        private int timmahangsanxuatdata(string mahsxtim)
        {
            int kq = 1;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from hangsanxuat where mahsx = '{0}'", mahsxtim);
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);

            int dem = dt.Rows.Count;
            // MessageBox.Show("so dong = "+dem);
            if (dem > 0)
                return 1;
            else
                return 0;
        }
        #endregion

        #region XoaHangSanXuat
        private DataTable xoahangsanxuatdata(string mahsx)
        {
            bool kt = false;
            DataTable dt2 = new DataTable();
            string sTruyVan1 = string.Format(@"delete from hangsanxuat where mahsx='{0}'", mahsx);
            //MessageBox.Show(sTruyVan1.ToString());
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan1, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from hangsanxuat";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt2);
            }
            return dt2;

        }
        #endregion

        #region CapNhatHangSanXuat
        private DataTable capnhathangsanxuatdata(string mahsx, string tenhsx)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"update hangsanxuat set tenhsx=N'{0}' where mahsx='{1}'", tenhsx, mahsx);
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from hangsanxuat";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt);
            }
            return dt;
        }
        #endregion

        #region TimHangSanXuat
        private DataTable timtenhangsanxuatdata(string tenhsxtim)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from hangsanxuat where tenhsx like '%{0}%'", tenhsxtim);
            //MessageBox.Show("sql: "+sTruyVan);
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);

            return dt;
        }
        #endregion

        // Đơn Hàng
        #region GetDataDonHang
        private DataTable getdataDonHang()
        {
            string sTruyVan = "select * from donhang";

            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
        #endregion

        #region ThemDonHang
        private DataTable themdonhangdata(string madh, string khachhang, string masp, DateTime ngaylap, float tongtien, float soluong)
        {
            bool kt = false;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"insert into donhang values(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',N'{5}')", madh, khachhang, masp, ngaylap, tongtien, soluong);
            try
            {
                SqlCommand cm = new SqlCommand(sTruyVan, KetNoi);
                cm.ExecuteNonQuery();
                kt = true;
            }
            catch (Exception ex)
            {
                kt = false;
            }
            if (kt == true)
            {
                string sTruyVan2 = "select * from donhang";
                SqlDataAdapter da = new SqlDataAdapter(sTruyVan2, KetNoi);
                da.Fill(dt);
            }
            return dt;
        }
        #endregion

        #region TimDonHangTheoMa
        private int timdonhangtheomadata(string madhtim)
        {
            int kq = 1;
            DataTable dt = new DataTable();
            string sTruyVan = string.Format(@"select * from donhang where madh = '{0}'", madhtim);
            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            da.Fill(dt);

            int dem = dt.Rows.Count;
            // MessageBox.Show("so dong = "+dem);
            if (dem > 0)
                return 1;
            else
                return 0;
        }
        #endregion

        // Sản Phẩm
        #region GetDataSanPham
        private DataTable getdataSanPham()
        {
            string sTruyVan = "select * from sanpham";

            SqlDataAdapter da = new SqlDataAdapter(sTruyVan, KetNoi);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }
        #endregion
    }
}
