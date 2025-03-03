using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 管理社团成员 : Node2D{
	[Export] public GridContainer 社团列表;
	[Export] public GridContainer 成员列表;
	[Export] public Label 成员列表名称;
	[Export] public Button 返回按钮;
	[Export] public Label 权限修改提示;
	[Export] public Button 改为权限1;
	[Export] public Button 改为权限2;
 	public override async void _Ready(){
		权限修改提示.Visible = false;
		改为权限1.Pressed +=()=> _权限改为1();
		改为权限2.Pressed +=()=> _权限改为2();
		返回按钮.Pressed +=()=> _返回();
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string query = "SELECT * FROM `社团名称视图`;";
			MySqlCommand cmd = new(query, connection);  // 指令 连接
			try{
				using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
					while(reader.Read()){
						Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
							Label 名称 = (Label)panel.GetChild(0);
							TextureButton 点击区域 = (TextureButton)panel.GetChild(1);
							点击区域.Pressed +=()=> _单个社团选择(panel);
							名称.Text = (string)reader["社团名称"];
							社团列表.AddChild(panel);
						}
						reader.Close();
					}
				connection.Close();
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
		}
	}

	public void _单个社团选择(Panel p){
		Label 社团名称 = (Label)p.GetChild(0);
		被选中的社团名称 = 社团名称.Text;
		_成员列表更新(社团名称.Text);
	}

	public void _成员列表更新(string 社团名称){
		for(int i=0;i<成员列表.GetChildCount();i++){
			成员列表.GetChild(i).QueueFree();
		}
		成员列表名称.Text = "成员列表\n("+社团名称+")";
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string query = "SELECT `真实姓名`, `权限等级` FROM `社团成员表` WHERE `社团名称` = @clubName";
			using (var cmd = new MySqlCommand(query, connection)){
				cmd.Parameters.AddWithValue("@clubName", 社团名称);
				using (var reader = cmd.ExecuteReader()){
					// 检查是否有返回的数据
					if (reader.HasRows){
						// 遍历返回的数据行
						while (reader.Read()){
							Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
							Label 名称 = (Label)panel.GetChild(0);
							TextureButton 点击区域 = (TextureButton)panel.GetChild(1);
							点击区域.Pressed +=()=> _单个成员选择(panel);
							名称.Text = "真实姓名:"+(string)reader["真实姓名"]+"\n权限等级:"+(string)reader["权限等级"];
							if((string)reader["真实姓名"]!="admin"){
								成员列表.AddChild(panel);
							}
						}
					}
				}
			}
		}
	}

	string 被选中的对象名称;
	string 被选中的社团名称;
	Label 被选中对象;

	public void _单个成员选择(Panel p){
		被选中对象 = p.GetChild(0) as Label;
		Label 对象名称 = (Label)p.GetChild(0);
		// 假设名称.Text已经包含了"真实姓名:"后跟真实姓名和"\n权限等级:"后跟权限等级
		string 名称Text = 对象名称.Text;
		// 使用Split方法按换行符分割字符串
		string[] parts = 名称Text.Split('\n');
		// 检查分割后的数组长度是否足够
		if (parts.Length > 1){
			// 第一部分是包含"真实姓名:"的字符串
			string 真实姓名部分 = parts[0];
			// 使用Substring和IndexOf方法找到"真实姓名"后面的实际姓名
			int 真实姓名起始索引 = 真实姓名部分.IndexOf("真实姓名:") + "真实姓名:".Length;
			string 真实姓名 = 真实姓名部分.Substring(真实姓名起始索引);
			被选中的对象名称 = 真实姓名;
		}
		权限修改提示.Text = "是否修改社团成员的权限\n("+被选中的对象名称+")";
		权限修改提示.Visible = true;
	}

	public void _权限改为1(){
		被选中对象.Text = "真实姓名:"+被选中的对象名称+"\n权限等级:1";
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string updateQuery = "UPDATE `社团成员表` SET `权限等级` = '1' WHERE `真实姓名` = @真实姓名 AND `社团名称` = @社团名称";
			using (var cmd = new MySqlCommand(updateQuery, connection)){
				cmd.Parameters.AddWithValue("@真实姓名", 被选中的对象名称);
				cmd.Parameters.AddWithValue("@社团名称", 被选中的社团名称);
				cmd.ExecuteScalar();	//执行
			}
    	}
	}

	public void _权限改为2(){
		被选中对象.Text = "真实姓名:"+被选中的对象名称+"\n权限等级:2";
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string updateQuery = "UPDATE `社团成员表` SET `权限等级` = '2' WHERE `真实姓名` = @真实姓名 AND `社团名称` = @社团名称";
			using (var cmd = new MySqlCommand(updateQuery, connection)){
				cmd.Parameters.AddWithValue("@真实姓名", 被选中的对象名称);
				cmd.Parameters.AddWithValue("@社团名称", 被选中的社团名称);
				cmd.ExecuteScalar(); //执行
			}
    	}
	}

	public void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}
}