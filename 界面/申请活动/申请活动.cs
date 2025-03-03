using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 申请活动 : Node2D{

	[Export] public Button 返回;
	[Export] public GridContainer 拥有申请活动权限的社团列表;

	[Export] public Button 确认申请;

	public override void _Ready(){
		活动申请填写提示.Visible = false;
		返回.Pressed +=()=> _返回();
		确认申请.Pressed +=()=> 插入活动申请记录();
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string query = "SELECT DISTINCT `社团名称` FROM `社团成员表` WHERE `权限等级` = '1' AND `真实姓名` = '"+ 全局脚本.真实姓名+"'";
			using (var cmd = new MySqlCommand(query, connection)){
				using (var reader = cmd.ExecuteReader()){
					while (reader.Read()){
						Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
						Label 名称 = (Label)panel.GetChild(0);
						TextureButton 点击区域 = (TextureButton)panel.GetChild(1);
						点击区域.Pressed +=()=> _单个社团选择(panel);
						名称.Text = (string)reader["社团名称"];
						拥有申请活动权限的社团列表.AddChild(panel);
					}
				}
			}
			connection.Close();
		}

	}

	public void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

	[Export] public Label 活动申请填写提示;
	Panel 被选中的社团;

	public void _单个社团选择(Panel p){
		活动申请填写提示.Visible = true;
		被选中的社团 = p;
		Label 名称 = (Label)p.GetChild(0);
		活动申请填写提示.Text = "活动申请表\n("+名称.Text+")";
		活动申请填写提示.Visible = true;
		TextEdit 理据 = (TextEdit)活动申请填写提示.GetChild(0).GetChild(0);
	}

	private void 插入活动申请记录(){
		Label 名称 = (Label)被选中的社团.GetChild(0);
		TextEdit 理据 = (TextEdit)活动申请填写提示.GetChild(0).GetChild(0);
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string insertQuery = "INSERT INTO `活动申请审核表` (`申请时间`, `社团名称`, `开展理据`) VALUES (CURDATE(), @社团名称, @开展理据)";
			using (var cmd = new MySqlCommand(insertQuery, connection)){
				cmd.Parameters.AddWithValue("@社团名称", 名称.Text);
				cmd.Parameters.AddWithValue("@开展理据", 理据.Text);
				cmd.ExecuteNonQuery(); // 执行插入操作
			}
			connection.Close();
		}
		理据.Text = "申请提交成功!";
	}

}