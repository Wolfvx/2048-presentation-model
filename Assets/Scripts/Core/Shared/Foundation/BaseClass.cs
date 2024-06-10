namespace Core.Shared.Foundation
{
	public class BaseClass
	{
		private BDebug _logger;
		protected BDebug Log
			=> _logger ??= new BDebug(GetType().Name);
	}
}