/*
程序由三个类（GameMessage、Game、Player）完成，使用了观察者模式。
GameMessage类：消息类，Result属性表示需要传递的字符串格式的消息，
Game类：可观察对象，暴露TakeAndNotify方法给Player调用，开放三个参数:
        player：调用者
        row：从哪一行拿取
        count：拿取数量
    TakeAndNotify方法调用逻辑：
    1、通过Player的CanTake属性判断player本轮是否有拿取权，false状态，提醒player;
    2、如果player有权拿取，判断拿取行为是否合规，由于业务比较简单，该部分没有做抽象；
    3、合规的拿取行为,通过修改_items（池数据），更新数据状态。程序中池数据由Game的生命周期维护，生产环境可设计由外部Infrastructure层维护；
    4、池数据和player的CanTake状态更新后，检查池数据中的item剩余数量，如果已清零，则分别提醒各player的输赢状态，完成后，提醒所有player游戏结束。
Player类：观察者对象，通过JoinGame订阅Game。游戏完成后，通过Exit方法解除订阅，完成析构；
    暴露TakeItems方法，通过调用Game对象的TakeAndNotify方法发送拿取请求；
    暴露Name属性作为实例化对象的身份标识，此处未作排重处理，生产环境可设计唯一键属性（如，GUID id）用于排重；
    暴露CanTake属性，用于拿取权检查，拿取状态由Game类维护。
*/

using System.Numerics;

Game game = new Game();
Player p1 = new Player("张三");
Player p2 = new Player("李四");

p1.JoinGame(game);
p2.JoinGame(game);

p1.TakeItems(1 , 1);
p2.TakeItems(1 , 1);

p1.TakeItems(0 , 1);
p2.TakeItems(2 , 5);

p1.TakeItems(0 , 1);
p2.TakeItems(2 , 4);

p2.TakeItems(2 , 4);
p1.TakeItems(0 , 1);

p2.TakeItems(0 , 1);
p2.TakeItems(1 , 4);


p1.Exit();
p2.Exit();

Console.ReadLine();
