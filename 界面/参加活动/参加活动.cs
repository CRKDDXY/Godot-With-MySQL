using Godot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public partial class 参加活动 : Node2D{
	[Export] public Button 返回;
	[Export] public GridContainer 所属社团;
	[Export] public Panel 未报名活动列表;
	[Export] public Button 确认;
	[Export] public Button 取消;
	public override async void _Ready(){
		返回.Pressed +=()=> _返回();
		确认.Pressed +=()=> _确认();
		取消.Pressed +=()=> _取消();
		未报名活动列表.Visible = false;
		活动提示.Visible = false;
		List<string> 全部社团名称 = new List<string>{};
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "SELECT * FROM `社团名称视图`;";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
                while(reader.Read()){
					全部社团名称.Add((string)reader["社团名称"]);	//获取全部社团名称 -> 暂时不打印
                }
				reader.Close();
            }
			query = "SELECT 社团名称 FROM 社团成员表 WHERE 真实姓名 = '"+全局脚本.真实姓名+"';";
			cmd = new(query, connection);  // 指令 连接
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
				reader.Close();
            }
			connection.Close();
		}
	}

	private void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}
	
	[Export] public GridContainer 活动列表;
	
	private async void _单个社团选择(Panel p){
		for(int i=0; i<活动列表.GetChildCount(); i++){
			活动列表.GetChild(i).QueueFree();
		}
		Label 社团名称 = (Label)p.GetChild(0);
		Label 社团活动提示标头文本 = (Label)未报名活动列表.GetChild(0);
		社团活动提示标头文本.Text = $"({社团名称.Text})现有活动\n(以下显示当前未报名活动)";
		未报名活动列表.Visible = true;
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			await connection.OpenAsync(); // 异步打开连接
			// 查询用户所属社团的所有活动
			string query = "SELECT `活动编号`, `活动简述` FROM `活动表` WHERE `社团名称` = @clubname;";
			MySqlCommand cmd = new(query, connection); // 指令连接
			cmd.Parameters.AddWithValue("@clubname", 社团名称.Text);
			using (var reader = await cmd.ExecuteReaderAsync()){ // 异步执行，避免阻塞
				while (await reader.ReadAsync()){
					// 这里获取选中到的社团的所有活动
					Panel panel = (Panel)GD.Load<PackedScene>("res://界面/参加活动/单个活动信息.tscn").Instantiate();
					Label 活动编号 = (Label)panel.GetChild(0);
					活动编号.Text = reader["活动编号"].ToString();
					Label 活动简述 = (Label)panel.GetChild(1);
					活动简述.Text = reader["活动简述"].ToString();
					TextureButton 点击区域 = (TextureButton)panel.GetChild(2);
					点击区域.Pressed +=()=> 活动点击(panel);
					活动列表.AddChild(panel);
				}
				reader.Close();
			}
			connection.Close();
		}
	}

	[Export] public Label 活动提示;	//已经报过了，或者说提示需不需要报
	public Panel 被选中的活动;

	private async void 活动点击(Panel p){
		活动提示.Visible = true;
		被选中的活动 = p;
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			await connection.OpenAsync(); // 异步打开连接
			// 检查活动报名表中是否存在指定的活动编号和用户名
			string query = "SELECT COUNT(*) FROM `活动报名表` WHERE `活动编号` = @activityId AND `真实姓名` = @username";
			using (var cmd = new MySqlCommand(query, connection)){
				Label 编号 = 被选中的活动.GetChild(0) as Label;
				被选中的活动编号 = 编号.Text;
				cmd.Parameters.AddWithValue("@activityId", 编号.Text);
				cmd.Parameters.AddWithValue("@username", 全局脚本.真实姓名);
				int count = Convert.ToInt32(await cmd.ExecuteScalarAsync()); // 异步执行查询
				// 如果计数大于0，则表示记录存在
				if(count > 0){
					活动提示.Text = "你已经报名过该活动了";
					确认.Visible = false;
				}else{
					活动提示.Text = "是否报名该活动？\n活动编号:"+编号.Text;
					确认.Visible = true;
				}
			}
			connection.Close();
		}
	}

	public string 被选中的活动编号;

	private async void _确认(){
		string insertQuery = "INSERT INTO `活动报名表` (`活动编号`, `真实姓名`) VALUES (@activityId, @username)";
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			using (var cmd = new MySqlCommand(insertQuery, connection)){
				cmd.Parameters.AddWithValue("@activityId", 被选中的活动编号);
				cmd.Parameters.AddWithValue("@username", 全局脚本.真实姓名);
				await cmd.ExecuteNonQueryAsync(); // 异步执行插入操作
			}
			connection.Close();
		}
		活动提示.Text = "报名成功";
		确认.Visible = false;
	}

	private void _取消(){
		活动提示.Visible = false;
	}

}