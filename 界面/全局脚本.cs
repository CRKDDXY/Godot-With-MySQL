using Godot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class 全局脚本 : Node{
	public static String level;		//全局权限等级
	public static String 用户名 = "admin" ;	//全局用户名
	public static String 真实姓名 ;	//真实姓名

	public static async void 获取真实姓名(){
		// 连接数据库
		using (var connection = new MySqlConnection("server=localhost;database=学生社团团员信息管理系统;user=root;password=1234")){
			connection.Open();
			// 准备查询语句
			string query = "SELECT `真实姓名` FROM `账号表` WHERE `用户名` = '"+用户名+"'";
			MySqlCommand cmd = new(query, connection);  // 指令 连接
			try{
				using (var reader = await cmd.ExecuteReaderAsync()){  // 异步执行，避免阻塞
					if(reader.Read()){
							真实姓名 = (string)reader["真实姓名"];
					}
					reader.Close();
				}
				connection.Close();
            }catch (MySqlException ex){
                GD.PrintErr("MySQL Error: " + ex.Message);
            }
		}
	}

	public static List<String> 上次使用的指令;	//用于展示后端使用的指令
}