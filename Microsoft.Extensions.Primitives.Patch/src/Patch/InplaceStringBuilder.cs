using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Primitives
{
	// Microsoft.Extensions.Primitives.InplaceStringBuilder
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using Microsoft.Extensions.Primitives;

	[DebuggerDisplay("Value = {_value}")]
	public struct InplaceStringBuilder
	{
		private int _offset;

		private int _capacity;

		private string _value;

		public int Capacity
		{
			get
			{
				return _capacity;
			}
			set
			{
				if (value < 0)
				{
					throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value)");
				}
				if (_offset > 0)
				{
					throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_CannotChangeAfterWriteStarted)");
				}
				_capacity = value;
			}
		}

		public InplaceStringBuilder(int capacity)
		{
			this = default(InplaceStringBuilder);
			if (capacity < 0)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity)");
			}
			_capacity = capacity;
		}

		public void Append(string value)
		{
			if (value == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value)");
			}
			Append(value, 0, value.Length);
		}

		public void Append(StringSegment segment)
		{
			Append(segment.Buffer, segment.Offset, segment.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Append(string value, int offset, int count)
		{
			EnsureValueIsInitialized();
			if (value == null || offset < 0 || value.Length - offset < count || Capacity - _offset < count)
			{
				ThrowValidationError(value, offset, count);
			}
			fixed (char* ptr = _value)
			{
				fixed (char* ptr2 = value)
				{
					Unsafe.CopyBlockUnaligned(ptr + _offset, ptr2 + offset, (uint)(count * 2));
					_offset += count;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Append(char c)
		{
			EnsureValueIsInitialized();
			if (_offset >= Capacity)
			{
				throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, 1, Capacity - _offset)");
			}
			fixed (char* ptr = _value)
			{
				ptr[_offset++] = c;
			}
		}

		public override string ToString()
		{
			if (Capacity != _offset)
			{
				throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotUsedEntirely, Capacity, _offset)");
			}
			return _value;
		}

		private void EnsureValueIsInitialized()
		{
			if (_value == null)
			{
				_value = new string('\0', _capacity);
			}
		}

		private void ThrowValidationError(string value, int offset, int count)
		{
			if (value == null)
			{
				throw new Exception("ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value)");
			}
			if (offset < 0 || value.Length - offset < count)
			{
				throw new Exception("ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.offset)");
			}
			if (Capacity - _offset < count)
			{
				throw new Exception("ThrowHelper.ThrowInvalidOperationException(ExceptionResource.Capacity_NotEnough, value.Length, Capacity - _offset)");
			}
		}
	}


}
