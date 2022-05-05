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

        private void dgDanhSachHang_Click(object sender, EventArgs e)
        {
            DataGridViewRow r = new DataGridViewRow();
            r = dgDanhSachHang.SelectedRows[0];
            txtMaHSX.Text = r.Cells["mahsx"].Value.ToString();
            txtTenHSX.Text = r.Cells["tenhsx"].Value.ToString();
        }
    }
}
