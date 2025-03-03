using Godot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public partial class 个人社团管理 : Node2D{
	[Export] public GridContainer 所属社团;
	[Export] public GridContainer 其他社团;
	[Export] public Button 返回;
	[Export] public Label 入社提示;
	[Export] public Label 出社提示;

	public override async void _Ready(){
		入社提示.Visible = false;
		出社提示.Visible = false;
		Button 入社提示_确认 = (Button)入社提示.GetChild(0);
		Button 入社提示_取消 = (Button)入社提示.GetChild(1);
		Button 出社提示_确认 = (Button)出社提示.GetChild(0);
		Button 出社提示_取消 = (Button)出社提示.GetChild(1);
		入社提示_确认.Pressed +=()=> _确定操作();
		出社提示_确认.Pressed +=()=> _确定操作();
		入社提示_取消.Pressed +=()=> _取消操作();
		出社提示_取消.Pressed +=()=> _取消操作();
		返回.Pressed +=()=> _返回上级页面();
		List<string> 全部社团名称 = new List<string>{};
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "SELECT * FROM `社团名称视图`;";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
                    while(reader.Read()){
						全部社团名称.Add((string)reader["社团名称"]);	//获取全部社团名称 -> 暂时不打印
                    }
					reader.Close();
                }
				query = "SELECT 社团名称 FROM 社团成员表 WHERE 真实姓名 = '"+全局脚本.真实姓名+"';";
				cmd = new(query, connection);  // 指令 连接
				try{
					using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
						while(reader.Read()){
							//所在社团集合
							全部社团名称.Remove((string)reader["社团名称"]);
							Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
							Label 名称 = (Label)panel.GetChild(0);
							TextureButton 点击区域 = (TextureButton)panel.GetChild(1);
							点击区域.Pressed +=()=> _单个社团选择(panel);
							名称.Text = (string)reader["社团名称"];
							所属社团.AddChild(panel);
						}
						//不在的社团集合展示
						foreach(string clubName in 全部社团名称){
							Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
							Label 名称 = (Label)panel.GetChild(0);
							TextureButton 点击区域 = (TextureButton)panel.GetChild(1);
							点击区域.Pressed +=()=> _单个社团选择(panel);
							名称.Text = clubName;
							其他社团.AddChild(panel);
						}
						reader.Close();
                	}
					//添加其他的
				}catch(MySqlException ex){
                	GD.PrintErr("MySQL Error: " + ex.Message);
            	}
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
			connection.Close();
        }
	}
	public void _返回上级页面(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

	Panel 选中的社团;

	public void _单个社团选择(Panel p){
		//先判定是所属 还是不所属
		if(p.GetNode("..").Name == "所属社团列表"){
			出社提示.Visible = true;
			入社提示.Visible = false;
			Label 名称 = (Label)p.GetChild(0);
			出社提示.Text = "是否退出社团？\n("+名称.Text+")";
		}else{
			入社提示.Visible = true;
			出社提示.Visible = false;
			Label 名称 = (Label)p.GetChild(0);
			入社提示.Text = "是否进入社团？\n("+名称.Text+")";
		}
		选中的社团 = p;
	}

	public void _确定操作(){

		Panel panel = (Panel)GD.Load<PackedScene>("res://界面/个人社团管理/单个社团名称.tscn").Instantiate();
		Label 名称 = (Label)panel.GetChild(0);
		TextureButton 点击区域 = (TextureButton)panel.GetChild(1);

		//名称获取并且添加
		Label 名称_ = (Label)选中的社团.GetChild(0);
		名称.Text = 名称_.Text;	//名称添加

		//点击事件绑定
		点击区域.Pressed +=()=> _单个社团选择(panel);

		选中的社团.QueueFree();		//原对象清除

		//先判定社团对操作者的性质(所属/不所属)
		if(选中的社团.GetNode("..").Name == "所属社团列表"){
			//进行退社处理 (显示修改，节点变化 + 数据库处理)
			其他社团.AddChild(panel);
			删除社团成员信息(全局脚本.真实姓名,名称.Text);
		}else{
			//进行入社处理	-> 权限等级默认为 2 ，需要 admin 权限提拔
			所属社团.AddChild(panel);
			添加社团成员(全局脚本.真实姓名,名称.Text);
		}
		_取消操作();	//操作成功，取消显示
	}

	public void _取消操作(){
		//清空显示即可
		入社提示.Visible = false;
		出社提示.Visible = false;
	}


	public void 添加社团成员(string 用户名, string 社团名称){
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			// 首先检查数据库中是否已经存在同名的社团成员
			string checkQuery = "SELECT COUNT(*) FROM `社团成员表` WHERE `真实姓名` = @用户名 AND `社团名称` = @社团名称";
			using (var checkCmd = new MySqlCommand(checkQuery, connection)){
				checkCmd.Parameters.AddWithValue("@用户名", 用户名);
				checkCmd.Parameters.AddWithValue("@社团名称", 社团名称);
				var count = checkCmd.ExecuteScalar(); // 执行查询并获取结果
				// 如果不存在记录，则插入新记录
				if (count != null && Convert.ToInt32(count) == 0){
					string insertQuery = "INSERT INTO `社团成员表` (`进入社团时间`, `真实姓名`, `社团名称`, `权限等级`) VALUES (CURDATE(), @用户名, @社团名称, '2')";
					using (var insertCmd = new MySqlCommand(insertQuery, connection)){
						insertCmd.Parameters.AddWithValue("@用户名", 用户名);
						insertCmd.Parameters.AddWithValue("@社团名称", 社团名称);
						int affectedRows = insertCmd.ExecuteNonQuery(); // 执行插入操作
					}
				}
			}
			connection.Close();
		}
	}

	private void 删除社团成员信息(string 用户名, string 社团名称){
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			// 首先检查数据库中是否已经存在该成员
			string checkQuery = "SELECT COUNT(*) FROM `社团成员表` WHERE `真实姓名` = @用户名 AND `社团名称` = @社团名称";
			using (var checkCmd = new MySqlCommand(checkQuery, connection)){
				checkCmd.Parameters.AddWithValue("@用户名", 用户名);
				checkCmd.Parameters.AddWithValue("@社团名称", 社团名称);
				var count = checkCmd.ExecuteScalar(); // 执行查询并获取结果
				// 如果存在记录，则删除
				if (count != null && Convert.ToInt32(count) > 0){
					string deleteQuery = "DELETE FROM `社团成员表` WHERE `真实姓名` = @用户名 AND `社团名称` = @社团名称";
					using (var deleteCmd = new MySqlCommand(deleteQuery, connection))
					{
						deleteCmd.Parameters.AddWithValue("@用户名", 用户名);
						deleteCmd.Parameters.AddWithValue("@社团名称", 社团名称);
						int affectedRows = deleteCmd.ExecuteNonQuery(); // 执行删除操作
					}
				}
			}
			connection.Close();
		}
	}
}