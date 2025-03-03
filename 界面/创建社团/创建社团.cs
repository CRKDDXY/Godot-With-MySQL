using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 创建社团 : Node2D{
	[Export] public TextEdit 社团名称;
	[Export] public	Button 返回按钮;
	[Export] public Button 确认按钮;
	[Export] public Label 提示;

	public override void _Ready(){
		社团名称.Text = "";
		提示.Visible = false;
		返回按钮.Pressed +=()=> _返回();
		确认按钮.Pressed +=()=> _确认();
	}
	private void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

	private async void _确认(){
		if(社团名称.Text == ""){
			提示.Text = "社团名不能为空!";
			提示.Visible = true;
			return;
		}
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
            connection.Open();
			// 首先检查数据库中是否已经存在同名的社团
			string checkQuery = "SELECT COUNT(*) FROM `社团表` WHERE `社团名称` = @clubName";
			MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
			checkCmd.Parameters.AddWithValue("@clubName", 社团名称.Text);
			var countResult = await checkCmd.ExecuteScalarAsync();
        	int count = Convert.ToInt32(countResult);
			if(count == 0){
				string query = "INSERT INTO `社团表` (`社团名称`, `创立时间`) VALUES ('"+社团名称.Text+"', DATE(NOW()));";
				MySqlCommand cmd = new(query, connection);
				using (var reader = await cmd.ExecuteReaderAsync()){
					reader.Close();
				}
				_返回();	//创建完毕后返回
			}else{
				提示.Text = "已有该名称的社团,请重新取名!";
				提示.Visible = true;
			}
			connection.Close();
		}
		_赋予adimn社团的0级权限(社团名称.Text);
	}

	public async void _赋予adimn社团的0级权限(string 社团名称){
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
				connection.Open();
				// 插入admin的信息到社团成员表
				string insertQuery = @"
					INSERT INTO `社团成员表` (`进入社团时间`, `真实姓名`, `社团名称`, `权限等级`)
					VALUES (NOW(), @真实姓名, @社团名称, @权限等级);";
				using (var cmd = new MySqlCommand(insertQuery, connection)){
					cmd.Parameters.AddWithValue("@真实姓名", "admin");
					cmd.Parameters.AddWithValue("@社团名称", 社团名称);
					cmd.Parameters.AddWithValue("@权限等级", "0");
					await cmd.ExecuteNonQueryAsync(); // 使用ExecuteNonQueryAsync异步执行插入操作
				}
					connection.Close();
				}
	}

}
