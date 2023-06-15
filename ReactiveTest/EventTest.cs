using System.Reactive.Linq;

// ReSharper disable UnusedParameter.Local

namespace ReactiveTest;

public static class EventTest
{
    public class Button
    {
        public event EventHandler? Click;

        public void SimulateClick()
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }

    public class CustomEventArgs : EventArgs
    {
        public string? Message { get; set; }
    }

    public class MyClass
    {
        public event EventHandler<CustomEventArgs>? CustomEvent;

        public void TriggerEvent(string message)
        {
            CustomEvent?.Invoke(this, new CustomEventArgs { Message = message });
        }
    }

    public delegate void BarHandler(int x, string y);

    public class Foo
    {
        private BarHandler? _delegateChain;

        public event BarHandler BarEvent
        {
            add
            {
                _delegateChain += value;
                Console.WriteLine("Event handler added");
            }
            remove
            {
                _delegateChain -= value;
                Console.WriteLine("Event handler removed");
            }
        }

        public void RaiseBar(int x, string y)
        {
            _delegateChain?.Invoke(x, y);
        }
    }

    public static void Test()
    {
        Console.WriteLine("== 使用 Observable.FromEventPattern 可以将任何 .NET 事件转换为 Observable ==");
        var button = new Button();
        // 将 Click 事件转换为 Observable
        var observable = Observable.FromEventPattern<EventHandler, EventArgs>(
            handler => button.Click += handler, // 提供了一个订阅事件的委托
            handler => button.Click -= handler // 取消订阅事件的委托
        );

        // 我们订阅这个 Observable，并指定了处理函数。当按钮被点击时，Observable 会发出一个 EventPattern<EventArgs> 对象
        using var subscription = observable.Subscribe(
            eventPattern => Console.WriteLine($"Button clicked {eventPattern.Sender} {eventPattern.EventArgs}"),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );

        button.SimulateClick(); // 模拟按钮点击事件
        /*
         * Button clicked ReactiveTest.EventTest+Button System.EventArgs
         */

        Console.WriteLine("== 使用 Observable.FromEventPattern eventName to Observable ==");
        var watch = new FileSystemWatcher
        {
            Path = "C:\\TEST",
            IncludeSubdirectories = true,
            Filter = "*.jpg",
            NotifyFilter = NotifyFilters.FileName |
                           NotifyFilters.LastWrite |
                           NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };
        using var watchSubscribe = Observable.FromEventPattern<FileSystemEventArgs>(watch, nameof(watch.Created))
            // where条件过滤, 类似于linq
            .Where(e => Path.GetExtension(e.EventArgs.FullPath).ToLower() == ".jpg")
            .Subscribe(e => { Console.WriteLine(e.EventArgs.FullPath); });

        /*
         * C:\TEST\TEST.jpg // 当有文件创建时，会触发事件触发订阅, 打印文件路径
         */

        Console.WriteLine("== 使用 Observable.FromEvent 可以将任何 .NET 事件转换为 Observable ==");

        var myClass = new MyClass();
        var observableFromEvent = Observable.FromEvent<EventHandler<CustomEventArgs>, CustomEventArgs>(
            handler =>
            {
                // ReSharper disable once ConvertToLocalFunction
                EventHandler<CustomEventArgs> eventHandler = (sender, e) => handler(e);
                return eventHandler;
            }, // FromEvent 运算符的存在是为了处理任何事件委托类型 简便写法：handler => (sender, e) => handler(e);
            h => myClass.CustomEvent += h,
            h => myClass.CustomEvent -= h);

        using var subscriptionFromEvent = observableFromEvent.Subscribe(args => Console.WriteLine(args.Message),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed"));

        myClass.TriggerEvent("Hello, World!");

        Console.WriteLine("== 一般事件使用 Observable.FromEvent ==");

        var foo = new Foo();

        var observableBar = Observable.FromEvent<BarHandler, (int x, string y)>(
            onNextHandler => (x, y) => onNextHandler((x, y)),
            h => foo.BarEvent += h,
            h => foo.BarEvent -= h);

        var xs = observableBar.Subscribe(x => Console.WriteLine("xs: " + x));
        foo.RaiseBar(1, "First");
        var ys = observableBar.Subscribe(x => Console.WriteLine("ys: " + x));
        foo.RaiseBar(1, "Second");
        xs.Dispose();
        foo.RaiseBar(1, "Third");
        ys.Dispose();

        /*
         * Event handler added // xs 订阅 触发 +=
         * xs: (1, First)
         * xs: (1, Second) // 订阅未释放， 所以xs可以接受事件
         * ys: (1, Second) // 订阅
         * ys: (1, Third) // xs释放，所以不触发
         * Event handler removed // 释放触发 -=
         */

        Console.ReadKey();
    }
}