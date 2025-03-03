using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 操作记录查看 : Node2D{
	[Export] public Button 返回;
	[Export] public GridContainer 记录列表;
	public override void _Ready(){
		返回.Pressed +=()=> _返回();
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			string query = "SELECT `时间`, `事件简述`, `操作者`, `被操作对象真实姓名` FROM `操作记录表`";
			using (var cmd = new MySqlCommand(query, connection)){
				using (var reader = cmd.ExecuteReader()){
					while (reader.Read()){
						// 获取日期并格式化为YYYY-MM-DD
						DateTime time = Convert.ToDateTime(reader["时间"]);
						Panel panel = (Panel)GD.Load<PackedScene>("res://界面/操作记录查看/单个记录.tscn").Instantiate();
						GridContainer 列表 = panel.GetChild(0) as GridContainer;
						Label 时间 = (Label)列表.GetChild(0);
						时间.Text = time.ToString("yyyy-MM-dd");
						Label 时间简述 = (Label)列表.GetChild(1);
						时间简述.Text = (string)reader["事件简述"];
						Label 操作者 = (Label)列表.GetChild(2);
						操作者.Text	= (string)reader["操作者"];
						Label 被操作对象真实姓名 = (Label)列表.GetChild(3);
						被操作对象真实姓名.Text = (string)reader["被操作对象真实姓名"];
						记录列表.AddChild(panel);
					}
				}
			}
			connection.Close();
		}
	}

	private void _返回(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

}
