using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

/// <summary>
/// 绘制文件数状目录
/// </summary>
public class FolderTreeView : TreeView
{
    /// <summary>
    /// 选择Item回调
    /// </summary>
    public CallBack<FolderTreeViewItem> selectCallBack;
    /// <summary>
    /// 双击Item回调
    /// </summary>
    public CallBack<FolderTreeViewItem> dblclickItemCallBack;

    static Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
    static Texture2D fileIcon = EditorGUIUtility.FindTexture("TextAsset Icon");
    private bool userSearch = true;


    private int index;
    private List<string> allPath = null;
    private SearchField m_SearchField;
    public FolderTreeView(TreeViewState state)
            : base(state)
    {
        index = 1;
        searchString = "";
        rowHeight = 20;
        showBorder = true;
        m_SearchField = new SearchField();
        m_SearchField.downOrUpArrowKeyPressed += SetFocusAndEnsureSelectedItem;
    }
    /// <summary>
    /// 是否开启使用搜索栏
    /// </summary>
    public bool UserSearch
    {
        get
        {
            return userSearch;
        }

        set
        {
            userSearch = value;
        }
    }
    /// <summary>
    /// 返回root节点
    /// </summary>
    public FolderTreeViewItem RootItem
    {
        get { return (FolderTreeViewItem)rootItem; }
    }
    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="datas"></param>
    public void SetData(List<string> datas)
    {
        allPath = datas;
        //BuildRoot();
        // BuildRows(rootItem);
        Reload();
    }
    private List<FolderTreeViewItem> GetFolderRows()
    {
        List<FolderTreeViewItem> list = new List<FolderTreeViewItem>();
        var rows = GetRows();
        if (rows != null)
        {
            foreach (var item in GetRows())
            {
                list.Add((FolderTreeViewItem)item);
            }
        }
        return list;
    }
    protected override TreeViewItem BuildRoot()
    {
        FolderTreeViewItem root = new FolderTreeViewItem { id = 0, depth = -1, displayName = "Root" };
        root.children = new List<TreeViewItem>();
        var rows = GetFolderRows();
        if (allPath == null)
            return root;
        foreach (var p in allPath)
        {
            string[] items = p.Split('/');

           
            string fullPath = "";
            for (int i = 0; i < items.Length; i++)
            {
               
                string displayName = items[i];
                if (string.IsNullOrEmpty(fullPath))
                    fullPath = displayName;
                else
                    fullPath += "/" + displayName;

                bool isDirectory = !(i == (items.Length - 1));

                bool isHaveItem = false;
                foreach (var item in rows)
                {
                    if (item.fullPath ==fullPath && item.isDirectory==isDirectory)
                    {
                        isHaveItem = true;
                        break;
                    }
                }

                if ( !isHaveItem)
                {
                    FolderTreeViewItem temp = new FolderTreeViewItem(index, i, displayName,fullPath,isDirectory);
                    string dir= Path.GetDirectoryName(fullPath);
                    FolderTreeViewItem parent = null;
                    foreach (var item in rows)
                    {
                        if (item.fullPath == dir && item.isDirectory)
                        {
                            parent = item;
                            break;
                        }
                    }
                    if (parent != null)
                        parent.AddChild(temp);
                    else
                        root.AddChild(temp);
                    rows.Add(temp);
                    index++;
                }
            }
        }
        List<TreeViewItem> list = new List<TreeViewItem>();
        foreach (var item in rows)
        {
            list.Add(item);
        }
        
        //SetupParentsAndChildrenFromDepths(root, list);
        foreach (var item in rows)
        {
            if (item.hasChildren)
                item.icon = folderIcon;
            else
                item.icon = fileIcon;
        }
        return root;
    }

     
    public override void OnGUI(Rect rect)
    {
        if (!userSearch)
            base.OnGUI(rect);
        else
        {
            Rect searchRect = new Rect(rect.x, rect.y, rect.width, EditorStyles.toolbar.fixedHeight);
            GUI.Box(searchRect, "", EditorStyles.toolbar);
            searchRect = new Rect(rect.x+ rect.width / 2, rect.y+3, rect.width / 2, EditorStyles.toolbar.fixedHeight);
            searchString = m_SearchField.OnToolbarGUI(searchRect, searchString);
       

            rect = new Rect(rect.x, rect.y + searchRect.height, rect.width, rect.height - searchRect.height);
            base.OnGUI(rect);
        }
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && args.rowRect.Contains(e.mousePosition) && selectCallBack != null)
        {
            selectCallBack((FolderTreeViewItem)args.item);
            
        }
        if (e.type == EventType.MouseDown && args.rowRect.Contains(e.mousePosition) && e.clickCount == 2 && dblclickItemCallBack != null)
        {
            dblclickItemCallBack((FolderTreeViewItem)args.item);
        }

        base.RowGUI(args);

    }
    
}

public class FolderTreeViewItem: TreeViewItem
{
    private string m_fullPath = "";
    private bool m_isDirectory;
    public FolderTreeViewItem(int id, int depth, string displayName,string fullPath,bool isDirectory) : base(id, depth, displayName)
    {
        m_fullPath = fullPath;
        m_isDirectory = isDirectory;
    }
    public FolderTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
    {
    }
    public FolderTreeViewItem(int id, int depth) : base(id, depth)
    {
    }
    public FolderTreeViewItem(int id) : base(id)
    {
    }
    public FolderTreeViewItem() : base()
    {
    }

    public string fullPath
    {
        get
        {
            return m_fullPath;
        }

        set
        {
            m_fullPath = value;
        }
    }

    public bool isDirectory
    {
        get
        {
            return m_isDirectory;
        }

        set
        {
            m_isDirectory = value;
        }
    }
}

