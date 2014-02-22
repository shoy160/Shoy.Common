using System;
using System.IO;

namespace Shoy.MemCached
{
	public class NestedIOException:IOException 
	{
		Exception _innerException;

		public NestedIOException()
		{

		}

		/// <summary>
		/// Create a new <c>NestedIOException</c> instance.
		/// </summary>
		/// <param name="cause">The inner exception</param>
		public NestedIOException(Exception ex)
		{
			_innerException = ex;
		}

		public NestedIOException(string message, Exception ex) 
			:base(message)
		{
			_innerException = ex;
		}

		public override Exception GetBaseException()
		{
			return _innerException;
		}

	}
}