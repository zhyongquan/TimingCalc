using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace TimingCalc
{
    
    public partial class Form1 : Form
    {
        List<Milestone> m_Milestone;
        public Form1()
        {
            InitializeComponent();
            dgvTiming.AutoGenerateColumns = false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
                bool flag = LoadData();
                if (flag)
                {
                    Calc();
                }
        }
        private void Calc() 
        {
            DateTime start = dtPicker.Value;
            m_Milestone[0].Time = start;
            for (int i = 1; i < m_Milestone.Count; i++) 
            {
                if (m_Milestone[i].StartTime.Length == 1)
                {
                    m_Milestone[i].Calc(m_Milestone[m_Milestone[i].StartTime[0]-1].Time);
                }
                else 
                {
                    DateTime time1 = start;
                    foreach (int j in m_Milestone[i].StartTime) 
                    {
                        if (time1 < m_Milestone[j-1].Time) 
                        {
                            time1 = m_Milestone[j-1].Time; 
                        }
                    }
                    m_Milestone[i].Calc(time1);
                }
            }
            dgvTiming.DataSource = m_Milestone;
            dgvTiming.Refresh();
        }
        private bool LoadData() 
        {
            dgvTiming.DataSource = null;
            dgvTiming.Refresh();
            DialogResult result = this.ofdImport.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    m_Milestone = new List<Milestone>();
                    DataSet ds = LoadDataFromExcel(this.ofdImport.FileName, "Sheet1");
                    DataTable table = ds.Tables["Sheet1"];

                    foreach (DataRow row in table.Rows)
                    {
                        if (row[0] == null) break;
                        Milestone stone = new Milestone();
                        stone.No = int.Parse(row[0].ToString());
                        stone.Description = row[1].ToString();
                        stone.BaseTime = row[2].ToString();
                        stone.Interval = int.Parse(row[3].ToString());
                        stone.WorkDay = row[4].ToString();
                        m_Milestone.Add(stone);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                return true;
            }
            return false;
        }
        //加载Excel   
        public static DataSet LoadDataFromExcel(string filePath, string sheetName)
        {
            try
            {
                string fileType = System.IO.Path.GetExtension(filePath);
                if (string.IsNullOrEmpty(fileType)) return null;
                string strConn = "";
                if (fileType == ".xls")
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                OleDbConnection OleConn = new OleDbConnection(strConn);
                OleConn.Open();
                String sql = "SELECT * FROM  [" + sheetName + "$]";//可是更改Sheet名称，比如sheet2，等等   

                OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(sql, OleConn);
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, sheetName);
                OleConn.Close();
                return OleDsExcle;
            }
            catch (Exception err)
            {
                MessageBox.Show("数据绑定Excel失败!失败原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (m_Milestone != null && m_Milestone.Count > 0) 
            {
                DataToExcel(dgvTiming);
            }
        }
        public static void DataToExcel(DataGridView dgv)
        {

            if (dgv.Rows.Count == 0)
            {

                MessageBox.Show("无数据"); return;

            }

            MessageBox.Show("开始生成要导出的数据", "导出提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Excel.Application excel = new Excel.Application();

            excel.Application.Workbooks.Add(true);

            excel.Visible = false;

            //for (int i = 0; i < dgv.ColumnCount; i++)

            //    excel.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            excel.Cells[1, 1] = "编号";
            excel.Cells[1, 2] = "内容";
            excel.Cells[1, 3] = "基础";
            excel.Cells[1, 4] = "时间间隔（天）";
            excel.Cells[1, 5] = "是否工作日";
            excel.Cells[1, 6] = "生效时间";
            //tempProgressBar.Visible = true;

            //tempProgressBar.Minimum = 1;

            //tempProgressBar.Maximum = dgv.RowCount;

            //tempProgressBar.Step = 1;

            //toolstrip.Visible = true;

            for (int i = 0; i < dgv.RowCount; i++)
            {

                for (int j = 0; j < dgv.ColumnCount; j++)
                {

                    if (dgv[j, i].ValueType == typeof(string))
                    {

                        excel.Cells[i + 2, j + 1] = "'" + dgv[j, i].Value.ToString();

                    }

                    else
                    {
                        if (dgv[j, i].Value != null)
                            excel.Cells[i + 2, j + 1] = dgv[j, i].Value.ToString();

                    }

                }

                //toolstrip.Text = "|| 状态：正在生成第 " + i + "/" + dgv.RowCount + " 个";

                //tempProgressBar.Value = i + 1;

            }

            //toolstrip.Text = "|| 状态：生成成功！";

            MessageBox.Show("生成成功，请保存。", "生成提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            excel.Visible = true;

        }
    }
}
