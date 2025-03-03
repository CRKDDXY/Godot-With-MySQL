using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class register : Node2D{
	[Export] public Button 返回;
	[Export] public Button 注册按钮;
	[Export] public TextEdit 用户名;
	[Export] public TextEdit 密码;
	[Export] public TextEdit 真实姓名;
	[Export] public TextEdit 年级;
	[Export] public Label 提示;

	private void 跳转_登录界面(){
		//注册按钮
		GetTree().ChangeSceneToFile("res://界面/登录界面/登录界面.tscn");
	}

private void 成功注册并跳转(string 指令,string 全局脚本调用){
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			MySqlCommand cmd2 = new MySqlCommand(指令, connection);
			cmd2.Parameters.AddWithValue("@权限等级", "2");
			cmd2.Parameters.AddWithValue("@用户名", 用户名.Text);
			cmd2.Parameters.AddWithValue("@密码", 密码.Text);
			cmd2.Parameters.AddWithValue("@真实姓名", 真实姓名.Text);
			cmd2.Parameters.AddWithValue("@年级", 年级.Text);
			cmd2.ExecuteNonQuery();
			//全局脚本.上次使用的指令.Add(全局脚本调用);
			GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
		}
	}

	private void 注册(){
		if(用户名.Text!="" && 密码.Text!="" && 真实姓名.Text!="" && 年级.Text!=""){
			using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
				// 各个输入框都不为空->数据库存储+页面跳转
				connection.Open();
				// 查询真实姓名是否已经存在
				string 查询真实姓名 = "SELECT `真实姓名` FROM `账号表` WHERE `真实姓名` = '" + 真实姓名.Text + "' OR `用户名` = '"+用户名.Text+"';";
				MySqlCommand cmd1 = new MySqlCommand(查询真实姓名, connection); // 创建MySQLCommand对象(指令,连接)
				// 使用 ExecuteReader 执行查询并读取数据
				using (MySqlDataReader reader1 = cmd1.ExecuteReader()){
					if(reader1.Read()){
						提示.Visible = true;
						提示.Text = "已有该真实姓名或者用户名的账号";
						//全局脚本.上次使用的指令.Add(查询真实姓名);
					}else{
						全局脚本.level = "2";
						全局脚本.用户名 = 用户名.Text;
						全局脚本.真实姓名 = 真实姓名.Text;
						成功注册并跳转("CALL 添加新账号(@权限等级, @用户名, @密码, @真实姓名, @年级);",
									   "CALL 添加新账号('2', '"+用户名.Text+"', '"+密码.Text+"', '"+真实姓名.Text+"', '"+年级.Text+"');");
					}
					reader1.Close();
				}
				connection.Close();
			}
		}else{
			// 提示有空的输入栏
			提示.Visible = true;
			提示.Text = "请按要求填写所有内容";
		}
	}
	public override void _Ready(){
		提示.Visible = false;
		返回.Pressed +=()=> 跳转_登录界面();
		注册按钮.Pressed +=()=> 注册();
	}
}