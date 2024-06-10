using System;

namespace Core.Shared.ReactiveExtensions
{
  public interface IReadOnlyReactiveTrigger
  {
    IDisposable Subscribe(Action action);
    IDisposable SubscribeOnce(Action action);
  }
  
  public interface IReadOnlyReactiveTrigger<T>
  {
    IDisposable Subscribe(Action<T> action);
    IDisposable SubscribeOnce(Action<T> action);
  }
  
  public interface IReadOnlyReactiveTrigger<T1, T2>
  {
    IDisposable Subscribe(Action<T1, T2> action);
    IDisposable SubscribeOnce(Action<T1, T2> action);
  }
}