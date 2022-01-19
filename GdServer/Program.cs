using System;
using GdServer;

internal class Program
{
    private static void Main(string[] args)
    {
        //创建服务器对象
        var server = new MyTcpServer();

        server.Log += Console.WriteLine;  //打印服务器内部信息
        server.Run(8888);     //启动6666端口
        Console.ReadLine();
    }
}