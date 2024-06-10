using System;
using System.Collections.Generic;
using Core.Shared.Foundation;
using Core.Shared.ReactiveExtensions;
using UniRx;
using UnityEngine;

namespace Core.Shared.Timers
{
	public class TickStreamPm : BaseDisposable, ITickStream
	{
		public struct Ctx
		{
			public float tps;
		}

		private class Stream : IDisposable
		{

			public bool isPaused;
			public delegate float GetTime();
			
			public ReactiveTrigger<float> trigger;
			private IDisposable _subscription;
			private readonly GetTime _getTime;

			public Stream(GetTime getTime)
			{
				_getTime = getTime;
				isPaused = false;
				trigger = new ReactiveTrigger<float>();
			}

			public void Init(IObservable<long> observable)
			{
				_subscription = observable.Subscribe(_ => update());
			}

			public void Dispose()
			{
				trigger?.Dispose();
				_subscription?.Dispose();
			}

			private void update()
			{
				if (isPaused) return;
				trigger.Notify(_getTime());
			}
		}

		private readonly Ctx _ctx;
		private Dictionary<ITickStream.StreamType, Stream> _streams;

		private const int NUM_OF_STREAMS = 5;
		
		public TickStreamPm(Ctx ctx)
		{
			_ctx = ctx;
			_streams = new Dictionary<ITickStream.StreamType, Stream>(NUM_OF_STREAMS);
		}

		protected override void OnDispose()
		{
			foreach (KeyValuePair<ITickStream.StreamType, Stream> streamPair in _streams)
			{
				streamPair.Value.Dispose();
			}
			_streams.Clear();
			base.OnDispose();
		}

		IDisposable ITickStream.Subscribe(ITickStream.StreamType stream, Action<float> callback)
		{
			return getStream(stream)?.trigger.Subscribe(callback);
		}

		void ITickStream.Play(ITickStream.StreamType stream)
		{
			_streams[stream].isPaused = false;
		}

		void ITickStream.Stop(ITickStream.StreamType stream)
		{
			_streams[stream].isPaused = true;
		}

		private Stream getStream(ITickStream.StreamType streamType)
		{
			if (_streams.TryGetValue(streamType, out var stream))
				return stream;
			stream = createStream(streamType);
			return stream;
		}

		private Stream createStream(ITickStream.StreamType streamType)
		{
			Stream stream;
			switch (streamType)
			{
				case ITickStream.StreamType.TICK_RATE:
					float tps = _ctx.tps;
					stream = new Stream(() => 1 / tps);
					stream.Init(Observable.Interval(TimeSpan.FromSeconds(1f / tps), Scheduler.MainThreadIgnoreTimeScale));
					break;
				case ITickStream.StreamType.PHYSICS:
					stream = new Stream(() => Time.deltaTime);
					stream.Init(Observable.EveryFixedUpdate());
					break;
				case ITickStream.StreamType.UPDATE:
					stream = new Stream(() => Time.time);
					stream.Init(Observable.EveryUpdate());
					break;
				case ITickStream.StreamType.LATE_UPDATE:
					stream = new Stream(() => Time.time);
					stream.Init(Observable.EveryLateUpdate());
					break;
				case ITickStream.StreamType.UNSCALED_UPDATE:
					stream = new Stream(() => Time.unscaledDeltaTime);
					stream.Init(Observable.EveryUpdate());
					break;
				default: throw new ArgumentOutOfRangeException(nameof(streamType), streamType, null);
			}
			return stream;
		}
	}
}