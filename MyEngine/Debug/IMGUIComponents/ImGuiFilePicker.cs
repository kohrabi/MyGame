using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconFonts;
using ImGuiNET.Renderer;
using Num = System.Numerics;

namespace MyEngine.Debug.IMGUIComponents;

public class ImGuiFilePicker : ImGuiComponent
{
	struct PickableFile
	{
		public string Name;
		public string Type;
		public string CreationDate;
		public string Path;
		public bool IsDirectory;
	}
	
	public string RootFolder = "";
	private string _currentFolder = "";
	public string CurrentFolder
	{
		get => _currentFolder;
		set
		{
			_currentFolder = value;
			GetFiles();
		}
	}

	public string PopupName = "Open File";
	public string ConfirmButtonName = "Open";
	public string SelectedFile = "";
	public string SelectedFolder = "";
	public List<string> AllowedExtensions;
	public bool HideHiddenFolders = true;
	public bool OnlyAllowFolders = true;
	public bool AllowUncreatedFile = true;
	public bool DontAllowTraverselBeyondRootFolder = false;
	
	private int sortIndex = 2;
	private ImGuiSortDirection sortDirection =  ImGuiSortDirection.Ascending;
	private List<PickableFile> _pickableFiles = new List<PickableFile>();
	private string inputFileName = "";
	private bool useInputPath = false;
	private string inputPath = "";
	
	public Action<string> OnItemConfirmed;

	public ImGuiFilePicker(ImGuiRenderer renderer, Scene scene, int id) : base(renderer, scene, id)
	{
		string startingPath = "";
		startingPath = Environment.CurrentDirectory;
		if (string.IsNullOrEmpty(startingPath))
			startingPath = AppContext.BaseDirectory;
		startingPath = new FileInfo(startingPath).DirectoryName;
		RootFolder = startingPath;
		CurrentFolder = startingPath;
		GetFiles();
	}
	
	public override void Update(GameTime gameTime)
	{ }

	public void OpenPopup(string popupName)
	{
		if (ImGui.IsPopupOpen(PopupName))
		{
			Console.WriteLine(PopupName + " FilePicker is still open");
			return;
		}
		PopupName = popupName;
		open = true;
		GetFiles();
	}
	
	private bool open = false;
	public override void Draw()
	{
		if (open)
		{
			ImGui.OpenPopup(PopupName);
			open = false;
		}

		var center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowSize(new Num.Vector2(767, 598));
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
		if (ImGui.BeginPopupModal(PopupName))
		{
			ImGui.Text("Current Folder: ");

			if (ImGui.BeginChild("PathBar", new Num.Vector2(0, ImGui.GetFrameHeight() + 30), ImGuiChildFlags.None,
				    ImGuiWindowFlags.HorizontalScrollbar))
			{
				if (useInputPath)
				{
					ImGui.Text("Enter Path: ");
					ImGui.SameLine();
					if (ImGui.InputText("##Path", ref inputPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
					{
						if (Directory.Exists(inputPath))
						{
							CurrentFolder = inputPath;
							useInputPath = false;
						}
					}
				}
				else
				{
					string[] subDirectories = CurrentFolder.Split(Path.DirectorySeparatorChar);
					for (int i = 0; i < subDirectories.Length; i++)
					{
						if (ImGui.Button(subDirectories[i]+ "/"))
						{
							if (i == 0)
								CurrentFolder = subDirectories[i] + "/";
							else
								CurrentFolder = string.Join(Path.DirectorySeparatorChar, subDirectories.Take(i + 1));
						}
						ImGui.SameLine();
					}
					ImGui.NewLine();
				}

				if (ImGui.Button("Edit Path"))
				{
					useInputPath = !useInputPath;
					inputPath = CurrentFolder;
				}
			}
			ImGui.EndChild();
			ImGui.NewLine();
			ImGui.Separator();
			ImGui.NewLine();
			bool result = false;

			if (ImGui.BeginChild("Sidebar", new Num.Vector2(150, 400), ImGuiChildFlags.FrameStyle | ImGuiChildFlags.Borders))
			{
				string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				string downloads = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
					"Downloads"
				);
				string history = Environment.GetFolderPath(Environment.SpecialFolder.History);
				string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

				
				if (ImGui.CollapsingHeader(FontAwesome4.Folder + " Places"))
				{
					ImGui.Separator();
					if (ImGui.Selectable("Desktop", CurrentFolder.Equals(desktop)))
						CurrentFolder = desktop;
					if (ImGui.Selectable("Documents", CurrentFolder.Equals(documents)))
						CurrentFolder = documents;
					if (ImGui.Selectable("Downloads", CurrentFolder.Equals(downloads)))
						CurrentFolder = downloads;
					if (ImGui.Selectable("History", CurrentFolder.Equals(history)))
						CurrentFolder = history;
					if (ImGui.Selectable("User", CurrentFolder.Equals(user)))
						CurrentFolder = user;
				}

				if (ImGui.CollapsingHeader( FontAwesome4.Archive + " Drives"))
				{
					ImGui.Separator();
					foreach (var drive in DriveInfo.GetDrives())
					{
						if (ImGui.Selectable(drive.Name, CurrentFolder.Contains(drive.Name))) 
							CurrentFolder = drive.RootDirectory.FullName;
					}
				}
			}
			ImGui.EndChild();
			ImGui.SameLine();
			
			
			const ImGuiTableFlags tableFlags = ImGuiTableFlags.BordersV | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.Resizable |
			                                   ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.Sortable | ImGuiTableFlags.NoSavedSettings;

			if (ImGui.BeginChild("Picker", new Num.Vector2(610f, 420f),
				    ImGuiChildFlags.Borders | ImGuiChildFlags.Borders))
			{
				if (ImGui.BeginTable("File Picker", 3, tableFlags, new Num.Vector2(600, 400)))
				{
					float TEXT_BASE_WIDTH = ImGui.CalcTextSize("A").X;
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoHide);
					ImGui.TableSetupColumn("Created", ImGuiTableColumnFlags.WidthFixed, TEXT_BASE_WIDTH * 12.0f);
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.DefaultSort, TEXT_BASE_WIDTH * 18.0f);
					ImGui.TableHeadersRow();

					var tableSpecs = ImGui.TableGetSortSpecs();
					if (tableSpecs.SpecsDirty)
					{
						sortIndex = tableSpecs.Specs.ColumnIndex;
						sortDirection = tableSpecs.Specs.SortDirection;
						SortFiles();
						tableSpecs.SpecsDirty = false;
					}
					
					var di = new DirectoryInfo(CurrentFolder);
					if (di.Exists)
					{
						if (di.Parent != null && (!DontAllowTraverselBeyondRootFolder || CurrentFolder != RootFolder))
						{
							ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.PackedValue);
							ImGui.TableNextRow();
							ImGui.TableSetColumnIndex(0);
							bool isSelected = SelectedFolder == di.Parent.FullName;
							if (ImGui.Selectable("../", isSelected, ImGuiSelectableFlags.None))
							{
								SelectedFolder = di.Parent.FullName;
								SelectedFile = "";
								inputFileName = "";
							}
							
							if (isSelected && ImGui.IsMouseDoubleClicked(0))
								CurrentFolder = di.Parent.FullName;
							
							ImGui.PopStyleColor();
						}
						
						string navigateToFolder = CurrentFolder;
						foreach (var file in _pickableFiles)
						{
							ImGui.TableNextRow();
							
							ImGui.TableSetColumnIndex(0);
							if (file.IsDirectory)
							{
								ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.PackedValue);
								bool isSelected = SelectedFolder == file.Path;
								if (ImGui.Selectable(file.Name + "/", isSelected, ImGuiSelectableFlags.None))
								{
									SelectedFolder = file.Path;
									SelectedFile = "";
									inputFileName = "";
									if (OnlyAllowFolders)
										inputFileName = file.Name + "/";
								}

								// Folder have to click open to select
								if (isSelected && ImGui.IsMouseDoubleClicked(0))
									navigateToFolder = file.Path;
								ImGui.PopStyleColor();
							}
							else
							{
								bool isSelected = SelectedFile == file.Path;
								if (ImGui.Selectable(file.Name, isSelected, ImGuiSelectableFlags.None))
								{
									SelectedFile = file.Path;
									SelectedFolder = "";
									inputFileName = Path.GetFileName(SelectedFile);
								}

								if (ImGui.IsMouseDoubleClicked(0) && isSelected)
								{
									ImGui.CloseCurrentPopup();
									OnItemConfirmed.Invoke(file.Path);
								}
							}
							
							ImGui.TableNextColumn();
							ImGui.Text(file.CreationDate);
							
							ImGui.TableNextColumn();
							ImGui.Text(file.Type);
							
						}
						if (CurrentFolder != navigateToFolder)
							CurrentFolder = navigateToFolder;
					}
					
				}
				ImGui.EndTable();
			}
			ImGui.EndChild();
			
			ImGui.PushStyleColor(ImGuiCol.FrameBg, new Num.Vector4(0.200f, 0.220f, 0.270f, 1.00f)); // RGBA
			ImGui.InputText("Current File ", ref inputFileName, 256);
			ImGui.PopStyleColor();
			if (AllowedExtensions != null && AllowedExtensions.Count > 0)
			{
				string allowedExtensions = "";
				bool first = true;
				foreach (var extension in AllowedExtensions)
				{
					if (!first)
						allowedExtensions += ", ";
					allowedExtensions += $"{extension}";
					first = false;
				}
				ImGui.Text("Allowed Extensions: " + allowedExtensions);
			}

			if (ImGui.Button("Cancel"))
			{
				ImGui.CloseCurrentPopup();
			}
			
			if (OnlyAllowFolders)
			{
				ImGui.SameLine();
				if (ImGui.Button(ConfirmButtonName))
				{
					SelectedFile = CurrentFolder;
					OnItemConfirmed.Invoke(SelectedFile);
					ImGui.CloseCurrentPopup();
				}
			}
			else if (!string.IsNullOrEmpty(SelectedFile))
			{
				ImGui.SameLine();
				if (ImGui.Button(ConfirmButtonName))
				{
					if (Path.GetFileName(SelectedFile) != inputFileName && AllowUncreatedFile)
						OnItemConfirmed.Invoke(Path.GetDirectoryName(SelectedFile) + "\\" + inputFileName);
					else if (Path.GetFileName(SelectedFile) == inputFileName)
						OnItemConfirmed.Invoke(SelectedFile);
					ImGui.CloseCurrentPopup();
				}
			}
			
			ImGui.EndPopup();
		}
	}

	bool TryGetFileInfo(string fileName, out FileInfo realFile)
	{
		try
		{
			realFile = new FileInfo(fileName);
			return true;
		}
		catch
		{
			realFile = null;
			return false;
		}
	}

	private void GetFiles()
	{
		var di = new DirectoryInfo(CurrentFolder);
		if (di.Exists)
		{
			_pickableFiles.Clear();
			var fileEntries = GetFileSystemEntries(di.FullName);
			foreach (var fileEntry in fileEntries)
			{
				PickableFile pickableFile = new PickableFile();
				pickableFile.Name = Path.GetFileName(fileEntry);
				pickableFile.CreationDate = File.GetCreationTime(fileEntry).ToString();
				if (Directory.Exists(fileEntry))
				{
					pickableFile.IsDirectory = true;
					pickableFile.Type = "File Folder";
				}
				else
				{
					pickableFile.IsDirectory = false;
					pickableFile.Type = string.Join("", Path.GetExtension(fileEntry).Skip(1)) + " File";
				}
				pickableFile.Path = fileEntry;
				_pickableFiles.Add(pickableFile);
			}
			SortFiles();	
		}
	}

	private void SortFiles()
	{
		
			
		if (sortIndex == 0)
		{
			if (sortDirection == ImGuiSortDirection.Ascending)
				_pickableFiles.Sort((a, b) 
					=> String.Compare(a.Name, b.Name, StringComparison.Ordinal));
			else
				_pickableFiles.Sort((a, b) 
					=> String.Compare(b.Name, a.Name, StringComparison.Ordinal));
		}
						
		if (sortIndex == 1)
		{
			if (sortDirection == ImGuiSortDirection.Ascending)
				_pickableFiles.Sort((a, b) 
					=> String.Compare(a.CreationDate, b.CreationDate, StringComparison.Ordinal));
			else
				_pickableFiles.Sort((a, b) 
					=> String.Compare(b.CreationDate, a.CreationDate, StringComparison.Ordinal));
		}
						
		if (sortIndex == 2)
		{
			if (sortDirection == ImGuiSortDirection.Ascending)
				_pickableFiles.Sort((a, b) 
					=> String.Compare(a.Type, b.Type, StringComparison.Ordinal));
			else
				_pickableFiles.Sort((a, b) 
					=> String.Compare(b.Type, a.Type, StringComparison.Ordinal));
		}
	}
	
	List<string> GetFileSystemEntries(string fullName)
	{
		var files = new List<string>();
		var dirs = new List<string>();

		foreach (var fse in Directory.GetFileSystemEntries(fullName))
		{
			var fileName = Path.GetFileName(fse);
			if (Directory.Exists(fse))
			{
				if (HideHiddenFolders && (fileName.StartsWith(".") || fileName.StartsWith("$")))
					continue;
				dirs.Add(fse);
			}
			else if (!OnlyAllowFolders)
			{
				if (AllowedExtensions != null)
				{
					var ext = Path.GetExtension(fse);
					if (AllowedExtensions.Contains(ext))
						files.Add(fse);
				}
				else
				{
					files.Add(fse);
				}
			}
		}
		
		dirs.Sort();
		files.Sort();
		
		var ret = new List<string>(dirs);
		ret.AddRange(files);

		return ret;
	}

	
	public static bool IsDirectory(string path)
	{
		if (!File.Exists(path) && !Directory.Exists(path))
		{
			// Path does not exist
			return false; 
		}

		FileAttributes attributes = File.GetAttributes(path);
		return (attributes & FileAttributes.Directory) == FileAttributes.Directory;
	}

	public static bool IsFile(string path)
	{
		if (!File.Exists(path) && !Directory.Exists(path))
		{
			return false;
		}

		FileAttributes attributes = File.GetAttributes(path);
		return (attributes & FileAttributes.Directory) != FileAttributes.Directory;
	}
}