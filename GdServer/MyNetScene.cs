using Net.Server;
using Net.Share;

namespace GdServer;

public class MyNetScene:NetScene<MyNetPlayer>
{
    public override void OnExit(MyNetPlayer client)
    {
        Console.WriteLine(client.UserID);
        AddOperation(new Operation(MyCommand.Destroy, client.UserID));
    }

    public override void OnOperationSync(MyNetPlayer client, OperationList list)
    {
        for (int i = 0; i < list.operations.Length; i++)
        {
            var opt = list.operations[i];
            switch (opt.cmd)
            {
                default:
                    AddOperation(opt);
                    break;
            }
        }
    }
}