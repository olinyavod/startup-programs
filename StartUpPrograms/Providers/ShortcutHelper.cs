using System;
using System.IO;
using Shell32;

namespace StartUpPrograms.Providers
{
	public static class ShortcutHelper
	{
		private static FolderItem GetFolderItem(Shell shell, string path)
		{
			var directory = Path.GetDirectoryName(path);
			var file = Path.GetFileName(path);
			var folder = shell.NameSpace(directory);
			return folder.ParseName(file);
		}

		private static bool IsShortcut(Shell shell, string path)
		{
			if(!File.Exists(path))
			{
				return false;
			}
			
			var folderItem = GetFolderItem(shell, path);

			return folderItem?.IsLink == true;
		}

		public static bool IsShortcut(string path)
		{
			return IsShortcut(new Shell(), path);
		}

		public static Tuple<string, string> GetPathAndArguments(string path)
		{
			var shell = new Shell();
			
			if(IsShortcut(shell, path))
			{
				var folderItem = GetFolderItem(shell, path);

				var link = (ShellLinkObject)folderItem.GetLink;
				return new Tuple<string, string>(link.Path, link.Arguments);
			}

			return new Tuple<string, string>(string.Empty, string.Empty);
		}
	}
}
