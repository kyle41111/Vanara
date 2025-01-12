﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;
using static Vanara.PInvoke.Ole32;
using static Vanara.PInvoke.Shell32;
using static Vanara.PInvoke.User32;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace Vanara.Windows.Shell
{
	/// <summary>Specifies the text data formats that can be used to query, get and set text data format with Clipboard.</summary>
	public enum TextDataFormat
	{
		/// <summary>Specifies the standard ANSI text format.</summary>
		Text,

		/// <summary>Specifies the standard Windows Unicode text format.</summary>
		UnicodeText,

		/// <summary>Specifies text consisting of Rich Text Format (RTF) data.</summary>
		Rtf,

		/// <summary>Specifies text consisting of HTML data.</summary>
		Html,

		/// <summary>Specifies a comma-separated value (CSV) format, which is a common interchange format used by spreadsheets.</summary>
		CommaSeparatedValue,
	}

	/// <summary>
	/// Initializes and closes a session using the Clipboard calling <see cref="OpenClipboard"/> and then <see cref="CloseClipboard"/> on
	/// disposal. This can be called multiple times in nested calls and will ensure the Clipboard is only opened and closed at the highest scope.
	/// </summary>
	/// <seealso cref="System.IDisposable"/>
	public static class NativeClipboard
	{
		private static readonly object objectLock = new();
		private static ListenerWindow listener;

		/// <summary>Occurs when whenever the contents of the Clipboard have changed.</summary>
		public static event EventHandler ClipboardUpdate
		{
			add
			{
				lock (objectLock)
				{
					listener ??= new ListenerWindow();
					InternalClipboardUpdate += value;
				}
			}
			remove
			{
				lock (objectLock)
				{
					InternalClipboardUpdate -= value;
					if (InternalClipboardUpdate is null || InternalClipboardUpdate.GetInvocationList().Length == 0)
						listener = null;
				}
			}
		}

		private static event EventHandler InternalClipboardUpdate;

		/// <summary>Retrieves the currently supported clipboard formats.</summary>
		/// <value>A sequence of the currently supported formats.</value>
		public static IEnumerable<uint> CurrentlySupportedFormats
		{
			get
			{
				GetUpdatedClipboardFormats(null, 0, out var cnt);
				var fmts = new uint[cnt];
				Win32Error.ThrowLastErrorIfFalse(GetUpdatedClipboardFormats(fmts, (uint)fmts.Length, out cnt));
				return fmts.Take((int)cnt).ToArray();
			}
		}

		/// <summary>Gets or sets a <see cref="IComDataObject"/> instance from the Windows Clipboard.</summary>
		/// <value>A <see cref="IComDataObject"/> instance.</value>
		public static IComDataObject DataObject
		{
			get
			{
				OleGetClipboard(out var idata).ThrowIfFailed();
				return idata;
			}
			set
			{
				OleSetClipboard(value).ThrowIfFailed();
				Flush();
			}
		}

		/// <summary>Retrieves the clipboard sequence number for the current window station.</summary>
		/// <returns>
		/// The clipboard sequence number. If you do not have <c>WINSTA_ACCESSCLIPBOARD</c> access to the window station, the function
		/// returns zero.
		/// </returns>
		/// <remarks>
		/// The system keeps a serial number for the clipboard for each window station. This number is incremented whenever the contents of
		/// the clipboard change or the clipboard is emptied. You can track this value to determine whether the clipboard contents have
		/// changed and optimize creating DataObjects. If clipboard rendering is delayed, the sequence number is not incremented until the
		/// changes are rendered.
		/// </remarks>
		public static uint SequenceNumber => GetClipboardSequenceNumber();

		/// <summary>Clears the clipboard of any data or formatting.</summary>
		public static void Clear() => DataObject = null;

		/// <summary>Enumerates the data formats currently available on the clipboard.</summary>
		/// <returns>An enumeration of the data formats currently available on the clipboard.</returns>
		/// <remarks>
		/// <para>
		/// The <c>EnumFormats</c> function enumerates formats in the order that they were placed on the clipboard. If you are copying
		/// information to the clipboard, add clipboard objects in order from the most descriptive clipboard format to the least descriptive
		/// clipboard format. If you are pasting information from the clipboard, retrieve the first clipboard format that you can handle.
		/// That will be the most descriptive clipboard format that you can handle.
		/// </para>
		/// <para>
		/// The system provides automatic type conversions for certain clipboard formats. In the case of such a format, this function
		/// enumerates the specified format, then enumerates the formats to which it can be converted.
		/// </para>
		/// </remarks>
		public static IEnumerable<uint> EnumAvailableFormats() => DataObject.EnumFormats().Select(f => unchecked((uint)f.cfFormat));

		/// <summary>Carries out the clipboard shutdown sequence. It also releases any IDataObject instances that were placed on the clipboard.</summary>
		public static void Flush() => OleFlushClipboard().ThrowIfFailed();

		/// <summary>Retrieves the window handle of the current owner of the clipboard.</summary>
		/// <returns>
		/// <para>If the function succeeds, the return value is the handle to the window that owns the clipboard.</para>
		/// <para>If the clipboard is not owned, the return value is <c>IntPtr.Zero</c>.</para>
		/// </returns>
		/// <remarks>
		/// <para>The clipboard can still contain data even if the clipboard is not currently owned.</para>
		/// <para>In general, the clipboard owner is the window that last placed data in clipboard.</para>
		/// </remarks>
		public static HWND GetClipboardOwner() => User32.GetClipboardOwner();

		/// <summary>Obtains data from the clipboard.</summary>
		/// <param name="formatId">Specifies the particular clipboard format of interest.</param>
		/// <param name="aspect">
		/// Indicates how much detail should be contained in the rendering. This parameter should be one of the DVASPECT enumeration values.
		/// A single clipboard format can support multiple aspects or views of the object. Most data and presentation transfer and caching
		/// methods pass aspect information. For example, a caller might request an object's iconic picture, using the metafile clipboard
		/// format to retrieve it. Note that only one DVASPECT value can be used in dwAspect. That is, dwAspect cannot be the result of a
		/// Boolean OR operation on several DVASPECT values.
		/// </param>
		/// <param name="index">
		/// Part of the aspect when the data must be split across page boundaries. The most common value is -1, which identifies all of the
		/// data. For the aspects DVASPECT_THUMBNAIL and DVASPECT_ICON, lindex is ignored.
		/// </param>
		/// <returns>The object associated with the request. If no object can be determined, a <see cref="byte"/>[] is returned.</returns>
		/// <exception cref="System.InvalidOperationException">Unrecognized TYMED value.</exception>
		public static object GetData(uint formatId, DVASPECT aspect = DVASPECT.DVASPECT_CONTENT, int index = -1) =>
			DataObject.GetData(formatId, aspect, index);

		/// <summary>Obtains data from the clipboard.</summary>
		/// <typeparam name="T">The type of the object being retrieved.</typeparam>
		/// <param name="formatId">Specifies the particular clipboard format of interest.</param>
		/// <param name="index">
		/// Part of the aspect when the data must be split across page boundaries. The most common value is -1, which identifies all of the
		/// data. For the aspects DVASPECT_THUMBNAIL and DVASPECT_ICON, lindex is ignored.
		/// </param>
		/// <returns>The object associated with the request. If no object can be determined, <c>default(T)</c> is returned.</returns>
		public static T GetData<T>(uint formatId, int index = -1) => DataObject.GetData<T>(formatId, index);

		/// <summary>
		/// This is used when a group of files in CF_HDROP (FileDrop) format is being renamed as well as transferred. The data consists of an
		/// array that contains a new name for each file, in the same order that the files are listed in the accompanying CF_HDROP format.
		/// The format of the character array is the same as that used by CF_HDROP to list the transferred files.
		/// </summary>
		/// <returns>A list of strings containing a new name for each file.</returns>
		public static string[] GetFileNameMap()
		{
			if (IsFormatAvailable(ShellClipboardFormat.CFSTR_FILENAMEMAPW))
				return DataObject.GetData(ShellClipboardFormat.CFSTR_FILENAMEMAPW) as string[];
			else if (IsFormatAvailable(ShellClipboardFormat.CFSTR_FILENAMEMAPA))
				return DataObject.GetData(ShellClipboardFormat.CFSTR_FILENAMEMAPA) as string[];
			return new string[0];
		}

		/// <summary>Retrieves the first available clipboard format in the specified list.</summary>
		/// <param name="idList">The clipboard formats, in priority order.</param>
		/// <returns>
		/// If the function succeeds, the return value is the first clipboard format in the list for which data is available. If the
		/// clipboard is empty, the return value is 0. If the clipboard contains data, but not in any of the specified formats, the return
		/// value is –1.
		/// </returns>
		public static int GetFirstFormatAvailable(params uint[] idList) => GetPriorityClipboardFormat(idList, idList.Length);

		/// <summary>Retrieves from the clipboard the name of the specified registered format.</summary>
		/// <param name="formatId">The type of format to be retrieved.</param>
		/// <returns>The format name.</returns>
		public static string GetFormatName(uint formatId) => ShellClipboardFormat.GetName(formatId);

		/// <summary>Retrieves the handle to the window that currently has the clipboard open.</summary>
		/// <returns>
		/// If the function succeeds, the return value is the handle to the window that has the clipboard open. If no window has the
		/// clipboard open, the return value is <c>IntPtr.Zero</c>.
		/// </returns>
		/// <remarks>
		/// If an application or DLL specifies a <c>NULL</c> window handle when calling the OpenClipboard function, the clipboard is opened
		/// but is not associated with a window. In such a case, <c>GetOpenClipboardWindow</c> returns <c>IntPtr.Zero</c>.
		/// </remarks>
		public static HWND GetOpenClipboardWindow() => User32.GetOpenClipboardWindow();

		/// <summary>Gets the shell item array associated with the data object, if possible.</summary>
		/// <returns>The <see cref="ShellItemArray"/> associated with the data object, if set. Otherwise, <see langword="null"/>.</returns>
		public static ShellItemArray GetShellItemArray() => IsFormatAvailable(ShellClipboardFormat.CFSTR_SHELLIDLIST) ? ShellItemArray.FromDataObject(DataObject) : null;

		/// <summary>Gets the text from the native Clipboard in the specified format.</summary>
		/// <param name="formatId">A clipboard format. For a description of the standard clipboard formats, see Standard Clipboard Formats.</param>
		/// <returns>The string value or <see langword="null"/> if the format is not available.</returns>
		public static string GetText(TextDataFormat formatId) => GetData(Txt2Id(formatId)) as string;

		/// <summary>Determines whether the data object pointer previously placed on the clipboard is still on the clipboard.</summary>
		/// <param name="dataObject">
		/// The IDataObject interface on the data object containing clipboard data of interest, which the caller previously placed on the clipboard.
		/// </param>
		/// <returns><see langword="true"/> on success; otherwise, <see langword="false"/>.</returns>
		public static bool IsCurrentDataObject(IComDataObject dataObject) => OleIsCurrentClipboard(dataObject) == HRESULT.S_OK;

		/// <summary>Determines whether the clipboard contains data in the specified format.</summary>
		/// <param name="id">A standard or registered clipboard format.</param>
		/// <returns>If the clipboard format is available, the return value is <see langword="true"/>; otherwise <see langword="false"/>.</returns>
		public static bool IsFormatAvailable(uint id) => DataObject.IsFormatAvailable(id); // EnumAvailableFormats().Contains(id);

		/// <summary>Determines whether the clipboard contains data in the specified format.</summary>
		/// <param name="id">A clipboard format string.</param>
		/// <returns>If the clipboard format is available, the return value is <see langword="true"/>; otherwise <see langword="false"/>.</returns>
		public static bool IsFormatAvailable(string id) => IsClipboardFormatAvailable(RegisterFormat(id));

		/// <summary>Registers a new clipboard format. This format can then be used as a valid clipboard format.</summary>
		/// <param name="format">The name of the new format.</param>
		/// <returns>The registered clipboard format identifier.</returns>
		/// <exception cref="System.ArgumentNullException">format</exception>
		/// <remarks>
		/// If a registered format with the specified name already exists, a new format is not registered and the return value identifies the
		/// existing format. This enables more than one application to copy and paste data using the same registered clipboard format. Note
		/// that the format name comparison is case-insensitive.
		/// </remarks>
		public static uint RegisterFormat(string format) => ShellClipboardFormat.Register(format);

		/// <summary>Places data on the clipboard in a specified clipboard format.</summary>
		/// <param name="formatId">The clipboard format. This parameter can be a registered format or any of the standard clipboard formats.</param>
		/// <param name="data">The binary data in the specified format.</param>
		/// <exception cref="System.ArgumentNullException">data</exception>
		public static void SetBinaryData(uint formatId, byte[] data) => DataObject.SetData(formatId, data);

		/// <summary>Places data on the clipboard in a specified clipboard format.</summary>
		/// <param name="formatId">The clipboard format. This parameter can be a registered format or any of the standard clipboard formats.</param>
		/// <param name="data">The data in the format dictated by <paramref name="formatId"/>.</param>
		public static void SetData<T>(uint formatId, T data) where T : struct => DataObject.SetData(formatId, data);

		/// <summary>Places data on the clipboard in a specified clipboard format.</summary>
		/// <param name="formatId">The clipboard format. This parameter can be a registered format or any of the standard clipboard formats.</param>
		/// <param name="values">The data in the format dictated by <paramref name="formatId"/>.</param>
		public static void SetData<T>(uint formatId, IEnumerable<T> values) where T : struct
		{
			var pMem = SafeMoveableHGlobalHandle.CreateFromList(values);
			Win32Error.ThrowLastErrorIfInvalid(pMem);
			DataObject.SetData(formatId, pMem);
		}

		/// <summary>Places data on the clipboard in a specified clipboard format.</summary>
		/// <param name="formatId">The clipboard format. This parameter can be a registered format or any of the standard clipboard formats.</param>
		/// <param name="values">The list of strings.</param>
		/// <param name="packing">The packing type for the strings.</param>
		/// <param name="charSet">The character set to use for the strings.</param>
		public static void SetData(uint formatId, IEnumerable<string> values, StringListPackMethod packing = StringListPackMethod.Concatenated, CharSet charSet = CharSet.Auto)
		{
			var pMem = SafeMoveableHGlobalHandle.CreateFromStringList(values, packing, charSet);
			Win32Error.ThrowLastErrorIfInvalid(pMem);
			DataObject.SetData(formatId, pMem);
		}

		/// <summary>Puts a list of shell items onto the clipboard.</summary>
		/// <param name="shellItems">The sequence of shell items. The PIDL of each shell item must be absolute.</param>
		public static void SetShellItems(IEnumerable<ShellItem> shellItems) => DataObject = (shellItems is ShellItemArray shia ? shia : new ShellItemArray(shellItems)).ToDataObject();

		/// <summary>Puts a list of shell items onto the clipboard.</summary>
		/// <param name="parent">The parent folder instance.</param>
		/// <param name="relativeShellItems">The sequence of shell items relative to <paramref name="parent"/>.</param>
		public static void SetShellItems(ShellFolder parent, IEnumerable<ShellItem> relativeShellItems)
		{
			if (parent is null) throw new ArgumentNullException(nameof(parent));
			if (relativeShellItems is null) throw new ArgumentNullException(nameof(relativeShellItems));
			SHCreateDataObject(parent.PIDL, relativeShellItems.Select(i => i.PIDL), default, out var dataObj).ThrowIfFailed();
			OleSetClipboard(dataObj).ThrowIfFailed();
		}

		/// <summary>Sets multiple text types to the Clipboard.</summary>
		/// <param name="text">The Unicode Text value.</param>
		/// <param name="htmlText">The HTML text value. If <see langword="null"/>, this format will not be set.</param>
		/// <param name="rtfText">The Rich Text Format value. If <see langword="null"/>, this format will not be set.</param>
		public static void SetText(string text, string htmlText = null, string rtfText = null)
		{
			if (text is not null) SetText(text, TextDataFormat.UnicodeText);
			if (htmlText is not null) SetText(htmlText, TextDataFormat.Html);
			if (rtfText is not null) SetText(rtfText, TextDataFormat.Rtf);
		}

		/// <summary>Sets a specific text type to the Clipboard.</summary>
		/// <param name="value">The text value.</param>
		/// <param name="format">The clipboard text format to set.</param>
		public static void SetText(string value, TextDataFormat format) => DataObject.SetData(Txt2Id(format), value);

		/// <summary>Sets a URL with optional title to the clipboard.</summary>
		/// <param name="url">The URL.</param>
		/// <param name="title">The title. This value can be <see langword="null"/>.</param>
		/// <exception cref="ArgumentNullException">url</exception>
		public static void SetUrl(string url, string title = null) => DataObject.SetUrl(url, title);

		/// <summary>Obtains data from a source data object.</summary>
		/// <typeparam name="T">The type of the object being retrieved.</typeparam>
		/// <param name="formatId">Specifies the particular clipboard format of interest.</param>
		/// <param name="obj">The object associated with the request. If no object can be determined, <c>default(T)</c> is returned.</param>
		/// <param name="index">
		/// Part of the aspect when the data must be split across page boundaries. The most common value is -1, which identifies all of the
		/// data. For the aspects DVASPECT_THUMBNAIL and DVASPECT_ICON, lindex is ignored.
		/// </param>
		/// <returns><see langword="true"/> if data is available and retrieved; otherwise <see langword="false"/>.</returns>
		public static bool TryGetData<T>(uint formatId, out T obj, int index = -1) => DataObject.TryGetData(formatId, out obj, index);

		private static uint Txt2Id(TextDataFormat tf) => tf switch
		{
			TextDataFormat.Text => CLIPFORMAT.CF_TEXT,
			TextDataFormat.UnicodeText => CLIPFORMAT.CF_UNICODETEXT,
			TextDataFormat.Rtf => ShellClipboardFormat.Register(ShellClipboardFormat.CF_RTF),
			TextDataFormat.Html => ShellClipboardFormat.Register(ShellClipboardFormat.CF_HTML),
			TextDataFormat.CommaSeparatedValue => ShellClipboardFormat.Register(ShellClipboardFormat.CF_CSV),
			_ => throw new ArgumentOutOfRangeException(nameof(tf)),
		};

		private class ListenerWindow : SystemEventHandler
		{
			protected override bool MessageFilter(HWND hwnd, uint msg, IntPtr wParam, IntPtr lParam, out IntPtr lReturn)
			{
				lReturn = default;
				switch (msg)
				{
					case (uint)WindowMessage.WM_DESTROY:
						RemoveClipboardFormatListener(MessageWindowHandle);
						break;

					case (uint)ClipboardNotificationMessage.WM_CLIPBOARDUPDATE:
						InternalClipboardUpdate?.Invoke(this, EventArgs.Empty);
						break;
				}
				return false;
			}

			protected override void OnMessageWindowHandleCreated()
			{
				base.OnMessageWindowHandleCreated();
				AddClipboardFormatListener(MessageWindowHandle);
			}
		}
	}
}