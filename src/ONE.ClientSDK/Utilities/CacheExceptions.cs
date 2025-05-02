using System;

namespace ONE.ClientSDK.Utilities
{
	public static class CacheExceptions
	{
		
		public static Exception UnloadedException(string entityName, string entityId)
		{
			return new ArgumentException($"{entityName} ({entityId}) is either not loaded or not part of this operation");
		}
			
		public static Exception NotInCacheException(string operationId)
		{
			return new ArgumentException($"Operation ({operationId}) is not part of this cache");
		}

		public static Exception FailedToAddException(string operationId)
		{
			return new ArgumentException($"Failed to add operation ({operationId})");
		}

		public static Exception NotDeserializedCacheException()
		{
			return new ArgumentException("Serialized cache could not be deserialized");
		}

		public static Exception IdMustBeGuidException(string nameOfId)
		{
			return new ArgumentException($"{nameOfId} must be a guid");
		}
	}
}
