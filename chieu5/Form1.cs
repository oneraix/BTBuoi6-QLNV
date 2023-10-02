using chieu5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace chieu5
{
    public partial class Form1 : Form
    {
        private NhanvienContextDB context;
        public Form1()
        {
            InitializeComponent();
            context = new NhanvienContextDB();
        }
        SqlConnection cnn = null;
        string strcnn = "Data Source=DESKTOP-B7D08GD\\MSSQLSERVER01;Initial Catalog=QLNhanSu;Integrated Security=True";
        DataSet ds = null;
        SqlDataAdapter adt = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                NhanvienContextDB  contex = new NhanvienContextDB();
                List<Nhanvien> dsnv = contex.Nhanviens.ToList();
                BindGrid(dsnv);
                var phongBanList = contex.Phongbans.ToList();
                cmbPhongban.DataSource = phongBanList;
                cmbPhongban.DisplayMember = "TenPB";
                cmbPhongban.ValueMember = "MaPB";
            }
            catch 
            {

            }
        }

 

        private void BindGrid  (List<Nhanvien> dsnv)
        {
            dgvNhanvien.Rows.Clear();
            foreach(var item in dsnv)
            {
                int index = dgvNhanvien.Rows.Add();
                dgvNhanvien.Rows[index].Cells[0].Value = item.MaNV;
                dgvNhanvien.Rows[index].Cells[1].Value = item.TenNV;
                dgvNhanvien.Rows[index].Cells[2].Value = item.Phongban.TenPB;
                dgvNhanvien.Rows[index].Cells[3].Value = item.Ngaysinh;
            }
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var context = new NhanvienContextDB())
            {
                string maNV = txtMaNV.Text;

                // Kiểm tra xem mã nhân viên đã tồn tại chưa
                bool maNVTonTai = context.Nhanviens.Any(nv => nv.MaNV == maNV);

                if (maNVTonTai)
                {
                    MessageBox.Show("Mã Nhân viên đã tồn tại. Vui lòng chọn mã khác.");
                }
                else
                {
                    // Nếu mã Nhân viên không trùng, thì thêm mới
                    var nhanvien = new Nhanvien()
                    {
                        MaNV = maNV,
                        TenNV = txtTenNV.Text,
                        MaPB = cmbPhongban.SelectedValue.ToString(),
                        Ngaysinh = dtpNgaysinh.Value
                    };

                    context.Nhanviens.Add(nhanvien);
                    context.SaveChanges();

                    // Sau khi thêm nhân viên, cập nhật DataGridView
                    List<Nhanvien> dsnv = context.Nhanviens.ToList();
                    BindGrid(dsnv);
                }
            }

        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Lấy mã nhân viên cần cập nhật từ TextBox hoặc DataGridView
            string maNV = txtMaNV.Text; // hoặc lấy từ DataGridView nếu có
            if (!string.IsNullOrEmpty(maNV))
            {
                using (var context = new NhanvienContextDB())
                {
                    // Tìm nhân viên theo mã nhân viên
                    var nhanvien = context.Nhanviens.FirstOrDefault(nv => nv.MaNV == maNV);

                    if (nhanvien != null)
                    {
                        // Kiểm tra nếu MaNV đã tồn tại và không được thay đổi
                        if (nhanvien.MaNV == maNV)
                        {
                            // Cập nhật thông tin mới từ các điều kiện nhập liệu (ví dụ: tên, mã phòng ban, ngày sinh)
                            nhanvien.TenNV = txtTenNV.Text;
                            nhanvien.MaPB = cmbPhongban.SelectedValue.ToString();
                            nhanvien.Ngaysinh = dtpNgaysinh.Value;

                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();

                            // Sau khi cập nhật thành công, cập nhật DataGridView
                            List<Nhanvien> dsnv = context.Nhanviens.ToList();
                            BindGrid(dsnv);
                        }
                        else
                        {
                            MessageBox.Show("Không được thay đổi Mã Nhân viên thành giá trị đã tồn tại.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy Nhân viên.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần cập nhật thông tin.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Lấy mã nhân viên cần xóa từ TextBox hoặc DataGridView
            string maNV = txtMaNV.Text; // hoặc lấy từ DataGridView nếu có
            if (!string.IsNullOrEmpty(maNV))
            {
                // Hiển thị hộp thoại xác nhận trước khi xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa Nhân viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    using (var context = new NhanvienContextDB())
                    {
                        // Tìm nhân viên theo mã nhân viên
                        var nhanvien = context.Nhanviens.FirstOrDefault(nv => nv.MaNV == maNV);

                        if (nhanvien != null)
                        {
                            // Xóa nhân viên
                            context.Nhanviens.Remove(nhanvien);
                            context.SaveChanges();

                            // Sau khi xóa thành công, cập nhật DataGridView
                            List<Nhanvien> dsnv = context.Nhanviens.ToList();
                            BindGrid(dsnv);

                            // Xóa các thông tin hiển thị
                            txtMaNV.Clear();
                            txtTenNV.Clear();
                            // Cập nhật ComboBox Phòng ban nếu cần
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy Nhân viên.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui chọn nhân viên cần xóa.");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Chương trình sẽ đóng", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dgvNhanvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRowIndex = e.RowIndex;
            if (selectedRowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvNhanvien.Rows[selectedRowIndex];
                txtMaNV.Text = selectedRow.Cells[0].Value.ToString();
                txtTenNV.Text = selectedRow.Cells[1].Value.ToString();
                cmbPhongban.Text = selectedRow.Cells[2].Value.ToString();
                dtpNgaysinh.Text = selectedRow.Cells[3].Value.ToString();
            }
        }
    }
}
