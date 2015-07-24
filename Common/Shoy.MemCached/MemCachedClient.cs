using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace Shoy.MemCached
{
	public class MemcachedClient 
	{
		// return codes
		private const string VALUE = "VALUE"; // start of value line from server
        private const string STATS = "STAT"; // start of stats line from server
        private const string DELETED = "DELETED"; // successful deletion
        private const string NOTFOUND = "NOT_FOUND"; // record not found for delete or incr/decr
        private const string STORED = "STORED"; // successful store of data
        private const string NOTSTORED = "NOT_STORED"; // data not stored
        private const string OK = "OK"; // success
        private const string END = "END"; // end of data from server
        private const string ERROR = "ERROR"; // invalid command name from client
        private const string CLIENT_ERROR = "CLIENT_ERROR"; // client error in input line - invalid protocol
        private const string SERVER_ERROR = "SERVER_ERROR";	// server error

		// default compression threshold
		private const int COMPRESS_THRESH = 30720;
    
		// values for cache flags 
		//
		// using 8 (1 << 3) so other clients don't try to unpickle/unstore/whatever
		// things that are serialized... I don't think they'd like it. :)
		private const int F_COMPRESSED = 2;
		private const int F_SERIALIZED = 8;
	
		// flags
		private bool _primitiveAsString;
		private bool _compressEnable;
		private long _compressThreshold;
		private string _defaultEncoding;

		// which pool to use
		private string _poolName;

	    public string Prefix { get; set; }

	    /// <summary>
		/// Creates a new instance of MemcachedClient.
		/// </summary>
		private MemcachedClient(string serverName) 
		{
            _primitiveAsString = false;
            _compressEnable = true;
            _compressThreshold = COMPRESS_THRESH;
            _defaultEncoding = "UTF-8";
		    _poolName = serverName;
		}

        public static MemcachedClient GetInstance(string serverName)
        {
            return new MemcachedClient(serverName);
        }

        public static MemcachedClient GetInstance()
        {
            return new MemcachedClient(string.Empty);
        }

		/// <summary>
		/// Sets the pool that this instance of the client will use.
		/// The pool must already be initialized or none of this will work.
		/// </summary>
        public string PoolName
        {
            get { return _poolName; }
            set { _poolName = value; }
        }

		/// <summary>
		/// Enables storing primitive types as their string values. 
		/// </summary>
		public bool PrimitiveAsString
		{
			get { return _primitiveAsString; }
			set { _primitiveAsString = value; }
		}

		/// <summary>
		/// Sets default string encoding when storing primitives as strings. 
		/// Default is UTF-8.
		/// </summary>
		public string DefaultEncoding
		{
			get { return _defaultEncoding; }
			set { _defaultEncoding = value; }
		}

		public bool EnableCompression
		{
			get { return _compressEnable; }
			set { _compressEnable = value; }
		}

		/// <summary>
		/// Sets the required length for data to be considered for compression.
		/// 
		/// If the length of the data to be stored is not equal or larger than this value, it will
		/// not be compressed.
		/// 
		/// This defaults to 15 KB.
		/// </summary>
		/// <value>required length of data to consider compression</value>
		public long CompressionThreshold
		{
			get { return _compressThreshold; }
			set { _compressThreshold = value; }
		}

		/// <summary>
		/// Checks to see if key exists in cache. 
		/// </summary>
		/// <param name="key">the key to look for</param>
		/// <returns><c>true</c> if key found in cache, <c>false</c> if not (or if cache is down)</returns>
		public bool KeyExists(string key) 
		{
			return(Get(key, null, true) != null);
		}
	
		/// <summary>
		/// Deletes an object from cache given cache key.
		/// </summary>
		/// <param name="key">the key to be removed</param>
		/// <returns><c>true</c>, if the data was deleted successfully</returns>
		public bool Delete(string key) 
		{
			return Delete(key, null, DateTime.MaxValue);
		}

		/// <summary>
		/// Deletes an object from cache given cache key and expiration date. 
		/// </summary>
		/// <param name="key">the key to be removed</param>
		/// <param name="expiry">when to expire the record.</param>
		/// <returns><c>true</c>, if the data was deleted successfully</returns>
		public bool Delete(string key, DateTime expiry) 
		{
			return Delete(key, null, expiry);
		}

		/// <summary>
		/// Deletes an object from cache given cache key, a delete time, and an optional hashcode.
		/// 
		/// The item is immediately made non retrievable.<br/>
		/// Keep in mind: 
		/// <see cref="add">add(string, object)</see> and <see cref="replace">replace(string, object)</see>
		///	will fail when used with the same key will fail, until the server reaches the
		///	specified time. However, <see cref="set">set(string, object)</see> will succeed
		/// and the new value will not be deleted.
		/// </summary>
		/// <param name="key">the key to be removed</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <param name="expiry">when to expire the record.</param>
		/// <returns><c>true</c>, if the data was deleted successfully</returns>
		public bool Delete(string key, object hashCode, DateTime expiry) 
		{
			if(key == null) 
			{
				return false;
			}
		    key = Prefix + key;

			// get SockIO obj from hash or from key
			SockIO sock = SockIOPool.GetInstance(_poolName).GetSock(key, hashCode);

			// return false if unable to get SockIO obj
			if(sock == null)
				return false;

			// build command
			StringBuilder command = new StringBuilder("delete ").Append(key);
			if(expiry != DateTime.MaxValue)
				command.Append(" " + GetExpirationTime(expiry) / 1000);

			command.Append("\r\n");

            try
            {
                sock.Write(Encoding.UTF8.GetBytes(command.ToString()));
                sock.Flush();

                // if we get appropriate response back, then we return true
                string line = sock.ReadLine();
                if (DELETED == line)
                {
                    // return sock to pool and bail here
                    sock.Close();
                    sock = null;
                    return true;
                }
            }
            catch
            {
                try
                {
                    if (sock != null)
                        sock.TrueClose();
                }
                finally
                {
                    sock = null;
                }
            }
		    if(sock != null)
				sock.Close();

			return false;
		}

	    /// <summary>
	    /// Converts a .NET date time to a UNIX timestamp
	    /// </summary>
	    /// <param name="expiration"></param>
	    /// <returns></returns>
	    private static int GetExpirationTime(DateTime expiration)
		{
			if(expiration <= DateTime.Now)
				return 0;

			TimeSpan thirtyDays = new TimeSpan(29, 23, 59, 59);
			if(expiration.Subtract(DateTime.Now) > thirtyDays)
				return (int)thirtyDays.TotalSeconds;
			
			return (int)expiration.Subtract(DateTime.Now).TotalSeconds;
		}
    
		/// <summary>
		/// Stores data on the server; only the key and the value are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Set(string key, object value) 
		{
			return Set("set", key, value, DateTime.MaxValue, null, _primitiveAsString);
		}

		/// <summary>
		/// Stores data on the server; only the key and the value are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Set(string key, object value, int hashCode) 
		{
			return Set("set", key, value, DateTime.MaxValue, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Stores data on the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Set(string key, object value, DateTime expiry) 
		{
			return Set("set", key, value, expiry, null, _primitiveAsString);
		}

		/// <summary>
		/// Stores data on the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Set(string key, object value, DateTime expiry, int hashCode) 
		{
			return Set("set", key, value, expiry, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Adds data to the server; only the key and the value are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Add(string key, object value) 
		{
			return Set("add", key, value, DateTime.MaxValue, null, _primitiveAsString);
		}

		/// <summary>
		/// Adds data to the server; the key, value, and an optional hashcode are passed in.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Add(string key, object value, int hashCode) 
		{
			return Set("add", key, value, DateTime.MaxValue, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Adds data to the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Add(string key, object value, DateTime expiry) 
		{
			return Set("add", key, value, expiry, null, _primitiveAsString);
		}

		/// <summary>
		/// Adds data to the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Add(string key, object value, DateTime expiry, int hashCode) 
		{
			return Set("add", key, value, expiry, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Updates data on the server; only the key and the value are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Replace(string key, object value) 
		{
			return Set("replace", key, value, DateTime.MaxValue, null, _primitiveAsString);
		}

		/// <summary>
		/// Updates data on the server; only the key and the value and an optional hash are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Replace(string key, object value, int hashCode) 
		{
			return Set("replace", key, value, DateTime.MaxValue, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Updates data on the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Replace(string key, object value, DateTime expiry) 
		{
			return Set("replace", key, value, expiry, null, _primitiveAsString);
		}

		/// <summary>
		/// Updates data on the server; the key, value, and an expiration time are specified.
		/// </summary>
		/// <param name="key">key to store data under</param>
		/// <param name="value">value to store</param>
		/// <param name="expiry">when to expire the record</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true, if the data was successfully stored</returns>
		public bool Replace(string key, object value, DateTime expiry, int hashCode) 
		{
			return Set("replace", key, value, expiry, hashCode, _primitiveAsString);
		}

		/// <summary>
		/// Stores data to cache.
		/// 
		/// If data does not already exist for this key on the server, or if the key is being
		/// deleted, the specified value will not be stored.
		/// The server will automatically delete the value when the expiration time has been reached.
		/// 
		/// If compression is enabled, and the data is longer than the compression threshold
		/// the data will be stored in compressed form.
		/// 
		/// As of the current release, all objects stored will use .NET serialization.
		/// </summary>
		/// <param name="cmdname">action to take (set, add, replace)</param>
		/// <param name="key">key to store cache under</param>
		/// <param name="obj">object to cache</param>
		/// <param name="expiry">expiration</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <param name="asString">store this object as a string?</param>
		/// <returns>true/false indicating success</returns>
		private bool Set(string cmdname, string key, object obj, DateTime expiry, object hashCode, bool asString) 
		{
			if(expiry < DateTime.Now)
				return true;

			if(cmdname == null || cmdname.Trim().Length == 0 || string.IsNullOrEmpty(key)) 
			{
				return false;
			}
		    key = Prefix + key;

			// get SockIO obj
			SockIO sock = SockIOPool.GetInstance(_poolName).GetSock(key, hashCode);
		
			if(sock == null)
				return false;
		
			if(expiry == DateTime.MaxValue)
				expiry = new DateTime(0);

			// store flags
			int flags = 0;
		
			// byte array to hold data
			byte[] val;
			int length = 0;

			// useful for sharing data between .NET and non-.NET
            // and also for storing ints for the increment method
			if(NativeHandler.IsHandled(obj)) 
			{
				if(asString) 
				{
                    if(obj != null)
                    {
                        try
                        {
                            val = Encoding.UTF8.GetBytes(obj.ToString());
							length = val.Length;
                        }
                        catch
                        {
                            sock.Close();
                            sock = null;
                            return false;
                        }
                    }
                    else
                    {
                        val = new byte[0];
						length = 0;
                    }
				}
				else 
				{

					try 
					{
						val = NativeHandler.Encode(obj);
						length = val.Length;
					}
					catch
					{
						sock.Close();
						sock = null;
						return false;
					}
				}
			}
			else 
			{
                if(obj != null)
                {
                    // always serialize for non-primitive types
					try
                    {
                        var memStream = new MemoryStream();
                        new BinaryFormatter().Serialize(memStream, obj);
                        val = memStream.GetBuffer();
						length = (int) memStream.Length;
                        flags |= F_SERIALIZED;
                    }
                    catch
                    {
                        // return socket to pool and bail
                        sock.Close();
                        sock = null;
                        return false;
                    }
                }
                else
                {
                    val = new byte[0];
					length = 0;
                }
			}
		
			// now try to compress if we want to
			// and if the length is over the threshold 
			if(_compressEnable && length > _compressThreshold)
			{
			    MemoryStream memoryStream=null;
			    GZipStream gos = null;
				try 
				{
					memoryStream = new MemoryStream();
				    gos = new GZipStream(memoryStream, CompressionMode.Compress);
					gos.Write(val, 0, length);
				    gos.Flush();
				
					// store it and set compression flag
					val = memoryStream.GetBuffer();
					length = (int)memoryStream.Length;
					flags |= F_COMPRESSED;
				}
                finally
				{
                    if (memoryStream != null)
                    {
                        memoryStream.Close();
                        memoryStream.Dispose();
                    }
                    if (gos != null)
                    {
                        gos.Close();
                        gos.Dispose();
                    }
				}
			}

			// now write the data to the cache server
			try 
			{
				string cmd = cmdname + " " + key + " " + flags + " "
					+ GetExpirationTime(expiry) + " " + length + "\r\n";
				sock.Write(Encoding.UTF8.GetBytes(cmd));
				sock.Write(val,0,length);
				sock.Write(Encoding.UTF8.GetBytes("\r\n"));
				sock.Flush();

				// get result code
				string line = sock.ReadLine();

				if(STORED == line) 
				{
					sock.Close();
					sock = null;
					return true;
				}
			}
			catch
			{
                try
                {
                    if (sock != null)
                        sock.TrueClose();
                }
                finally
                {
                    sock = null;
                }
			}

			if(sock != null)
				sock.Close();

			return false;
		}

		/// <summary>
		/// Store a counter to memcached given a key
		/// </summary>
		/// <param name="key">cache key</param>
		/// <param name="counter">number to store</param>
		/// <returns>true/false indicating success</returns>
		public bool StoreCounter(string key, long counter) 
		{
			return Set("set", key, counter, DateTime.MaxValue, null, true);
		}
    
		/// <summary>
		/// Store a counter to memcached given a key
		/// </summary>
		/// <param name="key">cache key</param>
		/// <param name="counter">number to store</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>true/false indicating success</returns>
		public bool StoreCounter(string key, long counter, int hashCode) 
		{
			return Set("set", key, counter, DateTime.MaxValue, hashCode, true);
		}

		/// <summary>
		/// Returns value in counter at given key as long. 
		/// </summary>
		/// <param name="key">cache ket</param>
		/// <returns>counter value or -1 if not found</returns>
		public long GetCounter(string key) 
		{
			return GetCounter(key, null);
		}

		/// <summary>
		/// Returns value in counter at given key as long. 
		/// </summary>
		/// <param name="key">cache ket</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>counter value or -1 if not found</returns>
		public long GetCounter(string key, object hashCode) 
		{
			if(key == null) 
			{
				return -1;
			}

			long counter = -1;
			try 
			{
				counter = long.Parse((string)Get(key, hashCode, true), new NumberFormatInfo());
			}
			catch(ArgumentException) 
			{
			}
			return counter;
		}

		/// <summary>
		/// Increment the value at the specified key by 1, and then return it.
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Increment(string key) 
		{
			return IncrementOrDecrement("incr", key, 1, null);
		}

		/// <summary>
		/// Increment the value at the specified key by passed in val. 
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <param name="inc">how much to increment by</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Increment(string key, long inc) 
		{
			return IncrementOrDecrement("incr", key, inc, null);
		}

		/// <summary>
		/// Increment the value at the specified key by the specified increment, and then return it.
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <param name="inc">how much to increment by</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Increment(string key, long inc, int hashCode) 
		{
			return IncrementOrDecrement("incr", key, inc, hashCode);
		}
	
		/// <summary>
		/// Decrement the value at the specified key by 1, and then return it.
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Decrement(string key) 
		{
			return IncrementOrDecrement("decr", key, 1, null);
		}

		/// <summary>
		/// Decrement the value at the specified key by passed in value, and then return it.
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <param name="inc">how much to increment by</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Decrement(string key, long inc) 
		{
			return IncrementOrDecrement("decr", key, inc, null);
		}

		/// <summary>
		/// Decrement the value at the specified key by the specified increment, and then return it.
		/// </summary>
		/// <param name="key">key where the data is stored</param>
		/// <param name="inc">how much to increment by</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>-1, if the key is not found, the value after incrementing otherwise</returns>
		public long Decrement(string key, long inc, int hashCode) 
		{
			return IncrementOrDecrement("decr", key, inc, hashCode);
		}

		/// <summary>
		/// Increments/decrements the value at the specified key by inc.
		/// 
		/// Note that the server uses a 32-bit unsigned integer, and checks for
		/// underflow. In the event of underflow, the result will be zero.  Because
		/// Java lacks unsigned types, the value is returned as a 64-bit integer.
		/// The server will only decrement a value if it already exists;
		/// if a value is not found, -1 will be returned.
		/// 
		/// TODO: C# has unsigned types.  We can fix this.
		/// </summary>
		/// <param name="cmdname">increment/decrement</param>
		/// <param name="key">cache key</param>
		/// <param name="inc">amount to incr or decr</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>new value or -1 if not exist</returns>
		private long IncrementOrDecrement(string cmdname, string key, long inc, object hashCode)
		{
		    key = Prefix + key;

			// get SockIO obj for given cache key
			SockIO sock = SockIOPool.GetInstance(_poolName).GetSock(key, hashCode);

			if(sock == null)
				return -1;

            try
            {
                string cmd = cmdname + " " + key + " " + inc + "\r\n";

                sock.Write(Encoding.UTF8.GetBytes(cmd));
                sock.Flush();

                // get result back
                string line = sock.ReadLine();

                if (new Regex("\\d+").Match(line).Success)
                {

                    // return sock to pool and return result
                    sock.Close();
                    return long.Parse(line, new NumberFormatInfo());

                }
            }
            catch (IOException e)
            {
                try
                {
                    sock.TrueClose();
                }
                finally
                {
                    sock = null;
                }
            }

		    if(sock != null)
				sock.Close();
			return -1;
		}

		/// <summary>
		/// Retrieve a key from the server, using a specific hash.
		/// 
		/// If the data was compressed or serialized when compressed, it will automatically
		/// be decompressed or serialized, as appropriate. (Inclusive or)
		/// 
		/// Non-serialized data will be returned as a string, so explicit conversion to
		/// numeric types will be necessary, if desired
		/// </summary>
		/// <param name="key">key where data is stored</param>
		/// <returns>the object that was previously stored, or null if it was not previously stored</returns>
		public object Get(string key) 
		{
			return Get(key, null, false);
		}

		/// <summary>
		/// Retrieve a key from the server, using a specific hash.
		/// 
		/// If the data was compressed or serialized when compressed, it will automatically
		/// be decompressed or serialized, as appropriate. (Inclusive or)
		/// 
		/// Non-serialized data will be returned as a string, so explicit conversion to
		/// numeric types will be necessary, if desired
		/// </summary>
		/// <param name="key">key where data is stored</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <returns>the object that was previously stored, or null if it was not previously stored</returns>
		public object Get(string key, int hashCode) 
		{
			return Get(key, hashCode, false);
		}

		/// <summary>
		/// Retrieve a key from the server, using a specific hash.
		/// 
		/// If the data was compressed or serialized when compressed, it will automatically
		/// be decompressed or serialized, as appropriate. (Inclusive or)
		/// 
		/// Non-serialized data will be returned as a string, so explicit conversion to
		/// numeric types will be necessary, if desired
		/// </summary>
		/// <param name="key">key where data is stored</param>
		/// <param name="hashCode">if not null, then the int hashcode to use</param>
		/// <param name="asString">if true, then return string val</param>
		/// <returns>the object that was previously stored, or null if it was not previously stored</returns>
		public object Get(string key, object hashCode, bool asString)
		{
            if (string.IsNullOrEmpty(key))
                return null;
		    key = Prefix + key;
			// get SockIO obj using cache key
			SockIO sock = SockIOPool.GetInstance(_poolName).GetSock(key, hashCode);
	    
			if(sock == null)
				return null;

			try 
			{
				string cmd = "get " + key + "\r\n";

				sock.Write(Encoding.UTF8.GetBytes(cmd));
				sock.Flush();

				// build empty map
				// and fill it from server
				Hashtable hm = new Hashtable();
				LoadItems(sock, hm, asString);

				// return the value for this key if we found it
				// else return null 
				sock.Close();
				return hm[key];

			}
			catch
			{
                try
                {
                    sock.TrueClose();
                }
                finally
                {
                    sock = null;
                }
			}
			return null;
		}

		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <returns>object array ordered in same order as key array containing results</returns>
		public object[] GetMultipleArray(string[] keys) 
		{
            return GetMultipleArray(keys, null, false);
		}

		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <param name="hashCodes">if not null, then the int array of hashCodes</param>
		/// <returns>object array ordered in same order as key array containing results</returns>
        public object[] GetMultipleArray(string[] keys, int[] hashCodes) 
		{
            return GetMultipleArray(keys, hashCodes, false);
		}

		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <param name="hashCodes">if not null, then the int array of hashCodes</param>
		/// <param name="asString">asString if true, retrieve string vals</param>
		/// <returns>object array ordered in same order as key array containing results</returns>
        public object[] GetMultipleArray(string[] keys, int[] hashCodes, bool asString) 
		{
			if(keys == null)
				return new object[0];

            Hashtable data = GetMultiple(keys, hashCodes, asString);

			object[] res = new object[keys.Length];
			for(int i = 0; i < keys.Length; i++)
			{
			    res[i] = data[Prefix + keys[i]];
			}

			return res;
		}

		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <returns>
		/// a hashmap with entries for each key is found by the server,
		/// keys that are not found are not entered into the hashmap, but attempting to
		/// retrieve them from the hashmap gives you null.
		/// </returns>
		public Hashtable GetMultiple(string[] keys) 
		{
            return GetMultiple(keys, null, false);
		}
    
		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <param name="hashCodes">hashCodes if not null, then the int array of hashCodes</param>
		/// <returns>
		/// a hashmap with entries for each key is found by the server,
		/// keys that are not found are not entered into the hashmap, but attempting to
		/// retrieve them from the hashmap gives you null.
		/// </returns>
        public Hashtable GetMultiple(string[] keys, int[] hashCodes) 
		{
            return GetMultiple(keys, hashCodes, false);
		}

		/// <summary>
		/// Retrieve multiple objects from the memcache.
		/// 
		/// This is recommended over repeated calls to <see cref="get">get(string)</see>, since it
		/// is more efficient.
		/// </summary>
		/// <param name="keys">string array of keys to retrieve</param>
		/// <param name="hashCodes">hashCodes if not null, then the int array of hashCodes</param>
		/// <param name="asString">if true then retrieve using string val</param>
		/// <returns>
		/// a hashmap with entries for each key is found by the server,
		/// keys that are not found are not entered into the hashmap, but attempting to
		/// retrieve them from the hashmap gives you null.
		/// </returns>
        public Hashtable GetMultiple(string[] keys, int[] hashCodes, bool asString) 
		{
			if(keys == null)
				return new Hashtable();

			Hashtable sockKeys = new Hashtable();

			for(int i = 0; i < keys.Length; ++i) 
			{
				object hash = null;
				if(hashCodes != null && hashCodes.Length > i)
					hash = hashCodes[i];

				// get SockIO obj from cache key
				SockIO sock = SockIOPool.GetInstance(_poolName).GetSock(Prefix+ keys[i], hash);

				if(sock == null)
					continue;

				// store in map and list if not already
				if(!sockKeys.ContainsKey(sock.Host))
					sockKeys[ sock.Host ] = new StringBuilder();

			    ((StringBuilder) sockKeys[sock.Host]).Append(" " + Prefix + keys[i]);

				// return to pool
				sock.Close();
			}

			// now query memcache
			Hashtable ret = new Hashtable();
			ArrayList toRemove = new ArrayList();
			foreach(string host in sockKeys.Keys) 
			{
				// get SockIO obj from hostname
				SockIO sock = SockIOPool.GetInstance(_poolName).GetConnection(host);

				try 
				{
					string cmd = "get" + (StringBuilder) sockKeys[ host ] + "\r\n";
					sock.Write(Encoding.UTF8.GetBytes(cmd));
					sock.Flush();
					LoadItems(sock, ret, asString);
				}
				catch
				{
					// clear this sockIO obj from the list
					// and from the map containing keys
					toRemove.Add(host);
                    try
                    {
                        sock.TrueClose();
                    }
                    finally
                    {
                        sock = null;
                    }
				}

				// Return socket to pool
				if(sock != null)
					sock.Close();
			}

			foreach(string host in toRemove)
			{
				sockKeys.Remove(host);
			}
			return ret;
		}
    
		/// <summary>
		/// This method loads the data from cache into a Hashtable.
		/// 
		/// Pass a SockIO object which is ready to receive data and a Hashtable
		/// to store the results.
		/// </summary>
		/// <param name="sock">socket waiting to pass back data</param>
		/// <param name="hm">hashmap to store data into</param>
		/// <param name="asString">if true, and if we are using NativehHandler, return string val</param>
		private void LoadItems(SockIO sock, Hashtable hm, bool asString) 
		{
			while(true) 
			{
				string line = sock.ReadLine();

				if(line.StartsWith(VALUE)) 
				{
					string[] info = line.Split(' ');
					string key    = info[1];
					int flag      = int.Parse(info[2], new NumberFormatInfo());
					int length    = int.Parse(info[3], new NumberFormatInfo());
				
					// read obj into buffer
					byte[] buf = new byte[length];
					sock.Read(buf);
					sock.ClearEndOfLine();

					// ready object
					object o=null;
				
					// check for compression
					if((flag & F_COMPRESSED) != 0)
					{
					    MemoryStream mem = null;
					    GZipStream gzi = null;
						try 
						{
							// read the input stream, and write to a byte array output stream since
							// we have to read into a byte array, but we don't know how large it
							// will need to be, and we don't want to resize it a bunch
                            mem = new MemoryStream(buf.Length);
						    gzi = new GZipStream(new MemoryStream(buf), CompressionMode.Compress);
							
							int count;
							var tmp = new byte[2048];
                            while ((count = gzi.Read(tmp, 0, tmp.Length)) > 0)
                            {
                                mem.Write(tmp, 0, count);
                            }

						    // store uncompressed back to buffer
							buf = mem.ToArray();
						}
						finally
						{
						    if(mem!=null)
						    {
						        mem.Close();
                                mem.Dispose();
						    }
                            if(gzi != null)
                            {
                                gzi.Close();
                                gzi.Dispose();
                            }
						}
					}

					// we can only take out serialized objects
					if((flag & F_SERIALIZED) == 0) 
					{
						if(_primitiveAsString || asString) 
						{
							o = Encoding.GetEncoding(_defaultEncoding).GetString(buf);
						}
						else 
						{
							// decoding object
							try 
							{
								o = NativeHandler.Decode(buf);    
							}
							catch(Exception e) 
							{
                                return;
							}
						}
					}
					else 
					{
						// deserialize if the data is serialized
					    MemoryStream memStream = null;
						try 
						{
							memStream = new MemoryStream(buf);
							o = new BinaryFormatter().Deserialize(memStream);
						}
						catch(SerializationException e) 
						{
						}
                        finally
						{
						    if(memStream != null)
						    {
						        memStream.Close();
                                memStream.Dispose();
						    }
						}
					}

					// store the object into the cache
					hm[ key ] =  o ;
				}
				else if(END == line) 
				{
					break;
				}
			}
		}

		/// <summary>
		/// Invalidates the entire cache.
		/// 
		/// Will return true only if succeeds in clearing all servers.
		/// </summary>
		/// <returns>success true/false</returns>
		public bool FlushAll() 
		{
			return FlushAll(null);
		}

		/// <summary>
		/// Invalidates the entire cache.
		/// 
		/// Will return true only if succeeds in clearing all servers.
		/// If pass in null, then will try to flush all servers.
		/// </summary>
		/// <param name="servers">optional array of host(s) to flush (host:port)</param>
		/// <returns>success true/false</returns>
		public bool FlushAll(ArrayList servers) 
		{
			// get SockIOPool instance
			SockIOPool pool = SockIOPool.GetInstance(_poolName);

			// return false if unable to get SockIO obj
			if(pool == null) 
			{
				return false;
			}

			// get all servers and iterate over them
            if (servers == null)
                servers = pool.Servers;

			// if no servers, then return early
			if(servers == null || servers.Count <= 0) 
			{
				return false;
			}

			bool success = true;

			for(int i = 0; i < servers.Count; i++) 
			{

				SockIO sock = pool.GetConnection((string)servers[i]);
				if(sock == null) 
				{
					success = false;
					continue;
				}

				// build command
				string command = "flush_all\r\n";

				try 
				{
					sock.Write(Encoding.UTF8.GetBytes(command));
					sock.Flush();

					// if we get appropriate response back, then we return true
					string line = sock.ReadLine();
					success = (OK == line)
						? success && true
						: false;
				}
				catch(IOException e) 
				{
					try 
					{
						sock.TrueClose();
					}
					catch{}

					success = false;
					sock = null;
				}

				if(sock != null)
					sock.Close();
			}

			return success;
		}

		/// <summary>
		/// Retrieves stats for all servers.
		/// 
		/// Returns a map keyed on the servername.
		/// The value is another map which contains stats
		/// with stat name as key and value as value.
		/// </summary>
		/// <returns></returns>
		public Hashtable Stats() 
		{
			return Stats(null);
		}

		/// <summary>
		/// Retrieves stats for passed in servers (or all servers).
		/// 
		/// Returns a map keyed on the servername.
		/// The value is another map which contains stats
		/// with stat name as key and value as value.
		/// </summary>
		/// <param name="servers">string array of servers to retrieve stats from, or all if this is null</param>
		/// <returns>Stats map</returns>
		public Hashtable Stats(ArrayList servers) 
		{

			// get SockIOPool instance
			SockIOPool pool = SockIOPool.GetInstance(_poolName);

			// return false if unable to get SockIO obj
			if(pool == null) 
			{
				return null;
			}

			// get all servers and iterate over them
            if (servers == null)
                servers = pool.Servers;

			// if no servers, then return early
			if(servers == null || servers.Count <= 0) 
			{
				return null;
			}

			// array of stats Hashtables
			Hashtable statsMaps = new Hashtable();

			for(int i = 0; i < servers.Count; i++) 
			{

				SockIO sock = pool.GetConnection((string)servers[i]);
				if(sock == null) 
				{
					continue;
				}

				// build command
				string command = "stats\r\n";

				try 
				{
					sock.Write(UTF8Encoding.UTF8.GetBytes(command));
					sock.Flush();

					// map to hold key value pairs
					Hashtable stats = new Hashtable();

					// loop over results
					while(true) 
					{
						string line = sock.ReadLine();

						if(line.StartsWith(STATS)) 
						{
							string[] info = line.Split(' ');
							string key    = info[1];
							string val  = info[2];

							stats[ key ] = val;

						}
						else if(END == line) 
						{
							break;
						}

						statsMaps[ servers[i] ] = stats;
					}
				}
				catch(IOException e) 
				{
					try 
					{
						sock.TrueClose();
					}
					catch(IOException) 
					{
					}

					sock = null;
				}

				if(sock != null)
					sock.Close();
			}

			return statsMaps;
		}
	}
}