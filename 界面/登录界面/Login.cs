using Godot;
using MySql.Data.MySqlClient;
using System;

public partial class Login : Node2D{

	[Export] public Button login;
	[Export] public Button register;
	[Export] public TextEdit username;
	[Export] public TextEdit password;
	[Export] public Label tishi;

	private async void _login(){
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string 用户名 = username.Text;   // 用户名
            string 秘密 = password.Text;   // 密码
			// 账号登录查询：首先查询数据库内有没有用户输入的用户名的数据
            string query = "SELECT * FROM 账号表 WHERE 用户名 = @用户名;";
            MySqlCommand cmd = new(query, connection);
			cmd.Parameters.AddWithValue("@用户名", 用户名);
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
                    if (reader.Read()){
                        // 登录成功, 页面跳转 -> 根据得到的权限等级添加功能组件
                        if ((string)reader["密码"] == 秘密){
							全局脚本.level = (string)reader["权限等级"];
							全局脚本.用户名 = username.Text;
							全局脚本.获取真实姓名();	//全局变量中真实姓名获取
							//全局脚本.上次使用的指令.Add(query);
							GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
                        }
                    }
					reader.Close();
                }
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
            tishi.Visible = true;   //登录失败&切换提示语句的可见性
			connection.Close();
        }
	}

	private void _register(){
		//注册按钮
		GetTree().ChangeSceneToFile("res://界面/注册界面/注册界面.tscn");
	}

	public override void _Ready(){
		GetWindow().SetFlag(Godot.Window.Flags.ResizeDisabled,true);    //设置窗口类型（不可缩放）
		tishi.Visible = false;
		register.Pressed +=()=> _register();
		login.Pressed +=()=> _login();
	}
}