using Godot;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

public partial class 数据库账号管理 : Node2D{
    [Export] public GridContainer 账号信息列表;
    [Export] public Label 注销操作;
    [Export] public Label 权限设置;
    [Export] public Button 返回上个界面;
    public override async void _Ready(){
        返回上个界面.Pressed +=()=> _返回上个界面();
        设置权限等级1按钮.Pressed +=()=> 设置权限等级1();
        设置权限等级2按钮.Pressed +=()=> 设置权限等级2();
        注销操作.Visible = false;
        权限设置.Visible = false;
        //先重置列表 应对数据库账号更新
        for(int i=0 ; i<账号信息列表.GetChildCount(); i++){
            账号信息列表.GetChild(i).QueueFree();
        }
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "SELECT * FROM 账号表 ORDER BY 权限等级 ASC;";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
                    while(reader.Read()){
                        // 创建单个信息面板
                        Panel panel = (Panel)GD.Load<PackedScene>("res://界面/数据库账号管理/单个账号信息.tscn").Instantiate();
                        GridContainer pg = panel.GetChild(0) as GridContainer;
                        // 信息赋值
                        Label L1 = (Label) pg.GetChild(0);
                        L1.Text = (string)reader["权限等级"];
                        Label L2 = (Label) pg.GetChild(1);
                        L2.Text = (string)reader["用户名"];
                        Label L3 = (Label) pg.GetChild(2);
                        L3.Text = (string)reader["密码"];
                        Label L4 = (Label) pg.GetChild(3);
                        L4.Text = (string)reader["真实姓名"];
                        Label L5 = (Label) pg.GetChild(4);
                        L5.Text = (string)reader["年级"];
                        账号信息列表.AddChild(panel);
                    }
					reader.Close();
                }
                //信号连接
                for(int i=0; i<账号信息列表.GetChildCount(); i++){
                    TextureButton 单个信息点击区域 =  (TextureButton)账号信息列表.GetChild(i).GetChild(1);
                    Panel p = (Panel)账号信息列表.GetChild(i);
                    单个信息点击区域.Pressed +=()=> _tb_pressed(p);   //链接点击
                }
                //确认与取消按钮点击连接
                Button 确认删除 = (Button)注销操作.GetChild(0);
                确认删除.Pressed +=()=> 确认账号删除();
                Button 取消删除 = (Button)注销操作.GetChild(1);
                取消删除.Pressed +=()=> 取消账号删除操作();
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
			connection.Close();
        }
    }
    public void _tb_pressed(Panel p){
        Label 用户名 = (Label)p.GetChild(0).GetChild(1);
        if(用户名.Text=="admin") return;    //管理员不可修改
        注销操作.Visible = true;    //弹出处理
        权限设置.Visible = true;    //弹出处理
        选中的信息对象 = p;
        Label 真实姓名 = (Label)p.GetChild(0).GetChild(3);
        注销操作.Text = "是否删除该账号：\n("+真实姓名.Text+")";
    }

    public void 取消账号删除操作(){
        注销操作.Visible = false;
        权限设置.Visible = false;
    }

    public Panel 选中的信息对象;
    public async void 确认账号删除(){
        Label 真实姓名 = (Label)选中的信息对象.GetChild(0).GetChild(3);
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
		    connection.Open();
            string query = "DELETE FROM `账号表` WHERE `真实姓名` = '" + 真实姓名.Text + "';";
            MySqlCommand cmd = new(query, connection);  // 指令 连接
            try{
                using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行 避免阻塞
                    if(reader.Read()){
                        GD.Print(reader["真实姓名"]);
                    }
					reader.Close();
                }
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
			connection.Close();
        }
        选中的信息对象.QueueFree();
        注销操作.Visible = false;
        权限设置.Visible = false;
    }
    public void _返回上个界面(){
        GetTree().ChangeSceneToFile("res://界面/功能选择界面/功能选择界面.tscn");
    }

    [Export] public Button 设置权限等级1按钮;
    [Export] public Button 设置权限等级2按钮;

    public void 设置权限等级1(){
        Label 真实姓名 = (Label)选中的信息对象.GetChild(0).GetChild(3);
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
            connection.Open();
            string query = "UPDATE `账号表` SET `权限等级` = '1' WHERE `真实姓名` = '"+真实姓名.Text+"';";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery(); // 异步执行非查询命令
            connection.Close();
        }
        Label 权限显示修改 = (Label)选中的信息对象.GetChild(0).GetChild(0);
        权限显示修改.Text = "1";
    }
    public void 设置权限等级2(){
        Label 真实姓名 = (Label)选中的信息对象.GetChild(0).GetChild(3);
        using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
            connection.Open();
            string query = "UPDATE `账号表` SET `权限等级` = '2' WHERE `真实姓名` = '"+真实姓名.Text+"';";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery(); // 异步执行非查询命令
            connection.Close();
        }
        Label 权限显示修改 = (Label)选中的信息对象.GetChild(0).GetChild(0);
        权限显示修改.Text = "2";
    }
}