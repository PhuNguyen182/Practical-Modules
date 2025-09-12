#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace PracticalUtilities.IOUtils
{
	public static class FileUtils
	{
		/// <summary>
		/// Logs all files and sub-folders within <paramref name="sourceDirectoryPath"/>,
		/// then creates a new folder under <paramref name="destinationParentDirectoryPath"/>
		/// and moves every item (files and directories) from the source into that new folder.
		/// Name collisions are resolved by appending numeric suffixes.
		/// </summary>
		/// <param name="sourceDirectoryPath">Absolute or project-relative directory to enumerate and move from.</param>
		/// <param name="destinationParentDirectoryPath">Absolute or project-relative parent directory where the new folder will be created.</param>
		/// <param name="newFolderBaseName">Base name for the newly created folder. A numeric suffix will be appended if needed.</param>
		public static void MoveAllToNewIndependentFolder(string sourceDirectoryPath,
			string destinationParentDirectoryPath, string newFolderBaseName = "MovedContents")
		{
			if (string.IsNullOrWhiteSpace(sourceDirectoryPath))
			{
				Debug.LogError("Source directory path is null or empty.");
				return;
			}

			if (string.IsNullOrWhiteSpace(destinationParentDirectoryPath))
			{
				Debug.LogError("Destination parent directory path is null or empty.");
				return;
			}

			var normalizedSource = GetAbsolutePath(sourceDirectoryPath);
			var normalizedDestParent = GetAbsolutePath(destinationParentDirectoryPath);

			if (!Directory.Exists(normalizedSource))
			{
				Debug.LogError($"Source directory does not exist: {normalizedSource}");
				return;
			}

			if (!Directory.Exists(normalizedDestParent))
			{
				Directory.CreateDirectory(normalizedDestParent);
			}

			if (IsSubPathOf(normalizedDestParent, normalizedSource))
			{
				Debug.LogError("Destination parent cannot be inside the source directory.");
				return;
			}

			Debug.Log($"[Move] Source: {normalizedSource}");
			Debug.Log($"[Move] Destination Parent: {normalizedDestParent}");

			// 1) Log directory tree
			LogDirectoryTree(normalizedSource);

			// 2) Create a unique destination folder under destination parent
			string destinationFolderPath = EnsureUniqueDirectory(normalizedDestParent,
				string.IsNullOrWhiteSpace(newFolderBaseName) ? "MovedContents" : newFolderBaseName);
			Directory.CreateDirectory(destinationFolderPath);
			Debug.Log($"[Move] Created destination folder: {destinationFolderPath}");

			// 3) Move all files
			foreach (var filePath in Directory.GetFiles(normalizedSource))
			{
				var fileName = Path.GetFileName(filePath);
				var targetPath = Path.Combine(destinationFolderPath, fileName);
				targetPath = EnsureUniqueFilePath(targetPath);

				try
				{
					File.Move(filePath, targetPath);
					Debug.Log($"[Move File] {filePath} -> {targetPath}");
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to move file '{filePath}' to '{targetPath}'. Error: {ex.Message}");
				}
			}

			// 4) Move all directories (top-level) under source
			foreach (var dirPath in Directory.GetDirectories(normalizedSource))
			{
				var dirName =
					Path.GetFileName(dirPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
				var targetDir = Path.Combine(destinationFolderPath, dirName);
				targetDir = EnsureUniqueDirectoryPath(targetDir);

				try
				{
					Directory.Move(dirPath, targetDir);
					Debug.Log($"[Move Dir] {dirPath} -> {targetDir}");
				}
				catch (Exception ex)
				{
					Debug.LogError($"Failed to move directory '{dirPath}' to '{targetDir}'. Error: {ex.Message}");
				}
			}
		}

		private static void LogDirectoryTree(string rootDirectoryPath)
		{
			Debug.Log($"[Tree] {rootDirectoryPath}");
			try
			{
				void Recurse(string dir, int depth)
				{
					var indent = new string(' ', depth * 2);
					foreach (var file in Directory.GetFiles(dir))
					{
						Debug.Log($"{indent}- File: {file}");
					}

					foreach (var sub in Directory.GetDirectories(dir))
					{
						Debug.Log($"{indent}+ Dir: {sub}");
						Recurse(sub, depth + 1);
					}
				}

				Recurse(rootDirectoryPath, 0);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Failed to enumerate directory tree. Error: {ex.Message}");
			}
		}

		private static string GetAbsolutePath(string path)
		{
			// Treat paths that start with "/" or drive letters as absolute, otherwise combine with project root
			if (Path.IsPathRooted(path))
			{
				return Path.GetFullPath(path);
			}

			// If user provides a path like "Assets/..", map it from project root
			var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
			return Path.GetFullPath(Path.Combine(projectRoot, path));
		}

		private static bool IsSubPathOf(string candidatePath, string basePath)
		{
			var candidateFull = Path.GetFullPath(candidatePath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var baseFull = Path.GetFullPath(basePath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			var comparison = Application.platform == RuntimePlatform.WindowsPlayer ||
			                 Application.platform == RuntimePlatform.WindowsEditor
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;

			return candidateFull.StartsWith(baseFull + Path.DirectorySeparatorChar, comparison) ||
			       string.Equals(candidateFull, baseFull, comparison);
		}

		private static string EnsureUniqueDirectory(string parentDirectoryPath, string baseName)
		{
			int index = 0;
			while (true)
			{
				var candidate = index == 0
					? Path.Combine(parentDirectoryPath, baseName)
					: Path.Combine(parentDirectoryPath, $"{baseName} ({index})");
				if (!Directory.Exists(candidate) && !File.Exists(candidate))
				{
					return candidate;
				}

				index++;
			}
		}

		private static string EnsureUniqueDirectoryPath(string directoryPath)
		{
			if (!Directory.Exists(directoryPath) && !File.Exists(directoryPath))
			{
				return directoryPath;
			}

			var parent = Path.GetDirectoryName(directoryPath) ?? Directory.GetCurrentDirectory();
			var name = Path.GetFileName(directoryPath);
			return EnsureUniqueDirectory(parent, name);
		}

		private static string EnsureUniqueFilePath(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return filePath;
			}

			var directory = Path.GetDirectoryName(filePath) ?? Directory.GetCurrentDirectory();
			var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
			var extension = Path.GetExtension(filePath);

			int index = 1;
			while (true)
			{
				var candidateName = $"{fileNameWithoutExt} ({index}){extension}";
				var candidatePath = Path.Combine(directory, candidateName);
				if (!File.Exists(candidatePath))
				{
					return candidatePath;
				}

				index++;
			}
		}
	}
}
#endif
