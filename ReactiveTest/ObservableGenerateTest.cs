using System.Reactive.Disposables;
using System.Reactive.Linq;

// ReSharper disable UnusedVariable

// ReSharper disable ConvertClosureToMethodGroup

namespace ReactiveTest;
/*
public interface IObservable<out T>
{
      //Notifies the provider that an observer is to receive notifications.
      IDisposable Subscribe(IObserver<T> observer);
}

public interface IObserver<in T>
{
    //Notifies the observer that the provider has finished sending push-based notifications.
    void OnCompleted();

    //Notifies the observer that the provider has experienced an error condition.
    void OnError(Exception error);

    //Provides the observer with new data.
    void OnNext(T value);
}
 */

public static class ObservableGenerateTest
{
    /// <summary>
    /// IObservable: 可观察的事物, 表示一个可以被订阅的数据源，它可以发出一系列的事件，比如数据项、错误或完成信号
    /// IObserver: 观察者, 则是一个订阅者，它可以接收并处理 Observable 发出的事件
    /// </summary>
    public static void Test()
    {
        Console.WriteLine("=============== 使用 Create 方法创建 Observable 手动创建 Observable 的方式 =====================");
        var observableCreate = Observable.Create<int>(observer =>
        {
            observer.OnNext(1); // 为观察者提供新的数据
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnError(new Exception("测试")); // 通知观察者提供进程遇到了错误情况, 也表示完成 // 后面completed不会执行
            observer.OnCompleted(); // 通知观察者提供进程已完成基于推送的通知的发送

            return Disposable.Empty;
        });

        // 将元素处理进程、异常处理进程和完成处理进程订阅到可观察串行。
        using var subscriptionCreate = observableCreate.Subscribe(
            value => Console.WriteLine(value), // 观察者会得到事件中的数据
            error => Console.WriteLine("Error: " + error), // 通知观察者事件流发生错误
            () => Console.WriteLine("Completed") // 通知观察者事件流已结束
        );

        /*
         * 1
         * 2
         * 3
         * Error: System.Exception: 测试
         */

        Console.WriteLine("=============== 使用 Create 方法创建 Observable 触发Dispose方法 =====================");
        var observableCreate1 = Observable.Create<int>(observer =>
        {
            observer.OnNext(1);
            observer.OnNext(2);
            observer.OnNext(3);
            observer.OnCompleted();
            return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed")); // 退出作用域执行Dispose()
        });
        {
            // 将元素处理进程、异常处理进程和完成处理进程订阅到可观察串行。
            using var subscriptionCreate1 = observableCreate1.Subscribe(
                value => Console.WriteLine(value), // 观察者会得到事件中的数据
                error => Console.WriteLine("Error: " + error), // 通知观察者事件流发生错误
                () => Console.WriteLine("Completed") // 通知观察者事件流已结束
            ); // 退出作用域执行Dispose()
            /*
             * 1
             * 2
             * 3
             * Completed
             * Observer has unsubscribed
             */
        }

        Console.WriteLine("=============== 使用 ToObservable 方法创建 Observable IEnumerable<T>转化为IObservable<T> =====================");
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var observableToObservable = numbers.ToObservable();

        using var subscriptionToObservable = observableToObservable.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );
        /*
         * 1
         * 2
         * 3
         * 4
         * 5
         * Completed
         */

        Console.WriteLine("=============== 使用 Return 方法创建 Observable 创建一个只发出单个值 =====================");
        // 使用 Observable.Return 可以方便地创建一个只发出单个值的 Observable，适用于需要立即获得一个值并完成的场景
        var observableReturn = Observable.Return("Hello, world!");

        using var subscriptionReturn = observableReturn.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );

        /*
         * Hello, world!
         * Completed
         */

        Console.WriteLine("=============== 使用 Range 方法创建 Observable 指定范围内连续整数值序列 =====================");
        // 使用 Observable.Return 可以方便地创建一个只发出单个值的 Observable，适用于需要立即获得一个值并完成的场景
        var observableRange = Observable.Range(1, 5);

        using var subscriptionRange = observableRange.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );

        /*
         * 1
         * 2
         * 3
         * 4
         * 5
         * Completed
         */

        Console.WriteLine("=============== 使用 Generate 方法创建 Observable 根据指定的初始状态和条件生成一个序列 =====================");
        // 使用 Observable.Return 可以方便地创建一个只发出单个值的 Observable，适用于需要立即获得一个值并完成的场景
        var observableGenerate = Observable.Generate(
            0, // 初始状态
            i => i < 5, // 条件，当为 false 时停止生成序列
            i => i + 1, // 迭代函数，生成下一个值
            i => i * i // 结果选择函数，指定生成的值
        );

        using var subscriptionGenerate = observableGenerate.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );

        /*
         * 0
         * 1
         * 4
         * 9
         * 16
         * Completed
         */

        Console.WriteLine("=============== Defer 延迟创建(传入工厂产生Observable) 延迟创建（当有观察者订阅时才创建） =====================");
        // 比如要连接数据库进行查询，如果没有观察者，那么数据库连接会一直被占用，这样会造成资源浪费。使用Deffer可以解决这个问题。
        // 使用 Observable.Defer 可以延迟创建 Observable，这在需要根据每个订阅动态生成值的场景中非常有用。例如，当你需要每次订阅时生成不同的初始值，或者需要在订阅时执行一些特定的逻辑来生成 Observable，都可以使用 Defer 方法。
        /*
         Observable.Defer 方法的主要使用场景之一是在需要延迟创建 Observable 的情况下。以下是几个示例，展示了 Defer 方法的不同应用场景：

        动态生成初始值：使用 Defer 方法可以在每次订阅时动态生成初始值。例如，生成一个每次订阅时发出当前时间的 Observable：

        csharp
        Copy code
        var observable = Observable.Defer(() =>
        {
            var currentTime = DateTime.Now;
            return Observable.Return(currentTime);
        });
        每次订阅时，Defer 的回调函数会被执行，生成当前时间并通过 Observable.Return 发出。

        延迟执行耗时操作：使用 Defer 方法可以在订阅时延迟执行耗时的操作。例如，使用 Defer 来封装网络请求的 Observable：

        csharp
        Copy code
        var observable = Observable.Defer(async () =>
        {
            var result = await MakeNetworkRequestAsync();
            return Observable.Return(result);
        });
        Defer 的回调函数可以包含异步操作，如网络请求，在订阅时执行这些操作并返回相应的 Observable。

        条件触发的 Observable：使用 Defer 方法可以根据条件动态创建 Observable。例如，根据某个开关状态来决定是否创建 Observable：

        csharp
        Copy code
        var isObservableEnabled = true;

        var observable = Observable.Defer(() =>
        {
            if (isObservableEnabled)
            {
                return Observable.Interval(TimeSpan.FromSeconds(1));
            }
            else
            {
                return Observable.Empty<long>();
            }
        });
        Defer 的回调函数根据 isObservableEnabled 变量的值，动态选择要创建的 Observable，可以根据需要决定是否启用 Observable。

        总之，Observable.Defer 方法可以在订阅时动态创建 Observable，并且灵活地适应不同的场景需求。它允许你在每次订阅时生成不同的初始值、延迟执行耗时操作或根据条件创建 Observable。
         */
        var observableDefer = Observable.Defer(() =>
        {
            var random = new Random();
            var value = random.Next(1, 100);
            return Observable.Return(value);
        });

        using var subscriptionDefer = observableDefer.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );
        /*
         * 66
         * Completed
         */

        Console.WriteLine("=============== Never 空的永远不会结束的可观察序列 =====================");
        // 1. 表示等待某些条件发生的无限等待状态。
        // 2. 在某些特殊情况下，需要创建一个 Observable，但不会发出任何值。
        var observableNever = Observable.Never<int>();

        using var subscriptionNever = observableNever.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );
        /*
         *
         */

        Console.WriteLine("=============== Throw 立即抛出指定的异常的可观察序列 =====================");
        /*
         * 1. 模拟错误条件：你可以使用 Observable.Throw 来模拟某些操作的错误条件，以测试错误处理逻辑。
         * 2. 引发异常：在某些情况下，当特定条件满足时，你可以使用 Observable.Throw 引发异常，以通知订阅者出现了不可恢复的错误。
         */
        var observableThrow = Observable.Throw<int>(new Exception("Something went wrong"));

        using var subscriptionThrow = observableThrow.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error.Message),
            () => Console.WriteLine("Completed")
        );
        /*
         * Error: Something went wrong
         */

        Console.WriteLine("=============== Empty 表示一个立即完成且不发出任何值的 表示一个空的事件流，不包含任何数据项 =====================");
        /*
         * 1. 表示一个空的事件流：当你需要创建一个不包含任何数据项的事件流，但需要表示完成状态时，可以使用 Observable.Empty。
         * 2. 结合其他操作符使用：Observable.Empty 可以与其他操作符组合使用，用于构建更复杂的事件流处理逻辑。例如，结合 Concat 操作符将多个 Observable 连接在一起，其中某些 Observable 可能为空。
         */
        var observableEmpty = Observable.Empty<int>();

        using var subscriptionEmpty = observableEmpty.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );

        Console.WriteLine("=============== Observable.Interval(TimeSpan.FromSeconds(1)).Take(5) 定时发出递增的长整型值 =====================");
        var observableTimer = Observable.Interval(TimeSpan.FromSeconds(1))
            .Take(5);

        using var subscriptionTimer = observableTimer.Subscribe(
            value => Console.WriteLine(value),
            error => Console.WriteLine("Error: " + error),
            () => Console.WriteLine("Completed")
        );
        /*
         * 0
         * 1
         * 2
         * 3
         * 4
         * Completed
         */
        Console.ReadKey();
    }
}