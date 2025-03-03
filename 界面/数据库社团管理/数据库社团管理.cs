using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class 数据库社团管理 : Node2D{
	[Export] public GridContainer 社团信息列表;
	[Export] public Button 返回上个界面;
	[Export] public Button 确认删除;
	[Export] public Button 取消删除;
	[Export] public Label 删除提示;
 	public override async void _Ready(){
		删除提示.Visible = false;
		返回上个界面.Pressed +=()=> _返回上个界面();
		确认删除.Pressed +=()=> _确认删除社团();
		取消删除.Pressed +=()=> _取消删除社团();
		//先重置列表 应对数据库账号更新
        for(int i=0 ; i<社团信息列表.GetChildCount(); i++){
            社团信息列表.GetChild(i).QueueFree();
        }
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "SELECT * FROM 社团表;";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
                    while(reader.Read()){
                        // 创建单个信息面板
                        Panel panel = (Panel)GD.Load<PackedScene>("res://界面/数据库社团管理/单个社团信息.tscn").Instantiate();
                        GridContainer pg = panel.GetChild(0) as GridContainer;
                        // 信息赋值
                        Label L1 = (Label) pg.GetChild(0);
                        L1.Text = (string)reader["社团名称"];
                        Label L2 = (Label) pg.GetChild(1);
						DateTime time = (DateTime)reader["创立时间"];
                        L2.Text = time.ToString("yyyy-MM-dd"); // 格式化日日期
                        社团信息列表.AddChild(panel);
                    }
					reader.Close();
                }
                //信号连接
                for(int i=0; i<社团信息列表.GetChildCount(); i++){
                    TextureButton 单个信息点击区域 =  (TextureButton)社团信息列表.GetChild(i).GetChild(1);
                    Panel p = (Panel)社团信息列表.GetChild(i);
                    单个信息点击区域.Pressed +=()=> _选着单个社团对象(p);   //链接点击
                }
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
			connection.Close();
        }
	}

	public void _选着单个社团对象(Panel p){
		删除提示.Visible = true;
		选中的社团 = p;
		Label 社团名称 = (Label)p.GetChild(0).GetChild(0);
		删除提示.Text = "是否删除该社团？\n("+社团名称.Text+")";
	}

	public Panel 选中的社团;

	public async void _确认删除社团(){
		Label 社团名称 = (Label)选中的社团.GetChild(0).GetChild(0);
		 using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "DELETE FROM 社团表 WHERE 社团名称 = '"+社团名称.Text+"';";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
					reader.Close();
				}
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
			connection.Close();
        }
		选中的社团.QueueFree();
		删除提示.Visible = false;
	}

	public void _取消删除社团(){
		删除提示.Visible = false;
	}

	public void _返回上个界面(){
		GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
	}

}
