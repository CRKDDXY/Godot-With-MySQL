using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 活动审核 : Node2D{
	[Export] public Button 返回;
	[Export] public GridContainer 申请活动列表; 
	public override void _Ready(){
		返回.Pressed +=()=> _返回();
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string query = "SELECT `申请时间`, `活动编号`, `社团名称`, `开展理据` FROM `活动申请审核表`";
			using (var cmd = new MySqlCommand(query, connection)){
				using (var reader = cmd.ExecuteReader()){
					while (reader.Read()){
						Panel panel = (Panel)GD.Load<PackedScene>("res://界面/活动审核/单条活动记录.tscn").Instantiate();
						Label 申请时间 = (Label)panel.GetChild(0).GetChild(0);
						DateTime time = (DateTime)reader["申请时间"];
						申请时间.Text = time.ToString("yyyy-MM-dd"); // 格式化为年-月-日的形式
						Label 活动编号 = (Label)panel.GetChild(0).GetChild(1);
						活动编号.Text = reader["活动编号"].ToString();
						Label 社团名称 = (Label)panel.GetChild(0).GetChild(2);
						社团名称.Text = (string)reader["社团名称"];
						Label 开展理据 = (Label)panel.GetChild(0).GetChild(3);
						开展理据.Text = (string)reader["开展理据"];
						Button 通过 = (Button)panel.GetChild(1);
						通过.Pressed +=()=> _该活动通过(panel);
						申请活动列表.AddChild(panel);
					}
				}
			}
			connection.Close();
		}
	}

	public void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

	private void _该活动通过(Panel panel){
		// 获取社团名称和活动编号
		Label 社团名称Label = (Label)panel.GetChild(0).GetChild(2);
		string 社团名称 = 社团名称Label.Text;
		Label 活动编号Label = (Label)panel.GetChild(0).GetChild(1);
		string 活动编号Str = 活动编号Label.Text;
		int 活动编号 = int.Parse(活动编号Str);
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			// 删除活动申请审核表中的记录
			string deleteQuery = "DELETE FROM `活动申请审核表` WHERE `社团名称` = @社团名称 AND `活动编号` = @活动编号";
			using (var deleteCmd = new MySqlCommand(deleteQuery, connection)){
				deleteCmd.Parameters.AddWithValue("@社团名称", 社团名称);
				deleteCmd.Parameters.AddWithValue("@活动编号", 活动编号);
				deleteCmd.ExecuteNonQuery();
			}
			// 插入活动表中的记录
			string insertQuery = "INSERT INTO `活动表` (`社团名称`,`活动简述`) VALUES (@社团名称, @活动简述)";
			using (var insertCmd = new MySqlCommand(insertQuery, connection)){
				Label 开展理据Label = (Label)panel.GetChild(0).GetChild(3);
				string 开展理据 = 开展理据Label.Text;
				insertCmd.Parameters.AddWithValue("@活动简述", 开展理据);
				insertCmd.Parameters.AddWithValue("@社团名称", 社团名称);
				insertCmd.ExecuteNonQuery();
			}
			connection.Close();
		}
		// 从申请活动列表中移除该面板
		申请活动列表.RemoveChild(panel);
		panel.QueueFree();
	}
}