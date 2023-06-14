using System.Reactive.Disposables;
using System.Reactive.Linq;

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

        var subscriptionToObservable = observableToObservable.Subscribe(
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

        var subscriptionGenerate = observableGenerate.Subscribe(
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