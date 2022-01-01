using System;
using System.Collections.Generic;
using System.Linq;

namespace design_in_memory_file_system
{
  class Program
  {
    static void Main(string[] args)
    {
      FileSystem fileSystem = new FileSystem();
      showFiles(fileSystem, "/");
      fileSystem.mkdir("/a/b/c");
      showFiles(fileSystem, "/a");
      showFiles(fileSystem, "/a/b");
      fileSystem.addContentToFile("/a/b/c/d", "hello");
      Console.WriteLine(fileSystem.readContentFromFile("/a/b/c/d"));
      fileSystem.mkdir("/a/b/c/e");
      showFiles(fileSystem, "/a/b/c");
      showFiles(fileSystem, "/a/b/c/d"); // file path
      fileSystem.mkdir("/a/b/c/e/f");
      fileSystem.mkdir("/a/b/c/e/g");
      showFiles(fileSystem, "/a/b/c/e"); // directory path
    }

    static void showFiles(FileSystem fileSystem, string path)
    {
      var lists = fileSystem.ls(path);
      Console.WriteLine(string.Join(",", lists));
    }
  }

  public class TrieNode
  {
    public string content = string.Empty;
    public Dictionary<string, TrieNode> children = new Dictionary<string, TrieNode>();
    public bool isFile = false;
  }

  public class FileSystem
  {
    TrieNode root = null;

    // initialize the object
    public FileSystem()
    {
      root = new TrieNode();
    }

    // if the path is filepath, returns a list that only contains the file name.
    // if it is a directory path, returns the file and directory names in this directory.
    public List<string> ls(string path)
    {
      string key = string.Empty;
      TrieNode current = root;
      if (path != "/")
      {
        var paths = path.Split("/");
        for (int i = 1; i < paths.Length; i++)
        {
          key = paths[i];
          current = current.children[key];
        }
        if (current.isFile)
        {
          return new List<string>() { key };
        }
      }

      return current.children.Keys.OrderBy(x => x).ToList();
    }

    // makes a new directory according to the path, if the given directory path does not exist.
    // if the middle directories does not exist you should create them as well.
    public void mkdir(string path)
    {
      TrieNode current = root;
      var paths = path.Split("/");
      for (int i = 1; i < paths.Length; i++)
      {
        string key = paths[i];
        if (!current.children.ContainsKey(key))
          current.children.Add(key, new TrieNode());
        current = current.children[key];
      }
    }

    // if the filePath does not exist, create the file containing given content.
    // if the path exist then append the new content.
    public void addContentToFile(string filePath, string content)
    {
      string key = string.Empty;
      TrieNode current = root;
      var paths = filePath.Split("/");
      for (int i = 1; i < paths.Length; i++)
      {
        key = paths[i];
        if (!current.children.ContainsKey(key))
          current.children.Add(key, new TrieNode());
        current = current.children[key];
      }
      current.isFile = true;
      current.content += content;
    }

    // returns the content in the file at filePath.
    public string readContentFromFile(string filePath)
    {
      TrieNode current = root;
      var paths = filePath.Split("/");
      for (int i = 1; i < paths.Length; i++)
      {
        current = current.children[paths[i]];
      }
      return current.content;
    }
  }
}
