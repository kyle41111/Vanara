﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vanara.Extensions;
using static Vanara.PInvoke.AdvApi32;

namespace Vanara.PInvoke
{
	/// <summary>Extension methods for PACE, PACL and PSECURITY_DESCRIPTOR.</summary>
	public static class WinNTExtensions
	{
		/// <summary>Gets the number of ACEs held by an ACL.</summary>
		/// <param name="pACL">The pointer to the ACL structure to query.</param>
		/// <returns>The ace count.</returns>
		public static uint AceCount(this PACL pACL) => IsValidAcl(pACL) && GetAclInformation(pACL, out ACL_SIZE_INFORMATION si) ? si.AceCount : 0;

		/// <summary>Gets the total number of bytes allocated to the ACL.</summary>
		/// <param name="pACL">The pointer to the ACL structure to query.</param>
		/// <returns>The total of the free and used bytes in the ACL.</returns>
		public static uint BytesAllocated(this PACL pACL) => IsValidAcl(pACL) && GetAclInformation(pACL, out ACL_SIZE_INFORMATION si) ? si.AclBytesFree + si.AclBytesInUse : 0;

		/// <summary>The <c>GetAce</c> function obtains a pointer to an access control entry (ACE) in an access control list (ACL).</summary>
		/// <param name="pAcl">A pointer to an ACL that contains the ACE to be retrieved.</param>
		/// <param name="aceIndex">
		/// The index of the ACE to be retrieved. A value of zero corresponds to the first ACE in the ACL, a value of one to the second ACE,
		/// and so on.
		/// </param>
		/// <returns>A pointer to the ACE.</returns>
		public static PACE GetAce(this PACL pAcl, uint aceIndex)
		{
			Win32Error.ThrowLastErrorIfFalse(AdvApi32.GetAce(pAcl, aceIndex, out var acePtr));
			return acePtr;
		}

		/// <summary>Enumerates the ACEs in an ACL.</summary>
		/// <param name="pAcl">A pointer to an ACL that contains the ACE to be retrieved.</param>
		/// <returns>A sequence of PACE values from the ACL.</returns>
		public static IEnumerable<PACE> EnumerateAces(this PACL pAcl)
		{
			for (var i = 0U; i < pAcl.AceCount(); i++)
				yield return GetAce(pAcl, i);
		}

		/// <summary>Gets the Flags for an ACE, if defined.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The Flags value, if this is an object ACE, otherwise <see langword="null"/>.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static AdvApi32.ObjectAceFlags? GetFlags(this PACE pAce)
		{
			if (pAce.IsNull) throw new ArgumentNullException(nameof(pAce));
			return !pAce.IsObjectAce() ? null : pAce.DangerousGetHandle().ToStructure<ACCESS_ALLOWED_OBJECT_ACE>().Flags;
		}

		/// <summary>Gets the header for an ACE.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The <see cref="ACE_HEADER"/> value.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static ACE_HEADER GetHeader(this PACE pAce) => !pAce.IsNull ? pAce.DangerousGetHandle().ToStructure<ACE_HEADER>() : throw new ArgumentNullException(nameof(pAce));

		/// <summary>Gets the InheritedObjectType for an ACE, if defined.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The InheritedObjectType value, if this is an object ACE, otherwise <see langword="null"/>.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static Guid? GetInheritedObjectType(this PACE pAce)
		{
			if (pAce.IsNull) throw new ArgumentNullException(nameof(pAce));
			return !pAce.IsObjectAce() ? null : pAce.DangerousGetHandle().ToStructure<ACCESS_ALLOWED_OBJECT_ACE>().InheritedObjectType;
		}

		/// <summary>Gets the mask for an ACE.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The ACCESS_MASK value.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static uint GetMask(this PACE pAce) => !pAce.IsNull ? pAce.DangerousGetHandle().ToStructure<ACCESS_ALLOWED_ACE>().Mask : throw new ArgumentNullException(nameof(pAce));

		/// <summary>Gets the ObjectType for an ACE, if defined.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The ObjectType value, if this is an object ACE, otherwise <see langword="null"/>.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static Guid? GetObjectType(this PACE pAce)
		{
			if (pAce.IsNull) throw new ArgumentNullException(nameof(pAce));
			return !pAce.IsObjectAce() ? null : pAce.DangerousGetHandle().ToStructure<ACCESS_ALLOWED_OBJECT_ACE>().ObjectType;
		}

		/// <summary>Gets the SID for an ACE.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns>The SID value.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		public static SafePSID GetSid(this PACE pAce)
		{
			if (pAce.IsNull) throw new ArgumentNullException(nameof(pAce));
			var offset = Marshal.SizeOf(typeof(ACE_HEADER)) + sizeof(uint);
			if (pAce.IsObjectAce()) offset += sizeof(uint) + Marshal.SizeOf(typeof(Guid)) * 2;
			unsafe
			{
				return SafePSID.CreateFromPtr((IntPtr)((byte*)pAce.DangerousGetHandle() + offset));
			}
		}

		/// <summary>Determines if a ACE is an object ACE.</summary>
		/// <param name="pAce">A pointer to an ACE.</param>
		/// <returns><see langword="true"/> if is this is an object ACE; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="System.ArgumentNullException">pAce</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">pAce - Unknown ACE type.</exception>
		public static bool IsObjectAce(this PACE pAce)
		{
			if (pAce.IsNull) throw new ArgumentNullException(nameof(pAce));
			var aceType = (byte)GetHeader(pAce).AceType;
			if (aceType > 0x15) throw new ArgumentOutOfRangeException(nameof(pAce), "Unknown ACE type.");
			return (aceType >= 0x5 && aceType <= 0x8) || aceType == 0xB || aceType == 0xC || aceType == 0xF || aceType == 0x10;
		}

		/// <summary>Determines whether the security descriptor is self-relative.</summary>
		/// <param name="pSD">The pointer to the SECURITY_DESCRIPTOR structure to query.</param>
		/// <returns><c>true</c> if it is self-relative; otherwise, <c>false</c>.</returns>
		public static bool IsSelfRelative(this PSECURITY_DESCRIPTOR pSD) => GetSecurityDescriptorControl(pSD, out var ctrl, out _) ? ctrl.IsFlagSet(SECURITY_DESCRIPTOR_CONTROL.SE_SELF_RELATIVE) : throw Win32Error.GetLastError().GetException();

		/// <summary>Validates an access control list (ACL).</summary>
		/// <param name="pAcl">The pointer to the ACL structure to query.</param>
		/// <returns><c>true</c> if the ACL is valid; otherwise, <c>false</c>.</returns>
		public static bool IsValidAcl(this PACL pAcl) => IsValidAcl(pAcl);

		/// <summary>Determines whether the components of a security descriptor are valid.</summary>
		/// <param name="pSD">The pointer to the SECURITY_DESCRIPTOR structure to query.</param>
		/// <returns>
		/// <c>true</c> if the components of the security descriptor are valid. If any of the components of the security descriptor are not
		/// valid, the return value is <c>false</c>.
		/// </returns>
		public static bool IsValidSecurityDescriptor(this PSECURITY_DESCRIPTOR pSD) => IsValidSecurityDescriptor(pSD);

		/// <summary>Gets the size, in bytes, of an ACE.</summary>
		/// <param name="pAce">The pointer to the ACE structure to query.</param>
		/// <returns>The size, in bytes, of the ACE.</returns>
		public static uint Length(this PACE pAce)
		{
			unsafe
			{
				var p = (ACE_HEADER*)(void*)pAce.DangerousGetHandle();
				return p == null ? 0U : p->AceSize;
			}
		}

		/// <summary>Gets the size, in bytes, of an ACL. If the ACL is not valid, 0 is returned.</summary>
		/// <param name="pACL">The pointer to the ACL structure to query.</param>
		/// <returns>The size, in bytes, of an ACL. If the ACL is not valid, 0 is returned.</returns>
		public static uint Length(this PACL pACL) => IsValidAcl(pACL) && GetAclInformation(pACL, out ACL_SIZE_INFORMATION si) ? si.AclBytesInUse : 0;

		/// <summary>Gets the size, in bytes, of a security descriptor. If it is not valid, 0 is returned.</summary>
		/// <param name="pSD">The pointer to the SECURITY_DESCRIPTOR structure to query.</param>
		/// <returns>The size, in bytes, of a security descriptor. If it is not valid, 0 is returned.</returns>
		public static uint Length(this PSECURITY_DESCRIPTOR pSD) => IsValidSecurityDescriptor(pSD) ? GetSecurityDescriptorLength(pSD) : 0U;
	}
}