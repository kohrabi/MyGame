using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IconFonts;
using ImGuiNET.Renderer;
using MyEngine.Debug.IMGUIComponents;
using Num = System.Numerics;

namespace MyEngine.Editor.SpriteEditor;

public class FilePicker : ImGuiComponent
{
	public bool Active = true;
	
	public string RootFolder = "";
	public string CurrentFolder = "";
	public string SelectedFile = "";
	public List<string> AllowedExtensions;
	public bool HideHiddenFolders = true;
	public bool OnlyAllowFolders = false;
	public bool DontAllowTraverselBeyondRootFolder = false;
	private int sortIndex = 0;

	public FilePicker(ImGuiRenderer renderer, Scene scene, int id) : base(renderer, scene, id)
	{
		string startingPath = "";
		startingPath = Environment.CurrentDirectory;
		if (string.IsNullOrEmpty(startingPath))
			startingPath = AppContext.BaseDirectory;
		startingPath = new FileInfo(startingPath).DirectoryName;
		RootFolder = startingPath;
		CurrentFolder = startingPath;
	}
	
	public override void Update(GameTime gameTime)
	{ }


	private bool open = true;
	public override void Draw()
	{
		if (open)
		{
			ImGui.OpenPopup("Open File");
			open = false; // only open once
		}

		var center = ImGui.GetMainViewport().GetCenter();
		ImGui.SetNextWindowPos(center, ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
		if (ImGui.BeginPopupModal("Open File"))
		{
			ImGui.Text("Current Folder: ");
			
			ImGui.BeginChild("PathBar", new Num.Vector2(0, ImGui.GetFrameHeight() + 15), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);
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

				
				if (ImGui.CollapsingHeader(FontAwesome4.Folder + "Places"))
				{
					ImGui.Separator();
					if (ImGui.Selectable("Desktop", CurrentFolder.Contains(desktop)))
						CurrentFolder = desktop;
					if (ImGui.Selectable("Documents", CurrentFolder.Contains(documents)))
						CurrentFolder = documents;
					if (ImGui.Selectable("Downloads", CurrentFolder.Contains(downloads)))
						CurrentFolder = downloads;
					if (ImGui.Selectable("History", CurrentFolder.Contains(history)))
						CurrentFolder = history;
					if (ImGui.Selectable("User", CurrentFolder.Contains(user)))
						CurrentFolder = user;
				}

				if (ImGui.CollapsingHeader( FontAwesome4.Archive + "Drives"))
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
			                                   ImGuiTableFlags.RowBg | ImGuiTableFlags.NoBordersInBody | ImGuiTableFlags.Sortable |
			                                   ImGuiTableFlags.Reorderable;

			if (ImGui.BeginChild("Picker", new Num.Vector2(610f, 420f),
				    ImGuiChildFlags.Borders | ImGuiChildFlags.Borders))
			{
				if (ImGui.BeginTable("File Picker", 3, tableFlags, new Num.Vector2(600, 400)))
				{
					float TEXT_BASE_WIDTH = ImGui.CalcTextSize("A").X;
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoHide);
					ImGui.TableSetupColumn("Created", ImGuiTableColumnFlags.WidthFixed, TEXT_BASE_WIDTH * 12.0f, 1);
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, TEXT_BASE_WIDTH * 18.0f, 2);
					ImGui.TableHeadersRow();

					var tableSpecs = ImGui.TableGetSortSpecs();
					if (tableSpecs.SpecsDirty)
					{
						var nameFlags = ImGui.TableGetColumnFlags(2);
						if (nameFlags.HasFlag(ImGuiTableColumnFlags.IsSorted))
						{
							if (nameFlags.HasFlag(ImGuiTableColumnFlags.PreferSortAscending))
							{
								
							}
							else
							{
								
							}
						}
						
						var columnFlags = ImGui.TableGetColumnFlags(2);
						if (columnFlags.HasFlag(ImGuiTableColumnFlags.IsSorted))
						{
							
							if (columnFlags.HasFlag(ImGuiTableColumnFlags.PreferSortAscending))
							{
								
							}
							else
							{
								
							}
						}
						
						var typeFlags = ImGui.TableGetColumnFlags(2);
						if (typeFlags.HasFlag(ImGuiTableColumnFlags.IsSorted))
						{
							
							if (typeFlags.HasFlag(ImGuiTableColumnFlags.PreferSortAscending))
							{
								
							}
							else
							{
								
							}
						}
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
							if (ImGui.Selectable("../", false, ImGuiSelectableFlags.None))
								CurrentFolder = di.Parent.FullName;
							
							ImGui.PopStyleColor();
						}

						
						var fileSystemEntries = GetFileSystemEntries(di.FullName);
						foreach (var fse in fileSystemEntries)
						{
							ImGui.TableNextRow();
							if (Directory.Exists(fse))
							{
								var name = Path.GetFileName(fse);
								ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.PackedValue);
								
								ImGui.TableSetColumnIndex(0);
								if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.None))
									CurrentFolder = fse;
								
								ImGui.TableNextColumn();
								var time = File.GetCreationTime(fse);
								ImGui.Text(time.ToString());
								
								ImGui.TableNextColumn();
								ImGui.Text("File Folder");
								
								ImGui.PopStyleColor();
							}
							else
							{
								var name = Path.GetFileName(fse);
								bool isSelected = SelectedFile == fse;
								ImGui.TableSetColumnIndex(0);
								if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.None))
									SelectedFile = fse;
								
								ImGui.TableNextColumn();
								var time = File.GetCreationTime(fse);
								ImGui.Text(time.ToString());
								
								ImGui.TableNextColumn();
								var fileType = string.Join("", Path.GetExtension(fse).Skip(1));
								ImGui.Text(fileType + " File");

								// if (ImGui.IsMouseDoubleClicked(0))
								// {
								// 	ImGui.CloseCurrentPopup();
								// }
							}
						}
					}
					
				}
				ImGui.EndTable();
			}
			ImGui.EndChild();


			if (ImGui.Button("Cancel"))
			{
				ImGui.CloseCurrentPopup();
			}
			
			if (OnlyAllowFolders)
			{
				ImGui.SameLine();
				if (ImGui.Button("Open"))
				{
					SelectedFile = CurrentFolder;
					ImGui.CloseCurrentPopup();
				}
			}
			else if (SelectedFile != null)
			{
				ImGui.SameLine();
				if (ImGui.Button("Open"))
				{
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

	List<string> GetFileSystemEntries(string fullName)
	{
		var files = new List<string>();
		var dirs = new List<string>();

		foreach (var fse in Directory.GetFileSystemEntries(fullName))
		{
			if (Directory.Exists(fse) && (!HideHiddenFolders || !Path.GetFileName(fse).StartsWith(".")))
			{
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

}