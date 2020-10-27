using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TVDCEG
{
	public partial class LegendManagerForm : Form
	{
		public LegendManagerForm(LegendManager data)
		{
			this._data = data;
			this.InitializeComponent();
			this.cbType.SelectedIndex = 0;
		}

		private void LegendManagerForm_Load(object sender, EventArgs e)
		{
			this.DisplayToView(this._data.Dic);
		}

		private void DisplayToView(Dictionary<int, LegendExtension> dic)
		{
			string text = this.cbType.Text;
			int num = -1;
			bool flag = text == "Legend has deleted";
			if (flag)
			{
				num = 3;
			}
			bool flag2 = text == "New Legend";
			if (flag2)
			{
				num = 2;
			}
			bool flag3 = text == "Legend has renamed";
			if (flag3)
			{
				num = 1;
			}
			this.dgv.Rows.Clear();
			IOrderedEnumerable<LegendExtension> orderedEnumerable = from x in dic.Values.ToList<LegendExtension>()
			orderby x.Name
			select x;
			foreach (LegendExtension legendExtension in orderedEnumerable)
			{
				bool flag4 = !legendExtension.Name.Contains(this.txtSearch.Text);
				if (!flag4)
				{
					bool flag5 = num == -1;
					if (!flag5)
					{
						bool flag6 = num != legendExtension.MoreInfo;
						if (flag6)
						{
							continue;
						}
					}
					int index = this.dgv.Rows.Add();
					this.dgv.Rows[index].Cells[0].Value = legendExtension.Name;
					this.dgv.Rows[index].Cells[1].Value = legendExtension.Creator;
					this.dgv.Rows[index].Cells[2].Value = legendExtension.LastChangedBy;
					this.dgv.Rows[index].Cells[3].Value = legendExtension.ListSheets;
					switch (legendExtension.MoreInfo)
					{
					case 1:
						this.dgv.Rows[index].DefaultCellStyle.BackColor = Color.Red;
						break;
					case 2:
						this.dgv.Rows[index].DefaultCellStyle.BackColor = Color.Green;
						break;
					case 3:
						this.dgv.Rows[index].DefaultCellStyle.BackColor = Color.Gray;
						break;
					}
				}
			}
		}
		private void btExport_Click(object sender, EventArgs e)
		{
			string name = this._data.Doc.ProjectInformation.Name;
			string contents = JsonConvert.SerializeObject(this._data.Dic, Formatting.Indented);
			string path = DateTime.Now.ToString("yyyy-MM-dd__hh-mm-ss") + ".json";
			string text = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2017\\Data\\";
			text = Path.Combine(text, name);
			Directory.CreateDirectory(text);
			string text2 = Path.Combine(text, path);
			File.WriteAllText(text2, contents);
			MessageBox.Show("Export Complete!\n" + text2);
		}
		private void btLoadData_Click(object sender, EventArgs e)
		{
			string name = this._data.Doc.ProjectInformation.Name;
			string text = "";
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = Path.Combine("C:\\ProgramData\\Autodesk\\Revit\\Addins\\2017\\Data", name),
				Title = "Browse Data Files",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "JsonFile1",
				Filter = "JsonFile (*.json)|*.json",
				FilterIndex = 2,
				RestoreDirectory = true,
				ReadOnlyChecked = true,
				ShowReadOnly = true
			};
			bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
			if (flag)
			{
				text = openFileDialog.FileName;
			}
			bool flag2 = text.Length > 1;
			if (flag2)
			{
				string value = File.ReadAllText(text);
				Dictionary<int, LegendExtension> old = JsonConvert.DeserializeObject<Dictionary<int, LegendExtension>>(value);
				this.Compare2Dic(this._data.Dic, old);
			}
		}
		private void Compare2Dic(Dictionary<int, LegendExtension> current, Dictionary<int, LegendExtension> old)
		{
			this.newDic.Clear();
			foreach (KeyValuePair<int, LegendExtension> keyValuePair in current)
			{
				LegendExtension value = keyValuePair.Value;
				int key = keyValuePair.Key;
				bool flag = old.ContainsKey(keyValuePair.Key);
				if (flag)
				{
					LegendExtension legendExtension = old[keyValuePair.Key];
					bool flag2 = value.Name != legendExtension.Name;
					if (flag2)
					{
						LegendExtension legendExtension2 = value;
						legendExtension2.Name = legendExtension2.Name + "_[" + legendExtension.Name + "]";
						value.MoreInfo = 1;
					}
					this.newDic.Add(key, value);
				}
				else
				{
					value.MoreInfo = 2;
					this.newDic.Add(key, value);
				}
			}
			foreach (KeyValuePair<int, LegendExtension> keyValuePair2 in old)
			{
				int key2 = keyValuePair2.Key;
				LegendExtension value2 = keyValuePair2.Value;
				bool flag3 = !current.ContainsKey(key2);
				if (flag3)
				{
					value2.MoreInfo = 3;
					this.newDic.Add(key2, value2);
				}
			}
			this.DisplayToView(this.newDic);
		}

		private void cbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = this.newDic.Count > 0;
			if (flag)
			{
				this.DisplayToView(this.newDic);
			}
			else
			{
				this.DisplayToView(this._data.Dic);
			}
		}
		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			bool flag = this.newDic.Count > 0;
			if (flag)
			{
				this.DisplayToView(this.newDic);
			}
			else
			{
				this.DisplayToView(this._data.Dic);
			}
		}
		private Dictionary<int, LegendExtension> newDic = new Dictionary<int, LegendExtension>();
		private LegendManager _data;
	}
}
