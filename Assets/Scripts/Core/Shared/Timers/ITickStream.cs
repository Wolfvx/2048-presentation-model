using System;

namespace Core.Shared.Timers
{
	public interface ITickStream
	{
		public enum StreamType
		{
			TICK_RATE,
			PHYSICS,
			UPDATE,
			LATE_UPDATE,
			UNSCALED_UPDATE
		}

		IDisposable Subscribe(StreamType stream, Action<float> callback);
		void Play(StreamType streamType);
		void Stop(StreamType streamType);
	}
}